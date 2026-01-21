using System;
using System.Collections;
using UnityEngine;

namespace Calluna
{
    public class Timer : IDisposable
    {
        public event Action OnDone;
        public float Percentage => Running ? Mathf.Clamp01(Time / _duration) : 1f;
        public float Time { get; private set; }
        public bool Running { get; private set; }
        
        private readonly CoroutineHelper _coroutineHelper;
        private readonly string _id;
        
        private float _duration;

        public Timer(CoroutineHelper coroutineHelper)
        {
            _coroutineHelper = coroutineHelper;
            _id = Guid.NewGuid().ToString();
        }

        public void Dispose()
        {
            StopTimer();
        }

        public Timer StartWith(float seconds, bool unscaled = false)
        {
            if (seconds <= 0)
            {
                throw new ArgumentException("Seconds must be greater than 0.");
            }
            
            StopTimer();
            _duration = seconds;
            Running = true;
            _coroutineHelper.StartWithID(StartTimer(unscaled), _id);
            return this;
        }

        public void StopTimer()
        {
            _coroutineHelper.StopWithID(_id);
            _duration = 0;
            Time = 0;
            Running = false;
        }

        private IEnumerator StartTimer(bool unscaled)
        {
            while (Time < _duration)
            {
                Time += unscaled ? UnityEngine.Time.unscaledDeltaTime : UnityEngine.Time.deltaTime;
                yield return null;
            }
            
            Running = false;
            OnDone?.Invoke();
        }
    }
}
