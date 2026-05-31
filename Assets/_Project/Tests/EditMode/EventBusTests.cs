using ExtractionRoom.Core;
using NUnit.Framework;

namespace ExtractionRoom.Tests.EditMode
{
    public sealed class EventBusTests
    {
        [Test]
        public void Publish_NotifiesMatchingSubscribers()
        {
            using var eventBus = new EventBus();
            var receivedValue = 0;

            using var subscription = eventBus.Subscribe<int>(value => receivedValue = value);

            eventBus.Publish(42);

            Assert.That(receivedValue, Is.EqualTo(42));
        }

        [Test]
        public void DisposedSubscription_StopsNotifications()
        {
            using var eventBus = new EventBus();
            var notificationCount = 0;
            var subscription = eventBus.Subscribe<int>(_ => notificationCount++);

            subscription.Dispose();
            eventBus.Publish(42);

            Assert.That(notificationCount, Is.Zero);
        }
    }
}
