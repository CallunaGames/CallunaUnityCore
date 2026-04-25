using System;
using System.Collections.Generic;

namespace Calluna
{
    /// <summary>
    /// Typed publish/subscribe message bus with breadth-first, re-entrancy-safe dispatch.
    ///
    /// When <see cref="Publish{TEvent}"/> is called from inside a listener the new event is
    /// appended to the back of the internal queue rather than dispatched immediately.
    /// All pending events are processed before <see cref="Publish{TEvent}"/> returns to its
    /// original caller, so every reaction to a given event completes before any second-order
    /// effects begin.
    ///
    /// Allocation profile: zero allocations per <see cref="Publish{TEvent}"/> call for
    /// reference-type events after initial warmup. Value-type events incur one box per publish.
    /// A typed dispatch delegate is allocated once per event type on the first
    /// <see cref="Subscribe{TEvent}"/> call and reused thereafter.
    ///
    /// <b>Threading:</b> main thread only — no locking is applied.
    ///
    /// <b>Lifetime:</b> intended as a singleton bound at <c>AppContext</c> scope. Listeners
    /// must call <see cref="Unsubscribe{TEvent}"/> in <c>Cleanable.Clean()</c>; failing to do
    /// so prevents garbage collection of the subscriber for the lifetime of the bus.
    /// </summary>
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, Delegate>        _listeners   = new();
        private readonly Dictionary<Type, Action<object>>  _dispatchers = new();
        private readonly Queue<(Type type, object evt)>    _queue       = new();
        private bool _isFlushing;

        /// <summary>Publish <paramref name="evt"/> to all current listeners of <typeparamref name="TEvent"/>.</summary>
        public void Publish<TEvent>(TEvent evt)
        {
            _queue.Enqueue((typeof(TEvent), evt));
            if (!_isFlushing) Flush();
        }

        /// <summary>
        /// Register <paramref name="listener"/> to be called whenever <typeparamref name="TEvent"/> is published.
        /// Registering the same listener twice results in it being called twice per publish.
        /// </summary>
        public void Subscribe<TEvent>(Action<TEvent> listener)
        {
            Type key = typeof(TEvent);
            _listeners[key] = _listeners.TryGetValue(key, out Delegate existing)
                ? Delegate.Combine(existing, listener)
                : listener;
            if (!_dispatchers.ContainsKey(key))
                _dispatchers[key] = obj => InvokeListeners<TEvent>((TEvent)obj);
        }

        /// <summary>
        /// Remove a previously registered <paramref name="listener"/>. Safe to call if the
        /// listener was never subscribed.
        /// </summary>
        public void Unsubscribe<TEvent>(Action<TEvent> listener)
        {
            Type key = typeof(TEvent);
            if (!_listeners.TryGetValue(key, out Delegate existing)) return;
            Delegate remaining = Delegate.Remove(existing, listener);
            if (remaining == null)
                _listeners.Remove(key);
            else
                _listeners[key] = remaining;
        }

        private void InvokeListeners<TEvent>(TEvent evt)
        {
            if (!_listeners.TryGetValue(typeof(TEvent), out Delegate listener)) return;
            ((Action<TEvent>)listener)(evt);
        }

        private void Flush()
        {
            _isFlushing = true;
            try
            {
                while (_queue.Count > 0)
                {
                    (Type type, object evt) = _queue.Dequeue();
                    if (_dispatchers.TryGetValue(type, out Action<object> dispatch))
                        dispatch(evt);
                }
            }
            finally
            {
                _isFlushing = false;
            }
        }
    }
}
