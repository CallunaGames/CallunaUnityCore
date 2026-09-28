using System;

namespace Calluna
{
    /// <summary>
    /// One contribution to an <see cref="AccumulatingValue{T}"/>, created by
    /// <see cref="AccumulatingValue{T}.AddPart()"/>. Its owner keeps it and switches its contribution on
    /// and off: setting <see cref="Value"/> contributes that value, <see cref="Clear"/> withdraws it
    /// again - the accumulated value then is the same as without this part. Disposing removes the part
    /// for good, e.g. when its owner outlives the accumulated value's user; setting the value of a
    /// disposed part throws.
    /// </summary>
    public sealed class AccumulatingValuePart<T> : IDisposable
    {
        private AccumulatingValue<T> _owner;
        private T _value;

        /// <summary>Whether the part contributes - true once <see cref="Value"/> was set, until <see cref="Clear"/>.</summary>
        public bool IsSet { get; private set; }

        public bool IsDisposed => _owner == null;

        /// <summary>The contributed value; default while the part isn't set.</summary>
        public T Value
        {
            get => _value;
            set
            {
                if (_owner == null)
                    throw new ObjectDisposedException(nameof(AccumulatingValuePart<T>));
                _value = value;
                IsSet = true;
                _owner.OnPartChanged();
            }
        }

        internal AccumulatingValuePart(AccumulatingValue<T> owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Withdraws the contribution until <see cref="Value"/> is set again. Does nothing if the part
        /// isn't set or was disposed.
        /// </summary>
        public void Clear()
        {
            if (!IsSet || _owner == null)
                return;
            _value = default;
            IsSet = false;
            _owner.OnPartChanged();
        }

        public void Dispose()
        {
            AccumulatingValue<T> owner = _owner;
            _owner = null;
            _value = default;
            IsSet = false;
            owner?.RemovePart(this);
        }

        internal void SetWithoutNotify(T value)
        {
            _value = value;
            IsSet = true;
        }
    }
}
