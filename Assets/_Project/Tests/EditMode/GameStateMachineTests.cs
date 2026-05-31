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
        public void GameEntryPoint_Start_TransitionsToPlaying()
        {
            using var eventBus = new EventBus();
            using var stateMachine = new GameStateMachine(eventBus);
            using var objectiveService = new ObjectiveService(eventBus, stateMachine);
            var entryPoint = new GameEntryPoint(stateMachine, objectiveService);

            entryPoint.Start();

            Assert.That(stateMachine.CurrentState, Is.EqualTo(GameState.Playing));
        }
    }
}
