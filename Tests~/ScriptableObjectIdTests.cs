using NUnit.Framework;
using UnityEngine;

namespace Calluna.Core.Tests
{
    public class ScriptableObjectIdTests
    {
        // Minimal concrete subclass — required because ScriptableObjectId is abstract.
        private class TestId : ScriptableObjectId { }

        // Creates a TestId and sets the backing field of the Id property via reflection.
        private static TestId CreateWithId(string id)
        {
            var instance = ScriptableObject.CreateInstance<TestId>();
            var field = typeof(ScriptableObjectId).GetField(
                "<Id>k__BackingField",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(instance, id);
            return instance;
        }

        // ---- Equals(ScriptableObjectId) ----

        [Test, Description("Equals(ScriptableObjectId) same reference => Returns true?")]
        public void ScriptableObjectId_EqualsScriptableObjectId_SameReference_ReturnsTrue()
        {
            var a = CreateWithId("alpha");
            Assert.IsTrue(a.Equals(a));
            Object.DestroyImmediate(a);
        }

        [Test, Description("Equals(ScriptableObjectId) different instances, same Id => Returns false?")]
        [TestCase("alpha")]
        [TestCase("beta")]
        [TestCase("")]
        public void ScriptableObjectId_EqualsScriptableObjectId_DifferentInstancesSameId_ReturnsFalse(string id)
        {
            var a = CreateWithId(id);
            var b = CreateWithId(id);
            Assert.IsFalse(a.Equals(b));
            Object.DestroyImmediate(a);
            Object.DestroyImmediate(b);
        }

        // ---- Equals(string) ----

        [Test, Description("Equals(string) matching Id => Returns true?")]
        [TestCase("alpha")]
        [TestCase("my-id")]
        [TestCase("")]
        public void ScriptableObjectId_EqualsString_MatchingId_ReturnsTrue(string id)
        {
            var a = CreateWithId(id);
            Assert.IsTrue(a.Equals(id));
            Object.DestroyImmediate(a);
        }

        [Test, Description("Equals(string) non-matching Id => Returns false?")]
        [TestCase("alpha", "beta")]
        [TestCase("x", "y")]
        [TestCase("", "something")]
        public void ScriptableObjectId_EqualsString_NonMatchingId_ReturnsFalse(string id, string other)
        {
            var a = CreateWithId(id);
            Assert.IsFalse(a.Equals(other));
            Object.DestroyImmediate(a);
        }

        // ---- Equals(object) ----

        [Test, Description("Equals(object) self ref => Returns true?")]
        public void ScriptableObjectId_EqualsObject_SelfRef_ReturnsTrue()
        {
            var a = CreateWithId("id1");
            Assert.IsTrue(a.Equals((object)a));
            Object.DestroyImmediate(a);
        }

        [Test, Description("Equals(object) string of same Id => Returns true?")]
        [TestCase("id1")]
        [TestCase("hello")]
        [TestCase("")]
        public void ScriptableObjectId_EqualsObject_StringOfSameId_ReturnsTrue(string id)
        {
            var a = CreateWithId(id);
            Assert.IsTrue(a.Equals((object)id));
            Object.DestroyImmediate(a);
        }

        [Test, Description("Equals(object) null => Returns false?")]
        public void ScriptableObjectId_EqualsObject_Null_ReturnsFalse()
        {
            var a = CreateWithId("id1");
            Assert.IsFalse(a.Equals((object)null));
            Object.DestroyImmediate(a);
        }

        // ---- operator== (two ScriptableObjectId) ----

        [Test, Description("operator== two ScriptableObjectId same ref => Returns true?")]
        public void ScriptableObjectId_OperatorEqual_SameRef_ReturnsTrue()
        {
            var a = CreateWithId("alpha");
            ScriptableObjectId b = a;
            Assert.IsTrue(a == b);
            Object.DestroyImmediate(a);
        }

        [Test, Description("operator== two ScriptableObjectId different instances => Returns false?")]
        [TestCase("alpha")]
        [TestCase("beta")]
        [TestCase("same-id")]
        public void ScriptableObjectId_OperatorEqual_DifferentInstances_ReturnsFalse(string id)
        {
            var a = CreateWithId(id);
            var b = CreateWithId(id);
            Assert.IsFalse(a == b);
            Object.DestroyImmediate(a);
            Object.DestroyImmediate(b);
        }

        // ---- operator== (ScriptableObjectId and string) ----

        [Test, Description("operator== ScriptableObjectId and matching string => Returns true?")]
        [TestCase("alpha")]
        [TestCase("my-id")]
        [TestCase("")]
        public void ScriptableObjectId_OperatorEqualString_MatchingId_ReturnsTrue(string id)
        {
            var a = CreateWithId(id);
            Assert.IsTrue(a == id);
            Object.DestroyImmediate(a);
        }

        [Test, Description("operator== ScriptableObjectId and non-matching string => Returns false?")]
        [TestCase("alpha", "beta")]
        [TestCase("x", "y")]
        [TestCase("a", "b")]
        public void ScriptableObjectId_OperatorEqualString_NonMatchingId_ReturnsFalse(string id, string other)
        {
            var a = CreateWithId(id);
            Assert.IsFalse(a == other);
            Object.DestroyImmediate(a);
        }

        // ---- operator!= ----

        [Test, Description("operator!= two ScriptableObjectId same ref => Returns false?")]
        public void ScriptableObjectId_OperatorNotEqual_SameRef_ReturnsFalse()
        {
            var a = CreateWithId("alpha");
            ScriptableObjectId b = a;
            Assert.IsFalse(a != b);
            Object.DestroyImmediate(a);
        }

        [Test, Description("operator!= two ScriptableObjectId different instances => Returns true?")]
        [TestCase("alpha")]
        [TestCase("beta")]
        [TestCase("same-id")]
        public void ScriptableObjectId_OperatorNotEqual_DifferentInstances_ReturnsTrue(string id)
        {
            var a = CreateWithId(id);
            var b = CreateWithId(id);
            Assert.IsTrue(a != b);
            Object.DestroyImmediate(a);
            Object.DestroyImmediate(b);
        }

        [Test, Description("operator!= ScriptableObjectId and matching string => Returns false?")]
        [TestCase("alpha")]
        [TestCase("my-id")]
        [TestCase("")]
        public void ScriptableObjectId_OperatorNotEqualString_MatchingId_ReturnsFalse(string id)
        {
            var a = CreateWithId(id);
            Assert.IsFalse(a != id);
            Object.DestroyImmediate(a);
        }

        [Test, Description("operator!= ScriptableObjectId and non-matching string => Returns true?")]
        [TestCase("alpha", "beta")]
        [TestCase("x", "y")]
        [TestCase("a", "b")]
        public void ScriptableObjectId_OperatorNotEqualString_NonMatchingId_ReturnsTrue(string id, string other)
        {
            var a = CreateWithId(id);
            Assert.IsTrue(a != other);
            Object.DestroyImmediate(a);
        }

        // ---- GetHashCode ----

        [Test, Description("GetHashCode => Same instance returns the same hash code on two calls?")]
        [TestCase("alpha")]
        [TestCase("my-id")]
        [TestCase("")]
        public void ScriptableObjectId_GetHashCode_SameInstance_ConsistentResult(string id)
        {
            var a = CreateWithId(id);
            int first = a.GetHashCode();
            int second = a.GetHashCode();
            Assert.AreEqual(first, second);
            Object.DestroyImmediate(a);
        }

        [Test, Description("GetHashCode => Two instances with the same Id return equal hash codes (Id-string-based)?")]
        [TestCase("alpha")]
        [TestCase("same-id")]
        [TestCase("")]
        public void ScriptableObjectId_GetHashCode_DifferentInstancesSameId_ReturnEqualHashCodes(string id)
        {
            var a = CreateWithId(id);
            var b = CreateWithId(id);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
            Object.DestroyImmediate(a);
            Object.DestroyImmediate(b);
        }

        // ---- ToString ----

        [Test, Description("ToString => Returns Id string?")]
        [TestCase("alpha")]
        [TestCase("my-unique-id")]
        [TestCase("")]
        public void ScriptableObjectId_ToString_ReturnsIdString(string id)
        {
            var a = CreateWithId(id);
            Assert.AreEqual(id, a.ToString());
            Object.DestroyImmediate(a);
        }

        // ---- operator== null left-hand side (ScriptableObjectId, ScriptableObjectId) ----

        [Test, Description("operator== null == null => Returns true?")]
        public void ScriptableObjectId_OperatorEqual_NullLhs_NullRhs_ReturnsTrue()
        {
            ScriptableObjectId left = null;
            ScriptableObjectId right = null;
            Assert.IsTrue(left == right);
        }

        [Test, Description("operator== null == nonNull => Returns false?")]
        [TestCase("alpha")]
        [TestCase("beta")]
        [TestCase("")]
        public void ScriptableObjectId_OperatorEqual_NullLhs_NonNullRhs_ReturnsFalse(string id)
        {
            ScriptableObjectId left = null;
            var right = CreateWithId(id);
            Assert.IsFalse(left == right);
            Object.DestroyImmediate(right);
        }

        // ---- operator== null left-hand side (ScriptableObjectId, string) ----

        [Test, Description("operator== (string) null == null => Returns true?")]
        public void ScriptableObjectId_OperatorEqualString_NullLhs_NullString_ReturnsTrue()
        {
            ScriptableObjectId left = null;
            string right = null;
            Assert.IsTrue(left == right);
        }

        [Test, Description("operator== (string) null == nonNull => Returns false?")]
        [TestCase("alpha")]
        [TestCase("beta")]
        [TestCase("id-123")]
        public void ScriptableObjectId_OperatorEqualString_NullLhs_NonNullString_ReturnsFalse(string id)
        {
            ScriptableObjectId left = null;
            Assert.IsFalse(left == id);
        }

        // ---- operator!= null left-hand side (ScriptableObjectId, ScriptableObjectId) ----

        [Test, Description("operator!= null != null => Returns false?")]
        public void ScriptableObjectId_OperatorNotEqual_NullLhs_NullRhs_ReturnsFalse()
        {
            ScriptableObjectId left = null;
            ScriptableObjectId right = null;
            Assert.IsFalse(left != right);
        }

        // ---- operator!= null left-hand side (ScriptableObjectId, string) ----

        [Test, Description("operator!= (string) null != null => Returns false?")]
        public void ScriptableObjectId_OperatorNotEqualString_NullLhs_NullString_ReturnsFalse()
        {
            ScriptableObjectId left = null;
            string right = null;
            Assert.IsFalse(left != right);
        }

        // ---- GetHashCode with null Id ----

        [Test, Description("GetHashCode when Id is null => Does not throw and returns a consistent value?")]
        public void ScriptableObjectId_GetHashCode_NullId_DoesNotThrowAndIsConsistent()
        {
            var a = ScriptableObject.CreateInstance<TestId>();
            // Id is not set, so it remains null (the default for a serialized string field).
            int first = -1;
            int second = -1;
            Assert.DoesNotThrow(() => first = a.GetHashCode());
            Assert.DoesNotThrow(() => second = a.GetHashCode());
            Assert.AreEqual(first, second);
            Object.DestroyImmediate(a);
        }

        // ---- Equals(object) with a different ScriptableObjectId instance ----

        [Test, Description("Equals(object) different ScriptableObjectId instance as object => Returns false?")]
        [TestCase("alpha")]
        [TestCase("beta")]
        [TestCase("")]
        public void ScriptableObjectId_EqualsObject_DifferentScriptableObjectIdInstance_ReturnsFalse(string id)
        {
            var a = CreateWithId(id);
            var b = CreateWithId(id);
            Assert.IsFalse(a.Equals((object)b));
            Object.DestroyImmediate(a);
            Object.DestroyImmediate(b);
        }
    }
}
