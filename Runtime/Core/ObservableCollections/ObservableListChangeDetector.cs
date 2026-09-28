using System;

namespace Calluna
{
    [Obsolete("Use ObservableList.SubscribeAny, which returns a subscription to dispose. Will be removed in 2.0.0.")]
    public class ObservableListChangeDetector<TValue> : IDisposable
    {
        public event Action OnChanged;

        private readonly IDisposable _subscription;

        public ObservableListChangeDetector(ReadonlyObservableList<TValue> list)
        {
            _subscription = list.SubscribeAny(RaiseOnChanged);
        }

        public void Dispose() => _subscription.Dispose();

        private void RaiseOnChanged() => OnChanged?.Invoke();
    }
}
