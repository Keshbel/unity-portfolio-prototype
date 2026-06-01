using System;
using System.Collections.Generic;
using ExtractionRoom.AI;
using ExtractionRoom.Core;
using ExtractionRoom.Gameplay;
using ExtractionRoom.Inventory;
using ExtractionRoom.Items;
using ExtractionRoom.Objectives;
using ExtractionRoom.Player;
using ExtractionRoom.Interaction;
using ExtractionRoom.Presentation;
using ExtractionRoom.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ExtractionRoom.DI
{
    public sealed class GameLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private ItemDefinition[] itemDefinitions = System.Array.Empty<ItemDefinition>();

        [SerializeField]
        private GameObject[] sceneInjectionRoots = System.Array.Empty<GameObject>();

        public void ConfigureScene(ItemDefinition[] definitions, IEnumerable<GameObject> injectionRoots)
        {
            itemDefinitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            sceneInjectionRoots = injectionRoots != null
                ? new List<GameObject>(injectionRoots).ToArray()
                : throw new ArgumentNullException(nameof(injectionRoots));
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IEventBus, EventBus>(Lifetime.Singleton);
            builder.Register<IGameStateMachine, GameStateMachine>(Lifetime.Singleton);
            builder.Register<IDamageService, DamageService>(Lifetime.Singleton);
            builder.Register<IGameInitializationService, GameInitializationService>(Lifetime.Singleton);
            builder.Register<ISceneLoadingService, SceneLoadingService>(Lifetime.Singleton);
            builder.RegisterInstance<IItemDefinitionProvider>(new ItemDefinitionProvider(itemDefinitions));
            builder.Register<IInventoryService>(
                resolver => new InventoryService(
                    resolver.Resolve<IItemDefinitionProvider>(),
                    resolver.Resolve<IEventBus>()),
                Lifetime.Singleton);
            builder.Register<IObjectiveService, ObjectiveService>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GameEntryPoint>();
            builder.RegisterBuildCallback(ConfigureSceneAdapters);
        }

        private void ConfigureSceneAdapters(IObjectResolver resolver)
        {
            var eventBus = resolver.Resolve<IEventBus>();
            var gameStateMachine = resolver.Resolve<IGameStateMachine>();
            var inventoryService = resolver.Resolve<IInventoryService>();
            var objectiveService = resolver.Resolve<IObjectiveService>();

            foreach (var root in sceneInjectionRoots)
            {
                if (root == null)
                {
                    throw new MissingReferenceException("A scene injection root is missing.");
                }

                foreach (var healthBinder in root.GetComponentsInChildren<PlayerHealthBinder>(true))
                {
                    healthBinder.Construct(eventBus, gameStateMachine);
                }

                foreach (var pickup in root.GetComponentsInChildren<PickupItemInteractable>(true))
                {
                    pickup.Construct(inventoryService);
                }

                foreach (var generator in root.GetComponentsInChildren<GeneratorInteractable>(true))
                {
                    generator.Construct(eventBus);
                }

                foreach (var generatorView in root.GetComponentsInChildren<GeneratorActivationView>(true))
                {
                    generatorView.Construct(eventBus);
                }

                foreach (var extractionZone in root.GetComponentsInChildren<ExtractionZone>(true))
                {
                    extractionZone.Construct(eventBus, objectiveService);
                }

            }

            foreach (var root in sceneInjectionRoots)
            {
                foreach (var enemy in root.GetComponentsInChildren<EnemyAIController>(true))
                {
                    enemy.Construct(
                        resolver.Resolve<IDamageService>(),
                        eventBus,
                        GetComponentFromSceneRoots<PlayerHealthBinder>());
                }
            }

            foreach (var root in sceneInjectionRoots)
            {
                foreach (var hudView in root.GetComponentsInChildren<HudView>(true))
                {
                    ConfigureHud(hudView, gameStateMachine, inventoryService, objectiveService);
                }
            }
        }

        private void ConfigureHud(
            HudView hudView,
            IGameStateMachine gameStateMachine,
            IInventoryService inventoryService,
            IObjectiveService objectiveService)
        {
            var playerHealthBinder = GetComponentFromSceneRoots<PlayerHealthBinder>();
            var interactionController = GetComponentFromSceneRoots<InteractionController>();
            var presenters = new CompositeDisposable(
                new HudPresenter(
                    playerHealthBinder.Health,
                    inventoryService,
                    objectiveService,
                    interactionController,
                    hudView),
                new GameStatePresenter(gameStateMachine, hudView.EndGameView));
            hudView.Bind(presenters);
        }

        private T GetComponentFromSceneRoots<T>() where T : Component
        {
            foreach (var root in sceneInjectionRoots)
            {
                var component = root.GetComponentInChildren<T>(true);
                if (component != null)
                {
                    return component;
                }
            }

            throw new MissingReferenceException($"Scene adapter {typeof(T).Name} is not configured.");
        }

        private sealed class CompositeDisposable : System.IDisposable
        {
            private readonly System.IDisposable[] disposables;

            public CompositeDisposable(params System.IDisposable[] disposables)
            {
                this.disposables = disposables;
            }

            public void Dispose()
            {
                foreach (var disposable in disposables)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}
