namespace Calluna
{
    public delegate void ItemChangeEvent<in TValue>(TValue item, int index);
    public delegate void ItemReplaceEvent<in TValue>(TValue nextItem, TValue formerItem, int index);
    public delegate void ItemSwapEvent<in TValue>(TValue newIndex1Item, int index1, TValue newIndex2Item, int index2);
}
