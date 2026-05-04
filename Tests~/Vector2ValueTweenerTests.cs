using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Calluna.Core.Tests
{
    public class Vector2ValueTweenerTests
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
        public void Vector2ValueTweener_Perform_ZeroDuration_InvokesUpdateWithEndValue()
        {
            var tweener = new Vector2ValueTweener(_coroutineHelper);
            Vector2 received = Vector2.negativeInfinity;

            tweener.Perform(Vector2.zero, Vector2.one * 10f, 0f, TweenType.EaseInOutSine, v => received = v);

            Assert.AreEqual(Vector2.one * 10f, received);
        }

        [Test]
        [Description("Perform with negative duration => update callback receives the end value?")]
        public void Vector2ValueTweener_Perform_NegativeDuration_InvokesUpdateWithEndValue()
        {
            var tweener = new Vector2ValueTweener(_coroutineHelper);
            Vector2 received = Vector2.negativeInfinity;

            tweener.Perform(Vector2.zero, Vector2.one * 10f, -1f, TweenType.EaseInOutSine, v => received = v);

            Assert.AreEqual(Vector2.one * 10f, received);
        }

        [Test]
        [Description("Perform with zero duration and start == end => update callback receives that value?")]
        public void Vector2ValueTweener_Perform_ZeroDuration_StartEqualsEnd_InvokesWithThatValue()
        {
            var tweener = new Vector2ValueTweener(_coroutineHelper);
            var target = new Vector2(3f, 7f);
            Vector2 received = Vector2.negativeInfinity;

            tweener.Perform(target, target, 0f, TweenType.EaseInOutSine, v => received = v);

            Assert.AreEqual(target, received);
        }

        // ── State ────────────────────────────────────────────────────────────────

        [Test]
        [Description("IsTweening after construction => false?")]
        public void Vector2ValueTweener_IsTweening_AfterConstruction_ReturnsFalse()
        {
            var tweener = new Vector2ValueTweener(_coroutineHelper);
            Assert.IsFalse(tweener.IsTweening);
        }

        [Test]
        [Description("Stop when not performing => does not throw?")]
        public void Vector2ValueTweener_Stop_WhenNotPerforming_DoesNotThrow()
        {
            var tweener = new Vector2ValueTweener(_coroutineHelper);
            Assert.DoesNotThrow(() => tweener.Stop());
        }

        [Test]
        [Description("Dispose when not performing => does not throw?")]
        public void Vector2ValueTweener_Dispose_WhenNotPerforming_DoesNotThrow()
        {
            var tweener = new Vector2ValueTweener(_coroutineHelper);
            Assert.DoesNotThrow(() => tweener.Dispose());
        }

        [Test]
        [Description("Perform with positive duration => IsTweening is true immediately after the call?")]
        public void Vector2ValueTweener_Perform_PositiveDuration_IsTweeningIsTrue()
        {
            var tweener = new Vector2ValueTweener(_coroutineHelper);

            tweener.Perform(Vector2.zero, Vector2.one, 5f, TweenType.EaseInOutSine, _ => { });

            Assert.IsTrue(tweener.IsTweening);
            tweener.Stop();
        }

        [Test]
        [Description("Stop after Perform with positive duration => IsTweening is false?")]
        public void Vector2ValueTweener_Stop_AfterPerformWithPositiveDuration_IsTweeningIsFalse()
        {
            var tweener = new Vector2ValueTweener(_coroutineHelper);
            tweener.Perform(Vector2.zero, Vector2.one, 5f, TweenType.EaseInOutSine, _ => { });

            tweener.Stop();

            Assert.IsFalse(tweener.IsTweening);
        }

        [Test]
        [Description("Dispose after Perform with positive duration => IsTweening is false?")]
        public void Vector2ValueTweener_Dispose_AfterPerformWithPositiveDuration_IsTweeningIsFalse()
        {
            var tweener = new Vector2ValueTweener(_coroutineHelper);
            tweener.Perform(Vector2.zero, Vector2.one, 5f, TweenType.EaseInOutSine, _ => { });

            tweener.Dispose();

            Assert.IsFalse(tweener.IsTweening);
        }
    }
}
