using System.Globalization;
using ConsoleUtilities;

namespace Calculate;

public class Program : ProgramBase
{
    public Program() { }

    public static void Main() => new Program().Run();

    public void Run()
    {
        Calculator calculator = new();
        WriteLine("Enter a calculation (for example, 3 + 4), or press Enter to exit.");

        while (ReadLine() is { Length: > 0 } calculation)
        {
            WriteLine(calculator.TryCalculate(calculation, out decimal result)
                ? result.ToString(CultureInfo.CurrentCulture)
                : "Invalid calculation. Use two integers and +, -, *, or /; division by zero is not allowed.");
        }
    }
}
