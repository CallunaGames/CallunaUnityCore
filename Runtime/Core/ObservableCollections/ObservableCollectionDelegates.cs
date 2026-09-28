using System;

namespace Calluna
{
    public delegate void ItemHandler<in TValue>(TValue item, int index);
    public delegate void ReplaceHandler<in TValue>(TValue newItem, TValue formerItem, int index);
    public delegate void SwapHandler<in TValue>(TValue newIndex1Item, int index1, TValue newIndex2Item, int index2);

    [Obsolete("Belongs to the obsolete ObservableList events - use ItemHandler with ObservableList.Subscribe. Will be removed in 2.0.0.")]
    public delegate void ItemChangeEvent<in TValue>(TValue item, int index);
    [Obsolete("Belongs to the obsolete ObservableList events - use ReplaceHandler with ObservableList.Subscribe. Will be removed in 2.0.0.")]
    public delegate void ItemReplaceEvent<in TValue>(TValue newItem, TValue formerItem, int index);
    [Obsolete("Belongs to the obsolete ObservableList events - use SwapHandler with ObservableList.Subscribe. Will be removed in 2.0.0.")]
    public delegate void ItemSwapEvent<in TValue>(TValue newIndex1Item, int index1, TValue newIndex2Item, int index2);
}
