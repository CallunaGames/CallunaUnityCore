using System;
using System.Collections;
using System.Collections.Generic;

namespace Calluna
{
    public class ObservableList<TValue> : IList<TValue>, ReadonlyObservableList<TValue>
    {
        public bool IsReadOnly => false;

#pragma warning disable CS0618 // The obsolete events use the obsolete delegate types.
        [Obsolete("Use Subscribe(added: ...). Will be removed in 2.0.0.")]
        public event ItemChangeEvent<TValue> OnItemAdded;
        [Obsolete("Use Subscribe(removed: ...). Will be removed in 2.0.0.")]
        public event ItemChangeEvent<TValue> OnItemRemoved;
        [Obsolete("Use Subscribe(replaced: ...). Will be removed in 2.0.0.")]
        public event ItemReplaceEvent<TValue> OnItemReplaced;
        [Obsolete("Use Subscribe(swapped: ...). Will be removed in 2.0.0.")]
        public event ItemSwapEvent<TValue> OnItemsSwapped;
#pragma warning restore CS0618
        [Obsolete("Use Subscribe(reset: ...) or SubscribeAny. Will be removed in 2.0.0.")]
        public event Action OnClean;
        [Obsolete("Use Subscribe(reset: ...) or SubscribeAny. Will be removed in 2.0.0.")]
        public event Action OnContentsReplaced;

        public int Count => _items.Count;
        int ICollection<TValue>.Count => _items.Count;
        int IReadOnlyCollection<TValue>.Count => _items.Count;

        public ObservableList(IEnumerable<TValue> values) => _items = new List<TValue>(values);
        public ObservableList() => _items = new List<TValue>();
        public ObservableList(int capacity) => _items = new List<TValue>(capacity);

        private readonly List<TValue> _items;
        // Replaced as a whole on (un)subscribe, so a notification iterates a snapshot that can't
        // change underneath it.
        private Sink[] _sinks = Array.Empty<Sink>();

        public IEnumerator<TValue> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Calls the given handlers for the kinds of changes they stand for; pass them by name:
        /// <code>list.Subscribe(added: OnAdded, removed: OnRemoved)</code>
        /// Without a <paramref name="reset"/> handler, a reset (<see cref="Clear"/>,
        /// <see cref="OverrideWith"/>) is delivered as the removal of every former item (from the last
        /// to the first) followed by the addition of every current item - so e.g. an added-only
        /// subscriber still sees the items added by <see cref="OverrideWith"/>. When those handlers
        /// run, the list already holds its new contents.
        /// </summary>
        /// <returns>A subscription that ends the notifications when disposed.</returns>
        public IDisposable Subscribe(ItemHandler<TValue> added = null, ItemHandler<TValue> removed = null,
            ReplaceHandler<TValue> replaced = null, SwapHandler<TValue> swapped = null, Action reset = null)
        {
            if (added == null && removed == null && replaced == null && swapped == null && reset == null)
                throw new ArgumentException("Subscribe needs at least one handler.");

            void OnChanged(ListChange<TValue> change)
            {
                switch (change.Kind)
                {
                    case ListChangeKind.Added: added?.Invoke(change.Item, change.Index); break;
                    case ListChangeKind.Removed: removed?.Invoke(change.Item, change.Index); break;
                    case ListChangeKind.Replaced: replaced?.Invoke(change.Item, change.FormerItem, change.Index); break;
                    case ListChangeKind.Swapped: swapped?.Invoke(change.Item, change.Index, change.OtherItem, change.OtherIndex); break;
                    case ListChangeKind.Reset: reset?.Invoke(); break;
                }
            }

            return AddSink(new Sink(OnChanged, expandsReset: reset == null));
        }

        /// <summary>
        /// Calls <paramref name="onChanged"/> with every change, a reset included as a single
        /// <see cref="ListChangeKind.Reset"/> change.
        /// </summary>
        /// <returns>A subscription that ends the notifications when disposed.</returns>
        public IDisposable Subscribe(Action<ListChange<TValue>> onChanged)
        {
            if (onChanged == null)
                throw new ArgumentNullException(nameof(onChanged));
            return AddSink(new Sink(onChanged, expandsReset: false));
        }

        /// <summary>Calls <paramref name="onChanged"/> once per change of any kind, a reset included.</summary>
        /// <returns>A subscription that ends the notifications when disposed.</returns>
        public IDisposable SubscribeAny(Action onChanged)
        {
            if (onChanged == null)
                throw new ArgumentNullException(nameof(onChanged));
            return AddSink(new Sink(_ => onChanged(), expandsReset: false));
        }

        public void Add(TValue item)
        {
            int index = _items.Count;
            _items.Add(item);
            RaiseAdded(item, index);
        }

