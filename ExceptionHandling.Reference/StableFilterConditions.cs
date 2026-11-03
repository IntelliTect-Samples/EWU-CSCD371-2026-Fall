using System.Net;
using ExceptionHandling.Shared;

namespace ExceptionHandling.Reference;

public static class StableFilterConditions
{
    public static void Run()
    {
        foreach (int apiVersion in (int[])[1, 2])
        {
            Console.WriteLine($"Weather API v{apiVersion}, city = Atlantis (404)");
            Console.WriteLine($"  Message filter:     {Describe(() => GetForecastByMessage("Atlantis", apiVersion))}");
            Console.WriteLine($"  Status code filter: {Describe(() => GetForecastByStatusCode("Atlantis", apiVersion))}");
            Console.WriteLine();
        }
    }

    // ----- BEFORE -----
    // Breaks when the message is reworded, localized (CurrentUICulture), or even re-capitalized.
    private static string GetForecastByMessage(string city, int apiVersion)
    {
        try
        {
            return WeatherApi.GetForecast(city, apiVersion);
        }
        catch (HttpRequestException exception) when (exception.Message.Contains("not found"))
        {
            return "No forecast available.";
        }
    }

    // ----- AFTER -----
    // Filter on data that is part of the contract: the exception type and its properties.
    private static string GetForecastByStatusCode(string city, int apiVersion)
    {
        try
        {
            return WeatherApi.GetForecast(city, apiVersion);
        }
        catch (HttpRequestException exception) when (exception.StatusCode == HttpStatusCode.NotFound)
        {
            return "No forecast available.";
        }
    }

    private static string Describe(Func<string> getForecast)
    {
        try
        {
            return getForecast();
        }
        catch (HttpRequestException exception)
        {
            return $"NOT handled -> \"{exception.Message}\"";
        }
    }
}
