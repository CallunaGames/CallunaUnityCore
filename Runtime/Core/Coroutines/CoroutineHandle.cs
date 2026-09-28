using UnityEngine;

namespace Calluna
{
    /// <summary>
    /// A routine started via <see cref="CoroutineHelper.Run"/>. Stays valid after the routine ended -
    /// <see cref="IsRunning"/> then is false and <see cref="Stop"/> does nothing. Can be yielded inside
    /// another coroutine to wait until the routine ended.
    /// </summary>
    public sealed class CoroutineHandle : CustomYieldInstruction
    {
        private readonly CoroutineHelper _owner;

        internal Coroutine Coroutine { get; set; }

        /// <summary>
        /// False once the routine completed, threw an exception, was stopped, or its
        /// <see cref="CoroutineHelper"/> was destroyed.
        /// </summary>
        public bool IsRunning { get; private set; } = true;

        public override bool keepWaiting => IsRunning;

        internal CoroutineHandle(CoroutineHelper owner)
        {
            _owner = owner;
        }

        /// <summary>Stops the routine. Does nothing if it already ended.</summary>
        public void Stop()
        {
            if (IsRunning)
                _owner.Stop(this);
        }

        internal void MarkEnded()
        {
            IsRunning = false;
            Coroutine = null;
        }
    }
}
