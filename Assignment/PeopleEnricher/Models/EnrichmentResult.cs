namespace PeopleEnricher;

public sealed record EnrichmentResult(
    IReadOnlyList<EnrichedPerson> People,
    IReadOnlyList<RejectedPerson> Rejections);