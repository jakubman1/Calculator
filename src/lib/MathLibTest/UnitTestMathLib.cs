using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathLibTest
{
    [TestClass]
    public class MathLibTests
    {

        [TestInitialize]
        public void Initialize()
        {
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        [TestMethod]
        public void Add()
        {
            Assert.AreEqual(42, MathLibrary.Math.Add(21, 21));
            Assert.AreEqual(1.42, MathLibrary.Math.Add(3.14, -1.72), 0.001);
            Assert.AreEqual(2.0 / 3, MathLibrary.Math.Add(1.0 / 3, 1.0 / 3), 0.001);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddOverflow()
        {
            MathLibrary.Math.Add(Double.MaxValue, Double.MaxValue);
        }

        [TestMethod]
        public void Substract()
        {
            Assert.AreEqual(0, MathLibrary.Math.Sub(42, 42));
            Assert.AreEqual(42, MathLibrary.Math.Sub(42, 0));
            Assert.AreEqual(-42, MathLibrary.Math.Sub(0, 42));
            Assert.AreEqual(42, MathLibrary.Math.Sub(21, -21));
            Assert.AreEqual(4.86, MathLibrary.Math.Sub(3.14, -1.72), 0.001);
            Assert.AreEqual(2.0 / 3, MathLibrary.Math.Sub(1.0 / 3, -1.0 / 3), 0.001);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SubOverflow1()
        {
            MathLibrary.Math.Sub(Double.MaxValue, -1 * Double.MaxValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SubOverflow2()
        {
            MathLibrary.Math.Sub(-1 * Double.MaxValue, Double.MaxValue);
        }

        [TestMethod]
        public void Multiply()
        {
            Assert.AreEqual(42, MathLibrary.Math.Multiply(21, 2));
            Assert.AreEqual(-5.4008, MathLibrary.Math.Multiply(3.14, -1.72), 0.001);
            Assert.AreEqual(0.111, MathLibrary.Math.Multiply(1.0 / 3, 1.0 / 3), 0.001);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MultiplyOverflow()
        {
            MathLibrary.Math.Multiply(Double.MaxValue, 2);
        }

        [TestMethod]
        public void Divide()
        {
            Assert.AreEqual(2, MathLibrary.Math.Divide(4, 2));
            Assert.AreEqual(-1.826, MathLibrary.Math.Divide(3.14, -1.72), 0.001);
            Assert.AreEqual(1, MathLibrary.Math.Divide(1.0 / 3, 1.0 / 3));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DivideByZeroException()
        {
            MathLibrary.Math.Divide(42, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DivideOverflow()
        {
            MathLibrary.Math.Divide(Double.MaxValue, 0.42);
        }

        [TestMethod]
        public void Factorial()
        {
            Assert.AreEqual(1, MathLibrary.Math.Factorial(0));
            Assert.AreEqual(1, MathLibrary.Math.Factorial(1));
            Assert.AreEqual(6, MathLibrary.Math.Factorial(3));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FactorialException()
        {
            MathLibrary.Math.Factorial(-1);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FactorialOverflow()
        {
            MathLibrary.Math.Factorial(int.MaxValue);
        }

        [TestMethod]
        public void Pow()
        {
            Assert.AreEqual(8, MathLibrary.Math.Pow(2, 3));
            Assert.AreEqual(42, MathLibrary.Math.Pow(42, 1));
            Assert.AreEqual(1, MathLibrary.Math.Pow(0, 1));
            Assert.AreEqual(0, MathLibrary.Math.Pow(0, 3));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PowExponentException()
        {
            MathLibrary.Math.Pow(42, 0);
            MathLibrary.Math.Pow(5, -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PowOverflow()
        {
            MathLibrary.Math.Pow(Double.MaxValue, 2);
        }

        [TestMethod]
        public void Root()
        {
            Assert.AreEqual(2, MathLibrary.Math.Root(4, 2), 0.0001);
            Assert.AreEqual(1.4142, MathLibrary.Math.Root(2, 2), 0.0001);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RootException()
        {
            MathLibrary.Math.Root(-1, 2);
            MathLibrary.Math.Root(2, 0);
            MathLibrary.Math.Root(3, -5);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RootOverflow()
        {
           // MathLibrary.Math.Root(Double.MaxValue, 0.5);
        }
    }
}
