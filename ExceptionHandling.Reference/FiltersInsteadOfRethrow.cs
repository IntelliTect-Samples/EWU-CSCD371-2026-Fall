using System.Net;
using ExceptionHandling.Shared;

namespace ExceptionHandling.Reference;

public static class FiltersInsteadOfRethrow
{
    public static void Run()
    {
        foreach (string city in (string[])["Spokane", "Atlantis"])
        {
            Console.WriteLine($"{city}: {GetForecastOrDefault(city)}");
        }

        Console.WriteLine();
        Console.WriteLine("BEFORE: catch, inspect, rethrow  (Overloaded = 503)");
        Report(() => GetForecastWithRethrow("Overloaded"));

        Console.WriteLine();
        Console.WriteLine("AFTER: exception filter  (Overloaded = 503)");
        Report(() => GetForecastOrDefault("Overloaded"));
    }

    // ----- BEFORE -----
    private static string GetForecastWithRethrow(string city)
    {
        try
        {
            return CallWeatherApi(city);
        }
        catch (HttpRequestException exception)
        {
            Output.Note("    catch block entered - checking status code...");
            if (exception.StatusCode != HttpStatusCode.NotFound)
            {
                throw;
            }
            return "No forecast available.";
        }
    }

    // ----- AFTER -----
    private static string GetForecastOrDefault(string city)
    {
        try
        {
            return CallWeatherApi(city);
        }
        catch (HttpRequestException exception) when (IsNotFound(exception))
        {
            return "No forecast available.";
        }
    }

    private static bool IsNotFound(HttpRequestException exception)
    {
        Output.Note("    filter evaluated - stack NOT unwound yet...");
        return exception.StatusCode == HttpStatusCode.NotFound;
    }

    private static string CallWeatherApi(string city)
    {
        try
        {
            return WeatherApi.GetForecast(city);
        }
        finally
        {
            Output.Note("    finally: connection closed");
        }
    }

    private static void Report(Func<string> getForecast)
    {
        try
        {
            Console.WriteLine(getForecast());
        }
        catch (HttpRequestException exception)
        {
            Console.WriteLine($"  Propagated to caller: {exception.StatusCode}");
        }
    }
}
