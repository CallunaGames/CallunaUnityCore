using System;
using UnityEngine;

namespace Calluna
{
    /// <summary>
    /// Source: https://easings.net/
    /// </summary>
    public static class Tween
    {
        public static float EaseInSine(float value)
        {
            ValidateValue(value);
            return 1 - Mathf.Cos((value * Mathf.PI) / 2);
        }
        
        public static float EaseOutSine(float value)
        {
            ValidateValue(value);
            return Mathf.Sin((value * Mathf.PI) / 2);
        }
        
        public static float EaseInOutSine(float value)
        {
            ValidateValue(value);
            return -(Mathf.Cos(Mathf.PI * value) - 1) / 2;
        }
        
        public static float EaseInCubic(float value)
        {
            ValidateValue(value);
            return value * value * value;
        }
        
        public static float EaseOutCubic(float value)
        {
            ValidateValue(value);
            return 1 - Mathf.Pow(1 - value, 3);
        }
        
        public static float EaseInOutCubic(float value)
        {
            ValidateValue(value);
            return value < 0.5f ? 4 * value * value * value : 1 - Mathf.Pow(-2 * value + 2, 3) / 2;
        }
        
        public static float EaseInBack(float value)
        {
            ValidateValue(value);
            // Overshoot amount — the standard Back easing constant from Robert Penner's equations (easings.net).
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;
            return c3 * value * value * value - c1 * value * value;
        }

        public static float EaseOutBack(float value)
        {
            ValidateValue(value);
            // Overshoot amount — the standard Back easing constant from Robert Penner's equations (easings.net).
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;
            return 1 + c3 * Mathf.Pow(value - 1, 3) + c1 * Mathf.Pow(value - 1, 2);
        }

        public static float EaseInOutBack(float value)
        {
            ValidateValue(value);
            // Overshoot amount — the standard Back easing constant from Robert Penner's equations (easings.net).
            const float c1 = 1.70158f;
            // Scaled overshoot for the InOut variant (c1 * 1.525 per the easings.net formula).
            const float c2 = c1 * 1.525f;

            return value < 0.5f
                ? (Mathf.Pow(2 * value, 2) * ((c2 + 1) * 2 * value - c2)) / 2
                : (Mathf.Pow(2 * value - 2, 2) * ((c2 + 1) * (value * 2 - 2) + c2) + 2) / 2;
            
        }

        public static float Linear(float value)
        {
            ValidateValue(value);
            return value;
        }

        // Pre-allocated delegates indexed by TweenType enum value (0–9) to avoid per-call allocation.
        private static readonly Func<float, float>[] _easeFunctions =
        {
            EaseInOutSine, EaseInSine, EaseOutSine,
            EaseInOutCubic, EaseInCubic, EaseOutCubic,
            EaseInOutBack, EaseInBack, EaseOutBack,
            Linear,
        };

        public static Func<float, float> GetEaseFunction(TweenType tweenType)
        {
            int index = (int)tweenType;
            if (index < 0 || index >= _easeFunctions.Length)
                throw new ArgumentOutOfRangeException(nameof(tweenType));
            return _easeFunctions[index];
        }

        private static void ValidateValue(float value)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (value < 0.0f || value > 1.0f)
                throw new ArgumentException("The easing value must be between 0.0 and 1.0", nameof(value));
#endif
        }
    }
}