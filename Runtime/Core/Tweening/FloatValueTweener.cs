using UnityEngine;

namespace Calluna
{
    public class FloatValueTweener : ValueTweener<float>
    {
        public FloatValueTweener(CoroutineHelper coroutineHelper) : base(coroutineHelper) { }

        protected override float CalculateNewValue(float t, float start, float end)
            => Mathf.LerpUnclamped(start, end, t);
    }
}
