using System;
using System.Collections.Generic;

namespace Calluna
{
    public interface ReadonlyObservableList<TValue> : IReadOnlyList<TValue>
    {
        public event ItemChangeEvent<TValue> OnItemAdded;
        public event ItemChangeEvent<TValue> OnItemRemoved;
        public event ItemReplaceEvent<TValue> OnItemReplaced;
        public event ItemSwapEvent<TValue> OnItemsSwapped;
        public int IndexOf(TValue item);
    }
}
