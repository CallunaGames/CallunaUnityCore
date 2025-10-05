using UnityEngine;

namespace Calluna.Process
{
    public class TimerProcess : ControllableProcessBase
    {
        private readonly float _duration;
        private float _targetTime;

        public TimerProcess(float duration)
        {
            _duration = duration;
            _name.Value = $"Timer ({duration:F1}s)";
        }

        public TimerProcess(float duration, string name) : this(duration)
        {
            _name.Value = name;
        }
        
        protected override void DoStart()
        {
            _targetTime = Time.time + _duration;
        }

        protected override void DoTick()
        {
            if (Time.time >= _targetTime)
            {
                FinishProcess();
            }
        }

        protected override void DoAbort()
        {
            _targetTime = float.MinValue;
        }

        protected override float GetProgress()
        {
            float timeLeft = _targetTime - Time.time;
            return 1 - timeLeft/_duration;
        }
    }
}