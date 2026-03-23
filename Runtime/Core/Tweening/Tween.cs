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
            return -(Mathf.Cos(Mathf.PI * value) - 1) / 2;
        }
        
        public static float EaseInBack(float value)
        {
            ValidateValue(value);
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;
            return c3 * value * value * value - c1 * value * value;
        }
        
        public static float EaseOutBack(float value)
        {
            ValidateValue(value);
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;
            return 1 + c3 * Mathf.Pow(value - 1, 3) + c1 * Mathf.Pow(value - 1, 2);
        }
        
        public static float EaseInOutBack(float value)
        {
            ValidateValue(value);
            const float c1 = 1.70158f;
            const float c2 = c1 * 1.525f;

            return value < 0.5f
                ? (Mathf.Pow(2 * value, 2) * ((c2 + 1) * 2 * value - c2)) / 2
                : (Mathf.Pow(2 * value - 2, 2) * ((c2 + 1) * (value * 2 - 2) + c2) + 2) / 2;
            
        }

        public static Func<float, float> GetEaseFunction(TweenType tweenType)
        {
            switch (tweenType)
            {
                case TweenType.EaseInOutSine: return EaseInOutSine;
                case TweenType.EaseInSine: return EaseInSine;
                case TweenType.EaseOutSine: return EaseOutSine;
                
                case TweenType.EaseInOutCubic: return EaseInOutCubic;
                case TweenType.EaseInCubic: return EaseInCubic;
                case TweenType.EaseOutCubic: return EaseOutCubic;
                
                case TweenType.EaseInOutBack: return EaseInOutBack;
                case TweenType.EaseInBack: return EaseInBack;
                case TweenType.EaseOutBack: return EaseOutBack;
                
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private static void ValidateValue(float value)
        {
            if(value < 0.0f || value > 1.0f)
                throw new ArgumentException("The easing value must be between 0.0 and 1.0", "value");
        }
    }
}