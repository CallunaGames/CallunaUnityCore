using System;
using System.Collections;

namespace Calluna.Process
{
    public abstract class ProcessBase : Process
    {
        public abstract event Action<Process> OnFinished;
        
        public ReadonlyObservable<string> Name => _name;
        public ReadonlyObservable<float> Progress => _progress;
        public ReadonlyObservable<ProcessStatus> Status => _status;

        protected readonly Observable<ProcessStatus> _status = new() { Value = ProcessStatus.Pending };
        protected readonly Observable<float> _progress = new() { Value = 0 };
        protected readonly Observable<string> _name = new() { Value = string.Empty };
        
        public IEnumerator Await()
        {
            if (_status.Value != ProcessStatus.Running)
            {
                yield break;
            }

            while (_status.Value is ProcessStatus.Running)
            {
                yield return null;
            }
        }
    }
}