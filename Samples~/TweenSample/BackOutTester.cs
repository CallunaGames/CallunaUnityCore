namespace Calluna.Core.Samples.TweenSample
{
    public class BackOutTester : TweenTester
    {
        protected override float GetTweenValue(float value)
        {
            return Tween.EaseOutBack(value);
        }
    }
}