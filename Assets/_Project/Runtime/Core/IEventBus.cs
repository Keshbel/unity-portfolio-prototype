using System;

namespace ExtractionRoom.Core
{
    public interface IEventBus
    {
        void Publish<TEvent>(TEvent eventData);

        IDisposable Subscribe<TEvent>(Action<TEvent> handler);
    }
}
