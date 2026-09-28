using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

// Also covers the obsolete implicit conversion until its removal in 2.0.0.
#pragma warning disable CS0618

namespace Calluna.Core.Tests
{
    public class ObservableTests
    {
        private static List<object> _testValues = new List<object>()
        {
            new TestValues<float>() { FormerValue = 0.1f, NewValue = 1.3f },
            new TestValues<string>() { FormerValue = "Former", NewValue = "New" },
            new TestValues<int>() { FormerValue = -23, NewValue = 555 },
            new TestValues<Foo>() { FormerValue = new Foo() { Value = 0 }, NewValue = new Foo() { Value = 73 } },
            new TestValues<char>() { FormerValue = 'i', NewValue = 'm' },
        };

        [Test, Description("Set value => Are former and new values of the callback as expected?")]
        public void Observable_OnChangedWithValues_ValuesOfCallback<T>(
            [ValueSource(nameof(_testValues))] TestValues<T> value)
        {
            Observable<T> observable = new Observable<T>() { Value = value.FormerValue };
            Observable<T>.ValueChangedWithValues listener = (T formerValue, T newValue) =>
            {
                Assert.AreEqual(value.FormerValue, formerValue);
                Assert.AreEqual(value.NewValue, newValue);
            };
            observable.OnChangedWithValues += listener;
            observable.Value = value.NewValue;
            observable.OnChangedWithValues -= listener;
        }

        [Test, Description("Set value => Value is as expected?")]
        public void Observable_SetValue_Value<T>(
            [ValueSource(nameof(_testValues))] TestValues<T> value)
        {
            Observable<T> observable = new Observable<T>() { Value = value.FormerValue };
            Assert.AreEqual(value.FormerValue, observable.Value);
            observable.Value = value.NewValue;
            Assert.AreEqual(value.NewValue, observable.Value);
        }

        [Test, Description("Set value => Is on changed called?")]
        public void Observable_SetValue_OnChangedCalled<T>(
            [ValueSource(nameof(_testValues))] TestValues<T> value)
        {
            Observable<T> observable = new Observable<T>() { Value = value.FormerValue };
            bool called = false;
            Observable<T>.ValueChanged listener = () => { called = true; };
            observable.OnChanged += listener;
            observable.Value = value.NewValue;
            observable.OnChanged -= listener;
            Assert.IsTrue(called);
        }

        [Test, Description("Set value without notify => Is value set without the events being called?")]
        public void Observable_SetValueWithoutNotify_NoCallbacksCalled<T>(
            [ValueSource(nameof(_testValues))] TestValues<T> value)
        {
            Observable<T> observable = new Observable<T>() { Value = value.FormerValue };
            bool called = false;

            Observable<T>.ValueChangedWithValues listenerWithValue = (T formerValue, T newValue) =>
            {
                called = true;
            };

            Observable<T>.ValueChanged listener = () => { called = true; };

            observable.OnChanged += listener;
            observable.OnChangedWithValues += listenerWithValue;
            observable.SetValueWithoutNotify(value.NewValue);
            observable.OnChanged -= listener;
            observable.OnChangedWithValues -= listenerWithValue;
            Assert.IsFalse(called);
        }

        [Test, Description("Set value implicitly => Is value set?")]
        public void Observable_SetValueImplicit_Value<T>(
            [ValueSource(nameof(_testValues))] TestValues<T> value)
        {
            Observable<T> observable = value.FormerValue;
            Assert.AreEqual(value.FormerValue, observable.Value);
        }

        [Test, Description("Get value implicitly => Is value as expected?")]
        public void Observable_GetValueImplicit_Value<T>(
            [ValueSource(nameof(_testValues))] TestValues<T> value)
        {
            Observable<T> observable = value.FormerValue;
            T result = observable.Value;
            Assert.AreEqual(value.FormerValue, result);
        }

