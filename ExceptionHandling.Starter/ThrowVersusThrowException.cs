using System.Runtime.CompilerServices;
using ExceptionHandling.Shared;

namespace ExceptionHandling.Starter;

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
            Output.StackTrace("throw exception;", exception);
        }

        Console.WriteLine();

        try
        {
            ParseAgeWithThrow("forty-two");
        }
        catch (FormatException exception)
        {
            Output.StackTrace("throw;", exception);
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
        catch (FormatException exception)
        {
            Console.WriteLine("  (imagine some cleanup here)");
            // TODO: Run first and compare the two stack traces. Then change this to throw;
            //       (delete the pragmas, the exception variable, and rerun).
#pragma warning disable CA2200
            throw exception;
#pragma warning restore CA2200
        }
    }
}
