using System;
using System.Collections;

namespace Calluna
{
    /// <summary>
    /// Runs at most one routine at a time on a <see cref="CoroutineHelper"/> - running a new one stops
    /// the former. Replaces the pattern of a class generating a unique id for
    /// <see cref="CoroutineHelper.ReplaceWithID"/>. Disposing stops the running routine.
    /// </summary>
    public sealed class CoroutineSlot : IDisposable
    {
        private readonly CoroutineHelper _coroutineHelper;
        private CoroutineHandle _handle;

        public bool IsRunning => _handle != null && _handle.IsRunning;

        public CoroutineSlot(CoroutineHelper coroutineHelper)
        {
            _coroutineHelper = coroutineHelper ?? throw new ArgumentNullException(nameof(coroutineHelper));
        }

        /// <summary>Stops the running routine, if any, and starts <paramref name="routine"/>.</summary>
        public CoroutineHandle Run(IEnumerator routine)
        {
            Stop();
            _handle = _coroutineHelper.Run(routine);
            return _handle;
        }

        public void Stop()
        {
            CoroutineHandle handle = _handle;
            _handle = null;
            handle?.Stop();
        }

        public void Dispose() => Stop();
    }
}