        [Test, Description("HasValue when Value is non-null reference => Returns true?")]
        public void Observable_HasValue_NonNullValue_ReturnsTrue()
        {
            var observable = new Observable<string>();
            observable.Value = "hello";
            Assert.IsTrue(observable.HasValue);
        }

        [Test, Description("HasValue when Value is null => Returns false?")]
        public void Observable_HasValue_NullValue_ReturnsFalse()
        {
            var observable = new Observable<string>();
            observable.Value = null;
            Assert.IsFalse(observable.HasValue);
        }

        // ── Notify only on change ───────────────────────────────────────────────

        [Test, Description("Set the current value again => No notification?")]
        public void Observable_SetSameValue_DoesNotNotify<T>(
            [ValueSource(nameof(_testValues))] TestValues<T> value)
        {
            Observable<T> observable = new Observable<T>() { Value = value.FormerValue };
            int calls = 0;
            observable.OnChanged += () => calls++;
            observable.OnChangedWithValues += (_, _) => calls++;

            observable.Value = value.FormerValue;

            Assert.AreEqual(0, calls);
        }

        [Test, Description("Set an equal but different instance (value equality) => No notification?")]
        public void Observable_SetEqualInstance_DoesNotNotify()
        {
            Observable<string> observable = new Observable<string>() { Value = "text" };
            bool called = false;
            observable.OnChanged += () => called = true;

            observable.Value = new string("text".ToCharArray());

            Assert.IsFalse(called);
        }

        [Test, Description("Set another instance of a class without value equality => Notified?")]
        public void Observable_SetOtherInstanceWithoutValueEquality_Notifies()
        {
            Observable<Foo> observable = new Observable<Foo>() { Value = new Foo { Value = 1 } };
            bool called = false;
            observable.OnChanged += () => called = true;

            observable.Value = new Foo { Value = 1 };

            Assert.IsTrue(called);
        }

        [Test, Description("Mutate the held array and set it again => No notification (same instance)?")]
        public void Observable_SetMutatedSameArray_DoesNotNotify()
        {
            int[] values = { 1, 2 };
            Observable<int[]> observable = new Observable<int[]>() { Value = values };
            bool called = false;
            observable.OnChanged += () => called = true;

            values[0] = 5;
            observable.Value = values;

            Assert.IsFalse(called, "In-place changes need a new instance to count as a change");
        }

        [Test, Description("Change from a destroyed Unity object to null => Notified?")]
        public void Observable_SetNullAfterUnityObjectDestroyed_Notifies()
        {
            GameObject gameObject = new GameObject("ObservableTest");
            Observable<GameObject> observable = new Observable<GameObject>() { Value = gameObject };
            bool called = false;
            observable.OnChanged += () => called = true;
            Object.DestroyImmediate(gameObject);

            observable.Value = null;

            Assert.IsTrue(called, "A destroyed Unity object equals null, but is a different reference");
        }

        // ── Constructor / Subscribe ──────────────────────────────────────────────

        [Test, Description("Constructed with a value => Holds it?")]
        public void Observable_ConstructedWithValue_HoldsValue()
        {
            Observable<int> observable = new Observable<int>(5);
            Assert.AreEqual(5, observable.Value);
        }

        [Test, Description("Subscribe(Action) => Called on change, not after dispose?")]
        public void Observable_SubscribeAction_CalledUntilDisposed()
        {
            Observable<int> observable = new Observable<int>(0);
            int calls = 0;
            IDisposable subscription = observable.Subscribe(() => calls++);

            observable.Value = 1;
            subscription.Dispose();
            observable.Value = 2;

            Assert.AreEqual(1, calls);
        }

        [Test, Description("Subscribe(Action<T, T>) => Receives former and new value, not after dispose?")]
        public void Observable_SubscribeWithValues_ReceivesValuesUntilDisposed()
        {
            Observable<string> observable = new Observable<string>("a");
            List<(string, string)> received = new List<(string, string)>();
            IDisposable subscription = observable.Subscribe((former, next) => received.Add((former, next)));

            observable.Value = "b";
            subscription.Dispose();
            observable.Value = "c";

            CollectionAssert.AreEqual(new[] { ("a", "b") }, received);
        }

