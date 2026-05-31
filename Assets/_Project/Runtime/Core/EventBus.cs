using System;
using System.Collections.Generic;
using R3;

namespace ExtractionRoom.Core
{
    public sealed class EventBus : IEventBus, IDisposable
    {
        private readonly Dictionary<Type, object> subjects = new();
        private bool isDisposed;

        public void Publish<TEvent>(TEvent eventData)
        {
            ThrowIfDisposed();

            if (subjects.TryGetValue(typeof(TEvent), out var subject))
            {
                ((Subject<TEvent>)subject).OnNext(eventData);
            }
        }

        public IDisposable Subscribe<TEvent>(Action<TEvent> handler)
        {
            ThrowIfDisposed();

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            return GetOrCreateSubject<TEvent>().Subscribe(handler);
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            foreach (var subject in subjects.Values)
            {
                ((IDisposable)subject).Dispose();
            }

            subjects.Clear();
            isDisposed = true;
        }

        private Subject<TEvent> GetOrCreateSubject<TEvent>()
        {
            if (subjects.TryGetValue(typeof(TEvent), out var subject))
            {
                return (Subject<TEvent>)subject;
            }

            var newSubject = new Subject<TEvent>();
            subjects.Add(typeof(TEvent), newSubject);
            return newSubject;
        }

        private void ThrowIfDisposed()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(EventBus));
            }
        }
    }
}
