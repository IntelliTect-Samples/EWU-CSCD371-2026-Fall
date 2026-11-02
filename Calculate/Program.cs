namespace Calculate;

public class Program
{
    public Action<string> WriteLine { get; init; } = Console.WriteLine;

    public Func<string?> ReadLine { get; init; } = Console.ReadLine;

    public Program()
    {
    }

    public void Run()
    {
        Calculator calculator = new();

        WriteLine("Enter a calculation (e.g., 3 + 4):");

        string? input = ReadLine();

        if (calculator.TryCalculate(input, out int result))
        {
            WriteLine($"Result: {result}");
        }
        else
        {
            WriteLine("Invalid calculation.");
        }
    }

    public static void Main(string[] args)
    {
        Program program = new();
        program.Run();
    }
}