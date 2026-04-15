using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Calluna.Core.Tests
{
    public class ObservableListTests
    {
        private static List<object> _testValues = new List<object>()
        {
            new TestValues<float>() { ItemA = 1.0f, ItemB = 2.5f },
            new TestValues<string>() { ItemA = "Alpha", ItemB = "Beta" },
            new TestValues<int>() { ItemA = 10, ItemB = 99 },
            new TestValues<Foo>() { ItemA = new Foo() { Value = 1 }, ItemB = new Foo() { Value = 2 } },
            new TestValues<char>() { ItemA = 'a', ItemB = 'z' },
        };

        [Test, Description("Add item => OnItemAdded fires with correct item and index?")]
        public void ObservableList_Add_OnItemAddedFiredWithCorrectArgs<T>(
            [ValueSource(nameof(_testValues))] TestValues<T> values)
        {
            var list = new ObservableList<T>();
            T receivedItem = default;
            int receivedIndex = -1;

            ItemChangeEvent<T> listener = (item, index) => { receivedItem = item; receivedIndex = index; };
            list.OnItemAdded += listener;
            list.Add(values.ItemA);
            list.OnItemAdded -= listener;

            Assert.AreEqual(values.ItemA, receivedItem);
            Assert.AreEqual(0, receivedIndex);
        }

        [Test, Description("Add item => Count increases?")]
        public void ObservableList_Add_CountIncreases<T>(
            [ValueSource(nameof(_testValues))] TestValues<T> values)
        {
            var list = new ObservableList<T>();
            list.Add(values.ItemA);
            list.Add(values.ItemB);
            Assert.AreEqual(2, list.Count);
        }

        [Test, Description("Remove existing item => OnItemRemoved fires with correct item and index?")]
        public void ObservableList_Remove_OnItemRemovedFiredWithCorrectArgs<T>(
            [ValueSource(nameof(_testValues))] TestValues<T> values)
        {
            var list = new ObservableList<T>();
            list.Add(values.ItemA);

            T receivedItem = default;
            int receivedIndex = -1;
            ItemChangeEvent<T> listener = (item, index) => { receivedItem = item; receivedIndex = index; };
            list.OnItemRemoved += listener;
            list.Remove(values.ItemA);
            list.OnItemRemoved -= listener;

            Assert.AreEqual(values.ItemA, receivedItem);
            Assert.AreEqual(0, receivedIndex);
        }

        [Test, Description("Remove existing item => Returns true?")]
        public void ObservableList_Remove_ReturnsTrueForExistingItem<T>(
            [ValueSource(nameof(_testValues))] TestValues<T> values)
        {
            var list = new ObservableList<T>();
            list.Add(values.ItemA);
            bool result = list.Remove(values.ItemA);
            Assert.IsTrue(result);
        }

        [Test, Description("Remove missing item => Returns false and no event fired?")]
        public void ObservableList_Remove_ReturnsFalseAndNoEventForMissingItem<T>(
            [ValueSource(nameof(_testValues))] TestValues<T> values)
        {
            var list = new ObservableList<T>();
            bool eventFired = false;
            ItemChangeEvent<T> listener = (item, index) => { eventFired = true; };
            list.OnItemRemoved += listener;
            bool result = list.Remove(values.ItemA);
            list.OnItemRemoved -= listener;

            Assert.IsFalse(result);
            Assert.IsFalse(eventFired);
        }

        [Test, Description("Set indexer => OnItemReplaced fires with correct former and new item?")]
        public void ObservableList_IndexerSet_OnItemReplacedFiredWithCorrectArgs<T>(
            [ValueSource(nameof(_testValues))] TestValues<T> values)
        {
            var list = new ObservableList<T>();
            list.Add(values.ItemA);

            T receivedNew = default;
            T receivedFormer = default;
            ItemReplaceEvent<T> listener = (newItem, formerItem, index) =>
            {
                receivedNew = newItem;
                receivedFormer = formerItem;
            };
            list.OnItemReplaced += listener;
            list[0] = values.ItemB;
            list.OnItemReplaced -= listener;

            Assert.AreEqual(values.ItemB, receivedNew);
            Assert.AreEqual(values.ItemA, receivedFormer);
        }

        [Test, Description("Set indexer => Value is updated at correct index?")]
        public void ObservableList_IndexerSet_ValueUpdated<T>(
            [ValueSource(nameof(_testValues))] TestValues<T> values)
        {
            var list = new ObservableList<T>();
            list.Add(values.ItemA);
            list[0] = values.ItemB;
            Assert.AreEqual(values.ItemB, list[0]);
        }

        [Test, Description("Swap => OnItemsSwapped fires with correct items and indices?")]
        public void ObservableList_Swap_OnItemsSwappedFiredWithCorrectArgs<T>(
            [ValueSource(nameof(_testValues))] TestValues<T> values)
        {
            var list = new ObservableList<T>();
            list.Add(values.ItemA);
            list.Add(values.ItemB);

            T receivedAt0 = default;
            T receivedAt1 = default;
            ItemSwapEvent<T> listener = (newIndex1Item, index1, newIndex2Item, index2) =>
            {
                receivedAt0 = newIndex1Item;
                receivedAt1 = newIndex2Item;
            };
            list.OnItemsSwapped += listener;
            list.Swap(0, 1);
            list.OnItemsSwapped -= listener;

            Assert.AreEqual(values.ItemB, receivedAt0);
            Assert.AreEqual(values.ItemA, receivedAt1);
        }

        [Test, Description("Swap => Items are at swapped positions in the list?")]
        public void ObservableList_Swap_ItemsAtCorrectPositions<T>(
            [ValueSource(nameof(_testValues))] TestValues<T> values)
        {
            var list = new ObservableList<T>();
            list.Add(values.ItemA);
            list.Add(values.ItemB);
            list.Swap(0, 1);

            Assert.AreEqual(values.ItemB, list[0]);
            Assert.AreEqual(values.ItemA, list[1]);
        }

        [Test, Description("Clear => Count becomes 0 and OnClean fires once?")]
        public void ObservableList_Clear_CountBecomesZeroAndOnCleanFires<T>(
            [ValueSource(nameof(_testValues))] TestValues<T> values)
        {
            var list = new ObservableList<T>();
            list.Add(values.ItemA);
            list.Add(values.ItemB);

            int cleanCount = 0;
            System.Action listener = () => { cleanCount++; };
            list.OnClean += listener;
            list.Clear();
            list.OnClean -= listener;

            Assert.AreEqual(0, list.Count);
            Assert.AreEqual(1, cleanCount);
        }

        [Test, Description("Clear => OnItemRemoved does not fire?")]
        public void ObservableList_Clear_OnItemRemovedNotFired<T>(
            [ValueSource(nameof(_testValues))] TestValues<T> values)
        {
            var list = new ObservableList<T>();
            list.Add(values.ItemA);
            list.Add(values.ItemB);

            bool removedFired = false;
            ItemChangeEvent<T> listener = (item, index) => { removedFired = true; };
            list.OnItemRemoved += listener;
            list.Clear();
            list.OnItemRemoved -= listener;

            Assert.IsFalse(removedFired);
        }

        [Test, Description("Constructor(IEnumerable<T>) => Count and contents correct?")]
        public void ObservableList_ConstructorFromEnumerable_CountAndContentsCorrect<T>(
            [ValueSource(nameof(_testValues))] TestValues<T> values)
        {
            var source = new System.Collections.Generic.List<T> { values.ItemA, values.ItemB };
            var list = new ObservableList<T>(source);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(values.ItemA, list[0]);
            Assert.AreEqual(values.ItemB, list[1]);
        }

        [Test, Description("Constructor(int capacity) => Empty list with Count 0?")]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(10)]
        public void ObservableList_ConstructorWithCapacity_EmptyList(int capacity)
        {
            var list = new ObservableList<int>(capacity);
            Assert.AreEqual(0, list.Count);
        }

        [Test, Description("Contains existing item => Returns true?")]
        public void ObservableList_Contains_ExistingItem_ReturnsTrue<T>(
            [ValueSource(nameof(_testValues))] TestValues<T> values)
        {
            var list = new ObservableList<T>();
            list.Add(values.ItemA);
            Assert.IsTrue(list.Contains(values.ItemA));
        }

        [Test, Description("Contains missing item => Returns false?")]
        public void ObservableList_Contains_MissingItem_ReturnsFalse<T>(
            [ValueSource(nameof(_testValues))] TestValues<T> values)
        {
            var list = new ObservableList<T>();
            list.Add(values.ItemA);
            Assert.IsFalse(list.Contains(values.ItemB));
        }

        [Test, Description("CopyTo => Array contains items at correct offset?")]
        public void ObservableList_CopyTo_CopiesItemsToArray<T>(
            [ValueSource(nameof(_testValues))] TestValues<T> values)
        {
            var list = new ObservableList<T>();
            list.Add(values.ItemA);
            list.Add(values.ItemB);
            var array = new T[2];
            list.CopyTo(array, 0);
            Assert.AreEqual(values.ItemA, array[0]);
            Assert.AreEqual(values.ItemB, array[1]);
        }

        [Test, Description("IndexOf existing item => Returns correct index?")]
        public void ObservableList_IndexOf_ExistingItem_ReturnsCorrectIndex<T>(
            [ValueSource(nameof(_testValues))] TestValues<T> values)
        {
            var list = new ObservableList<T>();
            list.Add(values.ItemA);
            list.Add(values.ItemB);
            Assert.AreEqual(0, list.IndexOf(values.ItemA));
            Assert.AreEqual(1, list.IndexOf(values.ItemB));
        }

        [Test, Description("IndexOf missing item => Returns -1?")]
        public void ObservableList_IndexOf_MissingItem_ReturnsMinusOne<T>(
            [ValueSource(nameof(_testValues))] TestValues<T> values)
        {
            var list = new ObservableList<T>();
            Assert.AreEqual(-1, list.IndexOf(values.ItemA));
        }

        [Test, Description("Insert at index => OnItemAdded fires with correct index, item present at index?")]
        public void ObservableList_Insert_OnItemAddedFiredAndItemAtCorrectIndex<T>(
            [ValueSource(nameof(_testValues))] TestValues<T> values)
        {
            var list = new ObservableList<T>();
            list.Add(values.ItemB);

            int receivedIndex = -1;
            T receivedItem = default;
            ItemChangeEvent<T> listener = (item, index) => { receivedItem = item; receivedIndex = index; };
            list.OnItemAdded += listener;
            list.Insert(0, values.ItemA);
            list.OnItemAdded -= listener;

            Assert.AreEqual(values.ItemA, receivedItem);
            Assert.AreEqual(0, receivedIndex);
            Assert.AreEqual(values.ItemA, list[0]);
        }

        [Test, Description("RemoveAt index => OnItemRemoved fires with correct item and index?")]
        public void ObservableList_RemoveAt_OnItemRemovedFiredWithCorrectArgs<T>(
            [ValueSource(nameof(_testValues))] TestValues<T> values)
        {
            var list = new ObservableList<T>();
            list.Add(values.ItemA);
            list.Add(values.ItemB);

            T receivedItem = default;
            int receivedIndex = -1;
            ItemChangeEvent<T> listener = (item, index) => { receivedItem = item; receivedIndex = index; };
            list.OnItemRemoved += listener;
            list.RemoveAt(0);
            list.OnItemRemoved -= listener;

            Assert.AreEqual(values.ItemA, receivedItem);
            Assert.AreEqual(0, receivedIndex);
            Assert.AreEqual(1, list.Count);
        }

        [Test, Description("OverrideWith shorter list => Excess items removed?")]
        [TestCase(new int[] { 1, 2, 3 }, new int[] { 10 })]
        [TestCase(new int[] { 5, 6, 7, 8 }, new int[] { 1, 2 })]
        public void ObservableList_OverrideWith_ShorterList_ExcessItemsRemoved(int[] initial, int[] replacement)
        {
            var list = new ObservableList<int>(initial);
            list.OverrideWith(replacement);
            Assert.AreEqual(replacement.Length, list.Count);
            for (int i = 0; i < replacement.Length; i++)
                Assert.AreEqual(replacement[i], list[i]);
        }

        [Test, Description("OverrideWith longer list => Extra items appended?")]
        [TestCase(new int[] { 1 }, new int[] { 10, 20, 30 })]
        [TestCase(new int[] { 5, 6 }, new int[] { 1, 2, 3, 4 })]
        public void ObservableList_OverrideWith_LongerList_ExtraItemsAppended(int[] initial, int[] replacement)
        {
            var list = new ObservableList<int>(initial);
            list.OverrideWith(replacement);
            Assert.AreEqual(replacement.Length, list.Count);
            for (int i = 0; i < replacement.Length; i++)
                Assert.AreEqual(replacement[i], list[i]);
        }

        [Test, Description("OverrideWith same-length list => All items replaced?")]
        [TestCase(new int[] { 1, 2, 3 }, new int[] { 4, 5, 6 })]
        [TestCase(new int[] { 0, 0 }, new int[] { 1, 2 })]
        public void ObservableList_OverrideWith_SameLengthList_AllItemsReplaced(int[] initial, int[] replacement)
        {
            var list = new ObservableList<int>(initial);
            list.OverrideWith(replacement);
            Assert.AreEqual(replacement.Length, list.Count);
            for (int i = 0; i < replacement.Length; i++)
                Assert.AreEqual(replacement[i], list[i]);
        }

        [Test, Description("OverrideWith shorter list => OnItemReplaced fires for each overlapping position with correct former and new items?")]
        [TestCase(new int[] { 1, 2, 3 }, new int[] { 10, 20 })]
        [TestCase(new int[] { 5, 6, 7, 8 }, new int[] { 11, 12 })]
        public void ObservableList_OverrideWith_ShorterList_OnItemReplacedFiredWithCorrectArgs(int[] initial, int[] replacement)
        {
            var list = new ObservableList<int>(initial);
            var replacedNew = new System.Collections.Generic.List<int>();
            var replacedFormer = new System.Collections.Generic.List<int>();
            var replacedIndices = new System.Collections.Generic.List<int>();

            ItemReplaceEvent<int> listener = (newItem, formerItem, index) =>
            {
                replacedNew.Add(newItem);
                replacedFormer.Add(formerItem);
                replacedIndices.Add(index);
            };
            list.OnItemReplaced += listener;
            list.OverrideWith(replacement);
            list.OnItemReplaced -= listener;

            Assert.AreEqual(replacement.Length, replacedNew.Count);
            for (int i = 0; i < replacement.Length; i++)
            {
                Assert.AreEqual(replacement[i], replacedNew[i]);
                Assert.AreEqual(initial[i], replacedFormer[i]);
                Assert.AreEqual(i, replacedIndices[i]);
            }
        }

        [Test, Description("OverrideWith shorter list => OnItemRemoved fires for each surplus item with correct item and index?")]
        [TestCase(new int[] { 1, 2, 3 }, new int[] { 10 })]
        [TestCase(new int[] { 5, 6, 7, 8 }, new int[] { 11, 12 })]
        public void ObservableList_OverrideWith_ShorterList_OnItemRemovedFiredForSurplusItems(int[] initial, int[] replacement)
        {
            var list = new ObservableList<int>(initial);
            var removedItems = new System.Collections.Generic.List<int>();
            var removedIndices = new System.Collections.Generic.List<int>();

            ItemChangeEvent<int> listener = (item, index) =>
            {
                removedItems.Add(item);
                removedIndices.Add(index);
            };
            list.OnItemRemoved += listener;
            list.OverrideWith(replacement);
            list.OnItemRemoved -= listener;

            int expectedRemovedCount = initial.Length - replacement.Length;
            Assert.AreEqual(expectedRemovedCount, removedItems.Count);
            // Items are removed from the tail one at a time; each removal reduces Count by 1,
            // so the reported index is always equal to the current list length (replacement.Length).
            for (int i = 0; i < expectedRemovedCount; i++)
            {
                Assert.AreEqual(initial[replacement.Length + i], removedItems[i]);
                Assert.AreEqual(replacement.Length, removedIndices[i]);
            }
        }

        [Test, Description("OverrideWith longer list => OnItemAdded fires for each appended item with correct item and index?")]
        [TestCase(new int[] { 1 }, new int[] { 10, 20, 30 })]
        [TestCase(new int[] { 5, 6 }, new int[] { 11, 12, 13, 14 })]
        public void ObservableList_OverrideWith_LongerList_OnItemAddedFiredForAppendedItems(int[] initial, int[] replacement)
        {
            var list = new ObservableList<int>(initial);
            var addedItems = new System.Collections.Generic.List<int>();
            var addedIndices = new System.Collections.Generic.List<int>();

            ItemChangeEvent<int> listener = (item, index) =>
            {
                addedItems.Add(item);
                addedIndices.Add(index);
            };
            list.OnItemAdded += listener;
            list.OverrideWith(replacement);
            list.OnItemAdded -= listener;

            int expectedAddedCount = replacement.Length - initial.Length;
            Assert.AreEqual(expectedAddedCount, addedItems.Count);
            for (int i = 0; i < expectedAddedCount; i++)
            {
                Assert.AreEqual(replacement[initial.Length + i], addedItems[i]);
                Assert.AreEqual(initial.Length + i, addedIndices[i]);
            }
        }

        [Test, Description("OverrideWith empty sequence => Count becomes 0?")]
        [TestCase(new int[] { 1, 2, 3 })]
        [TestCase(new int[] { 5 })]
        [TestCase(new int[] { 10, 20 })]
        public void ObservableList_OverrideWith_EmptySequence_CountBecomesZero(int[] initial)
        {
            var list = new ObservableList<int>(initial);
            list.OverrideWith(System.Array.Empty<int>());
            Assert.AreEqual(0, list.Count);
        }

        [Test, Description("OverrideWith empty sequence => OnItemRemoved fires once per original item with correct item and index?")]
        [TestCase(new int[] { 1, 2, 3 })]
        [TestCase(new int[] { 5 })]
        [TestCase(new int[] { 10, 20 })]
        public void ObservableList_OverrideWith_EmptySequence_OnItemRemovedFiredForEachItem(int[] initial)
        {
            var list = new ObservableList<int>(initial);
            var removedItems = new System.Collections.Generic.List<int>();
            var removedIndices = new System.Collections.Generic.List<int>();

            ItemChangeEvent<int> listener = (item, index) =>
            {
                removedItems.Add(item);
                removedIndices.Add(index);
            };
            list.OnItemRemoved += listener;
            list.OverrideWith(System.Array.Empty<int>());
            list.OnItemRemoved -= listener;

            Assert.AreEqual(initial.Length, removedItems.Count);
            // OverrideWith calls RemoveAt(0) each pass, so each item is reported at index 0
            for (int i = 0; i < initial.Length; i++)
            {
                Assert.AreEqual(initial[i], removedItems[i]);
                Assert.AreEqual(0, removedIndices[i]);
            }
        }

        [Test, Description("OverrideWith empty sequence => OnItemReplaced does not fire?")]
        [TestCase(new int[] { 1, 2, 3 })]
        [TestCase(new int[] { 5 })]
        [TestCase(new int[] { 10, 20 })]
        public void ObservableList_OverrideWith_EmptySequence_OnItemReplacedNotFired(int[] initial)
        {
            var list = new ObservableList<int>(initial);
            bool replacedFired = false;

            ItemReplaceEvent<int> listener = (newItem, formerItem, index) => { replacedFired = true; };
            list.OnItemReplaced += listener;
            list.OverrideWith(System.Array.Empty<int>());
            list.OnItemReplaced -= listener;

            Assert.IsFalse(replacedFired);
        }

        [Test, Description("OverrideWith empty sequence => OnItemAdded does not fire?")]
        [TestCase(new int[] { 1, 2, 3 })]
        [TestCase(new int[] { 5 })]
        [TestCase(new int[] { 10, 20 })]
        public void ObservableList_OverrideWith_EmptySequence_OnItemAddedNotFired(int[] initial)
        {
            var list = new ObservableList<int>(initial);
            bool addedFired = false;

            ItemChangeEvent<int> listener = (item, index) => { addedFired = true; };
            list.OnItemAdded += listener;
            list.OverrideWith(System.Array.Empty<int>());
            list.OnItemAdded -= listener;

            Assert.IsFalse(addedFired);
        }

        [Test, Description("GetEnumerator => Iterates all items in order?")]
        public void ObservableList_GetEnumerator_IteratesAllItemsInOrder<T>(
            [ValueSource(nameof(_testValues))] TestValues<T> values)
        {
            var list = new ObservableList<T>();
            list.Add(values.ItemA);
            list.Add(values.ItemB);

            var iterated = new System.Collections.Generic.List<T>();
            foreach (T item in list)
                iterated.Add(item);

            Assert.AreEqual(2, iterated.Count);
            Assert.AreEqual(values.ItemA, iterated[0]);
            Assert.AreEqual(values.ItemB, iterated[1]);
        }

        [Test, Description("Swap out-of-range index => Throws ArgumentException?")]
        [TestCase(0, 5)]
        [TestCase(5, 0)]
        [TestCase(3, 3)]
        public void ObservableList_Swap_OutOfRangeIndex_ThrowsArgumentException(int index1, int index2)
        {
            var list = new ObservableList<int>();
            list.Add(1);
            list.Add(2);
            Assert.Throws<ArgumentException>(() => list.Swap(index1, index2));
        }

        [Test, Description("IsReadOnly => Returns false?")]
        public void ObservableList_IsReadOnly_ReturnsFalse()
        {
            var list = new ObservableList<int>();
            Assert.IsFalse(list.IsReadOnly);
        }

        public struct TestValues<T>
        {
            public T ItemA { get; set; }
            public T ItemB { get; set; }
        }

        public class Foo
        {
            public int Value { get; set; }
        }
    }
}
