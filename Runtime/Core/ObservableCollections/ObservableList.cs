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
        public event Action OnClean;

        public int Count => _items.Count;
        int ICollection<TValue>.Count => _items.Count;
        int IReadOnlyCollection<TValue>.Count => _items.Count;

        public ObservableList(IEnumerable<TValue> values) => _items = new List<TValue>(values);
        public ObservableList() => _items = new List<TValue>();
        public ObservableList(int capacity) => _items = new List<TValue>(capacity);

        private readonly List<TValue> _items;

        public IEnumerator<TValue> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(TValue item)
        {
            int index = _items.Count;
            _items.Add(item);
            OnItemAdded?.Invoke(item, index);
        }

        public void Clear()
        {
            _items.Clear();
            OnClean?.Invoke();
        }

        public bool Contains(TValue item) => _items.Contains(item);

        public void CopyTo(TValue[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

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

        public int IndexOf(TValue item) => _items.IndexOf(item);

        public void Insert(int index, TValue item)
        {
            _items.Insert(index, item);
            OnItemAdded?.Invoke(item, index);
        }

        public void RemoveAt(int index)
        {
            TValue item = _items[index];
            _items.RemoveAt(index);
            OnItemRemoved?.Invoke(item, index);
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
        
        // Walk both sequences in lock-step: replace overlapping positions, then
        // remove any surplus items from the tail, then append any remaining new items.
        public void OverrideWith(IEnumerable<TValue> items)
        {
            using IEnumerator<TValue> e = items.GetEnumerator();

            int i = 0;
            int count = _items.Count;

            while (i < count && e.MoveNext())
            {
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