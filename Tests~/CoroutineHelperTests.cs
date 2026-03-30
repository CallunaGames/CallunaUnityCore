using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

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
