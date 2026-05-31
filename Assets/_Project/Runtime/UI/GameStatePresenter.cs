using System;
using ExtractionRoom.Core;
using R3;

namespace ExtractionRoom.UI
{
    public sealed class GameStatePresenter : IDisposable
    {
        private readonly IDisposable subscription;

        public GameStatePresenter(IGameStateMachine gameStateMachine, EndGameView view)
        {
            if (gameStateMachine == null)
            {
                throw new ArgumentNullException(nameof(gameStateMachine));
            }

            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            subscription = gameStateMachine.CurrentStateObservable.Subscribe(state => Display(state, view));
        }

        public void Dispose()
        {
            subscription.Dispose();
        }

        private static void Display(GameState state, EndGameView view)
        {
            switch (state)
            {
                case GameState.Won:
                    view.ShowWin();
                    break;
                case GameState.Lost:
                    view.ShowLose();
                    break;
                default:
                    view.Hide();
                    break;
            }
        }
    }
}
