using System;
using System.Collections.Generic;

namespace Calluna
{
    internal interface WrappedListener
    {
        Delegate Listener { get; }
    }

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

        public Observable() { }

        public Observable(T value)
        {
            _value = value;
        }

        public void SetValueWithoutNotify(T value) => _value = value;

        /// <summary>
        /// Calls <paramref name="onChanged"/> after every change until the returned subscription is disposed.
        /// </summary>
        public IDisposable Subscribe(Action onChanged)
        {
            if (onChanged == null)
                throw new ArgumentNullException(nameof(onChanged));
            ValueChanged handler = new ChangedListener(onChanged).Invoke;
            OnChanged += handler;
            return new Subscription(() => OnChanged -= handler);
        }

        /// <summary>
        /// Calls <paramref name="onChanged"/> with the former and the new value after every change until
        /// the returned subscription is disposed.
        /// </summary>
        public IDisposable Subscribe(Action<T, T> onChanged)
        {
            if (onChanged == null)
                throw new ArgumentNullException(nameof(onChanged));
            ValueChangedWithValues handler = new ChangedWithValuesListener(onChanged).Invoke;
            OnChangedWithValues += handler;
            return new Subscription(() => OnChangedWithValues -= handler);
        }

        [Obsolete("Use new Observable<T>(value) instead. The implicit conversion hides that a new observable - without the former one's listeners - is created. Will be removed in 2.0.0.")]
        public static implicit operator Observable<T>(T value) => new(value);
        public static implicit operator T(Observable<T> value) => value.Value;

        public delegate void ValueChangedWithValues(T formerValue, T newValue);

        // Adapt the Subscribe callbacks to the event delegate types. Named rather than lambdas, so
        // ObservableDiagnostics can report the subscribed callback instead of the adapter.
        private sealed class ChangedListener : WrappedListener
        {
            private readonly Action _listener;
            public ChangedListener(Action listener) => _listener = listener;
            public Delegate Listener => _listener;
            public void Invoke() => _listener();
        }

        private sealed class ChangedWithValuesListener : WrappedListener
        {
            private readonly Action<T, T> _listener;
            public ChangedWithValuesListener(Action<T, T> listener) => _listener = listener;
            public Delegate Listener => _listener;
            public void Invoke(T formerValue, T newValue) => _listener(formerValue, newValue);
        }

        private static bool AreEqual(T a, T b)
        {
            if (_isReferenceType && (a is UnityEngine.Object || b is UnityEngine.Object))
                return ReferenceEquals(a, b);
            return EqualityComparer<T>.Default.Equals(a, b);
        }
    }
}
