using UnityEngine;

namespace Calluna
{
    public class Vector2ValueTweener : ValueTweener<Vector2>
    {
        public Vector2ValueTweener(CoroutineHelper coroutineHelper) : base(coroutineHelper) { }

        protected override Vector2 CalculateNewValue(float t, Vector2 start, Vector2 end)
            => Vector2.LerpUnclamped(start, end, t);
    }
}
