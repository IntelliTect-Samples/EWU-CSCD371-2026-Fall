public class Program
{
    public Action<string> WriteLine { get; init; }
    public Func<string?> ReadLine { get; init; }

    public Program() {
        WriteLine = Console.WriteLine;
        ReadLine = Console.ReadLine;
    }

    public static int Main()
    {
        

        return 0;
    }
}