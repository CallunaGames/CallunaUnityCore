using System;
using System.Collections;
using UnityEngine;

namespace Calluna
{
    /// <summary>
    /// Interpolates a value from start to end over a given duration using a
    /// <see cref="TweenType"/> easing function. Construct with a <see cref="CoroutineHelper"/>
    /// to enable animated tweens. Implements <see cref="IDisposable"/>: disposing stops any
    /// in-progress tween.
    /// </summary>
    public abstract class ValueTweener<TValue> : IDisposable
    {
        private readonly CoroutineHelper _coroutineHelper;
        private readonly string _id = Guid.NewGuid().ToString();

        public bool IsTweening { get; private set; }

        protected ValueTweener(CoroutineHelper coroutineHelper)
        {
            _coroutineHelper = coroutineHelper;
        }

        /// <summary>
        /// Starts interpolating from <paramref name="start"/> to <paramref name="end"/> over
        /// <paramref name="duration"/> seconds, invoking <paramref name="updateAction"/> each frame.
        /// When <paramref name="duration"/> is zero or negative, <paramref name="updateAction"/>
        /// is invoked immediately with <paramref name="end"/> and no coroutine is started.
        /// Any in-progress tween is cancelled before the new one begins.
        /// </summary>
        public void Perform(TValue start, TValue end, float duration, TweenType tweenType, Action<TValue> updateAction)
        {
            if (duration <= 0f)
            {
                updateAction(end);
                return;
            }
            _coroutineHelper.ReplaceWithID(DoTween(start, end, duration, tweenType, updateAction), _id);
        }

        public void Stop()
        {
            _coroutineHelper.StopWithID(_id);
            IsTweening = false;
        }

        public void Dispose() => Stop();

        private IEnumerator DoTween(TValue start, TValue end, float duration, TweenType tweenType, Action<TValue> updateAction)
        {
            IsTweening = true;
            float elapsed = 0f;
            // Cache the reciprocal so every frame uses a multiply instead of a divide.
            float invDuration = 1f / duration;
            Func<float, float> ease = Tween.GetEaseFunction(tweenType);

            while (true)
            {
                // Clamp elapsed to duration so the normalised t never exceeds 1,
                // preventing floating-point accumulation from overshooting the end value.
                elapsed = Mathf.Min(elapsed + Time.deltaTime, duration);
                updateAction(CalculateNewValue(ease(elapsed * invDuration), start, end));
                // Update before breaking so the callback always receives the final t=1 value.
                if (elapsed >= duration) break;
                yield return null;
            }

            IsTweening = false;
        }

        protected abstract TValue CalculateNewValue(float t, TValue start, TValue end);
    }
}
