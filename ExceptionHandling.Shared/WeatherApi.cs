using System.Net;

namespace ExceptionHandling.Shared;

/// <summary>
/// A fake web service client that fails the same ways HttpClient does,
/// without needing the network. Treat it as third-party code you can't change.
/// </summary>
public static class WeatherApi
{
    /// <param name="city">
    /// "Spokane" and "Cheney" succeed; "Atlantis" is 404 Not Found;
    /// "Overloaded" is 503 Service Unavailable; "Offline" never reaches the server (no status code).
    /// </param>
    /// <param name="apiVersion">Version 2 of the API reworded its error messages.</param>
    public static string GetForecast(string city, int apiVersion = 1) => city switch
    {
        "Spokane" => "Sunny, 72F",
        "Cheney" => "Windy, 65F",
        "Overloaded" => throw new HttpRequestException(
            "Response status code does not indicate success: 503 (Service Unavailable).",
            null, HttpStatusCode.ServiceUnavailable),
        "Offline" => throw new HttpRequestException(
            "No such host is known. (weather.example.com:443)"),
        _ => throw new HttpRequestException(
            apiVersion == 1
                ? $"Forecast not found for '{city}'."
                : $"No forecast is available for '{city}'.",
            null, HttpStatusCode.NotFound),
    };
}
