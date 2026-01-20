namespace Calluna.Core.Samples.TweenSample
{
    public class CubicInTester : TweenTester
    {
        protected override float GetTweenValue(float value)
        {
            return Tween.EaseInCubic(value);
        }
    }
}