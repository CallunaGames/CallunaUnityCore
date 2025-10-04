using UnityEngine;

namespace Calluna.Process
{
    public class ProcessorLogger : MonoBehaviour
    {
        [SerializeField, Header("Options")] private string _loggerName;
        [SerializeField] private float _runningProcessLogFrequency = 0.5f;
        [SerializeField, Header("Text colors")] private Color _loggerTypeColor = Color.blue;
        [SerializeField] private Color _statusColor = Color.white;
        [SerializeField, Header("Dependencies")] private Processor _processor;

        private ProcessLogger _logger;

        private void Reset()
        {
            _loggerName = gameObject.name;
            _processor = GetComponent<Processor>();
        }

        private void OnEnable()
        {
            _processor.CurrentProcess.OnChangedWithValues += OnProcessChanged;
            CreateLogger(_processor.CurrentProcess.Value);
        }

        private void Update()
        {
            _logger?.Tick();
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
                _logger = new ProcessLogger(process, _loggerName, _runningProcessLogFrequency, _loggerTypeColor, _statusColor);
            }
        }
    }
}