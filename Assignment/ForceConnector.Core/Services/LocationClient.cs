
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

        IReadOnlyList<IReadOnlyList<string>> records =
            CsvRecordParser.Parse(csv);

        if (records.Count != 1 || records[0].Count != 2)
        {
            throw new InvalidDataException(
                "Location endpoint returned invalid data.");
        }

        return new Location
        {
            City = records[0][0].Trim(),
            State = records[0][1].Trim()
        };
    }
}