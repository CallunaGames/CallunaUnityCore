using System;
using System.Collections.Generic;

namespace Calluna
{
    public abstract class AccumulatingValue<T>
    {
        public ReadonlyObservable<T> Value => _value;
        private readonly Dictionary<string, T> _idToValuePart = new Dictionary<string, T>();
        // Reused for every recalculation, so it doesn't allocate an enumerator over the parts.
        private readonly List<T> _parts = new List<T>();

        private readonly Observable<T> _value = new Observable<T>();

        public void Set(string id, T value)
        {
            _idToValuePart[id] = value;
            Recalculate();
        }

        public void Add(string id, T value)
        {
            if(!_idToValuePart.TryAdd(id, value))
                throw new ArgumentException($"Failed to add accumulating value. Id {id} already exists.");
            Recalculate();
        }

        public void Remove(string id)
        {
            if(!_idToValuePart.Remove(id))
                throw new ArgumentException($"Failed to remove accumulating value. Id {id} does not exist.");
            Recalculate();
        }

        public bool TryGetValuePart(string id, out T value)
        {
            return _idToValuePart.TryGetValue(id, out value);
        }

        public T this[string id]
        {
            get => _idToValuePart[id];
            set => Set(id, value);
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
            _value.Value = CalculateValue(_parts);
        }

        protected abstract T CalculateValue(IReadOnlyList<T> values);
    }
}
