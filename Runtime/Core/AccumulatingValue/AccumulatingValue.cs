using System;
using System.Collections.Generic;

namespace Calluna
{
    public abstract class AccumulatingValue<T>
    {
        public ReadonlyObservable<T> Value => _value;
        private readonly Dictionary<string, T> _idToValuePart = new Dictionary<string, T>();
        private readonly List<AccumulatingValuePart<T>> _addedParts = new List<AccumulatingValuePart<T>>();
        // Reused for every recalculation, so it doesn't allocate an enumerator over the parts.
        private readonly List<T> _parts = new List<T>();

        private readonly Observable<T> _value = new Observable<T>();

        /// <summary>
        /// Adds a part contributing <paramref name="value"/>. Keep the returned part to change its value
        /// later, and dispose it to remove it again.
        /// </summary>
        public AccumulatingValuePart<T> AddPart(T value = default)
        {
            AccumulatingValuePart<T> part = new AccumulatingValuePart<T>(this, value);
            _addedParts.Add(part);
            Recalculate();
            return part;
        }

        internal void OnPartChanged() => Recalculate();

        internal void RemovePart(AccumulatingValuePart<T> part)
        {
            if (_addedParts.Remove(part))
                Recalculate();
        }

        [Obsolete("Use AddPart and set the value of the returned part. Will be removed in 2.0.0.")]
        public void Set(string id, T value)
        {
            _idToValuePart[id] = value;
            Recalculate();
        }

        [Obsolete("Use AddPart. Will be removed in 2.0.0.")]
        public void Add(string id, T value)
        {
            if(!_idToValuePart.TryAdd(id, value))
                throw new ArgumentException($"Failed to add accumulating value. Id {id} already exists.");
            Recalculate();
        }

        [Obsolete("Dispose the part returned by AddPart. Will be removed in 2.0.0.")]
        public void Remove(string id)
        {
            if(!_idToValuePart.Remove(id))
                throw new ArgumentException($"Failed to remove accumulating value. Id {id} does not exist.");
            Recalculate();
        }

        [Obsolete("Read the value of the part returned by AddPart. Will be removed in 2.0.0.")]
        public bool TryGetValuePart(string id, out T value)
        {
            return _idToValuePart.TryGetValue(id, out value);
        }

        [Obsolete("Use the part returned by AddPart. Will be removed in 2.0.0.")]
        public T this[string id]
        {
            get => _idToValuePart[id];
#pragma warning disable CS0618
            set => Set(id, value);
#pragma warning restore CS0618
        }

        /// <summary>
        /// Updates <see cref="Value"/> from the current parts. Subclasses call this at the end of their
        /// constructor - once their own settings (e.g. a mode) are set - so the value without any
        /// parts matches the value after all parts were removed again.
        /// </summary>
        protected void Recalculate()
        {
            _parts.Clear();
            _parts.AddRange(_idToValuePart.Values);
            for (int i = 0; i < _addedParts.Count; i++)
                _parts.Add(_addedParts[i].Value);
            _value.Value = CalculateValue(_parts);
        }

        protected abstract T CalculateValue(IReadOnlyList<T> values);
    }
}
