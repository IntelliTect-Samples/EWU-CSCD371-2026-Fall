namespace Calculate;

public class Program : ProgramBase
{
    public Program()
    {
    }

    public void Run()
    {
        var calculator = new Calculator();

        WriteLine("Enter a mathematical expression (e.g., 3 + 4):");
        string? input = ReadLine();

        if (calculator.TryCalculate(input, out float result))
        {
            WriteLine($"Result: {result}");
        }
        else
        {
            WriteLine("Invalid expression.");
        }
    }

    public static void Main(string[] args)
    {
        var program = new Program();
        program.Run();
    }
}