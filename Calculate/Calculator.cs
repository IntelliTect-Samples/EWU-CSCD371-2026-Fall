using System;
using System.Collections.Generic;
using System.Linq;

namespace Calculate;

public class Calculator
{
    public static double Add(double a, double b) => a + b;
    public static double Subtract(double a, double b) => a - b;
    public static double Multiply(double a, double b) => a * b;
    public static double Divide(double a, double b) => a / b;
    

    public static IReadOnlyDictionary<char, Func<double, double, double>> MathematicalOperations { get; } =
        new Dictionary<char, Func<double, double, double>>
        {
            { '+', Add },
            { '-', Subtract },
            { '*', Multiply },
            { '/', Divide }
        };

    public static bool TryCalculate(string expression, out double result)
    {
        result = 0;
        
        if (string.IsNullOrWhiteSpace(expression))
        {
            return false;
        }
        
        string[] parts = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 3)
        {
            return false;
        }

       if (parts[1].Length != 1)
        {
            return false;
        }

        char op = parts[1][0];

        if (!int.TryParse(parts[0], out int number1) || !int.TryParse(parts[2], out int number2))
        {
            return false;
        }

        if (MathematicalOperations.TryGetValue(op, out var operation))
        {
            if (op == '/' && number2 == 0)
            {
                return false;
            }
            result = operation(number1, number2);
            return true;
        }

        return false;
    }
}
