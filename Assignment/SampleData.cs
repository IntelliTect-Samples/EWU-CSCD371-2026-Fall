using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assignment;

/// <summary>
/// Represents a collection of sample data for testing and demonstration purposes.
/// </summary> 
/// <remarks>
/// This class provides methods to read and manipulate sample data from a CSV file.
/// </remarks>
/// <seealso cref="ISampleData" />
/// <example>
/// <code>
/// var sampleData = new SampleData();
/// var uniqueStates = sampleData.GetUniqueSortedListOfStatesGivenCsvRows();
/// </code>
/// </example>
public class SampleData : ISampleData
{
    // 1.
    public IEnumerable<string> CsvRows =>
        File.ReadLines(Path.Combine(AppContext.BaseDirectory, "People.csv"))
            .Skip(1);
            
    // 2.
    public IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows() =>
        CsvRows
            .Select(row => row.Split(',')[6])
            .Distinct()
            .OrderBy(state => state);

    // 3.
    public string GetAggregateSortedListOfStatesUsingCsvRows()
    {
        string[] states = GetUniqueSortedListOfStatesGivenCsvRows()
            .Select(state => state)
            .ToArray();

        return string.Join(", ", states);
    }

    // 4.
   public IEnumerable<IPerson> People =>
    CsvRows
        .Select(row =>
        {
            string[] columns = row.Split(',');

            if (columns.Length != 8)
            {
                throw new FormatException(
                    $"Expected 8 CSV columns, but found {columns.Length}.");
            }

            var address = new Address(
                columns[4],
                columns[5],
                columns[6],
                columns[7]);

            return new Person(
                columns[1],
                columns[2],
                address,
                columns[3]);
        })
        .OrderBy(person => person.Address.State)
        .ThenBy(person => person.Address.City)
        .ThenBy(person => person.Address.Zip);

    // 5.
    public IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(
        Predicate<string> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        return People
            .Where(person => filter(person.EmailAddress))
            .Select(person => (
                FirstName: person.FirstName,
                LastName: person.LastName));
    }

    // 6.
    public string GetAggregateListOfStatesGivenPeopleCollection(
        IEnumerable<IPerson> people)
    {
        ArgumentNullException.ThrowIfNull(people);

        return people
            .Select(person => person.Address.State)
            .Distinct()
            .OrderBy(state => state)
            .Aggregate(
                string.Empty,
                (result, state) => result.Length == 0
                    ? state
                    : $"{result}, {state}");
    }
}
