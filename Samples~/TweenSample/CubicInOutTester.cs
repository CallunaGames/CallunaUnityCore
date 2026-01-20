namespace Calluna.Core.Samples.TweenSample
{
    public class CubicInOutTester : TweenTester
    {
        protected override float GetTweenValue(float value)
        {
            return Tween.EaseInOutCubic(value);
        }
    }
}