        public void Clear()
        {
            List<TValue> formerItems = SnapshotForReset();
            _items.Clear();
#pragma warning disable CS0618
            OnClean?.Invoke();
#pragma warning restore CS0618
            NotifyReset(formerItems);
        }

        public bool Contains(TValue item) => _items.Contains(item);

        public void CopyTo(TValue[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

        public bool Remove(TValue item)
        {
            int index = _items.IndexOf(item);
            if (index >= 0)
            {
                _items.RemoveAt(index);
                RaiseRemoved(item, index);
                return true;
            }

            return false;
        }

        public int IndexOf(TValue item) => _items.IndexOf(item);

        public void Insert(int index, TValue item)
        {
            _items.Insert(index, item);
            RaiseAdded(item, index);
        }

        public void RemoveAt(int index)
        {
            TValue item = _items[index];
            _items.RemoveAt(index);
            RaiseRemoved(item, index);
        }

        public void Swap(int index1, int index2)
        {
            if (index1 >= _items.Count || index2 >= _items.Count)
                throw new ArgumentException("The provided indices need to be in range of the collection");

            TValue item1 = _items[index1];
            TValue item2 = _items[index2];
            _items[index1] = item2;
            _items[index2] = item1;
#pragma warning disable CS0618
            OnItemsSwapped?.Invoke(item2, index1, item1, index2);
#pragma warning restore CS0618
            Notify(ListChange<TValue>.Swapped(item2, index1, item1, index2));
        }

        public void OverrideWith(IEnumerable<TValue> items)
        {
            List<TValue> formerItems = SnapshotForReset();
            _items.Clear();
            _items.AddRange(items);
#pragma warning disable CS0618
            OnContentsReplaced?.Invoke();
#pragma warning restore CS0618
            NotifyReset(formerItems);
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
#pragma warning disable CS0618
            OnItemReplaced?.Invoke(item, formerItem, index);
#pragma warning restore CS0618
            Notify(ListChange<TValue>.Replaced(item, formerItem, index));
        }

        public TValue this[int index]
        {
            get => _items[index];
            set => Replace(value, index);
        }

        private void RaiseAdded(TValue item, int index)
        {
#pragma warning disable CS0618
            OnItemAdded?.Invoke(item, index);
#pragma warning restore CS0618
            Notify(ListChange<TValue>.Added(item, index));
        }

        private void RaiseRemoved(TValue item, int index)
        {
#pragma warning disable CS0618
            OnItemRemoved?.Invoke(item, index);
#pragma warning restore CS0618
            Notify(ListChange<TValue>.Removed(item, index));
        }

        private void Notify(ListChange<TValue> change)
        {
            foreach (Sink sink in _sinks)
                sink.OnChanged(change);
        }

        // The former items are only needed - and copied - if a subscriber receives resets as
        // single removals and additions.
        private List<TValue> SnapshotForReset()
        {
            foreach (Sink sink in _sinks)
            {
                if (sink.ExpandsReset)
                    return new List<TValue>(_items);
            }
            return null;
        }

        private void NotifyReset(List<TValue> formerItems)
        {
            ListChange<TValue> reset = ListChange<TValue>.Reset();
            foreach (Sink sink in _sinks)
            {
                if (!sink.ExpandsReset)
                {
                    sink.OnChanged(reset);
                    continue;
                }
                for (int i = formerItems.Count - 1; i >= 0; i--)
                    sink.OnChanged(ListChange<TValue>.Removed(formerItems[i], i));
                for (int i = 0; i < _items.Count; i++)
                    sink.OnChanged(ListChange<TValue>.Added(_items[i], i));
            }
        }

        private IDisposable AddSink(Sink sink)
        {
            Sink[] next = new Sink[_sinks.Length + 1];
            Array.Copy(_sinks, next, _sinks.Length);
            next[_sinks.Length] = sink;
            _sinks = next;
            return new Subscription(() => RemoveSink(sink));
        }

        private void RemoveSink(Sink sink)
        {
            int index = Array.IndexOf(_sinks, sink);
            if (index < 0)
                return;
            Sink[] next = new Sink[_sinks.Length - 1];
            Array.Copy(_sinks, 0, next, 0, index);
            Array.Copy(_sinks, index + 1, next, index, _sinks.Length - index - 1);
            _sinks = next;
        }

        private sealed class Sink
        {
            public readonly Action<ListChange<TValue>> OnChanged;
            public readonly bool ExpandsReset;

            public Sink(Action<ListChange<TValue>> onChanged, bool expandsReset)
            {
                OnChanged = onChanged;
                ExpandsReset = expandsReset;
            }
        }
    }
}
