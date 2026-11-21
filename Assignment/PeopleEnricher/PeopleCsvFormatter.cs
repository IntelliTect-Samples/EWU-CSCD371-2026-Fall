namespace PeopleEnricher;

public sealed class PeopleCsvFormatter : IPeopleCsvFormatter
{
    public string Format(IEnumerable<EnrichedPerson> people)
    {
        return "To be implemented by the partner.";
        // Partner implements CSV formatting here.
    }
}