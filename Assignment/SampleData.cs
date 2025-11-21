using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Assignment;

public class SampleData : ISampleData
{
    // 1.
    public IEnumerable<string> CsvRows { get; } = File.ReadLines("People.csv").Skip(1);

    // 2.
    public IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows()
    {
        string[] states = CsvRows.ToArray().Select(row => row.Split(',')[6]).ToArray();
        Console.WriteLine(states.ToString());
        return states.Distinct().OrderBy(state => state);
    }

    // 3.
    public string GetAggregateSortedListOfStatesUsingCsvRows()
    {
        IEnumerable<string> states = GetUniqueSortedListOfStatesGivenCsvRows();
        return string.Join(", ", states);
    }

    // 4.
    public SampleData()
    {
        People = LoadPeopleFromCsvRows();
    }
    public IEnumerable<IPerson> People { get; init; }

    private IEnumerable<IPerson> LoadPeopleFromCsvRows()
    {
        List<IPerson> people = new List<IPerson>();
        foreach (string row in CsvRows)
        {
            string[] columns = row.Split(',');
            IAddress address = new Address(
                columns[4],
                columns[5],
                columns[6],
                columns[7]);
            IPerson person = new Person(
                columns[1],
                columns[2],
                address,
                columns[3]);
            people.Add(person);
        }
        return people.OrderBy(s => s.Address.State).ThenBy(s => s.Address.City).ThenBy(s => s.Address.Zip);
    }

    // 5.
    public IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(
        Predicate<string> filter) 
    {
        ArgumentNullException.ThrowIfNull(filter);

        return People.Where(person => filter(person.EmailAddress)).Select(person => (person.FirstName, person.LastName));
    }

    // 6.
    public string GetAggregateListOfStatesGivenPeopleCollection(
        IEnumerable<IPerson> people)
    {
        ArgumentNullException.ThrowIfNull(people);

        var uniqueStates = people.Select(people => people.Address.State).Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct().OrderBy(s => s).ToList();

        if (uniqueStates.Count == 0)
            return string.Empty;

        return uniqueStates.Skip(1).Aggregate(uniqueStates[0], (acc, state) => $"{acc}, {state}");
    }
}