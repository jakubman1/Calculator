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
        public static double Add(double a, double b)
        {
            return a + b;
        }
        /// <summary>
        /// Substracts b from a
        /// </summary>
        /// <param name="a">First number</param>
        /// <param name="b">Second number</param>
        /// <returns>Result of a - b</returns>
        public static double Sub(double a, double b)
        {
            return a - b;
        }
        
        /// <summary>
        /// Multiplies two numbers
        /// </summary>
        /// <param name="a">First number</param>
        /// <param name="b">Second number</param>
        /// <returns>Result of a * b</returns>
        public static double Multiply(double a, double b)
        {
            return a * b;
        }

        /// <summary>
        /// Divides two numbers
        /// </summary>
        /// <param name="a">First number</param>
        /// <param name="b">Second number</param>
        /// <returns>Result of a / b</returns>
        public static double Divide(double a, double b)
        {
            if (b == 0) throw new Exception("Division by zero");
            return a / b;
        }

        /// <summary>
        /// Returns factorial of given number
        /// </summary>
        /// <param name="n">Number to get factorial from</param>
        /// <returns>Result of n!</returns>
        public static double Factorial(int n)
        {
            if (n < 0) throw new Exception("Factorial of negative number");
            
            if (n == 1)
            {
                return 1;
            }

            double tmp = n;
            while(n > 1)
            {
                n--;
                tmp *= n;
            }
            return tmp;
        }

        /// <summary>
        /// n to the power of exp
        /// </summary>
        /// <param name="n">Base</param>
        /// <param name="exp">Exponent</param>
        /// <returns>n to the power of exp</returns>
        public static double Pow(double n, int exp)
        {
            if (exp <= 0) throw new Exception("The exponent is not a natural number");

            double tmp = n;
            while (exp > 1)
            {
                n *= tmp;
                exp--;
            }
            return n;
        }

        /// <summary>
        /// exp root of n
        /// </summary>
        /// <param name="n">Base</param>
        /// <param name="exp">Exponent</param>
        /// <returns>exp root of n</returns>
        public static double Root(double n, double exp)
        {
            return n;
        }

        /// <summary>
        /// Return absolute value of a nuber
        /// </summary>
        /// <param name="a">Number to get absolute value from</param>
        /// <returns>Absolute value of a</returns>
        public static double Abs(double a)
        {
            if (a < 0)
            {
                return -a;
            }
            return a;
        }

    }
}
