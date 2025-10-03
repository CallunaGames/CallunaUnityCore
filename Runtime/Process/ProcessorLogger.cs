using UnityEngine;

namespace Calluna.Process
{
    public class ProcessorLogger : MonoBehaviour
    {
        [SerializeField] private Color _loggerTypeColor = Color.blue;
        [SerializeField] private Color _statusColor = Color.white;
        [SerializeField] private Processor _processor;

        private ProcessLogger _logger;
        
        private void OnEnable()
        {
            _processor.CurrentProcess.OnChangedWithValues += OnProcessChanged;
            CreateLogger(_processor.CurrentProcess.Value);
        }

        private void OnDisable()
        {
            _processor.CurrentProcess.OnChangedWithValues -= OnProcessChanged;
        }

        private void OnProcessChanged(Process formerValue, Process newValue)
        {
            _logger?.Dispose();
            CreateLogger(newValue);
        }

        private void CreateLogger(Process process)
        {
            if (process != null)
            {
                _logger = new ProcessLogger(process, _loggerTypeColor, _statusColor);
            }
        }
    }
}