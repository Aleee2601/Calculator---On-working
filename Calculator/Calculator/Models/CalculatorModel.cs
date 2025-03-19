﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Calculator.Models
{
    class CalculatorModel
    {
        #region Fields

        private static List<double> memoryStack = new List<double>();

        #endregion

        #region Math Operations

        public static double Add(double a, double b) => a + b;

        public static double Subtract(double a, double b) => a - b;

        public static double Multiply(double a, double b) => a * b;

        public static double Divide(double a, double b)
        {
            if (b == 0) throw new DivideByZeroException();
            return a / b;
        }

        public static double Modulo(double a, double b, string op) => op switch
        {
            "+" => a + (a * b / 100),
            "-" => a - (a * b / 100),
            "*" => a * (b / 100),
            "/" => a / (b / 100),
            _ => throw new InvalidOperationException("Operator necunoscut")
        };

        public static double Pow(double a) => Math.Pow(a, 2);

        public static double Sqrt(double a)
        {
            if (a < 0) throw new ArithmeticException();
            return Math.Sqrt(a);
        }

        public static double Negate(double a) => -a;

        public static double Reciprocal(double a)
        {
            if (a == 0) throw new DivideByZeroException();
            return 1 / a;
        }

        public static double Equals(double a) => a;

        #endregion

        #region Button Operations

        public static void Backspace(ref string text)
        {
            if (text.Length > 0)
                text = text.Remove(text.Length - 1);
        }

        public static void CE(ref string text) => text = "0";

        public static void C(ref string text) => text = "";

        public static void MC() => memoryStack.Clear();

        public static void MPlus(ref string text)
        {
            if (double.TryParse(text, out double value))
            {
                if (memoryStack.Any())
                    memoryStack[^1] += value;
                else
                    memoryStack.Add(value);
            }
        }


        public static void MMinus(ref string text)
        {
            if (double.TryParse(text, out double value))
            {
                if (memoryStack.Any())
                    memoryStack[^1] -= value;
                else
                    memoryStack.Add(-value);
            }
        }


        public static double? MS(string text)
        {
            if (double.TryParse(text, out double value))
            {
                memoryStack.Add(value);
                return value;
            }
            return null;
        }


        public static string MR() => memoryStack.Any() ? memoryStack.Last().ToString() : "0";


        public static string MGreater() => memoryStack.Any() ? string.Join(", ", memoryStack) : "0";


        #endregion

        #region Digital Grouping

        public static void AddDigit(ref string text, string digit) => text += digit;

        public static void AddDot(ref string text)
        {
            if (!text.Contains("."))
                text += ".";
        }

        #endregion
    }
}
