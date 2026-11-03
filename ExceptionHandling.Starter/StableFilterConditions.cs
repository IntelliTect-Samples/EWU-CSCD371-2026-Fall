using ExceptionHandling.Shared;

namespace ExceptionHandling.Starter;

public static class StableFilterConditions
{
    public static void Run()
    {
        foreach (int apiVersion in (int[])[1, 2])
        {
            Console.WriteLine($"Weather API v{apiVersion}, city = Atlantis (404)");
            Console.WriteLine($"  Message filter:     {Describe(() => GetForecastByMessage("Atlantis", apiVersion))}");
            // TODO: Write GetForecastByStatusCode(city, apiVersion) and add a line for it here.
            Console.WriteLine();
        }
    }

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
