using System;
using System.Collections.Generic;
using Calluna;

namespace Calluna
{
    public abstract class AccumulatingValue<T>
    {
        public ReadonlyObservable<T> Value => _value;
        private Dictionary<string, T> _idToValuePart = new Dictionary<string, T>();

        private Observable<T> _value = new Observable<T>();

        public void Set(string id, T value)
        {
            _idToValuePart[id] = value;
            _value.Value = CalculateValue(_idToValuePart.Values);
        }
        
        public void Add(string id, T value)
        {
            if(!_idToValuePart.TryAdd(id, value))
                throw new ArgumentException($"Failed to add accumulating value. Id {id} already exists.");
            _value.Value = CalculateValue(_idToValuePart.Values);
        }

        public void Remove(string id)
        {
            if(!_idToValuePart.Remove(id))
                throw new ArgumentException($"Failed to remove accumulating value. Id {id} does not exist.");
            _value.Value = CalculateValue(_idToValuePart.Values);
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

        protected abstract T CalculateValue(IEnumerable<T> values);
    }
}
