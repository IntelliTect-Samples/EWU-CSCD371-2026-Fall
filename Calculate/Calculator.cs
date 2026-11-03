using System;
using System.Collections.Generic;

namespace Calculate;

/// <summary>
/// A basic calculator class that can perform basic mathematical operations such as addition, subtraction, multiplication, and division.
/// </summary>
/// <remarks>
/// The calculator supports expressions in the form of "a op b", where 'a' and
/// 'b' are integers and 'op' is one of the supported mathematical operations: +, -, *, /.
/// </remarks>
/// <example>
/// var calculator = new Calculator();
/// bool success = calculator.TryCalculate("3 + 4", out float result);
/// if (success)
/// {
///   Console.WriteLine($"Result: {result}"); // Output: Result: 7
/// }
/// </example>
/// <exception cref="DivideByZeroException">Thrown when attempting to divide by zero.</exception>
/// <exception cref="FormatException">Thrown when the input expression is not in the expected format.</exception>
/// <exception cref="ArgumentNullException">Thrown when the input expression is null.</exception>
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

    // Dictionary to map mathematical operations to their corresponding functions
    public IReadOnlyDictionary<char, Func<int, int, float>> MathematicalOperations { get; } =
        new Dictionary<char, Func<int, int, float>>
        {
            { '+', Add },
            { '-', Subtract },
            { '*', Multiply },
            { '/', Divide }
        };

    // Method to try calculating the result of a mathematical expression
    public bool TryCalculate(string? expression, out float result)
    {
        result = 0;
        if (string.IsNullOrWhiteSpace(expression))
        {
            return false;
        }

        // Split the expression into parts, removing any extra whitespace
        string[] parts = expression.Split(
            (char[]?)null,
            StringSplitOptions.RemoveEmptyEntries);

        // Check if the expression is in the expected format: "a op b"
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