using System;
using UnityEngine;
using UnityEngine.UI;

namespace Calluna.Core.TimerExample
{
    public class TimerStarter : MonoBehaviour
    {
        [SerializeField] private float _timerDuration = 2.0f;
        [SerializeField] private Image _timerImage;

        private CoroutineHelper _coroutineHelper;
        private Timer _timer;
        private int _callCounter;
        
        private void Start()
        {
            _coroutineHelper = gameObject.AddComponent<CoroutineHelper>();
            _timer = new Timer(_coroutineHelper).StartWith(_timerDuration);
            _timer.OnDone += OnDone;
        }

        private void Update()
        {
            _timerImage.fillAmount = _timer.Percentage;
        }

        private void OnDestroy()
        {
            _timer.OnDone -= OnDone;
        }

        private void OnDone()
        {
            _callCounter++;
            Debug.Log($"Timer done for the {_callCounter} time");
            _timer.StartWith(_timerDuration);
        }
    }
}
