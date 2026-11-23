using ForceConnector.Core.Interfaces;
using ForceConnector.Core.Models;

namespace ForceConnector.Core.Services;

public sealed class PeopleEnricher : IPeopleEnricher
{
    private readonly IPeopleClient _peopleClient;

    private readonly ILocationClient _locationClient;

    public PeopleEnricher(
        IPeopleClient peopleClient,
        ILocationClient locationClient)
    {
        _peopleClient = peopleClient;
        _locationClient = locationClient;
    }

    public async Task<EnrichmentResult> EnrichAsync(
        string user,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}