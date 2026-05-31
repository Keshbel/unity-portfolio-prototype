using System;
using System.Collections.Generic;
using ExtractionRoom.Core;
using ExtractionRoom.Gameplay;
using ExtractionRoom.Inventory;
using ExtractionRoom.Items;
using R3;

namespace ExtractionRoom.Objectives
{
    public sealed class ObjectiveService : IObjectiveService
    {
        private const int RequiredFuseCount = 3;

        private readonly IEventBus eventBus;
        private readonly IGameStateMachine gameStateMachine;
        private readonly List<IDisposable> subscriptions = new();
        private readonly ReactiveProperty<string> currentObjectiveText;
        private readonly ReactiveProperty<ObjectiveProgress> progress;

        public ObjectiveService(IEventBus eventBus, IGameStateMachine gameStateMachine)
        {
            this.eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            this.gameStateMachine = gameStateMachine ?? throw new ArgumentNullException(nameof(gameStateMachine));

            CurrentState = CreateCollectFusesState(0);
            currentObjectiveText = new ReactiveProperty<string>(CurrentState.Text);
            progress = new ReactiveProperty<ObjectiveProgress>(CurrentState.Progress);

            subscriptions.Add(eventBus.Subscribe<InventoryChangedEvent>(OnInventoryChanged));
            subscriptions.Add(eventBus.Subscribe<GeneratorActivatedEvent>(_ => OnGeneratorActivated()));
            subscriptions.Add(eventBus.Subscribe<ExtractionReachedEvent>(_ => OnExtractionReached()));
        }

        public ObjectiveState CurrentState { get; private set; }

        public ReadOnlyReactiveProperty<string> CurrentObjectiveTextObservable => currentObjectiveText;

        public ReadOnlyReactiveProperty<ObjectiveProgress> ProgressObservable => progress;

        public void Dispose()
        {
            foreach (var subscription in subscriptions)
            {
                subscription.Dispose();
            }

            subscriptions.Clear();
            currentObjectiveText.Dispose();
            progress.Dispose();
        }

        private void OnInventoryChanged(InventoryChangedEvent eventData)
        {
            if (CurrentState.Step != ObjectiveStep.CollectFuses)
            {
                return;
            }

            var fuseCount = GetFuseCount(eventData.CurrentSnapshot);
            if (fuseCount >= RequiredFuseCount)
            {
                CompleteCurrentObjective(CreateActivateGeneratorState());
                return;
            }

            SetState(CreateCollectFusesState(fuseCount));
        }

        private void OnGeneratorActivated()
        {
            if (CurrentState.Step == ObjectiveStep.ActivateGenerator)
            {
                CompleteCurrentObjective(CreateReachExtractionState());
            }
        }

        private void OnExtractionReached()
        {
            if (CurrentState.Step != ObjectiveStep.ReachExtraction)
            {
                return;
            }

            CompleteCurrentObjective(CreateCompletedState());
            gameStateMachine.TryTransitionTo(GameState.Won);
        }

        private void CompleteCurrentObjective(ObjectiveState nextState)
        {
            var completedProgress = new ObjectiveProgress(CurrentState.Progress.Target, CurrentState.Progress.Target);
            var completedObjective = new ObjectiveState(CurrentState.Step, CurrentState.Text, completedProgress);
            eventBus.Publish(new ObjectiveCompletedEvent(completedObjective));
            SetState(nextState);
        }

        private void SetState(ObjectiveState nextState)
        {
            var previousState = CurrentState;
            CurrentState = nextState;
            currentObjectiveText.Value = nextState.Text;
            progress.Value = nextState.Progress;
            eventBus.Publish(new ObjectiveChangedEvent(previousState, nextState));
        }

        private static int GetFuseCount(InventorySnapshot snapshot)
        {
            var fuseCount = 0;

            foreach (var slot in snapshot.Slots)
            {
                if (slot.ItemId == ItemId.Fuse)
                {
                    fuseCount += slot.Count;
                }
            }

            return fuseCount;
        }

        private static ObjectiveState CreateCollectFusesState(int fuseCount)
        {
            return new ObjectiveState(
                ObjectiveStep.CollectFuses,
                "Collect 3 Fuses",
                new ObjectiveProgress(fuseCount, RequiredFuseCount));
        }

        private static ObjectiveState CreateActivateGeneratorState()
        {
            return new ObjectiveState(
                ObjectiveStep.ActivateGenerator,
                "Activate Generator",
                new ObjectiveProgress(0, 1));
        }

        private static ObjectiveState CreateReachExtractionState()
        {
            return new ObjectiveState(
                ObjectiveStep.ReachExtraction,
                "Reach Extraction Zone",
                new ObjectiveProgress(0, 1));
        }

        private static ObjectiveState CreateCompletedState()
        {
            return new ObjectiveState(
                ObjectiveStep.Completed,
                "Extraction Complete",
                new ObjectiveProgress(1, 1));
        }
    }
}
