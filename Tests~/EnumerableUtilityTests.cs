using NUnit.Framework;

namespace Calluna.Core.Tests
{
    public class EnumerableUtilityTests
    {
        // --- Sum(int[]) ---

        [Test, Description("Sum(int[]) => Correct total?")]
        [TestCase(new int[] { 1, 2, 3 }, 6)]
        [TestCase(new int[] { -5, 5, 10 }, 10)]
        [TestCase(new int[] { 0, 0, 0 }, 0)]
        [TestCase(new int[] { 100, -50, -50 }, 0)]
        public void EnumerableUtility_SumInt_ReturnsCorrectTotal(int[] values, int expected)
        {
            Assert.AreEqual(expected, EnumerableUtility.Sum(values));
        }

        [Test, Description("Sum(int[]) empty sequence => Returns 0?")]
        public void EnumerableUtility_SumInt_EmptySequence_ReturnsZero()
        {
            Assert.AreEqual(0, EnumerableUtility.Sum(new int[0]));
        }

        // --- Sum(float[]) ---

        [Test, Description("Sum(float[]) => Correct total?")]
        [TestCase(new float[] { 1.5f, 2.5f }, 4.0f)]
        [TestCase(new float[] { -1f, 1f, 0.5f }, 0.5f)]
        [TestCase(new float[] { 0f, 0f }, 0f)]
        [TestCase(new float[] { 100f, -50.5f }, 49.5f)]
        public void EnumerableUtility_SumFloat_ReturnsCorrectTotal(float[] values, float expected)
        {
            Assert.AreEqual(expected, EnumerableUtility.Sum(values), 0.0001f);
        }

        [Test, Description("Sum(float[]) empty sequence => Returns 0?")]
        public void EnumerableUtility_SumFloat_EmptySequence_ReturnsZero()
        {
            Assert.AreEqual(0f, EnumerableUtility.Sum(new float[0]), 0.0001f);
        }

        // --- Product(float[]) ---

        [Test, Description("Product(float[]) => Correct product?")]
        [TestCase(new float[] { 2f, 3f, 4f }, 24f)]
        [TestCase(new float[] { -2f, 3f }, -6f)]
        [TestCase(new float[] { 1f, 1f, 1f }, 1f)]
        [TestCase(new float[] { 0.5f, 2f }, 1f)]
        public void EnumerableUtility_Product_ReturnsCorrectProduct(float[] values, float expected)
        {
            Assert.AreEqual(expected, EnumerableUtility.Product(values), 0.0001f);
        }

        [Test, Description("Product single element => Returns that element?")]
        [TestCase(5f)]
        [TestCase(-3f)]
        [TestCase(0.25f)]
        public void EnumerableUtility_Product_SingleElement_ReturnsThatElement(float value)
        {
            Assert.AreEqual(value, EnumerableUtility.Product(new float[] { value }), 0.0001f);
        }

        [Test, Description("Product empty sequence => Returns 0?")]
        public void EnumerableUtility_Product_EmptySequence_ReturnsZero()
        {
            Assert.AreEqual(0f, EnumerableUtility.Product(new float[0]), 0.0001f);
        }

        // --- Any ---

        [Test, Description("Any with >= 1 true => Returns true?")]
        [TestCase(new bool[] { false, true, false }, true)]
        [TestCase(new bool[] { true, false }, true)]
        [TestCase(new bool[] { true, true }, true)]
        public void EnumerableUtility_Any_AtLeastOneTrue_ReturnsTrue(bool[] values, bool expected)
        {
            Assert.AreEqual(expected, EnumerableUtility.Any(values));
        }

        [Test, Description("Any with all false => Returns false?")]
        [TestCase(new bool[] { false, false, false })]
        [TestCase(new bool[] { false })]
        public void EnumerableUtility_Any_AllFalse_ReturnsFalse(bool[] values)
        {
            Assert.IsFalse(EnumerableUtility.Any(values));
        }

        [Test, Description("Any with empty sequence => Returns false?")]
        public void EnumerableUtility_Any_EmptySequence_ReturnsFalse()
        {
            Assert.IsFalse(EnumerableUtility.Any(new bool[0]));
        }

        // --- All ---

        [Test, Description("All with all true => Returns true?")]
        [TestCase(new bool[] { true, true, true })]
        [TestCase(new bool[] { true })]
        public void EnumerableUtility_All_AllTrue_ReturnsTrue(bool[] values)
        {
            Assert.IsTrue(EnumerableUtility.All(values));
        }

        [Test, Description("All with any false => Returns false?")]
        [TestCase(new bool[] { true, false, true })]
        [TestCase(new bool[] { false, false })]
        [TestCase(new bool[] { false })]
        public void EnumerableUtility_All_AnyFalse_ReturnsFalse(bool[] values)
        {
            Assert.IsFalse(EnumerableUtility.All(values));
        }

        [Test, Description("All with empty sequence => Returns true?")]
        public void EnumerableUtility_All_EmptySequence_ReturnsTrue()
        {
            Assert.IsTrue(EnumerableUtility.All(new bool[0]));
        }
    }
}
