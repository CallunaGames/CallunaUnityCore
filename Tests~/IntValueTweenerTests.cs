using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Calluna.Core.Tests
{
    public class IntValueTweenerTests
    {
        private GameObject _root;
        private CoroutineHelper _coroutineHelper;

        [SetUp]
        public void SetUp()
        {
            _root = new GameObject("test", typeof(CoroutineHelper));
            _coroutineHelper = _root.GetComponent<CoroutineHelper>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_root);
        }

        // ── Zero / negative duration (synchronous path — no coroutine started) ───

        [Test]
        [Description("Perform with zero duration => update callback receives the end value?")]
        public void IntValueTweener_Perform_ZeroDuration_InvokesUpdateWithEndValue()
        {
            var tweener = new IntValueTweener(_coroutineHelper);
            int received = -1;

            tweener.Perform(0, 10, 0f, TweenType.EaseInOutSine, v => received = v);

            Assert.AreEqual(10, received);
        }

        [Test]
        [Description("Perform with negative duration => update callback receives the end value?")]
        public void IntValueTweener_Perform_NegativeDuration_InvokesUpdateWithEndValue()
        {
            var tweener = new IntValueTweener(_coroutineHelper);
            int received = -1;

            tweener.Perform(5, 99, -1f, TweenType.EaseInOutSine, v => received = v);

            Assert.AreEqual(99, received);
        }

        [Test]
        [Description("Perform with zero duration and start == end => update callback receives that value?")]
        public void IntValueTweener_Perform_ZeroDuration_StartEqualsEnd_InvokesWithThatValue()
        {
            var tweener = new IntValueTweener(_coroutineHelper);
            int received = -1;

            tweener.Perform(7, 7, 0f, TweenType.EaseInOutSine, v => received = v);

            Assert.AreEqual(7, received);
        }

        // ── State ────────────────────────────────────────────────────────────────

        [Test]
        [Description("IsTweening after construction => false?")]
        public void IntValueTweener_IsTweening_AfterConstruction_ReturnsFalse()
        {
            var tweener = new IntValueTweener(_coroutineHelper);
            Assert.IsFalse(tweener.IsTweening);
        }

        [Test]
        [Description("Stop when not performing => does not throw?")]
        public void IntValueTweener_Stop_WhenNotPerforming_DoesNotThrow()
        {
            var tweener = new IntValueTweener(_coroutineHelper);
            Assert.DoesNotThrow(() => tweener.Stop());
        }

        [Test]
        [Description("Dispose when not performing => does not throw?")]
        public void IntValueTweener_Dispose_WhenNotPerforming_DoesNotThrow()
        {
            var tweener = new IntValueTweener(_coroutineHelper);
            Assert.DoesNotThrow(() => tweener.Dispose());
        }

        [Test]
        [Description("Perform with positive duration => IsTweening is true immediately after the call?")]
        public void IntValueTweener_Perform_PositiveDuration_IsTweeningIsTrue()
        {
            var tweener = new IntValueTweener(_coroutineHelper);

            tweener.Perform(0, 100, 5f, TweenType.EaseInOutSine, _ => { });

            Assert.IsTrue(tweener.IsTweening);
            tweener.Stop();
        }

        [Test]
        [Description("Stop after Perform with positive duration => IsTweening is false?")]
        public void IntValueTweener_Stop_AfterPerformWithPositiveDuration_IsTweeningIsFalse()
        {
            var tweener = new IntValueTweener(_coroutineHelper);
            tweener.Perform(0, 100, 5f, TweenType.EaseInOutSine, _ => { });

            tweener.Stop();

            Assert.IsFalse(tweener.IsTweening);
        }

        [Test]
        [Description("Dispose after Perform with positive duration => IsTweening is false?")]
        public void IntValueTweener_Dispose_AfterPerformWithPositiveDuration_IsTweeningIsFalse()
        {
            var tweener = new IntValueTweener(_coroutineHelper);
            tweener.Perform(0, 100, 5f, TweenType.EaseInOutSine, _ => { });

            tweener.Dispose();

            Assert.IsFalse(tweener.IsTweening);
        }

        [Test]
        [TestCase(0, 3, 3)]
        [TestCase(1, 4, 4)]
        [TestCase(-5, 5, 5)]
        [Description("Perform with zero duration => callback value is an exact integer with no fractional component?")]
        public void IntValueTweener_Perform_ZeroDuration_ReturnsInteger(int start, int end, int expected)
        {
            var tweener = new IntValueTweener(_coroutineHelper);
            int received = -999;

            tweener.Perform(start, end, 0f, TweenType.EaseInOutSine, v => received = v);

            Assert.AreEqual(expected, received);
        }
    }
}
