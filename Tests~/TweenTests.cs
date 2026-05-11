using System;
using NUnit.Framework;

namespace Calluna.Core.Tests
{
    public class TweenTests
    {
        private static TweenType[] _allTweenTypes = (TweenType[])Enum.GetValues(typeof(TweenType));

        [Test, Description("GetEaseFunction => Returns non-null function for every TweenType?")]
        public void Tween_GetEaseFunction_ReturnsNonNullForAllTypes(
            [ValueSource(nameof(_allTweenTypes))] TweenType tweenType)
        {
            Func<float, float> fn = Tween.GetEaseFunction(tweenType);
            Assert.IsNotNull(fn);
        }

        [Test, Description("GetEaseFunction => Returns 0 at input 0?")]
        public void Tween_GetEaseFunction_ReturnsZeroAtZero(
            [ValueSource(nameof(_allTweenTypes))] TweenType tweenType)
        {
            Func<float, float> fn = Tween.GetEaseFunction(tweenType);
            Assert.AreEqual(0f, fn(0f), 0.0001f);
        }

        [Test, Description("GetEaseFunction => Returns 1 at input 1?")]
        public void Tween_GetEaseFunction_ReturnsOneAtOne(
            [ValueSource(nameof(_allTweenTypes))] TweenType tweenType)
        {
            Func<float, float> fn = Tween.GetEaseFunction(tweenType);
            Assert.AreEqual(1f, fn(1f), 0.0001f);
        }

        [Test, Description("EaseFunction => Throws ArgumentException for input below 0?")]
        [TestCase(-0.001f)]
        [TestCase(-1f)]
        [TestCase(-100f)]
        public void Tween_EaseFunction_ThrowsForInputBelowZero(float input)
        {
            Assert.Throws<ArgumentException>(() => Tween.EaseInSine(input));
            Assert.Throws<ArgumentException>(() => Tween.EaseOutSine(input));
            Assert.Throws<ArgumentException>(() => Tween.EaseInOutSine(input));
            Assert.Throws<ArgumentException>(() => Tween.EaseInCubic(input));
            Assert.Throws<ArgumentException>(() => Tween.EaseOutCubic(input));
            Assert.Throws<ArgumentException>(() => Tween.EaseInOutCubic(input));
            Assert.Throws<ArgumentException>(() => Tween.EaseInBack(input));
            Assert.Throws<ArgumentException>(() => Tween.EaseOutBack(input));
            Assert.Throws<ArgumentException>(() => Tween.EaseInOutBack(input));
        }

        [Test, Description("EaseFunction => Throws ArgumentException for input above 1?")]
        [TestCase(1.001f)]
        [TestCase(2f)]
        [TestCase(100f)]
        public void Tween_EaseFunction_ThrowsForInputAboveOne(float input)
        {
            Assert.Throws<ArgumentException>(() => Tween.EaseInSine(input));
            Assert.Throws<ArgumentException>(() => Tween.EaseOutSine(input));
            Assert.Throws<ArgumentException>(() => Tween.EaseInOutSine(input));
            Assert.Throws<ArgumentException>(() => Tween.EaseInCubic(input));
            Assert.Throws<ArgumentException>(() => Tween.EaseOutCubic(input));
            Assert.Throws<ArgumentException>(() => Tween.EaseInOutCubic(input));
            Assert.Throws<ArgumentException>(() => Tween.EaseInBack(input));
            Assert.Throws<ArgumentException>(() => Tween.EaseOutBack(input));
            Assert.Throws<ArgumentException>(() => Tween.EaseInOutBack(input));
        }

        [Test, Description("EaseInSine => Returns expected values at midpoints?")]
        [TestCase(0.25f)]
        [TestCase(0.5f)]
        [TestCase(0.75f)]
        public void Tween_EaseInSine_ReturnsValueInRange(float input)
        {
            float result = Tween.EaseInSine(input);
            Assert.Greater(result, 0f);
            Assert.Less(result, 1f);
        }

        [Test, Description("EaseOutSine => Returns expected values at midpoints?")]
        [TestCase(0.25f)]
        [TestCase(0.5f)]
        [TestCase(0.75f)]
        public void Tween_EaseOutSine_ReturnsValueInRange(float input)
        {
            float result = Tween.EaseOutSine(input);
            Assert.Greater(result, 0f);
            Assert.Less(result, 1f);
        }

        [Test, Description("EaseInSine => Monotonically increasing at sample points?")]
        public void Tween_EaseInSine_MonotonicallyIncreasing()
        {
            float[] inputs = { 0f, 0.25f, 0.5f, 0.75f, 1f };
            float previous = -1f;
            foreach (float t in inputs)
            {
                float value = Tween.EaseInSine(t);
                Assert.GreaterOrEqual(value, previous);
                previous = value;
            }
        }

        [Test, Description("EaseOutSine => Monotonically increasing at sample points?")]
        public void Tween_EaseOutSine_MonotonicallyIncreasing()
        {
            float[] inputs = { 0f, 0.25f, 0.5f, 0.75f, 1f };
            float previous = -1f;
            foreach (float t in inputs)
            {
                float value = Tween.EaseOutSine(t);
                Assert.GreaterOrEqual(value, previous);
                previous = value;
            }
        }

        [Test, Description("EaseInCubic => Monotonically increasing at sample points?")]
        public void Tween_EaseInCubic_MonotonicallyIncreasing()
        {
            float[] inputs = { 0f, 0.25f, 0.5f, 0.75f, 1f };
            float previous = -1f;
            foreach (float t in inputs)
            {
                float value = Tween.EaseInCubic(t);
                Assert.GreaterOrEqual(value, previous);
                previous = value;
            }
        }

