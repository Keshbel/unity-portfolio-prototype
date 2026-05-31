using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace ExtractionRoom.Core
{
    public sealed class SceneLoadingService : ISceneLoadingService
    {
        public async UniTask LoadSceneAsync(string sceneName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                throw new ArgumentException("Scene name must be provided.", nameof(sceneName));
            }

            var operation = SceneManager.LoadSceneAsync(sceneName);
            if (operation == null)
            {
                throw new InvalidOperationException($"Unable to start loading scene '{sceneName}'.");
            }

            await operation.ToUniTask(cancellationToken: cancellationToken);
        }
    }
}
