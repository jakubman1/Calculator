using System;
using static MathLibrary.Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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


    }
}
