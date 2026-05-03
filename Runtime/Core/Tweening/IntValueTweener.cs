using UnityEngine;

namespace Calluna
{
    public class IntValueTweener : ValueTweener<int>
    {
        public IntValueTweener(CoroutineHelper coroutineHelper) : base(coroutineHelper) { }

        // RoundToInt snaps the continuous lerp to the nearest integer each frame,
        // producing discrete steps — expected behaviour for an integer tween.
        protected override int CalculateNewValue(float t, int start, int end)
            => Mathf.RoundToInt(Mathf.LerpUnclamped(start, end, t));
    }
}
