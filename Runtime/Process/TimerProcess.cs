using UnityEngine;

namespace Calluna.Process
{
    public class TimerProcess : ControllableProcessBase
    {
        private readonly float _duration;
        private float _targetTime = float.MinValue;

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
            if (Time.time > _targetTime)
            {
                FinishProcess();
            }
        }

        protected override void DoAbort()
        {
            _targetTime = float.MinValue;
        }

        protected override void UpdateProgress()
        {
            _progress.Value = 1 - (_targetTime - Time.time)/_duration;
        }
    }
}