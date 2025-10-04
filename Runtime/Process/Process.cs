using System;
using System.Collections;

namespace Calluna.Process
{
    public interface Process
    {
        public event Action<Process> OnFinished;

        public bool IsFinished => Status.Value is ProcessStatus.Aborted or ProcessStatus.Failed or ProcessStatus.Done;
        public bool HasFailed => Status.Value is ProcessStatus.Failed;
        public bool IsPending => Status.Value is ProcessStatus.Pending;
        public bool IsRunning => Status.Value is ProcessStatus.Running;
        
        public ReadonlyObservable<string> Name { get; }
        public ReadonlyObservable<float> Progress { get; }
        public ReadonlyObservable<ProcessStatus> Status { get; }

        public IEnumerator Await();
    }
}