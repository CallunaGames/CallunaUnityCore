using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Calluna.Core.Tests
{
    public class UpdateSchedulerTests
    {
        private GameObject _gameObject;
        private UpdateScheduler _scheduler;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject("UpdateSchedulerTest");
            _scheduler = _gameObject.AddComponent<UpdateScheduler>();
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(_gameObject);
        }

        // ---- LateUpdate phase ----

        [UnityTest, Description("ScheduleOnce LateUpdate phase => Callback fires after one frame?")]
        public IEnumerator UpdateScheduler_ScheduleOnce_LateUpdatePhase_CallbackFiresAfterOneFrame()
        {
            int count = 0;
            _scheduler.ScheduleOnce("a", () => count++, UpdateScheduler.SchedulePhase.LateUpdate);

            yield return null;

            Assert.AreEqual(1, count);
        }

        [UnityTest, Description("ScheduleOnce default phase => Callback fires after one frame (default is LateUpdate)?")]
        public IEnumerator UpdateScheduler_ScheduleOnce_DefaultPhase_CallbackFiresAfterOneFrame()
        {
            int count = 0;
            _scheduler.ScheduleOnce("a", () => count++);

            yield return null;

            Assert.AreEqual(1, count);
        }

        // ---- Update phase ----

        [UnityTest, Description("ScheduleOnce Update phase => Callback fires after one frame?")]
        public IEnumerator UpdateScheduler_ScheduleOnce_UpdatePhase_CallbackFiresAfterOneFrame()
        {
            int count = 0;
            _scheduler.ScheduleOnce("a", () => count++, UpdateScheduler.SchedulePhase.Update);

            yield return null;

            Assert.AreEqual(1, count);
        }

        // ---- Deduplication ----

        [UnityTest, Description("ScheduleOnce same id twice before frame ends => Callback fires exactly once?")]
        public IEnumerator UpdateScheduler_ScheduleOnce_SameIdTwice_CallbackFiresOnce()
        {
            int count = 0;
            _scheduler.ScheduleOnce("a", () => count++);
            _scheduler.ScheduleOnce("a", () => count += 10);

            yield return null;

            Assert.AreEqual(1, count);
        }

        // ---- Multiple ids ----

        [UnityTest, Description("ScheduleOnce multiple distinct ids => All callbacks fire?")]
        public IEnumerator UpdateScheduler_ScheduleOnce_MultipleIds_AllCallbacksFire()
        {
            int countA = 0;
            int countB = 0;
            int countC = 0;
            _scheduler.ScheduleOnce("a", () => countA++);
            _scheduler.ScheduleOnce("b", () => countB++);
            _scheduler.ScheduleOnce("c", () => countC++);

            yield return null;

            Assert.AreEqual(1, countA, "Callback for id 'a' should fire once.");
            Assert.AreEqual(1, countB, "Callback for id 'b' should fire once.");
            Assert.AreEqual(1, countC, "Callback for id 'c' should fire once.");
        }

        // ---- Cancel ----

        [UnityTest, Description("Cancel before frame ends => Callback does not fire?")]
        public IEnumerator UpdateScheduler_Cancel_BeforeFrameEnds_CallbackDoesNotFire()
        {
            int count = 0;
            _scheduler.ScheduleOnce("a", () => count++);
            _scheduler.Cancel("a");

            yield return null;

            Assert.AreEqual(0, count);
        }

        [UnityTest, Description("Cancel unknown id => No error and other callbacks still fire?")]
        public IEnumerator UpdateScheduler_Cancel_UnknownId_OtherCallbacksStillFire()
        {
            int count = 0;
            _scheduler.ScheduleOnce("b", () => count++);
            Assert.DoesNotThrow(() => _scheduler.Cancel("unknown"));

            yield return null;

            Assert.AreEqual(1, count);
        }

        // ---- CancelAll ----

        [UnityTest, Description("CancelAll before frame ends => No callbacks fire?")]
        public IEnumerator UpdateScheduler_CancelAll_BeforeFrameEnds_NoCallbacksFire()
        {
            int countA = 0;
            int countB = 0;
            _scheduler.ScheduleOnce("a", () => countA++);
            _scheduler.ScheduleOnce("b", () => countB++);
            _scheduler.CancelAll();

            yield return null;

            Assert.AreEqual(0, countA, "Callback for id 'a' should not fire after CancelAll.");
            Assert.AreEqual(0, countB, "Callback for id 'b' should not fire after CancelAll.");
        }

        // ---- One-shot behaviour ----

        [UnityTest, Description("Callback fires => Not re-invoked the following frame (one-shot)?")]
        public IEnumerator UpdateScheduler_ScheduleOnce_AfterFiring_NotReInvokedNextFrame()
        {
            int count = 0;
            _scheduler.ScheduleOnce("a", () => count++);

            yield return null; // fires here
            yield return null; // must not fire again

            Assert.AreEqual(1, count);
        }

        // ---- Re-register after firing ----

        [UnityTest, Description("ScheduleOnce same id again after it fired => Fresh callback fires on next frame?")]
        public IEnumerator UpdateScheduler_ScheduleOnce_SameIdAfterFired_FreshCallbackFires()
        {
            int firstCount = 0;
            int secondCount = 0;

            _scheduler.ScheduleOnce("a", () => firstCount++);
            yield return null; // first callback fires

            _scheduler.ScheduleOnce("a", () => secondCount++);
            yield return null; // second callback fires

            Assert.AreEqual(1, firstCount, "First callback should have fired exactly once.");
            Assert.AreEqual(1, secondCount, "Second callback should fire after being re-registered.");
        }

        // ---- Phase isolation ----

        [UnityTest, Description("Update phase callback does not fire on LateUpdate cycle and vice versa?")]
        public IEnumerator UpdateScheduler_ScheduleOnce_PhasesAreIsolated_EachCallbackFiresOnce()
        {
            int updateCount = 0;
            int lateUpdateCount = 0;
            _scheduler.ScheduleOnce("upd", () => updateCount++, UpdateScheduler.SchedulePhase.Update);
            _scheduler.ScheduleOnce("late", () => lateUpdateCount++, UpdateScheduler.SchedulePhase.LateUpdate);

            yield return null;

            Assert.AreEqual(1, updateCount, "Update-phase callback should fire exactly once.");
            Assert.AreEqual(1, lateUpdateCount, "LateUpdate-phase callback should fire exactly once.");
        }
    }
}
