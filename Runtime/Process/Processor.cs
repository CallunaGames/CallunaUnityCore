using System;
using UnityEngine;

namespace Calluna.Process
{
    public class Processor : MonoBehaviour
    {
        public ReadonlyObservable<Process> CurrentProcess => _currentProcess;
        
        private readonly Observable<Process> _currentProcess = new();
        private ControllableProcess _currentControllableProcess;

        public void Process(ControllableProcess process)
        {
            if (_currentControllableProcess is { IsRunning: true })
            {
                throw new InvalidOperationException("Please abort the current process before starting the next.");
            }

            if (_currentControllableProcess != null)
            {
                Clean();
            }

            _currentControllableProcess = process;
            _currentProcess.Value = process;
            _currentControllableProcess.Start();
        }

        public void StopProcess()
        {
            if (_currentControllableProcess is not { IsRunning: true })
            {
                throw new InvalidOperationException(
                    "The current process is not running and therefore can not be stopped.");
            }

            _currentControllableProcess.Abort();
            Clean();
        }

        private void Update()
        {
            if (_currentControllableProcess is { IsRunning: true })
            {
                _currentControllableProcess.Tick();
            }
        }

        private void OnDestroy()
        {
            if (_currentControllableProcess is { IsRunning: true })
            {
                _currentControllableProcess.Abort();
            }
            Clean();
        }

        private void Clean()
        {
            _currentProcess.Value = null;
            _currentControllableProcess = null;
        }
    }
}