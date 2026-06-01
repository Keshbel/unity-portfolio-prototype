using System;
using R3;

namespace ExtractionRoom.Core
{
    public sealed class GameStateMachine : IGameStateMachine
    {
        private readonly IEventBus eventBus;
        private readonly ReactiveProperty<GameState> currentState = new(GameState.Bootstrapping);

        public GameStateMachine(IEventBus eventBus)
        {
            this.eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        public GameState CurrentState => currentState.Value;

        public ReadOnlyReactiveProperty<GameState> CurrentStateObservable => currentState;

        public bool TryTransitionTo(GameState nextState)
        {
            if (nextState == CurrentState || !CanTransition(CurrentState, nextState))
            {
                return false;
            }

            var previousState = CurrentState;
            currentState.Value = nextState;
            eventBus.Publish(new GameStateChangedEvent(previousState, nextState));
            return true;
        }

        public void Dispose()
        {
            currentState.Dispose();
        }

        private static bool CanTransition(GameState current, GameState next)
        {
            return current switch
            {
                GameState.Bootstrapping => next == GameState.Playing,
                GameState.Playing => next is GameState.Won or GameState.Lost,
                _ => false
            };
        }
    }
}