        [Test, Description("EaseOutCubic => Monotonically increasing at sample points?")]
        public void Tween_EaseOutCubic_MonotonicallyIncreasing()
        {
            float[] inputs = { 0f, 0.25f, 0.5f, 0.75f, 1f };
            float previous = -1f;
            foreach (float t in inputs)
            {
                float value = Tween.EaseOutCubic(t);
                Assert.GreaterOrEqual(value, previous);
                previous = value;
            }
        }

        [Test, Description("GetEaseFunction invalid TweenType => Throws ArgumentOutOfRangeException?")]
        [TestCase(-1)]
        [TestCase(100)]
        [TestCase(10)]
        public void Tween_GetEaseFunction_InvalidTweenType_ThrowsArgumentOutOfRangeException(int invalidValue)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Tween.GetEaseFunction((TweenType)invalidValue));
        }

        [Test, Description("EaseInOutSine => Monotonically increasing at sample points?")]
        public void Tween_EaseInOutSine_MonotonicallyIncreasing()
        {
            float[] inputs = { 0f, 0.25f, 0.5f, 0.75f, 1f };
            float previous = -1f;
            foreach (float t in inputs)
            {
                float value = Tween.EaseInOutSine(t);
                Assert.GreaterOrEqual(value, previous);
                previous = value;
            }
        }

        [Test, Description("EaseInOutCubic => Monotonically increasing at sample points?")]
        public void Tween_EaseInOutCubic_MonotonicallyIncreasing()
        {
            float[] inputs = { 0f, 0.25f, 0.5f, 0.75f, 1f };
            float previous = -1f;
            foreach (float t in inputs)
            {
                float value = Tween.EaseInOutCubic(t);
                Assert.GreaterOrEqual(value, previous);
                previous = value;
            }
        }

        [Test, Description("EaseInBack => Returns 0 at 0 and 1 at 1 (despite overshoot in between)?")]
        public void Tween_EaseInBack_BoundaryValues()
        {
            Assert.AreEqual(0f, Tween.EaseInBack(0f), 0.0001f);
            Assert.AreEqual(1f, Tween.EaseInBack(1f), 0.0001f);
        }

        [Test, Description("EaseOutBack => Returns 0 at 0 and 1 at 1 (despite overshoot in between)?")]
        public void Tween_EaseOutBack_BoundaryValues()
        {
            Assert.AreEqual(0f, Tween.EaseOutBack(0f), 0.0001f);
            Assert.AreEqual(1f, Tween.EaseOutBack(1f), 0.0001f);
        }

        [Test, Description("EaseInOutBack => Returns 0 at 0 and 1 at 1 (despite overshoot in between)?")]
        public void Tween_EaseInOutBack_BoundaryValues()
        {
            Assert.AreEqual(0f, Tween.EaseInOutBack(0f), 0.0001f);
            Assert.AreEqual(1f, Tween.EaseInOutBack(1f), 0.0001f);
        }

        [Test, Description("EaseInOutBack => Returns value below 0 in the first quarter (overshoot going down)?")]
        public void Tween_EaseInOutBack_FirstQuarter_ReturnsBelowZero()
        {
            float result = Tween.EaseInOutBack(0.1f);
            Assert.Less(result, 0f);
        }

        [Test, Description("EaseInOutBack => Returns value above 1 in the third quarter (overshoot going up)?")]
        public void Tween_EaseInOutBack_ThirdQuarter_ReturnsAboveOne()
        {
            float result = Tween.EaseInOutBack(0.9f);
            Assert.Greater(result, 1f);
        }

        [Test, Description("EaseInBack => Returns value below 0 at midpoint (overshoot before arrival)?")]
        public void Tween_EaseInBack_Midpoint_ReturnsBelowZero()
        {
            float result = Tween.EaseInBack(0.25f);
            Assert.Less(result, 0f);
        }

        [Test, Description("EaseOutBack => Returns value above 1 at midpoint (overshoot past target)?")]
        public void Tween_EaseOutBack_Midpoint_ReturnsAboveOne()
        {
            float result = Tween.EaseOutBack(0.75f);
            Assert.Greater(result, 1f);
        }

        // Literal expected values at t=0.5f, computed from the formulas in Tween.cs.
        // c1=1.70158, c3=c1+1=2.70158, c2=c1*1.525≈2.5949
        private static readonly (TweenType type, float expected)[] _easeAt05 =
        {
            (TweenType.EaseInOutSine,  0.5f),
            (TweenType.EaseInSine,     0.29289f),
            (TweenType.EaseOutSine,    0.70711f),
            (TweenType.EaseInOutCubic, 0.5f),
            (TweenType.EaseInCubic,    0.125f),
            (TweenType.EaseOutCubic,   0.875f),
            (TweenType.EaseInOutBack,  0.5f),
            (TweenType.EaseInBack,    -0.0877f),
            (TweenType.EaseOutBack,    1.0877f),
        };

        [Test, Description("GetEaseFunction => Returned delegate produces the same result as calling the corresponding method directly?")]
        public void Tween_GetEaseFunction_DelegateMatchesDirectCall(
            [ValueSource(nameof(_easeAt05))] (TweenType type, float expected) testCase)
        {
            Func<float, float> fn = Tween.GetEaseFunction(testCase.type);
            float result = fn(0.5f);
            Assert.AreEqual(testCase.expected, result, 0.0001f);
        }
    }
}
