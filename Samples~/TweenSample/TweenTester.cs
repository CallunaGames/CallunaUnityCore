using System;
using UnityEngine;

namespace Calluna.Core.Samples.TweenSample
{
    public abstract class TweenTester : MonoBehaviour
    {
        [SerializeField] private float _duration = 2f;
        [SerializeField] private Transform _transform;
        [SerializeField] private Transform _start;
        [SerializeField] private Transform _end;

        private float _time = 0;
        private bool _increasing = true;

        private void Update()
        {
            if (_increasing)
            {
                _time += Time.deltaTime;
                if(_time >= _duration)
                    _increasing = false;
            }
            else
            {
                _time -= Time.deltaTime;
                if(_time <= 0)
                    _increasing = true;
            }
            
            float t = GetTweenValue(Mathf.Clamp01(_time / _duration));
            Vector3 distance = _end.position - _start.position;
            _transform.position = _start.position + distance * t;
        }

        protected abstract float GetTweenValue(float value);
    }
}
