namespace Calluna
{
    public enum TweenType
    {
        EaseInOutSine = 0,
        EaseInSine = 1,
        EaseOutSine = 2,

        EaseInOutCubic = 3,
        EaseInCubic = 4,
        EaseOutCubic = 5,

        EaseInOutBack = 6,
        EaseInBack = 7,
        EaseOutBack = 8,

        Linear = 9,

        // Appended, so the serialized numbers of the types above stay the same.
        EaseInOutQuint = 10,
        EaseInQuint = 11,
        EaseOutQuint = 12
    }
}