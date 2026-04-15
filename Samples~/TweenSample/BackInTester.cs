using Calluna;

namespace Calluna.Core.Samples.TweenSample
{
    public class BackInTester : TweenTester
    {
        protected override float GetTweenValue(float value)
        {
            return Tween.EaseInBack(value);
        }
    }
}