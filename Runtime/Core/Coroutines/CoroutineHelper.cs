using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calluna
{
    public class CoroutineHelper : MonoBehaviour
    {
        private Dictionary<string, CoroutinePair> _coroutines = new Dictionary<string, CoroutinePair>();
        
        private void OnDestroy()
        {
            StopAllCoroutines();
        }

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

        public bool HasRoutineWith(string id) => _coroutines.ContainsKey(id);

        public void ReplaceWithID(IEnumerator enumerator, string id)
        {
            StopWithID(id);
            StartWithID(enumerator, id);
        }

        // Waits for 'routine' to complete naturally, then removes it from the tracking dictionary.
        private IEnumerator WaitThenRemove(Coroutine routine, string id)
        {
            yield return routine;
            StopWithID(id);
        }

        private struct CoroutinePair
        {
            public Coroutine Routine;
            public Coroutine RemoveRoutine;
        }
    }
}
