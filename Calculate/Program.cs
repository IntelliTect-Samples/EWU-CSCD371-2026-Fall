namespace Calculate;

public class Program
{

    public Action<string> WriteLine {get; init;} = Console.WriteLine;

    public Func<string?> ReadLine {get; init; } = Console.ReadLine;

    public Program()
    {
    }

    public static void Main()
    {
        var program = new Program();
        var calculator = new Calculator();

        program.WriteLine("Enter calculation (e.g., '3 + 4') or 'exit' to quit:");

        while (true)
        {
            var input = program.ReadLine();
            if (string.IsNullOrWhiteSpace(input) || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                break;

            if (calculator.TryCalculate(input, out int result))
            {
                program.WriteLine($"Result: {result}");
            }
            else
            {
                program.WriteLine("Invalid calculation. Use format: number operator number (e.g., '3 + 4')");
            }
        }

        
    }
}