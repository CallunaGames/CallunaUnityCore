using System;

namespace Calluna
{
    public interface IEventBus
    {
        void Publish<TEvent>(TEvent evt);

        /// <returns>A subscription that unsubscribes <paramref name="listener"/> when disposed.</returns>
        IDisposable Subscribe<TEvent>(Action<TEvent> listener);

        void Unsubscribe<TEvent>(Action<TEvent> listener);
    }
}
