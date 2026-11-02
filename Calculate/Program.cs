using ConsoleUtilities;
 
namespace Calculate;
 
public class Program : ProgramBase
{
    public Program() { }
 
    public static void Main() => new Program().Run();
 
    public void Run()
    {
        Calculator calculator = new();
        WriteLine("Enter a calculation (e.g. 3 + 4), or a blank line to quit:");
 
        while (ReadLine() is string input && !string.IsNullOrWhiteSpace(input))
        {
            WriteLine(calculator.TryCalculate(input, out int result)
                ? $"{input} = {result}"
                : $"Invalid calculation: '{input}'");
        }
    }
}
 