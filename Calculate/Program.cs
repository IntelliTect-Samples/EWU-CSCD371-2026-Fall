namespace Calculate;

public class Program
{
	public Action<string?> WriteLine { get; init; } = Console.WriteLine;
	public Func<string?> ReadLine { get; init; } = Console.ReadLine;
	
	public static void Main()
	{
		new Program().Run();
	}

	public void Run()
	{
		Calculator calculator = new();
		WriteLine("Enter a calculation (e.g., 3 + 4):");
		string? calculation = ReadLine();

		if (calculator.TryCalculate(calculation, out int result))
		{
			WriteLine(result.ToString());
		}
		else
		{
			WriteLine("Invalid calculation.");
		}
	}
}