using System;
using UnityEngine;

namespace Calluna.Process
{
    public class ProcessLogger : IDisposable
    {
        private const float _defaultLogFrequency = 0.5f;
        private const string _whiteHexcode = "FFFFFF";
        
        private readonly float _runningProcessLogFrequency;
        private readonly Process _process;
        private readonly string _sourceName;
        private readonly string _loggerTypeHexCode;
        private readonly string _statusHexCode;
        private float _nextLogTime = float.MaxValue;

        public ProcessLogger(Process process, string sourceName)
        {
            _process = process;
            _sourceName = sourceName;
            _process.Status.OnChanged += OnStatusChanged;
            _loggerTypeHexCode = _whiteHexcode;
            _statusHexCode = _whiteHexcode;
            _runningProcessLogFrequency = _defaultLogFrequency;
        }

        public ProcessLogger(Process process, string sourceName, float runningProcessLogFrequency, Color loggerTypeColor, Color statusColor) :
            this(process, sourceName)
        {
            _runningProcessLogFrequency = runningProcessLogFrequency;
            _loggerTypeHexCode = ColorUtility.ToHtmlStringRGB(loggerTypeColor);
            _statusHexCode = ColorUtility.ToHtmlStringRGB(statusColor);
        }

        public void Tick()
        {
            if (_process.IsRunning && Time.time > _nextLogTime)
            {
                Log();
                UpdateLogTime();
            }
        }

        public void Dispose()
        {
            _process.Status.OnChanged -= OnStatusChanged;
        }

        private void OnStatusChanged()
        {
            Log();
            if (_process.IsRunning)
            {
                UpdateLogTime();
            }
        }

        private void Log()
        {
            string progress = _process.IsRunning ? $" ({(int)(_process.Progress.Value * 100)}%)" : string.Empty;
            Debug.Log(
                $"<color=#{_loggerTypeHexCode}>[{_sourceName}]</color> {_process.Name.Value} <color=#{_statusHexCode}>({_process.Status.Value}{progress})</color>");
        }

        private void UpdateLogTime()
        {
            _nextLogTime = Time.time + _runningProcessLogFrequency;
        }
    }
}