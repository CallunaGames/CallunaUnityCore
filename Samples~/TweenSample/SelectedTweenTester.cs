using Calluna;
using UnityEngine;

namespace Calluna.Core.Samples.TweenSample
{
    public class SelectedTweenTester : TweenTester
    {
        [SerializeField] private TweenType _tweenType;
        
        protected override float GetTweenValue(float value)
        {
            return Tween.GetEaseFunction(_tweenType).Invoke(value);
        }
    }
}
