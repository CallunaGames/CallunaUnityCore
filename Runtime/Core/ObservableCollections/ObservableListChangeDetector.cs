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
        }

        public void Dispose()
        {
            _list.OnItemAdded -= OnItemChanged;
            _list.OnItemRemoved -= OnItemChanged;
            _list.OnItemReplaced -= OnItemReplaced;
        }

        private void OnItemChanged(TValue item, int index)
        {
            InvokeOnChanged();
        }

        private void OnItemReplaced(TValue nextitem, TValue formeritem, int index)
        {
            InvokeOnChanged();
        }

        private void InvokeOnChanged()
        {
            OnChanged?.Invoke();
        }
    }
}
