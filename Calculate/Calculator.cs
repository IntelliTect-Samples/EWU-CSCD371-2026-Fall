using System;
using System.Collections.Generic;

namespace Calculate;

public class Calculator
{
    public static float Add(int a, int b)
    {
        return a + b;
    }

    public static float Subtract(int a, int b)
    {
        return a - b;
    }

    public static float Multiply(int a, int b)
    {
        return a * b;
    }

    public static float Divide(int a, int b)
    {
        if (b == 0)
        {
            throw new DivideByZeroException("Cannot divide by zero!");
        }
    
        return (float)a / (float)b;
    }

    public IReadOnlyDictionary<char, Func<int, int, float>> MathematicalOperations { get; } =
        new Dictionary<char, Func<int, int, float>>
        {
            { '+', Add },
            { '-', Subtract },
            { '*', Multiply },
            { '/', Divide }
        };

    public bool TryCalculate(string? expression, out float result)
    {
        result = 0;
        if (string.IsNullOrWhiteSpace(expression))
        {
            return false;
        }

        string[] parts = expression.Split(
            (char[]?)null,
            StringSplitOptions.RemoveEmptyEntries);

        if (parts is [string first, string op, string second] &&
            op is [char operation] &&
            int.TryParse(first, out int a) &&
            int.TryParse(second, out int b) &&
            MathematicalOperations.ContainsKey(operation))
        {
            result = MathematicalOperations[operation](a, b);
            return true;
        }

        return false;
    }
}