using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

// Also covers the obsolete string-id APIs until their removal in 2.0.0.
#pragma warning disable CS0618

namespace Calluna.Core.Tests
{
    public class CoroutineHelperTests
    {
        private GameObject _gameObject;
        private CoroutineHelper _helper;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject("CoroutineHelperTest");
            _helper = _gameObject.AddComponent<CoroutineHelper>();
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(_gameObject);
        }

        // ---- HasRoutineWith ----

        [Test, Description("HasRoutineWith unknown id => Returns false?")]
        public void CoroutineHelper_HasRoutineWith_UnknownId_ReturnsFalse()
        {
            Assert.IsFalse(_helper.HasRoutineWith("unknown"));
        }

        // ---- StopWithID ----

        [Test, Description("StopWithID unknown id => Returns false?")]
        public void CoroutineHelper_StopWithID_UnknownId_ReturnsFalse()
        {
            Assert.IsFalse(_helper.StopWithID("unknown"));
        }

        // ---- StartWithID ----

        [UnityTest, Description("StartWithID => HasRoutineWith returns true?")]
        public IEnumerator CoroutineHelper_StartWithID_HasRoutineWith_ReturnsTrue()
        {
            _helper.StartWithID(InfiniteRoutine(), "test");
            Assert.IsTrue(_helper.HasRoutineWith("test"));
            yield return null;
        }

        [UnityTest, Description("StartWithID duplicate id => Throws ArgumentException?")]
        public IEnumerator CoroutineHelper_StartWithID_DuplicateId_ThrowsArgumentException()
        {
            _helper.StartWithID(InfiniteRoutine(), "test");
            Assert.Throws<ArgumentException>(() => _helper.StartWithID(InfiniteRoutine(), "test"));
            yield return null;
        }

        // ---- StopWithID (with running routine) ----

        [UnityTest, Description("StopWithID running routine => Returns true and HasRoutineWith false?")]
        public IEnumerator CoroutineHelper_StopWithID_RunningRoutine_ReturnsTrueAndRemoves()
        {
            _helper.StartWithID(InfiniteRoutine(), "test");
            bool result = _helper.StopWithID("test");
            Assert.IsTrue(result);
            Assert.IsFalse(_helper.HasRoutineWith("test"));
            yield return null;
        }

        // ---- ReplaceWithID ----

        [UnityTest, Description("ReplaceWithID existing id => Does not throw?")]
        public IEnumerator CoroutineHelper_ReplaceWithID_ExistingId_DoesNotThrow()
        {
            _helper.StartWithID(InfiniteRoutine(), "test");
            Assert.DoesNotThrow(() => _helper.ReplaceWithID(InfiniteRoutine(), "test"));
            yield return null;
        }

