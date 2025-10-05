using System;
using System.Collections.Generic;
using System.Linq;

namespace Calluna.Process
{
    public class ProcessSequence : ControllableProcessBase
    {
        private readonly IEnumerable<ControllableProcess> _processes;
        private readonly int _processCount;
        private readonly string _nameOverride;
        
        private IEnumerator<ControllableProcess> _currentProcessEnumerator;
        private int _index = -1;

        public ProcessSequence(IEnumerable<ControllableProcess> subProcesses)
        {
            ControllableProcess[] mutableProcesses = subProcesses as ControllableProcess[] ?? subProcesses.ToArray();
            _processes = mutableProcesses;
            _processCount = mutableProcesses.Length;
        }

        public ProcessSequence(IEnumerable<ControllableProcess> subProcesses, string name) : this(subProcesses)
        {
            _nameOverride = name;
        }

        protected override void DoStart()
        {
            _currentProcessEnumerator = _processes.GetEnumerator();
            MoveToNextSubProcess();
        }

        protected override void DoTick()
        {
            ControllableProcess currentProcess = _currentProcessEnumerator.Current;

            if (currentProcess == null)
            {
                throw new InvalidOperationException("The enumerator points to null.");
            }

            if (currentProcess.IsPending)
            {
                currentProcess.Start();
            }

            if (currentProcess.IsRunning)
            {
                currentProcess.Tick();
            }
            else if (currentProcess.HasFailed)
            {
                SetFailed();
            }
            else if (currentProcess.IsFinished)
            {
                MoveToNextSubProcess();
            }
        }

        protected override void DoAbort()
        {
            _currentProcessEnumerator.Current?.Abort();
            _currentProcessEnumerator.Dispose();
        }

        private void MoveToNextSubProcess()
        {
            if (_currentProcessEnumerator.MoveNext())
            {
                _index++;
                _name.Value = GetName();
            }
            else
            {
                FinishSequence();
            }
        }

        protected override float GetProgress()
        {
            return _processCount > 0 ? CalculateProgress() : 0;
        }

        private void FinishSequence()
        {
            _currentProcessEnumerator.Dispose();
            _currentProcessEnumerator = null;
            _index = _processCount;
            FinishProcess();
        }

        private float CalculateProgress()
        {
            float delta = 1 / (float)_processCount;
            return _index * delta + _currentProcessEnumerator?.Current?.Progress.Value * delta ?? 0;
        }

        private string GetName()
        {
            string subProcessName = _currentProcessEnumerator?.Current?.Name.Value ?? string.Empty;
            string processName = _nameOverride ?? "Process Sequence";
            return $"{processName} ({_index + 1}/{_processCount}): {subProcessName}";
        }
    }
}