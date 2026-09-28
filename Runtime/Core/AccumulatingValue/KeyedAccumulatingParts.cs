using System;
using System.Collections.Generic;

namespace Calluna
{
    /// <summary>
    /// Parts of an <see cref="AccumulatingValue{T}"/> kept by key, for contributors identified by data
    /// rather than being objects that keep their own <see cref="AccumulatingValuePart{T}"/> - e.g.
    /// requester names passed to a <c>Show(string requesterId)</c> method, ids authored in assets, or
    /// pooled views. Prefer an own part wherever the contributor is an object: only the owner of this
    /// collection knows its keys.
    /// <code>
    /// private readonly AccumulatingBoolValue _requests = new AccumulatingBoolValue(AccumulatingBoolValue.Mode.Any);
    /// private readonly KeyedAccumulatingParts&lt;string, bool&gt; _requesters;
    ///
    /// public Blocker() => _requesters = new KeyedAccumulatingParts&lt;string, bool&gt;(_requests);
    /// public void Show(string requesterId) => _requesters.Set(requesterId, true);
    /// public void Hide(string requesterId) => _requesters.Clear(requesterId);
    /// </code>
    /// A key's part is created on its first <see cref="Set"/> and kept afterwards; disposing removes all parts.
    /// </summary>
    public sealed class KeyedAccumulatingParts<TKey, T> : IDisposable
    {
        private readonly AccumulatingValue<T> _value;
        private readonly Dictionary<TKey, AccumulatingValuePart<T>> _parts = new Dictionary<TKey, AccumulatingValuePart<T>>();

        public KeyedAccumulatingParts(AccumulatingValue<T> value)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>Sets the part of <paramref name="key"/> to <paramref name="value"/>.</summary>
        public void Set(TKey key, T value)
        {
            if (!_parts.TryGetValue(key, out AccumulatingValuePart<T> part))
            {
                part = _value.AddPart();
                _parts[key] = part;
            }
            part.Value = value;
        }

        /// <summary>Withdraws the contribution of <paramref name="key"/>. Does nothing if it has none.</summary>
        public void Clear(TKey key)
        {
            if (_parts.TryGetValue(key, out AccumulatingValuePart<T> part))
                part.Clear();
        }

        /// <summary>Withdraws the contributions of all keys.</summary>
        public void ClearAll()
        {
            foreach (AccumulatingValuePart<T> part in _parts.Values)
                part.Clear();
        }

        /// <returns>Whether <paramref name="key"/> contributes, and if so, its value.</returns>
        public bool TryGet(TKey key, out T value)
        {
            if (_parts.TryGetValue(key, out AccumulatingValuePart<T> part) && part.IsSet)
            {
                value = part.Value;
                return true;
            }
            value = default;
            return false;
        }

        public void Dispose()
        {
            foreach (AccumulatingValuePart<T> part in _parts.Values)
                part.Dispose();
            _parts.Clear();
        }
    }
}
