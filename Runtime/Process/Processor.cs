using System;
using UnityEngine;

namespace Calluna.Process
{
    public class Processor : MonoBehaviour
    {
        public ReadonlyObservable<Process> CurrentProcess => _currentProcess;
        
        private readonly Observable<Process> _currentProcess = new();
        private MutableProcess _currentMutableProcess;

        public void Process(MutableProcess process)
        {
            if (_currentMutableProcess is { IsRunning: true })
            {
                throw new InvalidOperationException("Please abort the current process before starting the next.");
            }

            if (_currentMutableProcess != null)
            {
                Clean();
            }

            _currentMutableProcess = process;
            _currentProcess.Value = process;
            _currentMutableProcess.Start();
        }

        public void StopProcess()
        {
            if (_currentMutableProcess is not { IsRunning: true })
            {
                throw new InvalidOperationException(
                    "The current process is not running and therefore can not be stopped.");
            }

            _currentMutableProcess.Abort();
            Clean();
        }

        private void Update()
        {
            if (_currentMutableProcess is { IsRunning: true })
            {
                _currentMutableProcess.Tick();
            }
        }

        private void OnDestroy()
        {
            if (_currentMutableProcess is { IsRunning: true })
            {
                _currentMutableProcess.Abort();
            }
            Clean();
        }

        private void Clean()
        {
            _currentProcess.Value = null;
            _currentMutableProcess = null;
        }
    }
}