using System;
using NUnit.Framework;

namespace Calluna.Core.Tests
{
    public class AccumulatingValueTests
    {
        // --- AccumulatingIntValue ---

        [Test, Description("Add two entries => Result is their sum?")]
        [TestCase(3, 7, 10)]
        [TestCase(-5, 5, 0)]
        [TestCase(0, 0, 0)]
        [TestCase(100, 200, 300)]
        public void AccumulatingIntValue_Add_ResultIsSum(int a, int b, int expected)
        {
            var acc = new AccumulatingIntValue();
            acc.Add("a", a);
            acc.Add("b", b);
            Assert.AreEqual(expected, acc.Value.Value);
        }

        [Test, Description("Remove entry => Result recalculates without it?")]
        [TestCase(3, 7, 3)]
        [TestCase(10, 10, 10)]
        [TestCase(-5, 5, -5)]
        public void AccumulatingIntValue_Remove_ResultUpdated(int a, int b, int expectedAfterRemove)
        {
            var acc = new AccumulatingIntValue();
            acc.Add("a", a);
            acc.Add("b", b);
            acc.Remove("b");
            Assert.AreEqual(expectedAfterRemove, acc.Value.Value);
        }

        [Test, Description("Set on existing id => Result updates?")]
        [TestCase(3, 10, 10)]
        [TestCase(0, -1, -1)]
        [TestCase(100, 50, 50)]
        public void AccumulatingIntValue_Set_ExistingId_ResultUpdated(int initial, int updated, int expected)
        {
            var acc = new AccumulatingIntValue();
            acc.Add("a", initial);
            acc.Set("a", updated);
            Assert.AreEqual(expected, acc.Value.Value);
        }

        [Test, Description("Set on new id => Entry added and result includes new value?")]
        [TestCase(5)]
        [TestCase(-10)]
        [TestCase(0)]
        [TestCase(999)]
        public void AccumulatingIntValue_Set_NewId_EntryAdded(int value)
        {
            var acc = new AccumulatingIntValue();
            acc.Set("a", value);
            Assert.AreEqual(value, acc.Value.Value);
        }

        [Test, Description("Indexer get => Returns last set value for id?")]
        [TestCase(42)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(int.MaxValue)]
        public void AccumulatingIntValue_IndexerGet_ReturnsCorrectValue(int value)
        {
            var acc = new AccumulatingIntValue();
            acc.Add("a", value);
            Assert.AreEqual(value, acc["a"]);
        }

        [Test, Description("Add duplicate id => Throws ArgumentException?")]
        [TestCase(1, 2)]
        [TestCase(0, 0)]
        [TestCase(-5, 10)]
        public void AccumulatingIntValue_Add_DuplicateId_Throws(int first, int second)
        {
            var acc = new AccumulatingIntValue();
            acc.Add("a", first);
            Assert.Throws<ArgumentException>(() => acc.Add("a", second));
        }

        [Test, Description("Remove missing id => Throws ArgumentException?")]
        [TestCase("missing")]
        [TestCase("x")]
        [TestCase("123")]
        public void AccumulatingIntValue_Remove_MissingId_Throws(string id)
        {
            var acc = new AccumulatingIntValue();
            Assert.Throws<ArgumentException>(() => acc.Remove(id));
        }

        [Test, Description("Add entry => Value observable fires OnChanged?")]
        [TestCase(1)]
        [TestCase(-100)]
        [TestCase(0)]
        public void AccumulatingIntValue_Add_ObservableOnChangedFired(int value)
        {
            var acc = new AccumulatingIntValue();
            bool fired = false;
            Observable<int>.ValueChanged listener = () => { fired = true; };
            acc.Value.OnChanged += listener;
            acc.Add("a", value);
            acc.Value.OnChanged -= listener;
            Assert.IsTrue(fired);
        }

        // --- AccumulatingFloatValue ---

        [Test, Description("AddUp mode => Result is sum?")]
        [TestCase(1.5f, 2.5f, 4.0f)]
        [TestCase(0f, 0f, 0f)]
        [TestCase(-1f, 1f, 0f)]
        [TestCase(10f, 0.5f, 10.5f)]
        public void AccumulatingFloatValue_AddUpMode_ResultIsSum(float a, float b, float expected)
        {
            var acc = new AccumulatingFloatValue(AccumulatingFloatValue.Mode.AddUp);
            acc.Add("a", a);
            acc.Add("b", b);
            Assert.AreEqual(expected, acc.Value.Value, 0.0001f);
        }

