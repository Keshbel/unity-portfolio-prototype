using ExtractionRoom.Core;
using VContainer.Unity;

namespace ExtractionRoom.DI
{
    public sealed class GameEntryPoint : IStartable
    {
        private readonly IGameStateMachine gameStateMachine;

        public GameEntryPoint(IGameStateMachine gameStateMachine)
        {
            this.gameStateMachine = gameStateMachine;
        }

        public void Start()
        {
            gameStateMachine.TryTransitionTo(GameState.Playing);
        }
    }
}
