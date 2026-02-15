using System;

namespace Calluna
{
    public class ObservableListChangeDetector<TValue> : IDisposable
    {
        public event Action OnChanged;
        
        private ReadonlyObservableList<TValue> _list;
        
        public ObservableListChangeDetector(ReadonlyObservableList<TValue> list)
        {
            _list = list;
            _list.OnItemAdded += OnItemChanged;
            _list.OnItemRemoved += OnItemChanged;
            _list.OnItemReplaced += OnItemReplaced;
            _list.OnItemsSwapped += OnItemsSwapped;
        }

        public void Dispose()
        {
            _list.OnItemAdded -= OnItemChanged;
            _list.OnItemRemoved -= OnItemChanged;
            _list.OnItemReplaced -= OnItemReplaced;
            _list.OnItemsSwapped -= OnItemsSwapped;
        }

        private void OnItemChanged(TValue item, int index)
        {
            InvokeOnChanged();
        }

        private void OnItemReplaced(TValue nextitem, TValue formeritem, int index)
        {
            InvokeOnChanged();
        }

        private void OnItemsSwapped(TValue newIndex1Item, int index1, TValue item2, int newIndex2Item)
        {
            InvokeOnChanged();
        }

        private void InvokeOnChanged()
        {
            OnChanged?.Invoke();
        }
    }
}
