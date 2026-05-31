using System.Threading;
using Cysharp.Threading.Tasks;

namespace ExtractionRoom.Core
{
    public interface ISceneLoadingService
    {
        UniTask LoadSceneAsync(string sceneName, CancellationToken cancellationToken);
    }
}
