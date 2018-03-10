using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

//TODO: Test extreme cases (eg. DOUBLE_MAX * DOUBLE_MAX)
namespace MathLibTest
{
    [TestClass]
    public class MathLibTests
    {
        private static MathLibrary.Math math;

        [TestInitialize]
        public void Initialize()
        {
            math = new MathLibrary.Math();
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        [TestMethod]
        public void Add()
        {
            Assert.AreEqual(42, math.Add(21, 21));
            Assert.AreEqual(1.42, math.Add(3.14, -1.72), 0.001);
            Assert.AreEqual(2.0 / 3, math.Add(1.0 / 3, 1.0 / 3), 0.001);
        }

        [TestMethod]
        public void Substract()
        {
            Assert.AreEqual(0, math.Sub(42, 42));
            Assert.AreEqual(42, math.Sub(42, 0));
            Assert.AreEqual(-42, math.Sub(0, 42));
            Assert.AreEqual(42, math.Sub(21, -21));
            Assert.AreEqual(4.86, math.Sub(3.14, -1.72), 0.001);
            Assert.AreEqual(2.0 / 3, math.Sub(1.0 / 3, -1.0 / 3), 0.001);
        }

        [TestMethod]
        public void Multiply()
        {
            Assert.AreEqual(42, math.Multiply(21, 2));
            Assert.AreEqual(-5.4008, math.Multiply(3.14, -1.72), 0.001);
            Assert.AreEqual(0.111, math.Multiply(1.0 / 3, 1.0 / 3), 0.001);
        }

        [TestMethod]
        public void Divide()
        {
            Assert.AreEqual(2, math.Divide(4, 2));
            Assert.AreEqual(-1.826, math.Divide(3.14, -1.72), 0.001);
            Assert.AreEqual(1, math.Divide(1.0 / 3, 1.0 / 3));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DivideException()
        {
            math.Divide(42, 0);
        }

        [TestMethod]
        public void Factorial()
        {
            Assert.AreEqual(1, math.Factorial(0));
            Assert.AreEqual(1, math.Factorial(1));
            Assert.AreEqual(6, math.Factorial(3));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FactorialException()
        {
            math.Factorial(-1);
        }

        [TestMethod]
        public void Root()
        {
            Assert.AreEqual(2, math.Root(4, 2));
            Assert.AreEqual(1 / 3, math.Root(9, -2), 0.001);
        }
    }
}
