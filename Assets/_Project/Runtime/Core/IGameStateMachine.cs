using System;
using R3;

namespace ExtractionRoom.Core
{
    public interface IGameStateMachine : IDisposable
    {
        GameState CurrentState { get; }

        ReadOnlyReactiveProperty<GameState> CurrentStateObservable { get; }

        bool TryTransitionTo(GameState nextState);
    }
}
