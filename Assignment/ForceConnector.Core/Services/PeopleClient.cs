using System;
using System.Collections.Generic;
using System.Text;

using ForceConnector.Core.Interfaces;

namespace ForceConnector.Core.Services;

/// <summary>
/// Downloads the people CSV from the API.
/// </summary>
public sealed class PeopleClient : IPeopleClient
{
    private readonly HttpClient _httpClient;

    public PeopleClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> DownloadPeopleCsvAsync(
        string user,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(user);

        string endpoint = $"/people/{user}";

        return await _httpClient.GetStringAsync(
            endpoint,
            cancellationToken);
    }
}