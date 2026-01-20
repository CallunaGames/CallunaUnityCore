namespace Calluna.Core.Samples.TweenSample
{
    public class SineInOutTester : TweenTester
    {
        protected override float GetTweenValue(float value)
        {
            return Tween.EaseInOutSine(value);
        }
    }
}