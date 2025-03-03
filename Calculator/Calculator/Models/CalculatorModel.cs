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
        private static List<double> memoryStack = new List<double>();


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
        static public double Equals(double a)
        {
            return a;
        }
        #endregion

        #region Button Operations
        static public void Backspace(ref string text)
        {
            if (text.Length > 0)
            {
                text = text.Remove(text.Length - 1);
            }
        }
        public static void CE(ref string text)
        {
            text = "0"; // Resetează ultima valoare introdusă
        }

        public static void C(ref string text)
        {
            text = "";
        }

        public static void MC()
        {
            memoryStack.Clear(); // Șterge întreaga memorie
        }

        public static void MPlus(ref string text)
        {
            if (double.TryParse(text, out double value))
            {
                if (memoryStack.Any())
                    memoryStack[memoryStack.Count - 1] += value;
                else
                    memoryStack.Add(value);
            }
        }

        public static void MMinus(ref string text)
        {
            if (double.TryParse(text, out double value))
            {
                if (memoryStack.Any())
                    memoryStack[memoryStack.Count - 1] -= value;
                else
                    memoryStack.Add(-value);
            }
        }

        public static void MS(ref string text)
        {
            if (double.TryParse(text, out double value))
            {
                memoryStack.Add(value);
            }
        }

        public static void MR(ref string text)
        {
            if (memoryStack.Any())
            {
                text = memoryStack.Last().ToString();
            }
        }

        public static void MGreater(ref string text)
        {
            if (memoryStack.Any())
            {
                text = string.Join(", ", memoryStack);
            }
        }
        #endregion


        #region Digital Grouping
        static public void AddDigit(ref string text, string digit)
        {
            text += digit;
        }
        static public void AddDot(ref string text)
        {
            if (!text.Contains("."))
            {
                text += ".";
            }
        }


        #endregion



    }
}
