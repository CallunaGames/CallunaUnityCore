using System;

namespace Calluna
{
    /// <summary>
    /// One contribution to an <see cref="AccumulatingValue{T}"/>, created by
    /// <see cref="AccumulatingValue{T}.AddPart"/>. Setting <see cref="Value"/> recalculates the
    /// accumulated value; disposing removes the part from it. Setting the value of a disposed part throws.
    /// </summary>
    public sealed class AccumulatingValuePart<T> : IDisposable
    {
        private AccumulatingValue<T> _owner;
        private T _value;

        public bool IsDisposed => _owner == null;

        public T Value
        {
            get => _value;
            set
            {
                if (_owner == null)
                    throw new ObjectDisposedException(nameof(AccumulatingValuePart<T>));
                _value = value;
                _owner.OnPartChanged();
            }
        }

        internal AccumulatingValuePart(AccumulatingValue<T> owner, T value)
        {
            _owner = owner;
            _value = value;
        }

        public void Dispose()
        {
            AccumulatingValue<T> owner = _owner;
            _owner = null;
            owner?.RemovePart(this);
        }
    }
}
