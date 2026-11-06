using System.Globalization;

namespace Calculator;

public class Program
{
    public const string Prompt = "Enter a calculation such as '3 + 4' (blank line to quit):";

    public Action<string?> WriteLine { get; init; } = Console.WriteLine;

    public Func<string?> ReadLine { get; init; } = Console.ReadLine;

    public Program()
    {
    }

    public static void Main() => new Program().Run();

    public void Run()
    {
        Calculator calculator = new();

        WriteLine(Prompt);
        while (ReadLine() is { Length: > 0 } line)
        {
            WriteLine(calculator.TryCalculate(line, out int result)
                ? result.ToString(CultureInfo.InvariantCulture)
                : $"Invalid calculation: '{line}'");
        }
    }
}