        [Test, Description("Subscribe same callback twice, dispose one => Other still called?")]
        public void Observable_SubscribeSameCallbackTwice_DisposeOne_OtherStillCalled()
        {
            Observable<int> observable = new Observable<int>(0);
            int calls = 0;
            Action callback = () => calls++;
            IDisposable first = observable.Subscribe(callback);
            observable.Subscribe(callback);

            first.Dispose();
            observable.Value = 1;

            Assert.AreEqual(1, calls);
        }

        [Test, Description("Subscribe via ReadonlyObservable => Called on change?")]
        public void Observable_SubscribeViaReadonlyInterface_Called()
        {
            Observable<int> observable = new Observable<int>(0);
            ReadonlyObservable<int> readonlyObservable = observable;
            int calls = 0;
            using (readonlyObservable.Subscribe(() => calls++))
                observable.Value = 1;
            observable.Value = 2;

            Assert.AreEqual(1, calls);
        }

        [Test, Description("Subscribe null => Throws ArgumentNullException?")]
        public void Observable_SubscribeNull_Throws()
        {
            Observable<int> observable = new Observable<int>();
            Assert.Throws<ArgumentNullException>(() => observable.Subscribe((Action)null));
            Assert.Throws<ArgumentNullException>(() => observable.Subscribe((Action<int, int>)null));
        }

#if UNITY_EDITOR
        // ── Diagnostics ──────────────────────────────────────────────────────────

        [Test, Description("Diagnostics on, same value set with listeners => Warning logged once per call site?")]
        public void ObservableDiagnostics_SameValueWithListeners_LogsOncePerSite()
        {
            ObservableDiagnostics.ResetReportedSites();
            ObservableDiagnostics.LogUnchangedValues = true;
            try
            {
                Observable<int> observable = new Observable<int>() { Value = 3 };
                observable.OnChanged += () => { };
                LogAssert.Expect(LogType.Warning, new Regex(@"Observable<Int32> set to its current value"));

                for (int i = 0; i < 3; i++)
                    observable.Value = 3;

                LogAssert.NoUnexpectedReceived();
            }
            finally
            {
                ObservableDiagnostics.LogUnchangedValues = false;
            }
        }

        [Test, Description("Diagnostics on, same value set without listeners => Nothing logged?")]
        public void ObservableDiagnostics_SameValueWithoutListeners_LogsNothing()
        {
            ObservableDiagnostics.ResetReportedSites();
            ObservableDiagnostics.LogUnchangedValues = true;
            try
            {
                Observable<int> observable = new Observable<int>() { Value = 3 };

                observable.Value = 3;

                LogAssert.NoUnexpectedReceived();
            }
            finally
            {
                ObservableDiagnostics.LogUnchangedValues = false;
            }
        }

        [Test, Description("Diagnostics on, listener added via Subscribe => Reports the subscribed callback, not its adapter?")]
        public void ObservableDiagnostics_SubscribedListener_ReportsCallbackOwner()
        {
            ObservableDiagnostics.ResetReportedSites();
            ObservableDiagnostics.LogUnchangedValues = true;
            try
            {
                Observable<int> observable = new Observable<int>(3);
                using (observable.Subscribe(OnDiagnosticsTestChanged))
                {
                    LogAssert.Expect(LogType.Warning, new Regex(@"no longer notifies: ObservableTests\.OnDiagnosticsTestChanged"));
                    observable.Value = 3;
                }
            }
            finally
            {
                ObservableDiagnostics.LogUnchangedValues = false;
            }
        }

        private static void OnDiagnosticsTestChanged() { }
#endif

        public struct TestValues<T>
        {
            public T FormerValue { get; set; }
            public T NewValue { get; set; }
        }

        public class Foo
        {
            public int Value { get; set; }
        }
    }
}