using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Calluna.Core.Tests
{
    public class TimerTests
    {
        private GameObject _gameObject;
        private CoroutineHelper _helper;
        private Timer _timer;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject("TimerTest");
            _helper = _gameObject.AddComponent<CoroutineHelper>();
            _timer = new Timer(_helper);
        }

        [TearDown]
        public void TearDown()
        {
            _timer.Dispose();
            Object.DestroyImmediate(_gameObject);
        }

        // ---- Input validation ----

        [Test, Description("StartWith zero duration => Throws ArgumentException?")]
        public void Timer_StartWith_ZeroDuration_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _timer.StartWith(0f));
        }

        [Test, Description("StartWith negative duration => Throws ArgumentException?")]
        public void Timer_StartWith_NegativeDuration_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _timer.StartWith(-1f));
        }

        // ---- Initial state after StartWith ----

        [UnityTest, Description("StartWith valid duration => Running is true?")]
        public IEnumerator Timer_StartWith_ValidDuration_RunningIsTrue()
        {
            _timer.StartWith(10f);
            Assert.IsTrue(_timer.Running);
            yield return null;
        }

        [UnityTest, Description("StartWith valid duration => Progress starts near zero?")]
        public IEnumerator Timer_StartWith_ValidDuration_ProgressNearZero()
        {
            _timer.StartWith(10f);
            Assert.Less(_timer.Progress, 0.1f);
            yield return null;
        }

        [UnityTest, Description("StartWith returns the Timer instance for fluent chaining?")]
        public IEnumerator Timer_StartWith_ReturnsSelf()
        {
            Timer result = _timer.StartWith(10f);
            Assert.AreSame(_timer, result);
            yield return null;
        }

        // ---- StopTimer ----

        [UnityTest, Description("StopTimer while running => Running false and Elapsed reset to zero?")]
        public IEnumerator Timer_StopTimer_WhileRunning_ResetsState()
        {
            _timer.StartWith(10f);
            yield return null;
            _timer.StopTimer();
            Assert.IsFalse(_timer.Running);
            Assert.AreEqual(0f, _timer.Elapsed);
        }

        // ---- Dispose ----

        [UnityTest, Description("Dispose while running => Running is false?")]
        public IEnumerator Timer_Dispose_WhileRunning_StopsTimer()
        {
            _timer.StartWith(10f);
            yield return null;
            _timer.Dispose();
            Assert.IsFalse(_timer.Running);
        }

        // ---- OnDone and completion ----

        [UnityTest, Description("Timer runs to completion => OnDone fires?")]
        public IEnumerator Timer_RunToCompletion_OnDoneFires()
        {
            bool doneFired = false;
            _timer.OnDone += () => doneFired = true;
            _timer.StartWith(0.05f);

            float timeout = Time.realtimeSinceStartup + 2f;
            while (!doneFired && Time.realtimeSinceStartup < timeout)
                yield return null;

            Assert.IsTrue(doneFired);
        }

        [UnityTest, Description("Timer runs to completion => Running is false?")]
        public IEnumerator Timer_RunToCompletion_RunningIsFalse()
        {
            _timer.StartWith(0.05f);

            float timeout = Time.realtimeSinceStartup + 2f;
            while (_timer.Running && Time.realtimeSinceStartup < timeout)
                yield return null;

            Assert.IsFalse(_timer.Running);
        }

        [UnityTest, Description("Timer runs to completion => Progress is 1?")]
        public IEnumerator Timer_RunToCompletion_ProgressIsOne()
        {
            _timer.StartWith(0.05f);

            float timeout = Time.realtimeSinceStartup + 2f;
            while (_timer.Running && Time.realtimeSinceStartup < timeout)
                yield return null;

            Assert.AreEqual(1f, _timer.Progress);
        }
    }
}
