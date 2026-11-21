namespace PeopleEnricher;

public interface IPeopleCsvFormatter
{
    string Format(IEnumerable<EnrichedPerson> people);
}