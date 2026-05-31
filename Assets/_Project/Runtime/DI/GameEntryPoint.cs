using System;
using ExtractionRoom.Core;
using ExtractionRoom.Objectives;
using VContainer.Unity;

namespace ExtractionRoom.DI
{
    public sealed class GameEntryPoint : IStartable
    {
        private readonly IGameStateMachine gameStateMachine;

        public GameEntryPoint(IGameStateMachine gameStateMachine, IObjectiveService objectiveService)
        {
            this.gameStateMachine = gameStateMachine;
            _ = objectiveService ?? throw new ArgumentNullException(nameof(objectiveService));
        }

        public void Start()
        {
            gameStateMachine.TryTransitionTo(GameState.Playing);
        }
    }
}
