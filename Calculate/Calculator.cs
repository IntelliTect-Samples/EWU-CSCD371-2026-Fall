using System;
using System.Collections.Generic;

namespace Calculate;

public class Calculator
{
    // Four arithmetic operations
    public static int Add(int a, int b) => a + b;

    public static int Subtract(int a, int b) => a - b;

    public static int Multiply(int a, int b) => a * b;

    public static int Divide(int a, int b) => a / b;

    // Dictionary connecting operators to methods
    public IReadOnlyDictionary<char, Func<int, int, int>>
        MathematicalOperations
    { get; } =
        new Dictionary<char, Func<int, int, int>>
        {
            ['+'] = Add,
            ['-'] = Subtract,
            ['*'] = Multiply,
            ['/'] = Divide
        };

    // Attempts to calculate a string expression
    public bool TryCalculate(string? input, out int result)
    {
        if (input?.Split(
                (char[]?)null,
                StringSplitOptions.RemoveEmptyEntries)
            is [var first, var opText, var second]
            && opText is [var op]
            && int.TryParse(first, out int a)
            && int.TryParse(second, out int b)
            && MathematicalOperations.TryGetValue(op, out var operation))
        {
            try
            {
                result = operation(a, b);
                return true;
            }
            catch (ArithmeticException)
            {
                // Invalid arithmetic, such as division by zero
            }
        }

        result = 0;
        return false;
    }
}