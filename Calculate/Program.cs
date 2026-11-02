namespace Calculate;

public class Program
{
    private readonly Calculator _calculator = new Calculator();

    public Action<string?> WriteLine { get; init; } = Console.WriteLine;
    public Func<string?> ReadLine { get; init; } = Console.ReadLine;

    public Program() { }

    public static void Main() => new Program().Run();

    public void Run()
    {
        WriteLine("Enter a calculation such as '3 + 4' (blank line to quit):");

        while (ReadLine() is { Length: > 0 } input)
        {
            try
            {
                WriteLine(_calculator.TryCalculate(input, out int result)
                    ? $"{input} = {result}"
                    : "Invalid calculation. Use: <integer> <+|-|*|/> <integer>");
            }
            catch (DivideByZeroException ex)
            {
                WriteLine(ex.Message);
            }
            catch (OverflowException)
            {
                WriteLine("Number overflow");
            }
            catch (ArgumentException)
            {
                WriteLine("Invalid Argument received");
            }
        }
    }
}