        [Test, Description("Multiply mode => Result is product?")]
        [TestCase(3.0f, 4.0f, 12.0f)]
        [TestCase(2.0f, 0.5f, 1.0f)]
        [TestCase(5.0f, 1.0f, 5.0f)]
        [TestCase(-2.0f, 3.0f, -6.0f)]
        public void AccumulatingFloatValue_MultiplyMode_ResultIsProduct(float a, float b, float expected)
        {
            var acc = new AccumulatingFloatValue(AccumulatingFloatValue.Mode.Multiply);
            acc.Add("a", a);
            acc.Add("b", b);
            Assert.AreEqual(expected, acc.Value.Value, 0.0001f);
        }

        [Test, Description("Default constructor => Uses AddUp mode?")]
        [TestCase(1f, 2f, 3f)]
        [TestCase(0.1f, 0.2f, 0.3f)]
        [TestCase(-5f, 10f, 5f)]
        public void AccumulatingFloatValue_DefaultConstructor_UsesAddUp(float a, float b, float expected)
        {
            var acc = new AccumulatingFloatValue();
            acc.Add("a", a);
            acc.Add("b", b);
            Assert.AreEqual(expected, acc.Value.Value, 0.0001f);
        }

        // --- AccumulatingBoolValue ---

        [Test, Description("Any mode => Result is true when at least one entry is true?")]
        [TestCase(false, true, true)]
        [TestCase(true, false, true)]
        [TestCase(true, true, true)]
        [TestCase(false, false, false)]
        public void AccumulatingBoolValue_AnyMode_ResultIsAny(bool a, bool b, bool expected)
        {
            var acc = new AccumulatingBoolValue(AccumulatingBoolValue.Mode.Any);
            acc.Add("a", a);
            acc.Add("b", b);
            Assert.AreEqual(expected, acc.Value.Value);
        }

