using System;

namespace Calluna
{
    /// <summary>
    /// Ends a subscription when disposed - returned by the Subscribe methods of this package. Disposing
    /// more than once is safe; only the first call unsubscribes.
    /// </summary>
    public sealed class Subscription : IDisposable
    {
        private Action _unsubscribe;

        public Subscription(Action unsubscribe)
        {
            _unsubscribe = unsubscribe;
        }

        public void Dispose()
        {
            Action unsubscribe = _unsubscribe;
            _unsubscribe = null;
            unsubscribe?.Invoke();
        }
    }
}
