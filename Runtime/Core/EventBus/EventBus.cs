using System;
using System.Collections.Generic;
using UnityEngine;

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
    /// <b>Exceptions:</b> a listener throwing is logged via <see cref="Debug.LogException(Exception)"/>
    /// and does not stop the remaining listeners of that event, nor the delivery of queued events.
    /// Listeners are expected to be independent of each other.
    ///
    /// <b>Listener changes during dispatch:</b> listeners subscribed or unsubscribed while an event
    /// is being delivered take effect from the next delivered event on.
    ///
    /// Allocation profile: zero allocations per <see cref="Publish{TEvent}"/> call for
    /// reference-type events after initial warmup. Value-type events incur one box per publish.
    /// Subscribing and unsubscribing allocate a new listener array for the event type.
    ///
    /// <b>Threading:</b> main thread only — no locking is applied.
    ///
    /// <b>Lifetime:</b> intended as a singleton bound at <c>AppContext</c> scope. Listeners
    /// must call <see cref="Unsubscribe{TEvent}"/> in <c>Cleanable.Clean()</c>; failing to do
    /// so prevents garbage collection of the subscriber for the lifetime of the bus.
    /// </summary>
    public class EventBus : IEventBus
    {
        // Per event type an Action<TEvent>[], replaced as a whole on every (un)subscribe - so a
        // dispatch iterates a snapshot that can't change underneath it.
        private readonly Dictionary<Type, object>          _listeners   = new();
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
            Action<TEvent>[] current = GetListeners<TEvent>();
            Action<TEvent>[] next = new Action<TEvent>[current.Length + 1];
            Array.Copy(current, next, current.Length);
            next[current.Length] = listener;
            _listeners[key] = next;
            _dispatchers.TryAdd(key, obj => InvokeListeners((TEvent)obj));
        }

        /// <summary>
        /// Remove a previously registered <paramref name="listener"/> - its most recent registration,
        /// if it was registered more than once. Safe to call if the listener was never subscribed.
        /// </summary>
        public void Unsubscribe<TEvent>(Action<TEvent> listener)
        {
            Action<TEvent>[] current = GetListeners<TEvent>();
            int index = Array.LastIndexOf(current, listener);
            if (index < 0) return;

            Type key = typeof(TEvent);
            if (current.Length == 1)
            {
                _listeners.Remove(key);
                return;
            }

            Action<TEvent>[] next = new Action<TEvent>[current.Length - 1];
            Array.Copy(current, 0, next, 0, index);
            Array.Copy(current, index + 1, next, index, current.Length - index - 1);
            _listeners[key] = next;
        }

        private Action<TEvent>[] GetListeners<TEvent>() =>
            _listeners.TryGetValue(typeof(TEvent), out object listeners)
                ? (Action<TEvent>[])listeners
                : Array.Empty<Action<TEvent>>();

        private void InvokeListeners<TEvent>(TEvent evt)
        {
            foreach (Action<TEvent> listener in GetListeners<TEvent>())
            {
                try
                {
                    listener(evt);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
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
