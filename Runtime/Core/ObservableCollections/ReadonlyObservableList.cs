using System;
using System.Collections.Generic;

namespace Calluna
{
    public interface ReadonlyObservableList<TValue> : IReadOnlyList<TValue>
    {
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
        /// <summary>Fired by <see cref="ObservableList{TValue}.OverrideWith"/> after all entries have been replaced in bulk. No per-item events are raised during that operation. Not fired by <see cref="ObservableList{TValue}.OverrideWithEvents"/>.</summary>
        [Obsolete("Use Subscribe(reset: ...) or SubscribeAny. Will be removed in 2.0.0.")]
        public event Action OnContentsReplaced;
        public int IndexOf(TValue item);

        /// <inheritdoc cref="ObservableList{TValue}.Subscribe(ItemHandler{TValue}, ItemHandler{TValue}, ReplaceHandler{TValue}, SwapHandler{TValue}, Action)"/>
        IDisposable Subscribe(ItemHandler<TValue> added = null, ItemHandler<TValue> removed = null,
            ReplaceHandler<TValue> replaced = null, SwapHandler<TValue> swapped = null, Action reset = null);

        /// <inheritdoc cref="ObservableList{TValue}.Subscribe(Action{ListChange{TValue}})"/>
        IDisposable Subscribe(Action<ListChange<TValue>> onChanged);

        /// <inheritdoc cref="ObservableList{TValue}.SubscribeAny"/>
        IDisposable SubscribeAny(Action onChanged);
    }
}
