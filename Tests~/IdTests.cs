using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace Calluna.Core.Tests
{
    public class IdTests
    {
        private class TestDefinition : ScriptableObjectId<TestDefinition> { }
        private class OtherDefinition : ScriptableObjectId<OtherDefinition> { }

        private readonly List<Object> _created = new List<Object>();

        [TearDown]
        public void TearDown()
        {
            foreach (Object created in _created)
                Object.DestroyImmediate(created);
            _created.Clear();
        }

        private T Create<T>(string id, string name = "asset") where T : ScriptableObjectId
        {
            T instance = ScriptableObject.CreateInstance<T>();
            instance.name = name;
            typeof(ScriptableObjectId)
                .GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(instance, id);
            _created.Add(instance);
            return instance;
        }

        // ---- Id<T> ----

        [Test, Description("Two ids with the same string => Equal, same hash?")]
        public void Id_SameString_EqualWithSameHash()
        {
            Id<TestDefinition> a = new Id<TestDefinition>("Knowledge");
            Id<TestDefinition> b = new Id<TestDefinition>(new string("Knowledge".ToCharArray()));

            Assert.IsTrue(a == b);
            Assert.IsFalse(a != b);
            Assert.IsTrue(a.Equals((object)b));
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test, Description("Ids differing in string or case => Not equal?")]
        [TestCase("Knowledge", "Charisma")]
        [TestCase("Knowledge", "knowledge")]
        public void Id_DifferentString_NotEqual(string first, string second)
        {
            Assert.AreNotEqual(new Id<TestDefinition>(first), new Id<TestDefinition>(second));
        }

        [Test, Description("Ids as dictionary keys => Found by an id with the same string?")]
        public void Id_AsDictionaryKey_FoundByEqualId()
        {
            Dictionary<Id<TestDefinition>, int> values = new Dictionary<Id<TestDefinition>, int>
            {
                [new Id<TestDefinition>("a")] = 1
            };
            Assert.AreEqual(1, values[new Id<TestDefinition>("a")]);
        }

        [Test, Description("Default and empty ids => Invalid; default equals only default?")]
        public void Id_DefaultAndEmpty_Invalid()
        {
            Assert.IsFalse(default(Id<TestDefinition>).IsValid);
            Assert.IsFalse(new Id<TestDefinition>("").IsValid);
            Assert.IsTrue(new Id<TestDefinition>("a").IsValid);
            Assert.AreEqual(default(Id<TestDefinition>), new Id<TestDefinition>(null));
            Assert.AreNotEqual(default(Id<TestDefinition>), new Id<TestDefinition>(""));
            Assert.AreEqual(string.Empty, default(Id<TestDefinition>).ToString());
        }

        // ---- ScriptableObjectId<TSelf>.Key ----

        [Test, Description("Two assets with the same id string => Equal keys?")]
        public void ScriptableObjectIdOfT_SameIdString_EqualKeys()
        {
            TestDefinition a = Create<TestDefinition>("Knowledge");
            TestDefinition b = Create<TestDefinition>("Knowledge");

            Assert.AreEqual(a.Key, b.Key);
            Assert.AreEqual(new Id<TestDefinition>("Knowledge"), a.Key);
        }

        [Test, Description("Key => Keeps using the serialized Id field?")]
        public void ScriptableObjectIdOfT_Key_UsesIdField()
        {
            TestDefinition definition = Create<TestDefinition>("Focus");
            Assert.AreEqual("Focus", definition.Id);
            Assert.AreEqual("Focus", definition.Key.Value);
        }

        // ---- ScriptableObjectIdValidation ----

        [Test, Description("Unique, non-empty ids => No problems?")]
        public void Validation_UniqueIds_NoProblems()
        {
            IReadOnlyList<string> problems = ScriptableObjectIdValidation.FindProblems(new[]
            {
                ((ScriptableObjectId)Create<TestDefinition>("a"), "a.asset"),
                (Create<TestDefinition>("b"), "b.asset"),
                // Same id on another type is fine.
                (Create<OtherDefinition>("a"), "other-a.asset"),
            });
            Assert.IsEmpty(problems);
        }

        [Test, Description("Empty id => Reported with its location?")]
        public void Validation_EmptyId_Reported()
        {
            IReadOnlyList<string> problems = ScriptableObjectIdValidation.FindProblems(new[]
            {
                ((ScriptableObjectId)Create<TestDefinition>("", "Nameless"), "nameless.asset"),
            });
            Assert.AreEqual(1, problems.Count);
            StringAssert.Contains("Nameless", problems[0]);
            StringAssert.Contains("nameless.asset", problems[0]);
        }

        [Test, Description("Duplicated id within a type => Reported once with both locations?")]
        public void Validation_DuplicateId_Reported()
        {
            IReadOnlyList<string> problems = ScriptableObjectIdValidation.FindProblems(new[]
            {
                ((ScriptableObjectId)Create<TestDefinition>("a"), "first.asset"),
                (Create<TestDefinition>("a"), "second.asset"),
                (Create<TestDefinition>("b"), "third.asset"),
            });
            Assert.AreEqual(1, problems.Count);
            StringAssert.Contains("first.asset", problems[0]);
            StringAssert.Contains("second.asset", problems[0]);
        }
    }
}
