namespace Calculate;

public class Program
{
    public Action<string?> WriteLine { get; init; } = Console.WriteLine;
    public Func<string?> ReadLine { get; init; } = Console.ReadLine;

    public Program() { }

    public static void Main()
    {
        new Program().Run();
    }

    public void Run()
    {
        Calculator calculator = new();

        WriteLine("Enter a calculation such as \"3 + 4\" (blank line to quit):");

        while (ReadLine() is string input && input.Length > 0)
        {
            if (calculator.TryCalculate(input, out int result))
            {
                WriteLine(result.ToString());
            }
            else
            {
                WriteLine("Invalid calculation.");
            }
        }
    }
}
