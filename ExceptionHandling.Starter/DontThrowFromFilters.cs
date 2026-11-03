using ExceptionHandling.Shared;

namespace ExceptionHandling.Starter;

public static class DontThrowFromFilters
{
    public static void Run()
    {
        foreach (string city in (string[])["Overloaded", "Offline"])
        {
            Report(city, () => GetForecast(city));
        }
    }

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

    // A server error (5xx) or a request that never reached the server is worth retrying.
    // TODO: Run it. Why isn't "Offline" handled? Where did the exception from this filter go?
    //       Then rewrite the filter so it cannot throw.
    private static bool IsTransient(HttpRequestException exception) =>
        (int)exception.StatusCode!.Value >= 500;

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