        [UnityTest, Description("ReplaceWithID new id => Does not throw?")]
        public IEnumerator CoroutineHelper_ReplaceWithID_NewId_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _helper.ReplaceWithID(InfiniteRoutine(), "test"));
            yield return null;
        }

        // ---- Natural completion ----

        [UnityTest, Description("Routine completes naturally => Removed from tracking?")]
        public IEnumerator CoroutineHelper_RoutineCompletesNaturally_RemovedFromTracking()
        {
            _helper.StartWithID(SingleFrameRoutine(), "test");
            yield return null; // SingleFrameRoutine completes
            yield return null; // WaitThenRemove runs and removes entry
            yield return null; // safety margin
            Assert.IsFalse(_helper.HasRoutineWith("test"));
        }

        // ---- Destruction ----

        [UnityTest, Description("Helper destroyed while a routine runs => Routine no longer tracked?")]
        public IEnumerator CoroutineHelper_Destroyed_ForgetsRunningRoutines()
        {
            _helper.StartWithID(InfiniteRoutine(), "test");
            yield return null;

            UnityEngine.Object.DestroyImmediate(_helper);

            Assert.IsFalse(_helper.HasRoutineWith("test"));
        }

        // ---- Run / CoroutineHandle ----

        [Test, Description("Run => Handle is running?")]
        public void CoroutineHelper_Run_HandleIsRunning()
        {
            CoroutineHandle handle = _helper.Run(InfiniteRoutine());
            Assert.IsTrue(handle.IsRunning);
        }

        [Test, Description("Run => Routine runs up to its first yield immediately?")]
        public void CoroutineHelper_Run_StepsToFirstYieldImmediately()
        {
            int steps = 0;
            _helper.Run(CountingRoutine(() => steps++));
            Assert.AreEqual(1, steps);
        }

        [Test, Description("Run routine without yield => Handle not running?")]
        public void CoroutineHelper_Run_RoutineWithoutYield_HandleNotRunning()
        {
            CoroutineHandle handle = _helper.Run(EmptyRoutine());
            Assert.IsFalse(handle.IsRunning);
        }

        [UnityTest, Description("Routine completes => Handle not running?")]
        public IEnumerator CoroutineHelper_RoutineCompletes_HandleNotRunning()
        {
            CoroutineHandle handle = _helper.Run(SingleFrameRoutine());
            yield return null;
            yield return null;
            Assert.IsFalse(handle.IsRunning);
        }

        [UnityTest, Description("Handle stopped => Routine no longer stepped?")]
        public IEnumerator CoroutineHelper_HandleStopped_RoutineNoLongerStepped()
        {
            int steps = 0;
            CoroutineHandle handle = _helper.Run(CountingRoutine(() => steps++));
            yield return null;

            handle.Stop();
            int stepsWhenStopped = steps;
            yield return null;
            yield return null;

            Assert.IsFalse(handle.IsRunning);
            Assert.AreEqual(stepsWhenStopped, steps);
        }

        [Test, Description("Handle stopped twice => No exception?")]
        public void CoroutineHelper_HandleStoppedTwice_DoesNothing()
        {
            CoroutineHandle handle = _helper.Run(InfiniteRoutine());
            handle.Stop();
            Assert.DoesNotThrow(handle.Stop);
        }

        [UnityTest, Description("Routine throws => Exception logged, handle not running?")]
        public IEnumerator CoroutineHelper_RoutineThrows_LoggedAndHandleNotRunning()
        {
            CoroutineHandle handle = _helper.Run(ThrowingRoutine());
            LogAssert.Expect(LogType.Exception, "InvalidOperationException: routine failed");
            yield return null;
            yield return null;
            Assert.IsFalse(handle.IsRunning);
        }

        [UnityTest, Description("Helper destroyed => Handles not running?")]
        public IEnumerator CoroutineHelper_Destroyed_HandlesNotRunning()
        {
            CoroutineHandle handle = _helper.Run(InfiniteRoutine());
            yield return null;

            UnityEngine.Object.DestroyImmediate(_helper);

            Assert.IsFalse(handle.IsRunning);
        }

        [UnityTest, Description("Handle yielded in another coroutine => Waits until routine ended?")]
        public IEnumerator CoroutineHelper_HandleYielded_WaitsForRoutine()
        {
            bool done = false;
            CoroutineHandle handle = _helper.Run(FramesRoutine(3, () => done = true));

            yield return handle;

            Assert.IsTrue(done);
            Assert.IsFalse(handle.IsRunning);
        }

        [UnityTest, Description("Routine yielding a nested routine => Nested routine completes first?")]
        public IEnumerator CoroutineHelper_Run_NestedRoutine_Completes()
        {
            bool nestedDone = false;
            bool outerSawNestedDone = false;
            CoroutineHandle handle = _helper.Run(OuterRoutine(() => nestedDone = true, () => outerSawNestedDone = nestedDone));

            yield return handle;

            Assert.IsTrue(outerSawNestedDone);
        }

        [Test, Description("Run null => Throws ArgumentNullException?")]
        public void CoroutineHelper_RunNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _helper.Run(null));
        }

        // ---- CoroutineSlot ----

        [UnityTest, Description("Slot runs a second routine => First stopped?")]
        public IEnumerator CoroutineSlot_RunTwice_StopsFirst()
        {
            CoroutineSlot slot = new CoroutineSlot(_helper);
            int firstSteps = 0;
            CoroutineHandle first = slot.Run(CountingRoutine(() => firstSteps++));
            yield return null;

            CoroutineHandle second = slot.Run(InfiniteRoutine());
            int firstStepsWhenReplaced = firstSteps;
            yield return null;

            Assert.IsFalse(first.IsRunning);
            Assert.IsTrue(second.IsRunning);
            Assert.IsTrue(slot.IsRunning);
            Assert.AreEqual(firstStepsWhenReplaced, firstSteps);
        }

        [Test, Description("Slot disposed => Routine stopped?")]
        public void CoroutineSlot_Disposed_StopsRoutine()
        {
            CoroutineSlot slot = new CoroutineSlot(_helper);
            CoroutineHandle handle = slot.Run(InfiniteRoutine());

            slot.Dispose();

            Assert.IsFalse(handle.IsRunning);
            Assert.IsFalse(slot.IsRunning);
        }

        [UnityTest, Description("Slot routine completes => Slot not running?")]
        public IEnumerator CoroutineSlot_RoutineCompletes_NotRunning()
        {
            CoroutineSlot slot = new CoroutineSlot(_helper);
            slot.Run(SingleFrameRoutine());
            yield return null;
            yield return null;
            Assert.IsFalse(slot.IsRunning);
        }

        [UnityTest, Description("Routine replaces itself via its slot => Replacement runs, original stops?")]
        public IEnumerator CoroutineSlot_RoutineReplacesItself_ReplacementRuns()
        {
            CoroutineSlot slot = new CoroutineSlot(_helper);
            bool replacementRan = false;
            bool replaced = false;
            int originalStepsAfterReplace = 0;
            slot.Run(SelfReplacingRoutine(slot,
                () => replaced = true,
                () => { if (replaced) originalStepsAfterReplace++; },
                FramesRoutine(1, () => replacementRan = true)));

            yield return null;
            yield return null;
            yield return null;

            Assert.IsTrue(replacementRan);
            Assert.AreEqual(0, originalStepsAfterReplace);
        }

        private static IEnumerator EmptyRoutine()
        {
            yield break;
        }

        private static IEnumerator CountingRoutine(Action onStep)
        {
            while (true)
            {
                onStep();
                yield return null;
            }
        }

        private static IEnumerator FramesRoutine(int frames, Action onDone)
        {
            for (int i = 0; i < frames; i++)
                yield return null;
            onDone();
        }

        private static IEnumerator ThrowingRoutine()
        {
            yield return null;
            throw new InvalidOperationException("routine failed");
        }

        private static IEnumerator OuterRoutine(Action onNestedDone, Action onOuterDone)
        {
            yield return FramesRoutine(2, onNestedDone);
            onOuterDone();
        }

        // Replaces itself after one frame; would keep stepping on later frames if it weren't stopped.
        // (Code after slot.Run within the same step still runs - C# can't abort a method midway.)
        private static IEnumerator SelfReplacingRoutine(CoroutineSlot slot, Action onReplace, Action onStepAfter, IEnumerator replacement)
        {
            yield return null;
            onReplace();
            slot.Run(replacement);
            yield return null;
            while (true)
            {
                onStepAfter();
                yield return null;
            }
        }

        private static IEnumerator InfiniteRoutine()
        {
            while (true) yield return null;
        }

        private static IEnumerator SingleFrameRoutine()
        {
            yield return null;
        }
    }
}
