using UnityEngine;

namespace Calluna
{
    public class Vector3ValueTweener : ValueTweener<Vector3>
    {
        public Vector3ValueTweener(CoroutineHelper coroutineHelper) : base(coroutineHelper) { }

        protected override Vector3 CalculateNewValue(float t, Vector3 start, Vector3 end)
            => Vector3.LerpUnclamped(start, end, t);
    }
}
