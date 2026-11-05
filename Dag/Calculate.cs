namespace Dag;

using System;
using System.Collections.Generic;

/// <summary>
/// Handles console interaction and a small expression loop for calculator input.
/// </summary>
public class Program
{
    private readonly Calculator _calculator;


    /// <summary>
    /// Initializes a new instance of the Program class with default console interaction methods.
    /// </summary>
    public Program()
        : this(Console.WriteLine, Console.ReadLine)
    {
    }

    /// <summary>
    /// Initializes a new instance of the Program class with specified write and read methods for console interaction.
    /// </summary>
    /// <param name="writeLine"></param>
    /// <param name="readLine"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public Program(Action<string> writeLine, Func<string?> readLine)
    {
        WriteLine = writeLine ?? throw new ArgumentNullException(nameof(writeLine));
        ReadLine = readLine ?? throw new ArgumentNullException(nameof(readLine));
        _calculator = new Calculator();
    }

    public Calculator CalculatorInstance => _calculator;

    public Action<string> WriteLine { get; init; }
    public Func<string?> ReadLine { get; init; }


    /// <summary>
    /// The main entry point for the application. It creates a new instance of the Program class and calls the Run method to start the calculator loop.
    /// </summary>
    public static void Main()
    {
        new Program().Run();
    }

    /// <summary>
    /// Starts the calculator loop, prompting the user for input and displaying results or error messages.
    /// </summary>
    public void Run()
    {
        WriteLine("Enter a calculation in the form: number operator number");

        while (true)
        {
            WriteLine("Expression: ");
            string? input = ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                WriteLine("Goodbye.");
                return;
            }

            if (_calculator.TryCalculate(input, out int result))
            {
                WriteLine($"Result: {result}");
            }
            else
            {
                WriteLine("Invalid calculation. Use the format: 3 + 4");
            }
        }
    }
}

/// <summary>
/// Contains the supported arithmetic methods and parse logic for calculator expressions.
/// </summary>
public class Calculator
{
    public static int Add(int left, int right) => left + right;

    public static int Subtract(int left, int right) => left - right;

    public static int Multiply(int left, int right) => left * right;

    public static int Divide(int left, int right)
    {
        if (right == 0)
        {
            throw new DivideByZeroException();
        }

        return left / right;
    }

    /// <summary>
    /// Gets the dictionary of supported mathematical operations.
    /// </summary>
    public IReadOnlyDictionary<char, Func<int, int, int>> MathematicalOperations { get; } =
        new Dictionary<char, Func<int, int, int>>
        {
            { '+', Add },
            { '-', Subtract },
            { '*', Multiply },
            { '/', Divide },
        };

    /// <summary>
    /// Attempts to calculate the result of a mathematical expression.
    /// </summary>
    /// <param name="calculation">The mathematical expression to calculate.</param>
    /// <param name="result">The result of the calculation, if successful.</param>
    /// <returns>true if the calculation was successful; otherwise, false.</returns>
    public bool TryCalculate(string calculation, out int result)
    {
        result = 0;

        if (string.IsNullOrWhiteSpace(calculation))
        {
            return false;
        }

        string[] parts = calculation.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (parts.Length != 3)
        {
            return false;
        }

        if (!int.TryParse(parts[0], out int left) || !int.TryParse(parts[2], out int right))
        {
            return false;
        }

        string operatorToken = parts[1];

        if (operatorToken.Length != 1)
        {
            return false;
        }

        char op = operatorToken[0];

        if (!MathematicalOperations.TryGetValue(op, out Func<int, int, int>? operation))
        {
            return false;
        }

        if (op == '/' && right == 0)
        {
            return false;
        }

        result = operation(left, right);
        return true;
    }
}