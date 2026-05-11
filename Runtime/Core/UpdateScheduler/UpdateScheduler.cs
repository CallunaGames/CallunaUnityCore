using System;
using System.Collections.Generic;
using UnityEngine;

namespace Calluna
{
    public class UpdateScheduler : MonoBehaviour
    {
        public enum SchedulePhase { Update, LateUpdate }

        private struct Entry
        {
            public Action Callback;
            public SchedulePhase Phase;
        }

        private readonly Dictionary<string, Entry> _pending = new();
        private readonly List<(string key, Action callback)> _frameCallbacks = new();

        // First registration wins; subsequent calls with the same id within the same frame are ignored.
        public void ScheduleOnce(string id, Action callback, SchedulePhase phase = SchedulePhase.LateUpdate)
        {
            _pending.TryAdd(id, new Entry { Callback = callback, Phase = phase });
        }

        public void Cancel(string id) => _pending.Remove(id);

        public void CancelAll() => _pending.Clear();

        private void Update() => Flush(SchedulePhase.Update);

        private void LateUpdate() => Flush(SchedulePhase.LateUpdate);

        private void Flush(SchedulePhase phase)
        {
            foreach (var (key, entry) in _pending)
                if (entry.Phase == phase)
                    _frameCallbacks.Add((key, entry.Callback));

            foreach (var (key, _) in _frameCallbacks)
                _pending.Remove(key);

            foreach (var (_, callback) in _frameCallbacks)
                callback?.Invoke();

            _frameCallbacks.Clear();
        }
    }
}
