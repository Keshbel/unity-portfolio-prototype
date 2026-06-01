using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ExtractionRoom.Core;
using ExtractionRoom.DI;
using ExtractionRoom.Objectives;
using NUnit.Framework;

namespace ExtractionRoom.Tests.EditMode
{
    public sealed class GameStateMachineTests
    {
        [Test]
        public void Constructor_StartsInBootstrappingState()
        {
            using var eventBus = new EventBus();
            using var stateMachine = new GameStateMachine(eventBus);

            Assert.That(stateMachine.CurrentState, Is.EqualTo(GameState.Bootstrapping));
            Assert.That(stateMachine.CurrentStateObservable.CurrentValue, Is.EqualTo(GameState.Bootstrapping));
        }

        [Test]
        public void Constructor_WithMissingEventBus_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new GameStateMachine(null));
        }

        [Test]
        public void TryTransitionTo_Playing_PublishesStateChange()
        {
            using var eventBus = new EventBus();
            using var stateMachine = new GameStateMachine(eventBus);
            GameStateChangedEvent? publishedEvent = null;
            using var subscription = eventBus.Subscribe<GameStateChangedEvent>(value => publishedEvent = value);

            var didTransition = stateMachine.TryTransitionTo(GameState.Playing);

            Assert.That(didTransition, Is.True);
            Assert.That(stateMachine.CurrentState, Is.EqualTo(GameState.Playing));
            Assert.That(publishedEvent?.PreviousState, Is.EqualTo(GameState.Bootstrapping));
            Assert.That(publishedEvent?.CurrentState, Is.EqualTo(GameState.Playing));
        }

        [Test]
        public void TryTransitionTo_DuplicateState_DoesNotPublish()
        {
            using var eventBus = new EventBus();
            using var stateMachine = new GameStateMachine(eventBus);
            var notificationCount = 0;
            using var subscription = eventBus.Subscribe<GameStateChangedEvent>(_ => notificationCount++);

            var didTransition = stateMachine.TryTransitionTo(GameState.Bootstrapping);

            Assert.That(didTransition, Is.False);
            Assert.That(notificationCount, Is.Zero);
        }

        [Test]
        public void TryTransitionTo_InvalidState_DoesNotTransition()
        {
            using var eventBus = new EventBus();
            using var stateMachine = new GameStateMachine(eventBus);

            var didTransition = stateMachine.TryTransitionTo(GameState.Won);

            Assert.That(didTransition, Is.False);
            Assert.That(stateMachine.CurrentState, Is.EqualTo(GameState.Bootstrapping));
        }

        [Test]
        public void TryTransitionTo_FromTerminalState_DoesNotTransition()
        {
            using var eventBus = new EventBus();
            using var stateMachine = new GameStateMachine(eventBus);
            stateMachine.TryTransitionTo(GameState.Playing);
            stateMachine.TryTransitionTo(GameState.Won);

            var didTransition = stateMachine.TryTransitionTo(GameState.Lost);

            Assert.That(didTransition, Is.False);
            Assert.That(stateMachine.CurrentState, Is.EqualTo(GameState.Won));
        }

        [Test]
        public async System.Threading.Tasks.Task GameEntryPoint_StartAsync_TransitionsToPlayingAfterInitialization()
        {
            using var eventBus = new EventBus();
            using var stateMachine = new GameStateMachine(eventBus);
            using var objectiveService = new ObjectiveService(eventBus, stateMachine);
            var initializationService = new ControlledInitializationService();
            var entryPoint = new GameEntryPoint(stateMachine, initializationService, objectiveService);

            var initialization = entryPoint.StartAsync().AsTask();

            Assert.That(stateMachine.CurrentState, Is.EqualTo(GameState.Bootstrapping));
            initializationService.Complete();
            await initialization;
            Assert.That(stateMachine.CurrentState, Is.EqualTo(GameState.Playing));
        }

        [Test]
        public async System.Threading.Tasks.Task GameEntryPoint_StartAsync_WhenCancelled_RemainsBootstrapping()
        {
            using var eventBus = new EventBus();
            using var stateMachine = new GameStateMachine(eventBus);
            using var objectiveService = new ObjectiveService(eventBus, stateMachine);
            using var cancellation = new CancellationTokenSource();
            var initializationService = new ControlledInitializationService();
            var entryPoint = new GameEntryPoint(stateMachine, initializationService, objectiveService);

            var initialization = entryPoint.StartAsync(cancellation.Token).AsTask();
            cancellation.Cancel();
            initializationService.Complete();
            await initialization;

            Assert.That(stateMachine.CurrentState, Is.EqualTo(GameState.Bootstrapping));
        }

        private sealed class ControlledInitializationService : IGameInitializationService
        {
            private readonly UniTaskCompletionSource completionSource = new();

            public async UniTask InitializeAsync(CancellationToken cancellationToken)
            {
                await completionSource.Task.AttachExternalCancellation(cancellationToken);
            }

            public void Complete()
            {
                completionSource.TrySetResult();
            }
        }
    }
}
