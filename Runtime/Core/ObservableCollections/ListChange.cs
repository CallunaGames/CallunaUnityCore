namespace Calluna
{
    public enum ListChangeKind
    {
        Added,
        Removed,
        Replaced,
        Swapped,
        /// <summary>The whole content changed at once (<c>Clear</c>, <c>OverrideWith</c>) - read the list itself.</summary>
        Reset
    }

    /// <summary>One change of an <see cref="ObservableList{TValue}"/>, delivered by its Subscribe methods.</summary>
    public readonly struct ListChange<T>
    {
        public ListChangeKind Kind { get; }
        /// <summary>Added, Removed, Replaced: the item's index. Swapped: the first index.</summary>
        public int Index { get; }
        /// <summary>Added: the added item. Removed: the removed item. Replaced: the new item. Swapped: the item now at <see cref="Index"/>.</summary>
        public T Item { get; }
        /// <summary>Replaced: the item that was at <see cref="Index"/> before.</summary>
        public T FormerItem { get; }
        /// <summary>Swapped: the second index.</summary>
        public int OtherIndex { get; }
        /// <summary>Swapped: the item now at <see cref="OtherIndex"/>.</summary>
        public T OtherItem { get; }

        private ListChange(ListChangeKind kind, int index, T item, T formerItem = default, int otherIndex = -1, T otherItem = default)
        {
            Kind = kind;
            Index = index;
            Item = item;
            FormerItem = formerItem;
            OtherIndex = otherIndex;
            OtherItem = otherItem;
        }

        internal static ListChange<T> Added(T item, int index) => new ListChange<T>(ListChangeKind.Added, index, item);
        internal static ListChange<T> Removed(T item, int index) => new ListChange<T>(ListChangeKind.Removed, index, item);
        internal static ListChange<T> Replaced(T item, T formerItem, int index) =>
            new ListChange<T>(ListChangeKind.Replaced, index, item, formerItem);
        internal static ListChange<T> Swapped(T item, int index, T otherItem, int otherIndex) =>
            new ListChange<T>(ListChangeKind.Swapped, index, item, default, otherIndex, otherItem);
        internal static ListChange<T> Reset() => new ListChange<T>(ListChangeKind.Reset, -1, default);

        public override string ToString() => Kind switch
        {
            ListChangeKind.Replaced => $"Replaced [{Index}] {FormerItem} -> {Item}",
            ListChangeKind.Swapped => $"Swapped [{Index}] {Item} <-> [{OtherIndex}] {OtherItem}",
            ListChangeKind.Reset => "Reset",
            _ => $"{Kind} [{Index}] {Item}"
        };
    }
}
