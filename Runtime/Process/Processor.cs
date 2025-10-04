using System;
using UnityEngine;

namespace Calluna.Process
{
    public class Processor : MonoBehaviour
    {
        public ReadonlyObservable<Process> CurrentProcess => _currentProcess;
        
        private readonly Observable<Process> _currentProcess = new();
        private ControllableProcess _currentControllableProcess;

        private void Update()
        {
            TickCurrentProcess();
        }

        private void OnDestroy()
        {
            TryAbortCurrentProcess();
        }
        
        public void Process(ControllableProcess process)
        {
            if (_currentControllableProcess is { IsRunning: true })
            {
                throw new InvalidOperationException("Please abort the current process before starting the next.");
            }
            
            SetProcess(process);
            _currentControllableProcess?.Start();
        }

        public void AbortCurrentProcess()
        {
            if (_currentControllableProcess is not { IsRunning: true })
            {
                throw new InvalidOperationException(
                    "The current process is not running and therefore can not be stopped.");
            }

            _currentControllableProcess.Abort();
            Clean();
        }

        private void TickCurrentProcess()
        {
            if (_currentControllableProcess is { IsRunning: true })
            {
                _currentControllableProcess.Tick();
            }
        }

        private void TryAbortCurrentProcess()
        {
            if (_currentControllableProcess is { IsRunning: true })
            {
                _currentControllableProcess.Abort();
            }
        }

        private void Clean()
        {
            SetProcess(null);
        }

        private void SetProcess(ControllableProcess process)
        {
            _currentProcess.Value = process;
            _currentControllableProcess = process;
        }
    }
}