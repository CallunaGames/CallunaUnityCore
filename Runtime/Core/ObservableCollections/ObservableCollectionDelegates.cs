namespace Calluna
{
    public delegate void ItemChangeEvent<in TValue>(TValue item, int index);
    public delegate void ItemReplaceEvent<in TValue>(TValue nextItem, TValue formerItem, int index);
}
