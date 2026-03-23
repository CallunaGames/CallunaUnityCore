using System;
using System.Collections;
using System.Collections.Generic;

namespace Calluna
{
    public class ObservableList<TValue> : IList<TValue>, ReadonlyObservableList<TValue>
    {
        public bool IsReadOnly => false;

        public event ItemChangeEvent<TValue> OnItemAdded;
        public event ItemChangeEvent<TValue> OnItemRemoved;
        public event ItemReplaceEvent<TValue> OnItemReplaced;
        public event ItemSwapEvent<TValue> OnItemsSwapped;

        public int Count => _items.Count;
        int ICollection<TValue>.Count => Count;
        int IReadOnlyCollection<TValue>.Count => Count;

        public ObservableList(IEnumerable<TValue> values) => _items = new List<TValue>(values);
        public ObservableList() => _items = new List<TValue>();
        public ObservableList(int capacity) => _items = new List<TValue>(capacity);

        private readonly List<TValue> _items;

        public IEnumerator<TValue> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TValue item)
        {
            int index = _items.Count;
            _items.Add(item);
            OnItemAdded?.Invoke(item, index);
        }

        public void Clear()
        {
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                TValue value = _items[i];
                _items.RemoveAt(i);
                OnItemRemoved?.Invoke(value, i);
            }
        }

        public bool Contains(TValue item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public bool Remove(TValue item)
        {
            int index = _items.IndexOf(item);
            if (index >= 0)
            {
                _items.RemoveAt(index);
                OnItemRemoved?.Invoke(item, index);
                return true;
            }

            return false;
        }

        public int IndexOf(TValue item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, TValue item)
        {
            _items.Insert(index, item);
            OnItemAdded?.Invoke(item, index);
        }

        public void RemoveAt(int index)
        {
            TValue value = _items[index];
            _items.RemoveAt(index);
            OnItemRemoved?.Invoke(value, index);
        }

        public void Swap(int index1, int index2)
        {
            if (index1 >= _items.Count || index2 >= _items.Count)
                throw new ArgumentException("The provided indices need to be in range of the collection");
            
            TValue item1 = _items[index1];
            TValue item2 = _items[index2];
            _items[index1] = item2;
            _items[index2] = item1;
            OnItemsSwapped?.Invoke(item2, index1, item1, index2);
        }
        
        public void OverrideWith(IEnumerable<TValue> items)
        {
            using IEnumerator<TValue> e = items.GetEnumerator();

            int i = 0;
            int count = _items.Count;

            while (i < count && e.MoveNext())
            {
                if (!_items[i].Equals(e.Current))
                    Replace(e.Current, i);
                i++;
            }

            while (_items.Count > i)
            {
                RemoveAt(i);
            }

            while (e.MoveNext())
            {
                Add(e.Current);
            }
        }

        private void Replace(TValue item, int index)
        {
            TValue formerItem = _items[index];
            _items[index] = item;
            OnItemReplaced?.Invoke(item, formerItem, index);
        }

        public TValue this[int index]
        {
            get => _items[index];
            set => Replace(value, index);
        }
    }
}