using System.Collections.Generic;

namespace Calluna
{
    public abstract class Observable
    {
        public abstract event ValueChanged OnChanged;
        public delegate void ValueChanged();
    }

    /// <summary>
    /// A value that notifies its listeners when it changes. Setting a value equal to the current one
    /// (per <see cref="EqualityComparer{T}.Default"/>) doesn't notify. Unity objects are compared by
    /// reference, since a destroyed one equals null and a change from it to null would be lost.
    /// Values that are mutated in place (e.g. arrays, lists) have to be replaced by a new instance to
    /// count as a change.
    /// </summary>
    public class Observable<T> : Observable, ReadonlyObservable<T>
    {
        private static readonly bool _isReferenceType = !typeof(T).IsValueType;

        public event ValueChangedWithValues OnChangedWithValues;
        public override event ValueChanged OnChanged;

        public T Value
        {
            get => _value;
            set
            {
                T former = _value;
                if (AreEqual(former, value))
                {
#if UNITY_EDITOR
                    if (ObservableDiagnostics.LogUnchangedValues && (OnChanged != null || OnChangedWithValues != null))
                        ObservableDiagnostics.ReportUnchangedValue(typeof(T), OnChanged, OnChangedWithValues);
#endif
                    return;
                }
                _value = value;
                OnChangedWithValues?.Invoke(former, _value);
                OnChanged?.Invoke();
            }
        }

        /// <summary>Returns true if the current value is non-null. Always true for value types.</summary>
        public bool HasValue => _value != null;

        private T _value;

        public void SetValueWithoutNotify(T value) => _value = value;

        public static implicit operator Observable<T>(T value) => new(){_value = value};
        public static implicit operator T(Observable<T> value) => value.Value;

        public delegate void ValueChangedWithValues(T formerValue, T newValue);

        private static bool AreEqual(T a, T b)
        {
            if (_isReferenceType && (a is UnityEngine.Object || b is UnityEngine.Object))
                return ReferenceEquals(a, b);
            return EqualityComparer<T>.Default.Equals(a, b);
        }
    }
}
