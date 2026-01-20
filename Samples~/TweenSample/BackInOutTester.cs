namespace Calluna.Core.Samples.TweenSample
{
    public class BackInOutTester : TweenTester
    {
        protected override float GetTweenValue(float value)
        {
            return Tween.EaseInOutBack(value);
        }
    }
}