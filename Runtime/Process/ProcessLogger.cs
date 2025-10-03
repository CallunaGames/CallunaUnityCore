using System;
using UnityEngine;

namespace Calluna.Process
{
    public class ProcessLogger : IDisposable
    {
        private readonly Process _process;
        private readonly string _loggerTypeHexCode;
        private readonly string _statusHexCode;

        public ProcessLogger(Process process)
        {
            _process = process;
            _process.Status.OnChanged += OnStatusChanged;
            _process.Name.OnChanged += OnNameChanged;
            _loggerTypeHexCode = "FFFFFF";
            _statusHexCode = "FFFFFF";
        }

        public ProcessLogger(Process process, Color loggerTypeColor, Color statusColor) : this(process)
        {
            _loggerTypeHexCode = ColorUtility.ToHtmlStringRGB(loggerTypeColor);
            _statusHexCode = ColorUtility.ToHtmlStringRGB(statusColor);
        }

        public void Dispose()
        {
            _process.Status.OnChanged -= OnStatusChanged;
            _process.Name.OnChanged -= OnNameChanged;
        }

        private void OnStatusChanged()
        {
            Log();
        }

        private void OnNameChanged()
        {
            Log();
        }

        private void Log()
        {
            Debug.Log($"<color=#{_loggerTypeHexCode}>[Process Logger]</color> <color=#{_statusHexCode}>[{_process.Status.Value}]</color> {_process.Name.Value}");
        }
    }
}