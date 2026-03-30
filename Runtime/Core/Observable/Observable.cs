namespace Calluna
{
    public abstract class Observable
    {
        public abstract event ValueChanged OnChanged;
        public delegate void ValueChanged();
    }
    
    public class Observable<T> : Observable, ReadonlyObservable<T>
    {
        public event ValueChangedWithValues OnChangedWithValues;
        public override event ValueChanged OnChanged;

        public T Value
        {
            get => _value;
            set
            {
                T former = _value;
                _value = value;
                OnChangedWithValues?.Invoke(former, _value);
                OnChanged?.Invoke();
            }
        }

        /// <summary>Returns true if the current value is non-null. Always true for value types.</summary>
        public bool HasValue => _value != null;

        private T _value;

        public void SetValueWithoutNotify(T value)
        {
            _value = value;
        }
        
        public static implicit operator Observable<T>(T value) => new(){_value = value};
        public static implicit operator T(Observable<T> value) => value.Value;
        
        public delegate void ValueChangedWithValues(T formerValue, T newValue);
    }
}
