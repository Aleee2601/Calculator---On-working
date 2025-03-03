using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Contine logica aplicatiei
/// </summary>

namespace Calculator.Models
{
    class CalculatorModel
    {
        #region Math Operations
        static public double Add(double a, double b)
        {
            return a + b;
        }
        static public double Subtract(double a, double b)
        {
            return a - b;
        }
        static public double Multiply(double a, double b)
        {
            return a * b;
        }
        static public double Divide(double a, double b)
        {
            if (b == 0)
            {
                throw new DivideByZeroException();
            }
            return a / b;
        }
        static public double Modulo(double a, double b)
        {
            return a % b;
        }
        
        static public double Pow(double a)
        {
            return Math.Pow(a, 2);
        }
        static public double Sqrt(double a)
        {
            if (a < 0)
            {
                throw new ArithmeticException();
            }
            return Math.Sqrt(a);
        }
        static public double Negate(double a)
        {
            return -a;
        }
        static public double Reciprocal(double a)
        {
            if (a == 0)
            {
                throw new DivideByZeroException();
            }
            return 1 / a;
        }
        #endregion






    }
}
