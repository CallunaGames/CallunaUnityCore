using System;
using NUnit.Framework;

namespace Calluna.Core.Tests
{
    public class KeyedAccumulatingPartsTests
    {
        [Test, Description("Set => Contributes per key?")]
        public void KeyedAccumulatingParts_Set_ContributesPerKey()
        {
            var acc = new AccumulatingIntValue();
            var parts = new KeyedAccumulatingParts<string, int>(acc);

            parts.Set("a", 3);
            parts.Set("b", 4);
            parts.Set("a", 5);

            Assert.AreEqual(9, acc.Value.Value);
        }

        [Test, Description("Clear key => Only that key withdrawn, set again contributes?")]
        public void KeyedAccumulatingParts_ClearKey_WithdrawsOnlyThatKey()
        {
            var acc = new AccumulatingIntValue();
            var parts = new KeyedAccumulatingParts<string, int>(acc);
            parts.Set("a", 3);
            parts.Set("b", 4);

            parts.Clear("a");
            Assert.AreEqual(4, acc.Value.Value);

            parts.Set("a", 1);
            Assert.AreEqual(5, acc.Value.Value);
        }

        [Test, Description("Clear unknown key => Nothing happens?")]
        public void KeyedAccumulatingParts_ClearUnknownKey_DoesNothing()
        {
            var parts = new KeyedAccumulatingParts<string, int>(new AccumulatingIntValue());
            Assert.DoesNotThrow(() => parts.Clear("unknown"));
        }

        [Test, Description("Mode.All requester cleared => No longer blocks?")]
        public void KeyedAccumulatingParts_AllModeCleared_NoLongerBlocks()
        {
            var allowed = new AccumulatingBoolValue(AccumulatingBoolValue.Mode.All);
            var blockers = new KeyedAccumulatingParts<string, bool>(allowed);

            blockers.Set("dialogue", false);
            Assert.IsFalse(allowed.Value.Value);

            blockers.Clear("dialogue");
            Assert.IsTrue(allowed.Value.Value);
        }

        [Test, Description("TryGet => Value only for keys that contribute?")]
        public void KeyedAccumulatingParts_TryGet_OnlyForSetKeys()
        {
            var parts = new KeyedAccumulatingParts<string, int>(new AccumulatingIntValue());
            parts.Set("a", 3);
            parts.Set("b", 4);
            parts.Clear("b");

            Assert.IsTrue(parts.TryGet("a", out int a));
            Assert.AreEqual(3, a);
            Assert.IsFalse(parts.TryGet("b", out _));
            Assert.IsFalse(parts.TryGet("c", out _));
        }

        [Test, Description("ClearAll => All keys withdrawn?")]
        public void KeyedAccumulatingParts_ClearAll_WithdrawsAll()
        {
            var acc = new AccumulatingIntValue();
            acc.AddPart(1);
            var parts = new KeyedAccumulatingParts<string, int>(acc);
            parts.Set("a", 3);
            parts.Set("b", 4);

            parts.ClearAll();

            Assert.AreEqual(1, acc.Value.Value);
        }

        [Test, Description("Disposed => All parts removed, usable again?")]
        public void KeyedAccumulatingParts_Disposed_RemovesAllParts()
        {
            var acc = new AccumulatingIntValue();
            var parts = new KeyedAccumulatingParts<string, int>(acc);
            parts.Set("a", 3);

            parts.Dispose();
            Assert.AreEqual(0, acc.Value.Value);

            parts.Set("a", 2);
            Assert.AreEqual(2, acc.Value.Value);
        }

        [Test, Description("Object keys => Parts kept per instance?")]
        public void KeyedAccumulatingParts_ObjectKeys_PerInstance()
        {
            var hovered = new AccumulatingBoolValue(AccumulatingBoolValue.Mode.Any);
            var views = new KeyedAccumulatingParts<object, bool>(hovered);
            object first = new object();
            object second = new object();

            views.Set(first, true);
            views.Set(second, true);
            views.Clear(first);

            Assert.IsTrue(hovered.Value.Value);
            views.Clear(second);
            Assert.IsFalse(hovered.Value.Value);
        }

        [Test, Description("Constructed with null => Throws ArgumentNullException?")]
        public void KeyedAccumulatingParts_NullValue_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new KeyedAccumulatingParts<string, int>(null));
        }
    }
}
