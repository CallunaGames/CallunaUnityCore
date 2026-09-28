using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Calluna.Core.Tests
{
    public class ObservableListSubscribeTests
    {
        // ---- Named handlers ----

        [Test, Description("Subscribe(added:) => Called with item and index on Add and Insert?")]
        public void Subscribe_Added_CalledOnAddAndInsert()
        {
            ObservableList<string> list = new ObservableList<string> { "a" };
            List<(string, int)> added = new List<(string, int)>();
            list.Subscribe(added: (item, index) => added.Add((item, index)));

            list.Add("b");
            list.Insert(0, "c");

            CollectionAssert.AreEqual(new[] { ("b", 1), ("c", 0) }, added);
        }

        [Test, Description("Subscribe(removed:) => Called on Remove and RemoveAt?")]
        public void Subscribe_Removed_CalledOnRemoveAndRemoveAt()
        {
            ObservableList<string> list = new ObservableList<string> { "a", "b", "c" };
            List<(string, int)> removed = new List<(string, int)>();
            list.Subscribe(removed: (item, index) => removed.Add((item, index)));

            list.Remove("b");
            list.RemoveAt(0);

            CollectionAssert.AreEqual(new[] { ("b", 1), ("a", 0) }, removed);
        }

        [Test, Description("Subscribe(replaced:) => Called with new item, former item and index?")]
        public void Subscribe_Replaced_CalledOnIndexer()
        {
            ObservableList<string> list = new ObservableList<string> { "a", "b" };
            (string, string, int) received = default;
            list.Subscribe(replaced: (newItem, formerItem, index) => received = (newItem, formerItem, index));

            list[1] = "x";

            Assert.AreEqual(("x", "b", 1), received);
        }

        [Test, Description("Subscribe(swapped:) => Called with the items now at both indices?")]
        public void Subscribe_Swapped_CalledOnSwap()
        {
            ObservableList<string> list = new ObservableList<string> { "a", "b", "c" };
            (string, int, string, int) received = default;
            list.Subscribe(swapped: (item1, index1, item2, index2) => received = (item1, index1, item2, index2));

            list.Swap(0, 2);

            Assert.AreEqual(("c", 0, "a", 2), received);
        }

        [Test, Description("Only some handlers given => Other kinds ignored?")]
        public void Subscribe_OnlyAdded_OtherKindsIgnored()
        {
            ObservableList<int> list = new ObservableList<int> { 1, 2 };
            int calls = 0;
            list.Subscribe(added: (_, _) => calls++);

            list.RemoveAt(0);
            list[0] = 5;

            Assert.AreEqual(0, calls);
        }

        [Test, Description("Subscribe without handlers => Throws ArgumentException?")]
        public void Subscribe_NoHandlers_Throws()
        {
            ObservableList<int> list = new ObservableList<int>();
            Assert.Throws<ArgumentException>(() => list.Subscribe());
        }

        // ---- Reset ----

        [Test, Description("Reset handler given => Called once on Clear and OverrideWith, no per-item calls?")]
        public void Subscribe_WithReset_ResetCalledOnce()
        {
            ObservableList<int> list = new ObservableList<int> { 1, 2 };
            int resets = 0;
            int items = 0;
            list.Subscribe(added: (_, _) => items++, removed: (_, _) => items++, reset: () => resets++);

            list.OverrideWith(new[] { 3, 4, 5 });
            list.Clear();

            Assert.AreEqual(2, resets);
            Assert.AreEqual(0, items);
        }

        [Test, Description("No reset handler, OverrideWith => Former items removed last to first, then new items added?")]
        public void Subscribe_WithoutReset_OverrideWithExpandedToRemovalsAndAdditions()
        {
            ObservableList<string> list = new ObservableList<string> { "a", "b" };
            List<string> log = new List<string>();
            list.Subscribe(added: (item, index) => log.Add($"+{item}{index}"),
                removed: (item, index) => log.Add($"-{item}{index}"));

            list.OverrideWith(new[] { "x", "y", "z" });

            CollectionAssert.AreEqual(new[] { "-b1", "-a0", "+x0", "+y1", "+z2" }, log);
        }

        [Test, Description("No reset handler, Clear => Each former item removed?")]
        public void Subscribe_WithoutReset_ClearExpandedToRemovals()
        {
            ObservableList<string> list = new ObservableList<string> { "a", "b" };
            List<string> removed = new List<string>();
            list.Subscribe(removed: (item, _) => removed.Add(item));

            list.Clear();

            CollectionAssert.AreEqual(new[] { "b", "a" }, removed);
        }

        [Test, Description("Added-only subscriber, OverrideWith => Sees every new item?")]
        public void Subscribe_AddedOnly_SeesItemsOfOverrideWith()
        {
            ObservableList<int> list = new ObservableList<int>();
            List<int> added = new List<int>();
            list.Subscribe(added: (item, _) => added.Add(item));

            list.OverrideWith(new[] { 7, 8 });

            CollectionAssert.AreEqual(new[] { 7, 8 }, added);
        }

        [Test, Description("Expanded reset handlers run => List already holds the new contents?")]
        public void Subscribe_WithoutReset_ListHoldsNewContentsDuringExpansion()
        {
            ObservableList<int> list = new ObservableList<int> { 1, 2 };
            List<int> countsSeen = new List<int>();
            list.Subscribe(removed: (_, _) => countsSeen.Add(list.Count));

            list.OverrideWith(new[] { 3 });

            CollectionAssert.AreEqual(new[] { 1, 1 }, countsSeen);
        }

        // ---- Full channel ----

        [Test, Description("Subscribe(Action<ListChange>) => Every change in order, reset as one change?")]
        public void Subscribe_FullChannel_ReceivesAllChanges()
        {
            ObservableList<string> list = new ObservableList<string>();
            List<ListChangeKind> kinds = new List<ListChangeKind>();
            list.Subscribe(change => kinds.Add(change.Kind));

            list.Add("a");
            list.Add("b");
            list.Swap(0, 1);
            list[0] = "c";
            list.RemoveAt(1);
            list.OverrideWith(new[] { "x", "y" });
            list.Clear();

            CollectionAssert.AreEqual(new[]
            {
                ListChangeKind.Added, ListChangeKind.Added, ListChangeKind.Swapped, ListChangeKind.Replaced,
                ListChangeKind.Removed, ListChangeKind.Reset, ListChangeKind.Reset
            }, kinds);
        }

        [Test, Description("Replaced change => Carries new item, former item and index?")]
        public void Subscribe_FullChannel_ReplacedCarriesBothItems()
        {
            ObservableList<string> list = new ObservableList<string> { "a" };
            ListChange<string> received = default;
            list.Subscribe(change => received = change);

            list[0] = "b";

            Assert.AreEqual(ListChangeKind.Replaced, received.Kind);
            Assert.AreEqual("b", received.Item);
            Assert.AreEqual("a", received.FormerItem);
            Assert.AreEqual(0, received.Index);
        }

        [Test, Description("OverrideWithEvents => Granular changes, no reset?")]
        public void Subscribe_OverrideWithEvents_NoReset()
        {
            ObservableList<int> list = new ObservableList<int> { 1, 2 };
            List<ListChangeKind> kinds = new List<ListChangeKind>();
            list.Subscribe(change => kinds.Add(change.Kind));

            list.OverrideWithEvents(new[] { 1, 3, 4 });

            CollectionAssert.AreEqual(new[] { ListChangeKind.Replaced, ListChangeKind.Added }, kinds);
        }

        // ---- SubscribeAny ----

        [Test, Description("SubscribeAny => One call per change, one per reset?")]
        public void SubscribeAny_OneCallPerChange()
        {
            ObservableList<int> list = new ObservableList<int> { 1, 2 };
            int calls = 0;
            list.SubscribeAny(() => calls++);

            list.Add(3);
            list.Swap(0, 1);
            list.OverrideWith(new[] { 5, 6, 7 });
            list.Clear();

            Assert.AreEqual(4, calls);
        }

        [Test, Description("SubscribeAny null => Throws ArgumentNullException?")]
        public void SubscribeAny_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new ObservableList<int>().SubscribeAny(null));
        }

        // ---- Subscriptions ----

        [Test, Description("Subscription disposed => No further calls, other subscribers still called?")]
        public void Subscription_Disposed_StopsOnlyThatSubscriber()
        {
            ObservableList<int> list = new ObservableList<int>();
            int first = 0;
            int second = 0;
            IDisposable subscription = list.SubscribeAny(() => first++);
            list.SubscribeAny(() => second++);

            list.Add(1);
            subscription.Dispose();
            list.Add(2);

            Assert.AreEqual(1, first);
            Assert.AreEqual(2, second);
        }

        [Test, Description("Subscriber unsubscribing during a notification => Others of that change still called?")]
        public void Subscription_DisposedDuringNotification_OthersStillCalled()
        {
            ObservableList<int> list = new ObservableList<int>();
            IDisposable first = null;
            int second = 0;
            first = list.SubscribeAny(() => first.Dispose());
            list.SubscribeAny(() => second++);

            list.Add(1);
            list.Add(2);

            Assert.AreEqual(2, second);
        }

        [Test, Description("Via ReadonlyObservableList => Subscribe works?")]
        public void Subscribe_ViaReadonlyInterface()
        {
            ObservableList<int> list = new ObservableList<int>();
            ReadonlyObservableList<int> readonlyList = list;
            int added = 0;
            using (readonlyList.Subscribe(added: (_, _) => added++))
                list.Add(1);
            list.Add(2);

            Assert.AreEqual(1, added);
        }
    }
}
