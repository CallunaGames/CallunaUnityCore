using System.Collections.Generic;
using NUnit.Framework;

namespace Calluna.Core.Tests
{
    public class EventBusTests
    {
        private IEventBus _bus;

        [SetUp]
        public void SetUp() => _bus = new EventBus();

        // ── Subscribe / Publish ──────────────────────────────────────────────────

        [Test]
        public void EventBus_Subscribe_HandlerReceivesPublishedEvent()
        {
            bool received = false;
            _bus.Subscribe<EventA>(_ => received = true);

            _bus.Publish(new EventA());

            Assert.IsTrue(received);
        }

        [Test]
        public void EventBus_Subscribe_MultipleHandlers_AllHandlersCalled()
        {
            int callCount = 0;
            _bus.Subscribe<EventA>(_ => callCount++);
            _bus.Subscribe<EventA>(_ => callCount++);

            _bus.Publish(new EventA());

            Assert.AreEqual(2, callCount);
        }

        [Test]
        public void EventBus_Subscribe_DifferentEventTypes_OnlyMatchingHandlerCalled()
        {
            bool aReceived = false;
            bool bReceived = false;
            _bus.Subscribe<EventA>(_ => aReceived = true);
            _bus.Subscribe<EventB>(_ => bReceived = true);

            _bus.Publish(new EventA());

            Assert.IsTrue(aReceived,  "EventA handler must be called");
            Assert.IsFalse(bReceived, "EventB handler must not be called when EventA is published");
        }

        // ── Unsubscribe ──────────────────────────────────────────────────────────

        [Test]
        public void EventBus_Unsubscribe_HandlerNotCalledAfterUnsubscribe()
        {
            int callCount = 0;
            void Handler(EventA _) => callCount++;
            _bus.Subscribe<EventA>(Handler);
            _bus.Unsubscribe<EventA>(Handler);

            _bus.Publish(new EventA());

            Assert.AreEqual(0, callCount);
        }

        [Test]
        public void EventBus_Unsubscribe_NonSubscribedHandler_NoException()
        {
            Assert.DoesNotThrow(() => _bus.Unsubscribe<EventA>(_ => { }));
        }

        [Test]
        [Description("Subscribe same handler instance twice => Handler called exactly twice per publish?")]
        public void EventBus_Subscribe_SameHandlerTwice_CalledTwicePerPublish()
        {
            int callCount = 0;
            void Handler(EventA _) => callCount++;

            _bus.Subscribe<EventA>(Handler);
            _bus.Subscribe<EventA>(Handler);

            _bus.Publish(new EventA());

            _bus.Unsubscribe<EventA>(Handler);
            _bus.Unsubscribe<EventA>(Handler);

            Assert.AreEqual(2, callCount);
        }

        // ── Edge cases ───────────────────────────────────────────────────────────

        [Test]
        public void EventBus_Publish_NoSubscribers_NoException()
        {
            Assert.DoesNotThrow(() => _bus.Publish(new EventA()));
        }

        // ── Re-entrancy / breadth-first ordering ─────────────────────────────────

        [Test]
        public void EventBus_Publish_ReentrantPublish_BreadthFirstOrder()
        {
            // HandlerA1 publishes EventB during EventA dispatch.
            // Expected breadth-first order: HandlerA1, HandlerA2, HandlerB.
            var order = new List<string>();

            _bus.Subscribe<EventA>(_ =>
            {
                order.Add("A1");
                _bus.Publish(new EventB()); // re-entrant — must queue, not recurse
            });
            _bus.Subscribe<EventA>(_ => order.Add("A2"));
            _bus.Subscribe<EventB>(_ => order.Add("B"));

            _bus.Publish(new EventA());

            Assert.AreEqual(new List<string> { "A1", "A2", "B" }, order,
                "Re-entrant publish must queue to the back; A2 must run before B");
        }

        [Test]
        public void EventBus_Publish_ChainPublish_AllEventsDelivered()
        {
            // A → publishes B → publishes C; all three handlers must eventually run.
            var received = new List<string>();

            _bus.Subscribe<EventA>(_ => { received.Add("A"); _bus.Publish(new EventB()); });
            _bus.Subscribe<EventB>(_ => { received.Add("B"); _bus.Publish(new EventC()); });
            _bus.Subscribe<EventC>(_ => received.Add("C"));

            _bus.Publish(new EventA());

            Assert.AreEqual(new List<string> { "A", "B", "C" }, received);
        }

        // ── Payload integrity ────────────────────────────────────────────────────

        [Test]
        public void EventBus_Publish_ValueTypeEvent_PayloadPreserved()
        {
            var received = new ValueEvent();
            _bus.Subscribe<ValueEvent>(e => received = e);

            _bus.Publish(new ValueEvent { Number = 42, Text = "hello" });

            Assert.AreEqual(42,      received.Number);
            Assert.AreEqual("hello", received.Text);
        }

        [Test]
        public void EventBus_Publish_ReferenceTypeEvent_SameInstance()
        {
            EventA published = new EventA();
            EventA received  = null;
            _bus.Subscribe<EventA>(e => received = e);

            _bus.Publish(published);

            Assert.AreSame(published, received);
        }

        // ── Test event types ─────────────────────────────────────────────────────

        private class EventA { }
        private class EventB { }
        private class EventC { }

        private struct ValueEvent
        {
            public int    Number;
            public string Text;
        }
    }
}
