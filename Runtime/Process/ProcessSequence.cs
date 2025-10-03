using System;
using System.Collections.Generic;
using System.Linq;

namespace Calluna.Process
{
    public class ProcessSequence : MutableProcessBase
    {
        private readonly IEnumerable<MutableProcess> _processes;

        private IEnumerator<MutableProcess> _currentProcessEnumerator;
        private readonly int _processCount;
        private int _index = -1;
        private readonly string _nameOverride;

        public ProcessSequence(IEnumerable<MutableProcess> subProcesses)
        {
            MutableProcess[] mutableProcesses = subProcesses as MutableProcess[] ?? subProcesses.ToArray();
            _processes = mutableProcesses;
            _processCount = mutableProcesses.Length;
        }

        public ProcessSequence(IEnumerable<MutableProcess> subProcesses, string name) : this(subProcesses)
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
            MutableProcess currentProcess = _currentProcessEnumerator.Current;

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
                _currentProcessEnumerator = null;
                _index = _processCount;
                FinishProcess();
            }
        }

        protected override void UpdateProgress()
        {
            float delta = 1 / (float)_processCount;
            _progress.Value = _index * delta + _currentProcessEnumerator?.Current?.Progress.Value ?? 0;
        }

        private string GetName()
        {
            string subProcessName = _currentProcessEnumerator?.Current?.Name.Value ?? string.Empty;
            string processName = _nameOverride ?? "Process Sequence";
            return $"{processName} ({_index + 1}/{_processCount}): {subProcessName}";
        }
    }
}