namespace Calluna.Core.Samples.TweenSample
{
    public class CubicOutTester : TweenTester
    {
        protected override float GetTweenValue(float value)
        {
            return Tween.EaseOutCubic(value);
        }
    }
}