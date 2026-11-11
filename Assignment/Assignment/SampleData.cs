using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assignment;

public class SampleData : ISampleData
{
    // Column layout of People.csv: Id,FirstName,LastName,Email,StreetAddress,City,State,Zip
    private const int ColumnCount = 8;
    private const int FirstNameColumn = 1;
    private const int LastNameColumn = 2;
    private const int EmailColumn = 3;
    private const int StreetAddressColumn = 4;
    private const int CityColumn = 5;
    private const int StateColumn = 6;
    private const int ZipColumn = 7;

    private readonly string _csvPath;

    public SampleData() : this(Path.Combine(AppContext.BaseDirectory, "People.csv"))
    {
    }

    public SampleData(string csvPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(csvPath);
        _csvPath = csvPath;
    }

    // 1.
    // File.ReadLines is lazy and closes the underlying reader when enumeration finishes (or the
    // enumerator is disposed), so there is no IDisposable for this class to manage. A missing
    // file surfaces as FileNotFoundException/DirectoryNotFoundException when the property is read.
    public IEnumerable<string> CsvRows =>
        File.ReadLines(_csvPath)
            .Skip(1)
            .Where(row => !string.IsNullOrWhiteSpace(row));

    // 2.
    public IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows() =>
        CsvRows
            .Select(row => SplitRow(row)[StateColumn].Trim())
            .Distinct()
            .OrderBy(state => state, StringComparer.Ordinal);

    // 3.
    public string GetAggregateSortedListOfStatesUsingCsvRows() =>
        string.Join(",", GetUniqueSortedListOfStatesGivenCsvRows().ToArray());

    // 4.
    public IEnumerable<IPerson> People =>
        CsvRows
            .Select(ToPerson)
            .OrderBy(person => person.Address.State, StringComparer.Ordinal)
            .ThenBy(person => person.Address.City, StringComparer.Ordinal)
            .ThenBy(person => person.Address.Zip, StringComparer.Ordinal);

    // 5.
    public IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(
        Predicate<string> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        return People
            .Where(person => filter(person.EmailAddress))
            .Select(person => (person.FirstName, person.LastName));
    }

    // 6.
    public string GetAggregateListOfStatesGivenPeopleCollection(IEnumerable<IPerson> people)
    {
        ArgumentNullException.ThrowIfNull(people);

        return people
            .Select(person => person.Address.State)
            .Distinct()
            .OrderBy(state => state, StringComparer.Ordinal)
            .Aggregate(string.Empty, (states, state) =>
                states.Length == 0 ? state : $"{states},{state}");
    }

    private static string[] SplitRow(string row)
    {
        string[] columns = row.Split(',');
        if (columns.Length != ColumnCount)
        {
            throw new FormatException(
                $"Expected {ColumnCount} columns but found {columns.Length} in row: '{row}'.");
        }

        return columns;
    }

    private static Person ToPerson(string row)
    {
        string[] columns = SplitRow(row);

        Address address = new(
            columns[StreetAddressColumn].Trim(),
            columns[CityColumn].Trim(),
            columns[StateColumn].Trim(),
            columns[ZipColumn].Trim());

        return new Person(
            columns[FirstNameColumn].Trim(),
            columns[LastNameColumn].Trim(),
            address,
            columns[EmailColumn].Trim());
    }
}
