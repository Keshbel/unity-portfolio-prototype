using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace ExtractionRoom.Core
{
    public sealed class GameInitializationService : IGameInitializationService
    {
        private static readonly TimeSpan DemoLoadingDelay = TimeSpan.FromMilliseconds(250);

        public async UniTask InitializeAsync(CancellationToken cancellationToken)
        {
            await UniTask.Delay(DemoLoadingDelay, cancellationToken: cancellationToken);
        }
    }
}
