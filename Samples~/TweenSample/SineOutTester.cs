using UnityEngine;
using Calluna;

namespace Calluna.Core.Samples.TweenSample
{
    public class SineOutTester : TweenTester
    {
        protected override float GetTweenValue(float value)
        {
            return Tween.EaseOutSine(value);
        }
    }
}
