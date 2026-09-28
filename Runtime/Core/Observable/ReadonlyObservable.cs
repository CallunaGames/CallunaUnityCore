using System;

namespace Calluna
{
    public interface ReadonlyObservable<T>
    {
        public event Observable<T>.ValueChangedWithValues OnChangedWithValues;
        public event Observable<T>.ValueChanged OnChanged;
        T Value { get; }
        bool HasValue { get; }

        /// <inheritdoc cref="Observable{T}.Subscribe(Action)"/>
        IDisposable Subscribe(Action onChanged);

        /// <inheritdoc cref="Observable{T}.Subscribe(Action{T, T})"/>
        IDisposable Subscribe(Action<T, T> onChanged);
    }
}
