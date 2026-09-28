using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Calluna.Core.Tests
{
    public class SubscriptionTests
    {
        // ---- Subscription ----

        [Test, Description("Subscription disposed twice => Unsubscribes once?")]
        public void Subscription_DisposedTwice_UnsubscribesOnce()
        {
            int calls = 0;
            Subscription subscription = new Subscription(() => calls++);

            subscription.Dispose();
            subscription.Dispose();

            Assert.AreEqual(1, calls);
        }

        // ---- SubscriptionBag ----

        [Test, Description("SubscriptionBag disposed => Disposes all subscriptions in reverse order?")]
        public void SubscriptionBag_Disposed_DisposesAllInReverseOrder()
        {
            List<int> order = new List<int>();
            SubscriptionBag bag = new SubscriptionBag();
            bag.Add(new Subscription(() => order.Add(1)));
            bag.Add(new Subscription(() => order.Add(2)));
            bag.Add(new Subscription(() => order.Add(3)));

            bag.Dispose();

            CollectionAssert.AreEqual(new[] { 3, 2, 1 }, order);
        }

        [Test, Description("SubscriptionBag disposed => Empty and reusable?")]
        public void SubscriptionBag_Disposed_IsEmptyAndReusable()
        {
            int calls = 0;
            SubscriptionBag bag = new SubscriptionBag();
            bag.Add(new Subscription(() => calls++));
            bag.Dispose();

            bag.Add(new Subscription(() => calls++));
            Assert.AreEqual(1, bag.Count);
            bag.Dispose();

            Assert.AreEqual(2, calls);
            Assert.AreEqual(0, bag.Count);
        }

        [Test, Description("SubscriptionBag subscription throws on dispose => Logged, others still disposed?")]
        public void SubscriptionBag_SubscriptionThrows_LogsAndDisposesOthers()
        {
            int calls = 0;
            SubscriptionBag bag = new SubscriptionBag();
            bag.Add(new Subscription(() => calls++));
            bag.Add(new Subscription(() => throw new InvalidOperationException("dispose failed")));
            bag.Add(new Subscription(() => calls++));

            LogAssert.Expect(LogType.Exception, "InvalidOperationException: dispose failed");
            bag.Dispose();

            Assert.AreEqual(2, calls);
        }

        [Test, Description("SubscriptionBag Add null => Throws ArgumentNullException?")]
        public void SubscriptionBag_AddNull_Throws()
        {
            SubscriptionBag bag = new SubscriptionBag();
            Assert.Throws<ArgumentNullException>(() => bag.Add(null));
        }

        [Test, Description("SubscriptionBag Add => Returns the added subscription?")]
        public void SubscriptionBag_Add_ReturnsSubscription()
        {
            SubscriptionBag bag = new SubscriptionBag();
            Subscription subscription = new Subscription(() => { });
            Assert.AreSame(subscription, bag.Add(subscription));
        }
    }
}
