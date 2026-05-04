using System;

namespace Calluna
{
    public class ObservableListChangeDetector<TValue> : IDisposable
    {
        public event Action OnChanged;
        
        private readonly ReadonlyObservableList<TValue> _list;
        
        public ObservableListChangeDetector(ReadonlyObservableList<TValue> list)
        {
            _list = list;
            _list.OnItemAdded += OnItemChanged;
            _list.OnItemRemoved += OnItemChanged;
            _list.OnItemReplaced += OnItemReplaced;
            _list.OnItemsSwapped += OnItemsSwapped;
            _list.OnClean += RaiseOnChanged;
        }

        public void Dispose()
        {
            _list.OnItemAdded -= OnItemChanged;
            _list.OnItemRemoved -= OnItemChanged;
            _list.OnItemReplaced -= OnItemReplaced;
            _list.OnItemsSwapped -= OnItemsSwapped;
            _list.OnClean -= RaiseOnChanged;
        }

        private void RaiseOnChanged() => OnChanged?.Invoke();

        private void OnItemChanged(TValue item, int index) => OnChanged?.Invoke();

        private void OnItemReplaced(TValue nextItem, TValue formerItem, int index) => OnChanged?.Invoke();

        private void OnItemsSwapped(TValue newIndex1Item, int index1, TValue newIndex2Item, int index2) => OnChanged?.Invoke();
    }
}
