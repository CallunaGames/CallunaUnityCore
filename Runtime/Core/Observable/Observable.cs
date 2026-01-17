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

        public bool HasValue => Value != null;

        private T _value;

        public void SetValueWithoutNotify(T newValue)
        {
            _value = newValue;
        }
        
        public static implicit operator Observable<T>(T value) => new(){_value = value};
        public static implicit operator T(Observable<T> value) => value.Value;
        
        public delegate void ValueChangedWithValues(T formerValue, T newValue);
    }
}
