using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathLibrary
{
    public class Math
    {
        /// <summary>
        /// Adds two numbers
        /// </summary>
        /// <param name="a">First number</param>
        /// <param name="b">Second number</param>
        /// <returns>Result of a + b</returns>
        public double Add(double a, double b)
        {
            return a + b;
        }
        /// <summary>
        /// Substracts b from a
        /// </summary>
        /// <param name="a">First number</param>
        /// <param name="b">Second number</param>
        /// <returns>Result of a - b</returns>
        public double Sub(double a, double b)
        {
            return a - b;
        }
        
        /// <summary>
        /// Multiplies two numbers
        /// </summary>
        /// <param name="a">First number</param>
        /// <param name="b">Second number</param>
        /// <returns>Result of a * b</returns>
        public double Multiply(double a, double b)
        {
            return a * b;
        }

        /// <summary>
        /// Divides two numbers
        /// </summary>
        /// <param name="a">First number</param>
        /// <param name="b">Second number</param>
        /// <returns>Result of a / b</returns>
        public double Divide(double a, double b)
        {
            return a / b;
        }


    }
}
