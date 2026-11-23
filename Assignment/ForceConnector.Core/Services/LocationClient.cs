
using ForceConnector.Core.Interfaces;

using ForceConnector.Core.Models;

namespace ForceConnector.Core.Services;
/// <summary>
/// Retrieves city and state information.
/// </summary>
public sealed class LocationClient : ILocationClient
{
    private readonly HttpClient _httpClient;

    public LocationClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Location> LookupAsync(
        string zipCode,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(zipCode);

        string endpoint = $"/people/zip/{zipCode}";

        string csv =
            await _httpClient.GetStringAsync(
                endpoint,
                cancellationToken);

        string[] parts =
            csv.Replace("\"", string.Empty)
               .Split(',');

        if (parts.Length != 2)
        {
            throw new InvalidDataException(
                "Location endpoint returned invalid data.");
        }

        return new Location
        {
            City = parts[0].Trim(),
            State = parts[1].Trim()
        };
    }
}