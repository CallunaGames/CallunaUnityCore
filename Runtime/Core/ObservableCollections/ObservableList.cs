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
        public event Action OnContentsReplaced;

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
        
        public void OverrideWith(IEnumerable<TValue> items)
        {
            _items.Clear();
            _items.AddRange(items);
            OnContentsReplaced?.Invoke();
        }

        // Fires per-item events (OnItemsSwapped, OnItemReplaced, OnItemAdded, OnItemRemoved)
        // for each change. Use Swap when the desired item already exists at a later index
        // (same item moved), Replace when the slot truly changes content, Add/RemoveAt for
        // items entering/leaving the list. OnContentsReplaced is NOT fired.
        public void OverrideWithEvents(IEnumerable<TValue> items)
        {
            var newItems = new List<TValue>(items);
            var comparer = EqualityComparer<TValue>.Default;

            int i = 0;
            while (i < newItems.Count)
            {
                if (i < _items.Count)
                {
                    if (!comparer.Equals(_items[i], newItems[i]))
                    {
                        int swapIdx = _items.IndexOf(newItems[i], i + 1);
                        if (swapIdx >= 0)
                            Swap(i, swapIdx);
                        else
                            Replace(newItems[i], i);
                    }
                }
                else
                {
                    Add(newItems[i]);
                }
                i++;
            }

            while (_items.Count > newItems.Count)
                RemoveAt(_items.Count - 1);
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