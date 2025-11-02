using Calculate;

namespace Calculate;

public class Program
{
    public Action<string> WriteLine { get; init; }
    public Func<string?> ReadLine { get; init; }

    public Program() {
        WriteLine = Console.WriteLine;
        ReadLine = Console.ReadLine;
    }

    public static int Main()
    {
        var program = new Program();

        program.WriteLine("Enter a calculation, or press enter to quit:");
        string? input;

        while (!string.IsNullOrWhiteSpace(input = program.ReadLine()))
        {
            if (Calculator.TryCalculate(input, out int result))
                program.WriteLine($"Result: {result}");
            else
                program.WriteLine("Invalid input or calculation error.");

            program.WriteLine("Enter another calculation, or press Enter to quit:");
        }
        program.WriteLine("Ciao!");
        return 0;
    }
}