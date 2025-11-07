using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Calculate;

public class Calculator
{
    public static T Add<T>(T a, T b) where T : INumber<T> => a + b;
    public static T Subtract<T>(T a, T b) where T : INumber<T> => a - b;
    public static T Multiple<T>(T a, T b ) where T : INumber<T> => a * b;
    public static T Divide<T>(T a, T b) where T : INumber<T>
    {
        if (b == T.Zero) throw new DivideByZeroException("Cannot divide by zero");
        return a / b;
    }

    public static IReadOnlyDictionary<char, Func<T, T, T>> MathematicalOperations<T>() where T : INumber<T>
        => new Dictionary<char, Func<T, T, T>>
        {
            {'+', Add<T> },
            {'-', Subtract<T> },
            {'*', Multiple<T> },
            {'/', Divide<T> }
        };

    public static bool TryCalculate<T>(string calculation, out T result) where T : INumber<T>
    {
        result = T.Zero;

        if (string.IsNullOrWhiteSpace(calculation))
            return false;

        string[] parts = calculation.Split(' ');
        if (parts.Length != 3)
            return false;

        string leftTemp = parts[0];
        string opTemp = parts[1];
        string rightTemp = parts[2];

        if (opTemp.Length != 1)
            return false;

        T left, right;
        try
        {
            if (typeof(T) == typeof(int))
            {
                left = (T)(object)int.Parse(leftTemp, CultureInfo.InvariantCulture);
                right = (T)(object)int.Parse(rightTemp, CultureInfo.InvariantCulture);
            }
            else if (typeof(T) == typeof(double))
            {
                left = (T)(object)double.Parse(leftTemp, CultureInfo.InvariantCulture);
                right = (T)(object)double.Parse(rightTemp, CultureInfo.InvariantCulture);
            }
            else return false;
        }
        catch
        {
            return false;
        }

        char op = opTemp[0];

        var operations = MathematicalOperations<T>();
        if (!operations.TryGetValue(op, out var operation))
            return false;

        if (op == '/' && right == T.Zero)
            return false;

        result = operation(left, right);
        return true;
    }



}
