using System;
using System.Collections;
using UnityEngine;

namespace Calluna
{
    public class Timer : IDisposable
    {
        public event Action OnDone;
        public float Progress => Running ? Mathf.Clamp01(Elapsed / _duration) : 1f;
        public float Elapsed { get; private set; }
        public bool Running { get; private set; }
        
        private readonly CoroutineSlot _slot;
        
        private float _duration;

        public Timer(CoroutineHelper coroutineHelper)
        {
            _slot = new CoroutineSlot(coroutineHelper);
        }

        public void Dispose() => Stop();

        public Timer StartWith(float duration, bool unscaled = false)
        {
            if (duration <= 0)
            {
                throw new ArgumentException("Duration must be greater than 0.");
            }

            Stop();
            _duration = duration;
            Running = true;
            _slot.Run(RunTimer(unscaled));
            return this;
        }

        public void Stop()
        {
            _slot.Stop();
            _duration = 0;
            Elapsed = 0;
            Running = false;
        }

        private IEnumerator RunTimer(bool unscaled)
        {
            while (Elapsed < _duration)
            {
                Elapsed += unscaled ? UnityEngine.Time.unscaledDeltaTime : UnityEngine.Time.deltaTime;
                yield return null;
            }
            
            Running = false;
            OnDone?.Invoke();
        }
    }
}