        [Test, Description("All mode => Result is true only when all entries are true?")]
        [TestCase(true, true, true)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        public void AccumulatingBoolValue_AllMode_ResultIsAll(bool a, bool b, bool expected)
        {
            var acc = new AccumulatingBoolValue(AccumulatingBoolValue.Mode.All);
            acc.Add("a", a);
            acc.Add("b", b);
            Assert.AreEqual(expected, acc.Value.Value);
        }

        [Test, Description("Default constructor => Uses Any mode?")]
        [TestCase(false, true, true)]
        [TestCase(false, false, false)]
        [TestCase(true, true, true)]
        public void AccumulatingBoolValue_DefaultConstructor_UsesAny(bool a, bool b, bool expected)
        {
            var acc = new AccumulatingBoolValue();
            acc.Add("a", a);
            acc.Add("b", b);
            Assert.AreEqual(expected, acc.Value.Value);
        }

        // --- TryGetValuePart ---

        [Test, Description("TryGetValuePart with existing id => Returns true and correct value?")]
        [TestCase(42)]
        [TestCase(-1)]
        [TestCase(0)]
        public void AccumulatingIntValue_TryGetValuePart_ExistingId_ReturnsTrueAndValue(int value)
        {
            var acc = new AccumulatingIntValue();
            acc.Add("a", value);
            bool found = acc.TryGetValuePart("a", out int result);
            Assert.IsTrue(found);
            Assert.AreEqual(value, result);
        }

        [Test, Description("TryGetValuePart with missing id => Returns false?")]
        [TestCase("missing")]
        [TestCase("x")]
        [TestCase("123")]
        public void AccumulatingIntValue_TryGetValuePart_MissingId_ReturnsFalse(string id)
        {
            var acc = new AccumulatingIntValue();
            bool found = acc.TryGetValuePart(id, out _);
            Assert.IsFalse(found);
        }

        // --- Indexer setter ---

        [Test, Description("Indexer setter on existing id => Value updated?")]
        [TestCase(5, 10)]
        [TestCase(0, -1)]
        [TestCase(100, 50)]
        public void AccumulatingIntValue_IndexerSetter_ExistingId_ValueUpdated(int initial, int updated)
        {
            var acc = new AccumulatingIntValue();
            acc.Add("a", initial);
            acc["a"] = updated;
            Assert.AreEqual(updated, acc["a"]);
            Assert.AreEqual(updated, acc.Value.Value);
        }

        [Test, Description("Indexer setter on new id => Entry created?")]
        [TestCase(99)]
        [TestCase(-7)]
        [TestCase(0)]
        public void AccumulatingIntValue_IndexerSetter_NewId_EntryCreated(int value)
        {
            var acc = new AccumulatingIntValue();
            acc["newKey"] = value;
            Assert.AreEqual(value, acc["newKey"]);
        }

        // --- Remove and Set fire OnChanged ---

        [Test, Description("Remove entry => Value observable fires OnChanged?")]
        [TestCase(3, 7)]
        [TestCase(10, 20)]
        [TestCase(-5, 5)]
        public void AccumulatingIntValue_Remove_ObservableOnChangedFired(int a, int b)
        {
            var acc = new AccumulatingIntValue();
            acc.Add("a", a);
            acc.Add("b", b);

            bool fired = false;
            Observable<int>.ValueChanged listener = () => { fired = true; };
            acc.Value.OnChanged += listener;
            acc.Remove("b");
            acc.Value.OnChanged -= listener;

            Assert.IsTrue(fired);
        }

        [Test, Description("Set entry => Value observable fires OnChanged?")]
        [TestCase(1, 2)]
        [TestCase(0, 100)]
        [TestCase(-10, 10)]
        public void AccumulatingIntValue_Set_ObservableOnChangedFired(int initial, int updated)
        {
            var acc = new AccumulatingIntValue();
            acc.Add("a", initial);

            bool fired = false;
            Observable<int>.ValueChanged listener = () => { fired = true; };
            acc.Value.OnChanged += listener;
            acc.Set("a", updated);
            acc.Value.OnChanged -= listener;

            Assert.IsTrue(fired);
        }

        // --- Float Remove and Set recalculation ---

        [Test, Description("AccumulatingFloatValue Remove => Result recalculates without removed entry?")]
        [TestCase(2.0f, 3.0f, 2.0f)]
        [TestCase(0.5f, 1.5f, 0.5f)]
        [TestCase(-1f, 4f, -1f)]
        public void AccumulatingFloatValue_Remove_ResultRecalculated(float a, float b, float expectedAfterRemove)
        {
            var acc = new AccumulatingFloatValue(AccumulatingFloatValue.Mode.AddUp);
            acc.Add("a", a);
            acc.Add("b", b);
            acc.Remove("b");
            Assert.AreEqual(expectedAfterRemove, acc.Value.Value, 0.0001f);
        }

        [Test, Description("AccumulatingFloatValue Set => Result recalculates with updated value?")]
        [TestCase(1.0f, 5.0f, 5.0f)]
        [TestCase(0f, 2.5f, 2.5f)]
        [TestCase(-1f, 0f, 0f)]
        public void AccumulatingFloatValue_Set_ResultRecalculated(float initial, float updated, float expected)
        {
            var acc = new AccumulatingFloatValue(AccumulatingFloatValue.Mode.AddUp);
            acc.Add("a", initial);
            acc.Set("a", updated);
            Assert.AreEqual(expected, acc.Value.Value, 0.0001f);
        }

        // --- Bool Remove and Set recalculation ---

        [Test, Description("AccumulatingBoolValue Remove => Result recalculates without removed entry?")]
        [TestCase(true, false, true)]
        [TestCase(false, true, false)]
        [TestCase(true, true, true)]
        public void AccumulatingBoolValue_Remove_ResultRecalculated(bool a, bool b, bool expectedAfterRemoveB)
        {
            var acc = new AccumulatingBoolValue(AccumulatingBoolValue.Mode.Any);
            acc.Add("a", a);
            acc.Add("b", b);
            acc.Remove("b");
            Assert.AreEqual(expectedAfterRemoveB, acc.Value.Value);
        }

        [Test, Description("AccumulatingBoolValue Set => Result recalculates with updated value?")]
        [TestCase(false, true, true)]
        [TestCase(true, false, false)]
        [TestCase(false, false, false)]
        public void AccumulatingBoolValue_Set_ResultRecalculated(bool initial, bool updated, bool expected)
        {
            var acc = new AccumulatingBoolValue(AccumulatingBoolValue.Mode.Any);
            acc.Add("a", initial);
            acc.Set("a", updated);
            Assert.AreEqual(expected, acc.Value.Value);
        }
    }
}
