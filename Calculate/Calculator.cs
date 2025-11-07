using System.Data;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace Calculate;

public class Calculator
{
    public IReadOnlyDictionary<char, Func<int, int, double>> MathematicalOperations {  get; }
    public Calculator()
    {
        MathematicalOperations = new Dictionary<char, Func<int, int, double>>()
        {
            {'+', Add },
            {'-', Subtract },
            {'*', Multiply },
            {'/', Divide },
        };
    }
    public static double Add(int a, int b)
    {
        return checked(a + b);
    }

    public static double Subtract(int a, int b)
    {
        return checked(a - b);
    }

    public static double Multiply(int a, int b)
    {
        return checked(a * b);
    }

    public static double Divide(int a, int b)
    {
        if (b == 0)
        {
            throw new DivideByZeroException("Denominator cannot be zero.");
        }
        return (double)a / b;
    }

    public bool TryCalculate(string? expression, out double answer)
    {
        if (expression == null)
        {
            answer = default;
            return false;
        }
        string[] expressionArray = expression.Split(' ');
        if (expressionArray.Length == 3 && int.TryParse(expressionArray[0], out int left)
            && int.TryParse(expressionArray[2], out int right) && char.TryParse(expressionArray[1], out char op))
        {
            if (MathematicalOperations.TryGetValue(op, out Func<int, int, double>? func))
            {
                answer = func(left, right);
                return true;
            }
        }
        answer = default;
        return false;
    }
}