using System;
using System.Collections.Generic;
using UnityEngine;

namespace Calluna
{
    /// <summary>
    /// Collects subscriptions (or any <see cref="IDisposable"/>) so they can be ended together:
    /// <code>
    /// _subscriptions.Add(_eventBus.Subscribe&lt;ShelvesChangedEvent&gt;(OnShelvesChanged));
    /// _subscriptions.Add(_state.Subscribe(OnStateChanged));
    /// ...
    /// _subscriptions.Dispose(); // e.g. in Cleanable.Clean()
    /// </code>
    /// Disposing ends the collected subscriptions in reverse order and empties the bag, which can then
    /// be filled again - e.g. by a pooled object on its next Initialize. A subscription throwing while
    /// being disposed is logged and doesn't stop the others.
    /// </summary>
    public sealed class SubscriptionBag : IDisposable
    {
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();

        public int Count => _subscriptions.Count;

        /// <returns>The added subscription, to dispose it individually as well if needed.</returns>
        public IDisposable Add(IDisposable subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));
            _subscriptions.Add(subscription);
            return subscription;
        }

        public void Dispose()
        {
            for (int i = _subscriptions.Count - 1; i >= 0; i--)
            {
                try
                {
                    _subscriptions[i].Dispose();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            _subscriptions.Clear();
        }
    }
}
