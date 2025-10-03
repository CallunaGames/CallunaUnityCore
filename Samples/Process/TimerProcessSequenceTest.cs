using UnityEngine;
using UnityEngine.Serialization;

namespace Calluna.Process.Samples
{
    public class TimerProcessSequenceTest : MonoBehaviour
    {
        [SerializeField, Header("Options")] private float _timerDuration = 3;
        [SerializeField] private int _amountOfSubprocesses = 3;
        [SerializeField] private string _processName = "Timer Sequence";
        [SerializeField] private bool _useDefaultName = false;

        [SerializeField, Header("Dependencies")]
        private Processor _processor;

        private void Start()
        {
            MutableProcess[] processes = new MutableProcess[_amountOfSubprocesses];
            
            for (int i = 0; i < _amountOfSubprocesses; i++)
            {
                processes[i] = new TimerProcess(_timerDuration);
            }

            MutableProcess process = _useDefaultName
                ? new ProcessSequence(processes)
                : new ProcessSequence(processes, _processName);
            _processor.Process(process);
        }
    }
}
