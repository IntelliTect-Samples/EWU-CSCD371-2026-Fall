namespace Calculate;

public class Program
{
    private Action<string> _WriteLine = Console.WriteLine;
    public Action<string> WriteLine
    { 
        get => _WriteLine;
        init => _WriteLine = value ?? throw new ArgumentNullException(nameof(value));
    }

    private Func<string?> _ReadLine = Console.ReadLine;
    public Func<string?> ReadLine
    {
        get => _ReadLine;
        init => _ReadLine = value ?? throw new ArgumentNullException(nameof(value));
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