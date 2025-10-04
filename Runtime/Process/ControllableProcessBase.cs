using System;

namespace Calluna.Process
{
    public abstract class ControllableProcessBase : ProcessBase, ControllableProcess
    {
        public override event Action<Process> OnFinished;
        
        public void Start()
        {
            if (_status.Value != ProcessStatus.Pending)
            {
                throw new InvalidOperationException("You can not start a process that is not pending");
            }

            DoStart();
            _status.Value = ProcessStatus.Running;
        }

        public void Tick()
        {
            if (_status.Value != ProcessStatus.Running)
            {
                throw new InvalidOperationException($"You can not call {nameof(Tick)} on a process that is not running.");
            }
            
            DoTick();
            UpdateProgress();
        }

        public void Abort()
        {
            if (_status.Value != ProcessStatus.Running)
            {
                throw new InvalidOperationException("You can not abort a process that is not running");
            }

            DoAbort();
            _status.Value = ProcessStatus.Aborted;
            OnFinished?.Invoke(this);
        }

        protected void FinishProcess()
        {
            _progress.Value = 1;
            _status.Value = ProcessStatus.Done;
            OnFinished?.Invoke(this);
        }
        
        protected void SetFailed()
        {
            _status.Value = ProcessStatus.Failed;
            OnFinished?.Invoke(this);
        }

        protected abstract void DoStart();
        protected abstract void DoTick();
        protected abstract void DoAbort();
        protected abstract void UpdateProgress();
    }
}