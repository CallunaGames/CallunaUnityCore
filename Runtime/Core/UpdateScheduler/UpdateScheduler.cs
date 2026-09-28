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

        // Keyed by the callback itself, or by the id of the obsolete string API.
        private readonly Dictionary<object, Entry> _pending = new();
        private readonly List<(object key, Action callback)> _frameCallbacks = new();

        /// <summary>
        /// Runs <paramref name="callback"/> once in the given phase of this frame (or of the next frame,
        /// if that phase already ran). Scheduling the same callback again before it ran is ignored, so
        /// several changes in one frame cause a single call. Callbacks count as the same when they are
        /// equal delegates - the same method on the same instance, e.g. a method group like
        /// <c>ScheduleOnce(Rebuild)</c>. A lambda capturing variables is a new delegate on every call and
        /// therefore never deduplicated; keep it in a field instead.
        /// </summary>
        public void ScheduleOnce(Action callback, SchedulePhase phase = SchedulePhase.LateUpdate)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));
            _pending.TryAdd(callback, new Entry { Callback = callback, Phase = phase });
        }

        /// <summary>Removes <paramref name="callback"/> if it's scheduled and hasn't run yet.</summary>
        public void Cancel(Action callback)
        {
            if (callback != null)
                _pending.Remove(callback);
        }

        // First registration wins; subsequent calls with the same id within the same frame are ignored.
        [Obsolete("Use ScheduleOnce(Action, SchedulePhase), keyed by the callback. Will be removed in 2.0.0.")]
        public void ScheduleOnce(string id, Action callback, SchedulePhase phase = SchedulePhase.LateUpdate)
        {
            _pending.TryAdd(id, new Entry { Callback = callback, Phase = phase });
        }

        [Obsolete("Use Cancel(Action). Will be removed in 2.0.0.")]
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
