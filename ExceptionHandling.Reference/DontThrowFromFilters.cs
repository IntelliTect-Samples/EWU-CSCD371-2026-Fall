using System.Net;
using ExceptionHandling.Shared;

namespace ExceptionHandling.Reference;

public static class DontThrowFromFilters
{
    public static void Run()
    {
        Console.WriteLine("BEFORE: a filter that can throw");
        foreach (string city in (string[])["Overloaded", "Offline"])
        {
            Report(city, () => GetForecastBuggy(city));
        }

        Console.WriteLine();
        Console.WriteLine("AFTER: a filter that cannot throw");
        foreach (string city in (string[])["Overloaded", "Offline"])
        {
            Report(city, () => GetForecast(city));
        }
    }

    // ----- BEFORE -----
    private static string GetForecastBuggy(string city)
    {
        try
        {
            return WeatherApi.GetForecast(city);
        }
        catch (HttpRequestException exception) when (IsTransientBuggy(exception))
        {
            return "Service busy - will retry later.";
        }
    }

    // StatusCode is null when the request never reached the server (Offline).
    // .Value then throws InvalidOperationException, the runtime treats the filter as FALSE,
    // and the InvalidOperationException silently disappears. Note the ! we used to silence
    // the compiler's nullable warning - the compiler tried to tell us.
    private static bool IsTransientBuggy(HttpRequestException exception) =>
        (int)exception.StatusCode!.Value >= 500;

    // ----- AFTER -----
    private static string GetForecast(string city)
    {
        try
        {
            return WeatherApi.GetForecast(city);
        }
        catch (HttpRequestException exception) when (IsTransient(exception))
        {
            return "Service busy - will retry later.";
        }
    }

    // No status code (network failure) is transient too, and nothing here can throw.
    private static bool IsTransient(HttpRequestException exception) =>
        exception.StatusCode is null or >= HttpStatusCode.InternalServerError;

    private static void Report(string city, Func<string> getForecast)
    {
        try
        {
            Console.WriteLine($"  {city}: {getForecast()}");
        }
        catch (HttpRequestException exception)
        {
            Console.WriteLine($"  {city}: NOT handled -> {exception.Message}");
        }
    }
}
