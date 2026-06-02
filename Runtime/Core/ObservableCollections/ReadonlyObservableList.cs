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
        public event Action OnClean;
        /// <summary>Fired by <see cref="ObservableList{TValue}.OverrideWith"/> after all entries have been replaced in bulk. No per-item events are raised during that operation. Not fired by <see cref="ObservableList{TValue}.OverrideWithEvents"/>.</summary>
        public event Action OnContentsReplaced;
        public int IndexOf(TValue item);
    }
}
