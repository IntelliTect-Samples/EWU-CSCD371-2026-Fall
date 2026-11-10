namespace Assignment;

public class SampleData : ISampleData
{
    private readonly IEnumerable<string>? _CsvRows;

    public IEnumerable<string> CsvRows
    {
        get => _CsvRows ?? File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "People.csv"))
            .Skip(1).Where(row => !string.IsNullOrWhiteSpace(row));
        init => _CsvRows = value ?? throw new ArgumentNullException(nameof(value));
    }

    public IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows() =>
        CsvRows.Select(row => row.Split(',')[6]).Distinct().Order();

    public string GetAggregateSortedListOfStatesUsingCsvRows() =>
        string.Join(", ", GetUniqueSortedListOfStatesGivenCsvRows());

    public IEnumerable<IPerson> People => CsvRows
        .Select(row => row.Split(','))
        .Select(fields => new Person(fields[1], fields[2], fields[3],
            new Address(fields[4], fields[5], fields[6], fields[7])))
        .OrderBy(person => person.Address.State)
        .ThenBy(person => person.Address.City)
        .ThenBy(person => person.Address.Zip);

    public IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(Predicate<string> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        return People.Where(person => filter(person.EmailAddress))
            .Select(person => (person.FirstName, person.LastName));
    }

    public string GetAggregateListOfStatesGivenPeopleCollection(IEnumerable<IPerson> people)
    {
        ArgumentNullException.ThrowIfNull(people);

        return people.Select(person => person.Address.State).Distinct().Order()
            .Aggregate(string.Empty, (states, state) => states.Length == 0 ? state : $"{states}, {state}");
    }
}
