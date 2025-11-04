using System;
using System.Numerics;

namespace Calculate;

public class Program
{
    public Action<string?> WriteLine { get; init; }
    public Func<string?> ReadLine { get; init; }
    public Program()
    {
        WriteLine = Console.WriteLine;
        ReadLine = Console.ReadLine;
    }

    public Program(Action<string?> writeLine, Func<string?> readLine)
    {
        WriteLine = writeLine ?? throw new ArgumentNullException(nameof(writeLine));
        ReadLine = readLine ?? throw new ArgumentNullException(nameof(readLine));
    }

    public int Run<T>(Calculator<T> calculator) where T : INumber<T>
    {
        ArgumentNullException.ThrowIfNull(calculator);

        while (true)
        {
            WriteLine("Enter calculation (or enter blank line to exit):");
            var input = ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                WriteLine("Exiting Calculator.");
                return 0;
            }
            if (calculator.TryCalculate(input, out var result)) 
            {
                WriteLine(result.ToString());
                continue;
            }

            WriteLine("Invalid input. Try again.");
        }
    }
    
    public static int Main()
    {
        var prog = new Program();
        var calc = new Calculator<int>();

        int runResult = prog.Run<int>(calc);
        
        return runResult;
    }
}