using Calluna;

namespace Calluna.Core.Samples.TweenSample
{
    public class SineInTester : TweenTester
    {
        protected override float GetTweenValue(float value)
        {
            return Tween.EaseInSine(value);
        }
    }
}
