using System.Text.RegularExpressions;

namespace ExceptionHandling.Shared;

public static partial class Output
{
    public static void Heading(string title, string guideline)
    {
        Console.WriteLine();
        WriteLine(ConsoleColor.Cyan, $"=== {title} ===");
        WriteLine(ConsoleColor.DarkCyan, guideline);
        Console.WriteLine();
    }

    public static void Note(string text) => WriteLine(ConsoleColor.DarkGray, text);

    public static void Unhandled(Exception exception)
    {
        WriteLine(ConsoleColor.Red,
            $"[Top-level handler] {exception.GetType().Name}: {exception.Message}");
    }

    public static void StackTrace(string label, Exception exception)
    {
        WriteLine(ConsoleColor.Yellow, label);
        foreach (string line in (exception.StackTrace ?? "").Split(Environment.NewLine))
        {
            // Trim "in C:\long\path\File.cs:line 12" down to ":line 12" so it fits on a projector.
            Console.WriteLine("  " + SourcePath().Replace(line.Trim(), ":line "));
        }
    }

    public static void ExceptionChain(Exception exception)
    {
        string indent = "  ";
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            Console.WriteLine($"{indent}{current.GetType().Name}: {current.Message}");
            indent += "  ---> ";
        }

        if (exception.InnerException is null)
        {
            WriteLine(ConsoleColor.Red, $"{indent}(no InnerException - the original cause is gone)");
        }
    }

    internal static void WriteLine(ConsoleColor color, string text)
    {
        ConsoleColor original = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ForegroundColor = original;
    }

    [GeneratedRegex(@" in .*:line ")]
    private static partial Regex SourcePath();
}
