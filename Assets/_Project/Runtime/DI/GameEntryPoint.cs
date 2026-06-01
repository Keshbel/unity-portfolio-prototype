using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ExtractionRoom.Core;
using ExtractionRoom.Objectives;
using UnityEngine;
using VContainer.Unity;

namespace ExtractionRoom.DI
{
    public sealed class GameEntryPoint : IAsyncStartable
    {
        private readonly IGameStateMachine gameStateMachine;
        private readonly IGameInitializationService initializationService;

        public GameEntryPoint(
            IGameStateMachine gameStateMachine,
            IGameInitializationService initializationService,
            IObjectiveService objectiveService)
        {
            this.gameStateMachine = gameStateMachine
                ?? throw new ArgumentNullException(nameof(gameStateMachine));
            this.initializationService = initializationService
                ?? throw new ArgumentNullException(nameof(initializationService));
            _ = objectiveService ?? throw new ArgumentNullException(nameof(objectiveService));
        }

        public async UniTask StartAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await initializationService.InitializeAsync(cancellationToken);
                gameStateMachine.TryTransitionTo(GameState.Playing);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }
    }
}
