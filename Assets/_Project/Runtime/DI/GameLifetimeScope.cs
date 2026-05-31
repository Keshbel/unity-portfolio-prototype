using System.Collections.Generic;
using ExtractionRoom.Core;
using ExtractionRoom.Gameplay;
using ExtractionRoom.Inventory;
using ExtractionRoom.Items;
using ExtractionRoom.Objectives;
using ExtractionRoom.Player;
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
            itemDefinitions = definitions;
            sceneInjectionRoots = new List<GameObject>(injectionRoots).ToArray();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IEventBus, EventBus>(Lifetime.Singleton);
            builder.Register<IGameStateMachine, GameStateMachine>(Lifetime.Singleton);
            builder.Register<IDamageService, DamageService>(Lifetime.Singleton);
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

                foreach (var extractionZone in root.GetComponentsInChildren<ExtractionZone>(true))
                {
                    extractionZone.Construct(eventBus, objectiveService);
                }
            }
        }
    }
}
