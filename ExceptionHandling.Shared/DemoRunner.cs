namespace ExceptionHandling.Shared;

public record Demo(string Title, string Guideline, Action Run);

public static class DemoRunner
{
    public static int Run(string[] args, IReadOnlyList<Demo> demos)
    {
        if (args.Length > 0)
        {
            return TryGetDemo(args[0], demos, out Demo? demo)
                ? RunDemo(demo)
                : Usage(demos);
        }

        while (true)
        {
            PrintMenu(demos);
            Console.Write("Choose a demo (Enter to quit): ");
            string? choice = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(choice))
            {
                return 0;
            }

            if (TryGetDemo(choice, demos, out Demo? selected))
            {
                RunDemo(selected);
            }
        }
    }

    private static int RunDemo(Demo demo)
    {
        Output.Heading(demo.Title, demo.Guideline);
        try
        {
            demo.Run();
            return 0;
        }
        // The top-level handler: the ONE place in this app that catches System.Exception.
        // It sits at the top of the call stack, reports the failure once, and decides the app's fate.
        catch (Exception exception)
        {
            Output.Unhandled(exception);
            return 1;
        }
    }

    private static bool TryGetDemo(string text, IReadOnlyList<Demo> demos, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Demo? demo)
    {
        demo = int.TryParse(text, out int number) && number >= 1 && number <= demos.Count
            ? demos[number - 1]
            : null;
        return demo is not null;
    }

    private static void PrintMenu(IReadOnlyList<Demo> demos)
    {
        Console.WriteLine();
        for (int index = 0; index < demos.Count; index++)
        {
            Console.WriteLine($"  {index + 1}. {demos[index].Title}");
        }
    }

    private static int Usage(IReadOnlyList<Demo> demos)
    {
        Console.WriteLine($"Usage: dotnet run -- <1-{demos.Count}>");
        PrintMenu(demos);
        return 2;
    }
}
