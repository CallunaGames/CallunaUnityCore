using System.Collections.Generic;
using NUnit.Framework;

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