using ConsoleUtilities;
using System;

namespace Calculate;

public class Program : ProgramBase
{

    private readonly Calculator _calc = new();

    public Program() : base() { }

    public Program(Action<string?> writeLine, Func<string?> readLine)
        : base(writeLine, readLine) { }

    /// <summary>
    /// Runs the main calculator loop, reading expressions and displaying results.
    /// Encapsulating the interaction logic in a method makes it easier to test
    /// the logic directly without launching a separate process.
    /// It returns an integer so tests can assert exit codes or termination conditions.
    /// </summary>
    /// <returns>0 for normal exit, non-zero for abnormal termination.</returns>
    public int Run()
    {
        while (true)
        {
            WriteLine("Enter an expression like \"3 + 4\" (or 'q' to quit):");
            var input = ReadLine();

            if (input is null)
                return 1;

            if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase)) return 0;

            if (_calc.TryCalculate(input, out var result))
                WriteLine($"{input} = {result}");
            else
                WriteLine("Invalid input. Please include spaces around the operator and use integers.");
        }
    }

    /// <summary>
    /// Application entry point.
    /// Keeps Main minimal by instantiating Program with default console delegates.
    /// This separation simplifies testing and aligns with the Single Responsibility Principle.
    /// </summary>
    public static void Main() => new Program().Run();
}
