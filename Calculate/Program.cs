using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;

namespace Calculate;

public class Program
{
    public Func<string?> ReadLine { get; init; } = () => { return Console.ReadLine(); };
    public Action<string> WriteLine { get; init; } = (value) => { Console.WriteLine(value); };

    public Program()
    {
    }

    public Program(Func<string> readLine, Action<string> writeLine)
    {
        ReadLine = readLine;
        WriteLine = writeLine;
    }

    static void Main()
    {
        Program program = new();
        Calculator calculator = new();

        program.WriteLine("Enter a calculation (e.g.,3 + 4), or blank to quit");

        string? input;
        while (!string.IsNullOrWhiteSpace(input = program.ReadLine()))
        {
            if (calculator.TryCalculate(input, out double answer))
            {
                program.WriteLine($"Answer: {answer}");
            }
            else
            {
                program.WriteLine("Invalid input. Please use the format: [number] [operator] [number]");
            }

            program.WriteLine("Enter another calculation, or blank to quit:");
        }

        program.WriteLine("Goodbye!");
    }
}