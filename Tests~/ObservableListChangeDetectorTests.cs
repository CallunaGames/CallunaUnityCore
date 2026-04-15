using NUnit.Framework;

namespace Calluna.Core.Tests
{
    public class ObservableListChangeDetectorTests
    {
        [Test, Description("Add item => OnChanged fires?")]
        [TestCase(1)]
        [TestCase(42)]
        [TestCase(-7)]
        public void ObservableListChangeDetector_OnChanged_FiresOnAdd(int item)
        {
            var list = new ObservableList<int>();
            var detector = new ObservableListChangeDetector<int>(list);

            int callCount = 0;
            System.Action listener = () => { callCount++; };
            detector.OnChanged += listener;
            list.Add(item);
            detector.OnChanged -= listener;

            Assert.AreEqual(1, callCount);
            detector.Dispose();
        }

        [Test, Description("Remove item => OnChanged fires?")]
        [TestCase(1)]
        [TestCase(42)]
        [TestCase(-7)]
        public void ObservableListChangeDetector_OnChanged_FiresOnRemove(int item)
        {
            var list = new ObservableList<int>();
            list.Add(item);
            var detector = new ObservableListChangeDetector<int>(list);

            int callCount = 0;
            System.Action listener = () => { callCount++; };
            detector.OnChanged += listener;
            list.Remove(item);
            detector.OnChanged -= listener;

            Assert.AreEqual(1, callCount);
            detector.Dispose();
        }

        [Test, Description("Set indexer (Replace) => OnChanged fires?")]
        [TestCase(1, 2)]
        [TestCase(10, 99)]
        [TestCase(-5, 0)]
        public void ObservableListChangeDetector_OnChanged_FiresOnReplace(int initial, int replacement)
        {
            var list = new ObservableList<int>();
            list.Add(initial);
            var detector = new ObservableListChangeDetector<int>(list);

            int callCount = 0;
            System.Action listener = () => { callCount++; };
            detector.OnChanged += listener;
            list[0] = replacement;
            detector.OnChanged -= listener;

            Assert.AreEqual(1, callCount);
            detector.Dispose();
        }

        [Test, Description("Swap => OnChanged fires?")]
        [TestCase(1, 2)]
        [TestCase(10, 99)]
        [TestCase(-5, 0)]
        public void ObservableListChangeDetector_OnChanged_FiresOnSwap(int itemA, int itemB)
        {
            var list = new ObservableList<int>();
            list.Add(itemA);
            list.Add(itemB);
            var detector = new ObservableListChangeDetector<int>(list);

            int callCount = 0;
            System.Action listener = () => { callCount++; };
            detector.OnChanged += listener;
            list.Swap(0, 1);
            detector.OnChanged -= listener;

            Assert.AreEqual(1, callCount);
            detector.Dispose();
        }

        [Test, Description("Clear => OnChanged fires once?")]
        [TestCase(1, 2)]
        [TestCase(10, 99)]
        [TestCase(-5, 0)]
        public void ObservableListChangeDetector_OnChanged_FiresOnClear(int itemA, int itemB)
        {
            var list = new ObservableList<int>();
            list.Add(itemA);
            list.Add(itemB);
            var detector = new ObservableListChangeDetector<int>(list);

            int callCount = 0;
            System.Action listener = () => { callCount++; };
            detector.OnChanged += listener;
            list.Clear();
            detector.OnChanged -= listener;

            Assert.AreEqual(1, callCount);
            detector.Dispose();
        }

        [Test, Description("Dispose => OnChanged no longer fires after any mutation?")]
        [TestCase(1, 2)]
        [TestCase(10, 99)]
        [TestCase(-5, 0)]
        public void ObservableListChangeDetector_AfterDispose_OnChangedDoesNotFire(int itemA, int itemB)
        {
            var list = new ObservableList<int>();
            list.Add(itemA);
            list.Add(itemB);
            var detector = new ObservableListChangeDetector<int>(list);

            int callCount = 0;
            System.Action listener = () => { callCount++; };
            detector.OnChanged += listener;
            detector.Dispose();

            list.Add(99);
            list.Remove(itemA);
            list[0] = itemA;
            list.Swap(0, 1);
            list.Clear();

            detector.OnChanged -= listener;
            Assert.AreEqual(0, callCount);
        }
    }
}
