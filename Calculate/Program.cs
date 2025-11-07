using ConsoleUtilities;

namespace Calculate;
public class Program : ProgramBase
{
    public static void Main()
    {
        Program program = new();
        Calculator calculator = new();

        program.WriteLine("Enter calculation (examples: 3 + 4 or 2 - 5)");
        program.WriteLine("Make sure there is a space between each input:");

        string? input = program.ReadLine();

        if (input is null)
        {
            program.WriteLine("There wasn't any input");
            return;
        }

        if (Calculator.TryCalculate<int>(input, out int intResult))
        {
            program.WriteLine($"The result is: {intResult}");
        }
        else if (Calculator.TryCalculate<double>(input, out double doubleResult))
        {
            program.WriteLine($"The result is: {doubleResult}");
        }
        else
        {
            program.WriteLine("Something went wrong with the calculation");
        }
    }
}
