using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calluna
{
    public class CoroutineHelper : MonoBehaviour
    {
        private Dictionary<string, CoroutinePair> _coroutines = new Dictionary<string, CoroutinePair>();
        private readonly HashSet<CoroutineHandle> _running = new HashSet<CoroutineHandle>();

        private void OnDestroy()
        {
            StopAllCoroutines();
            // Otherwise the stopped routines' ids would stay registered, and HasRoutineWith would
            // keep reporting them as running.
            _coroutines.Clear();
            foreach (CoroutineHandle handle in _running)
                handle.MarkEnded();
            _running.Clear();
        }

        /// <summary>
        /// Starts <paramref name="routine"/> and returns a handle to stop it or check whether it's
        /// still running. Like <see cref="MonoBehaviour.StartCoroutine(IEnumerator)"/>, the routine
        /// runs up to its first yield before this returns. To keep only one routine of a kind running,
        /// use a <see cref="CoroutineSlot"/>.
        /// </summary>
        public CoroutineHandle Run(IEnumerator routine)
        {
            if (routine == null)
                throw new ArgumentNullException(nameof(routine));

            CoroutineHandle handle = new CoroutineHandle(this);
            _running.Add(handle);
            try
            {
                Coroutine coroutine = StartCoroutine(Drive(routine, handle));
                // A routine ending before its first yield has already been marked as ended.
                if (handle.IsRunning)
                    handle.Coroutine = coroutine;
            }
            catch
            {
                End(handle);
                throw;
            }
            return handle;
        }

        internal void Stop(CoroutineHandle handle)
        {
            if (!_running.Contains(handle))
                return;
            if (handle.Coroutine != null)
                StopCoroutine(handle.Coroutine);
            End(handle);
        }

        // Steps the routine itself instead of handing it to StartCoroutine, so the handle learns about
        // the routine ending - also by an exception, which Unity would only log.
        private IEnumerator Drive(IEnumerator routine, CoroutineHandle handle)
        {
            while (true)
            {
                bool hasNext;
                try
                {
                    hasNext = routine.MoveNext();
                }
                catch (Exception e)
                {
                    Debug.LogException(e, this);
                    hasNext = false;
                }

                // The routine may have stopped itself (e.g. via its slot) while being stepped.
                if (!hasNext || !handle.IsRunning)
                    break;
                yield return routine.Current;
            }
            End(handle);
        }

        private void End(CoroutineHandle handle)
        {
            _running.Remove(handle);
            handle.MarkEnded();
        }

        [Obsolete("Use Run, which returns a CoroutineHandle, or a CoroutineSlot. Will be removed in 2.0.0.")]
        public void StartWithID(IEnumerator enumerator, string id)
        {
            if (_coroutines.ContainsKey(id))
            {
                throw new ArgumentException($"A routine with id {id} has already been started.");
            }

            Coroutine coroutine = StartCoroutine(enumerator);
            Coroutine removeCoroutine = StartCoroutine(WaitThenRemove(coroutine, id));
            _coroutines[id] = new CoroutinePair() { RemoveRoutine = removeCoroutine, Routine = coroutine };
        }

        [Obsolete("Use CoroutineHandle.Stop or CoroutineSlot.Stop. Will be removed in 2.0.0.")]
        public bool StopWithID(string id)
        {
            if (!_coroutines.Remove(id, out CoroutinePair coroutines))
            {
                return false;
            }

            if(coroutines.Routine != null)
                StopCoroutine(coroutines.Routine);
            if(coroutines.RemoveRoutine != null)
                StopCoroutine(coroutines.RemoveRoutine);
            return true;
        }

        [Obsolete("Use CoroutineHandle.IsRunning or CoroutineSlot.IsRunning. Will be removed in 2.0.0.")]
        public bool HasRoutineWith(string id) => _coroutines.ContainsKey(id);

        [Obsolete("Use CoroutineSlot.Run. Will be removed in 2.0.0.")]
        public void ReplaceWithID(IEnumerator enumerator, string id)
        {
#pragma warning disable CS0618
            StopWithID(id);
            StartWithID(enumerator, id);
#pragma warning restore CS0618
        }

        // Waits for 'routine' to complete naturally, then removes it from the tracking dictionary.
        private IEnumerator WaitThenRemove(Coroutine routine, string id)
        {
            yield return routine;
#pragma warning disable CS0618
            StopWithID(id);
#pragma warning restore CS0618
        }

        private struct CoroutinePair
        {
            public Coroutine Routine;
            public Coroutine RemoveRoutine;
        }
    }
}
