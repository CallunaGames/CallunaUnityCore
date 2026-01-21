using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calluna
{
    public class CoroutineHelper : MonoBehaviour
    {
        private Dictionary<string, Routines> _coroutines = new Dictionary<string, Routines>();
        
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
            Coroutine removeCoroutine = StartCoroutine(StartRemove(coroutine, id));
            _coroutines[id] = new Routines() { RemoveRoutine = removeCoroutine, Routine = coroutine };
        }

        public bool StopWithID(string id)
        {
            if (!_coroutines.Remove(id, out Routines coroutines))
            {
                return false;
            }
            
            StopCoroutine(coroutines.Routine);
            StopCoroutine(coroutines.RemoveRoutine);
            return true;
        }

        public bool HasRoutineWith(string id)
        {
            return _coroutines.ContainsKey(id);
        }

        public void ReplaceWithID(IEnumerator enumerator, string id)
        {
            StopWithID(id);
            StartWithID(enumerator, id);
        }

        private IEnumerator StartRemove(Coroutine routine, string id)
        {
            yield return routine;
            StopWithID(id);
        }

        private struct Routines
        {
            public Coroutine Routine;
            public Coroutine RemoveRoutine;
        }
    }
}
