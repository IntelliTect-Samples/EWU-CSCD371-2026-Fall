using System.Runtime.CompilerServices;
using ExceptionHandling.Shared;

namespace ExceptionHandling.Reference;

public static class ThrowVersusThrowException
{
    public static void Run()
    {
        try
        {
            ParseAgeWithThrowException("forty-two");
        }
        catch (FormatException exception)
        {
            Output.StackTrace("throw exception;  - where did it REALLY fail?", exception);
        }

        Console.WriteLine();

        try
        {
            ParseAgeWithThrow("forty-two");
        }
        catch (FormatException exception)
        {
            Output.StackTrace("throw;  - original throw point preserved", exception);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int ParseAge(string text) =>
        int.TryParse(text, out int age)
            ? age
            : throw new FormatException($"'{text}' is not a valid age.");

    private static int ParseAgeWithThrowException(string text)
    {
        try
        {
            return ParseAge(text);
        }
        catch (FormatException exception)
        {
            Console.WriteLine("  (imagine some cleanup here)");
#pragma warning disable CA2200 // The analyzer flags exactly the mistake this demo shows.
            throw exception;
#pragma warning restore CA2200
        }
    }

    private static int ParseAgeWithThrow(string text)
    {
        try
        {
            return ParseAge(text);
        }
        catch (FormatException)
        {
            Console.WriteLine("  (imagine some cleanup here)");
            throw;
        }
    }
}
