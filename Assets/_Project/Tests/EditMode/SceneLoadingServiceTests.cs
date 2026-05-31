using System;
using ExtractionRoom.Core;
using NUnit.Framework;

namespace ExtractionRoom.Tests.EditMode
{
    public sealed class SceneLoadingServiceTests
    {
        [Test]
        public void LoadSceneAsync_WithMissingSceneName_ThrowsArgumentException()
        {
            var service = new SceneLoadingService();

            Assert.ThrowsAsync<ArgumentException>(
                async () => await service.LoadSceneAsync(string.Empty, default));
        }
    }
}
