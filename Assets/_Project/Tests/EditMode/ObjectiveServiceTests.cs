using ExtractionRoom.Core;
using ExtractionRoom.Gameplay;
using ExtractionRoom.Inventory;
using ExtractionRoom.Items;
using ExtractionRoom.Objectives;
using NUnit.Framework;

namespace ExtractionRoom.Tests.EditMode
{
    public sealed class ObjectiveServiceTests
    {
        [Test]
        public void Constructor_StartsWithCollectThreeFusesObjective()
        {
            using var eventBus = new EventBus();
            using var stateMachine = new GameStateMachine(eventBus);
            using var objectives = new ObjectiveService(eventBus, stateMachine);

            Assert.That(objectives.CurrentState.Step, Is.EqualTo(ObjectiveStep.CollectFuses));
            Assert.That(objectives.CurrentObjectiveTextObservable.CurrentValue, Is.EqualTo("Collect 3 Fuses"));
            Assert.That(objectives.ProgressObservable.CurrentValue.Current, Is.Zero);
            Assert.That(objectives.ProgressObservable.CurrentValue.Target, Is.EqualTo(3));
        }

        [Test]
        public void InventoryChanged_WithLessThanThreeFuses_DoesNotCompleteStep()
        {
            using var eventBus = new EventBus();
            using var stateMachine = new GameStateMachine(eventBus);
            using var objectives = new ObjectiveService(eventBus, stateMachine);

            PublishFuseCount(eventBus, 2);

            Assert.That(objectives.CurrentState.Step, Is.EqualTo(ObjectiveStep.CollectFuses));
            Assert.That(objectives.ProgressObservable.CurrentValue.Current, Is.EqualTo(2));
        }

        [Test]
        public void InventoryChanged_WithThreeFuses_AdvancesToActivateGenerator()
        {
            using var eventBus = new EventBus();
            using var stateMachine = new GameStateMachine(eventBus);
            using var objectives = new ObjectiveService(eventBus, stateMachine);

            PublishFuseCount(eventBus, 3);

            Assert.That(objectives.CurrentState.Step, Is.EqualTo(ObjectiveStep.ActivateGenerator));
            Assert.That(objectives.CurrentObjectiveTextObservable.CurrentValue, Is.EqualTo("Activate Generator"));
        }

        [Test]
        public void GeneratorActivated_AdvancesToReachExtraction()
        {
            using var eventBus = new EventBus();
            using var stateMachine = new GameStateMachine(eventBus);
            using var objectives = new ObjectiveService(eventBus, stateMachine);
            PublishFuseCount(eventBus, 3);

            eventBus.Publish(new GeneratorActivatedEvent());

            Assert.That(objectives.CurrentState.Step, Is.EqualTo(ObjectiveStep.ReachExtraction));
            Assert.That(objectives.CurrentObjectiveTextObservable.CurrentValue, Is.EqualTo("Reach Extraction Zone"));
        }

        [Test]
        public void GeneratorActivated_BeforeCollectingFuses_DoesNotAdvanceObjective()
        {
            using var eventBus = new EventBus();
            using var stateMachine = new GameStateMachine(eventBus);
            using var objectives = new ObjectiveService(eventBus, stateMachine);

            eventBus.Publish(new GeneratorActivatedEvent());

            Assert.That(objectives.CurrentState.Step, Is.EqualTo(ObjectiveStep.CollectFuses));
        }

        [Test]
        public void ExtractionReached_BeforeGeneratorActivation_DoesNotCompleteObjective()
        {
            using var eventBus = new EventBus();
            using var stateMachine = new GameStateMachine(eventBus);
            using var objectives = new ObjectiveService(eventBus, stateMachine);
            PublishFuseCount(eventBus, 3);

            eventBus.Publish(new ExtractionReachedEvent());

            Assert.That(objectives.CurrentState.Step, Is.EqualTo(ObjectiveStep.ActivateGenerator));
            Assert.That(stateMachine.CurrentState, Is.EqualTo(GameState.Bootstrapping));
        }

        [Test]
        public void ExtractionReached_CompletesFinalObjective()
        {
            using var eventBus = new EventBus();
            using var stateMachine = new GameStateMachine(eventBus);
            using var objectives = new ObjectiveService(eventBus, stateMachine);
            ObjectiveCompletedEvent? completedEvent = null;
            using var subscription = eventBus.Subscribe<ObjectiveCompletedEvent>(value => completedEvent = value);
            AdvanceToExtraction(eventBus);

            eventBus.Publish(new ExtractionReachedEvent());

            Assert.That(objectives.CurrentState.Step, Is.EqualTo(ObjectiveStep.Completed));
            Assert.That(objectives.CurrentState.IsCompleted, Is.True);
            Assert.That(completedEvent?.CompletedObjective.Step, Is.EqualTo(ObjectiveStep.ReachExtraction));
            Assert.That(completedEvent?.CompletedObjective.Progress.IsComplete, Is.True);
        }

        [Test]
        public void ExtractionReached_WhenGameIsPlaying_TransitionsGameStateToWon()
        {
            using var eventBus = new EventBus();
            using var stateMachine = new GameStateMachine(eventBus);
            using var objectives = new ObjectiveService(eventBus, stateMachine);
            stateMachine.TryTransitionTo(GameState.Playing);
            AdvanceToExtraction(eventBus);

            eventBus.Publish(new ExtractionReachedEvent());

            Assert.That(stateMachine.CurrentState, Is.EqualTo(GameState.Won));
        }

        private static void AdvanceToExtraction(EventBus eventBus)
        {
            PublishFuseCount(eventBus, 3);
            eventBus.Publish(new GeneratorActivatedEvent());
        }

        private static void PublishFuseCount(EventBus eventBus, int fuseCount)
        {
            var previousSnapshot = new InventorySnapshot(System.Array.Empty<InventorySlot>());
            var currentSnapshot = new InventorySnapshot(new[] { new InventorySlot(ItemId.Fuse, fuseCount) });
            eventBus.Publish(new InventoryChangedEvent(previousSnapshot, currentSnapshot));
        }
    }
}
