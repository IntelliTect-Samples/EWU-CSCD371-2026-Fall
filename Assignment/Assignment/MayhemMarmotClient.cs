using Assignment9.Generated;

namespace Assignment9;

/// <summary>
/// Calls the Mayhem Marmot API through the Kiota-generated <see cref="MayhemMarmotApiClient"/>
/// and validates what comes back. Each call is a single attempt: it either returns validated data or
/// throws (<see cref="Microsoft.Kiota.Abstractions.ApiException"/> for an error status,
/// <see cref="HttpRequestException"/>/<see cref="HttpIOException"/> for connection problems,
/// <see cref="InvalidResponseException"/> for bad data, or <see cref="OperationCanceledException"/>).
/// Retrying is the caller's job (see <see cref="RetryPolicy"/>).
/// <para>The generated client and its request adapter are borrowed, not owned: whoever created
/// them disposes them.</para>
/// </summary>
public sealed class MayhemMarmotClient(MayhemMarmotApiClient api)
{
    private readonly MayhemMarmotApiClient api = api ?? throw new ArgumentNullException(nameof(api));

    public async Task<PeopleParseResult> GetPeopleAsync(string user, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(user);
        Stream? body = await api.People[user].GetAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        return PeopleCsv.Parse(await ReadToEndAsync(body, cancellationToken).ConfigureAwait(false));
    }

    public async Task<Location> GetLocationAsync(string zipCode, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(zipCode);
        Stream? body = await api.People.Zip[zipCode].GetAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        return LocationCsv.Parse(await ReadToEndAsync(body, cancellationToken).ConfigureAwait(false));
    }

    /// <summary>Reads and disposes the response stream. An empty response (no stream) reads as "".</summary>
    private static async Task<string> ReadToEndAsync(Stream? body, CancellationToken cancellationToken)
    {
        if (body is null)
        {
            return string.Empty;
        }

        using var reader = new StreamReader(body);
        return await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
    }
}
