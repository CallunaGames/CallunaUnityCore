namespace Calluna
{
    public interface ReadonlyObservable<T>
    {
        public event Observable<T>.ValueChangedWithValues OnChangedWithValues;
        public event Observable<T>.ValueChanged OnChanged;
        T Value { get; }
        bool HasValue { get; }
    }
}
