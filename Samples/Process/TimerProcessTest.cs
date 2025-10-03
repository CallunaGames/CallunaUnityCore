using UnityEngine;

namespace Calluna.Process.Samples
{
    public class TimerProcessTest : MonoBehaviour
    {
        [SerializeField, Header("Options")] private float _timerDuration = 3;
        [SerializeField] private string _timerName = "Timer Process Test";
        [SerializeField] private bool _useDefaultName = false;

        [SerializeField, Header("Dependencies")]
        private Processor _processor;

        private void Start()
        {
            MutableProcess process = _useDefaultName
                ? new TimerProcess(_timerDuration)
                : new TimerProcess(_timerDuration, _timerName);
            _processor.Process(process);
        }
    }
}