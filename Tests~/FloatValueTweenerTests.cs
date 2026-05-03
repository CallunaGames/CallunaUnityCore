using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Calluna.Core.Tests
{
    public class FloatValueTweenerTests
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
        public void FloatValueTweener_Perform_ZeroDuration_InvokesUpdateWithEndValue()
        {
            var tweener = new FloatValueTweener(_coroutineHelper);
            float received = -1f;

            tweener.Perform(0f, 10f, 0f, TweenType.EaseInOutSine, v => received = v);

            Assert.AreEqual(10f, received, 0.0001f);
        }

        [Test]
        public void FloatValueTweener_Perform_NegativeDuration_InvokesUpdateWithEndValue()
        {
            var tweener = new FloatValueTweener(_coroutineHelper);
            float received = -1f;

            tweener.Perform(5f, 99f, -1f, TweenType.EaseInOutSine, v => received = v);

            Assert.AreEqual(99f, received, 0.0001f);
        }

        [Test]
        public void FloatValueTweener_Perform_ZeroDuration_StartEqualsEnd_InvokesWithThatValue()
        {
            var tweener = new FloatValueTweener(_coroutineHelper);
            float received = -1f;

            tweener.Perform(7f, 7f, 0f, TweenType.EaseInOutSine, v => received = v);

            Assert.AreEqual(7f, received, 0.0001f);
        }

        // ── State ────────────────────────────────────────────────────────────────

        [Test]
        public void FloatValueTweener_IsPerforming_AfterConstruction_ReturnsFalse()
        {
            var tweener = new FloatValueTweener(_coroutineHelper);
            Assert.IsFalse(tweener.IsPerforming);
        }

        [Test]
        public void FloatValueTweener_Stop_WhenNotPerforming_DoesNotThrow()
        {
            var tweener = new FloatValueTweener(_coroutineHelper);
            Assert.DoesNotThrow(() => tweener.Stop());
        }

        [Test]
        public void FloatValueTweener_Dispose_WhenNotPerforming_DoesNotThrow()
        {
            var tweener = new FloatValueTweener(_coroutineHelper);
            Assert.DoesNotThrow(() => tweener.Dispose());
        }
    }
}
