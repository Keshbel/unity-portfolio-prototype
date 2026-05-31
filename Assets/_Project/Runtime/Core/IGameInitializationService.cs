using System.Threading;
using Cysharp.Threading.Tasks;

namespace ExtractionRoom.Core
{
    public interface IGameInitializationService
    {
        UniTask InitializeAsync(CancellationToken cancellationToken);
    }
}
