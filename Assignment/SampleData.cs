using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assignment;

/// <summary>
/// Concrete implementation of ISampleData that reads from People.csv
/// </summary>
/// <remarks>
/// Goals:
/// -Keep each operation pure and composable by returning IEnumerable where practical.
/// -Prefer LINQ operators per assignment instructions.
/// -Make sorting/uniqueness case-insensitive and deterministic.
/// -Centralize CSV parsing to a single helper to avoid duplication.
/// </remarks>
public class SampleData : ISampleData
{
    // 1.
    public IEnumerable<string> CsvRows => File.ReadAllLines("People.csv").Skip(1).Select(line => line.Trim());

    /// <summary>
    /// Splits a comma-delimited row into columns.
    /// This centralizes parsing to keep other methods focused on their logic.
    /// This dataset does not contain quoted commas, so a simple split is sufficient.
    /// </summary>
    private static string[] ParseColumns(string row) => row.Split(',', StringSplitOptions.TrimEntries);

    /// <summary>
    /// Returns an alphabetical, case-insensitive, unique list of state abbreviations
    /// derived from CsvRows.
    /// Uses Distinct with StringComparer.OrdinalIgnoreCase so that "CA" and "ca" are
    /// treated as the same key.
    /// </summary>
    public IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows()
        => CsvRows.Select(ParseColumns)
        .Select(cols => cols[6])
        .Where(s => !string.IsNullOrWhiteSpace(s))
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
        .ToArray();

    /// <summary>
    /// Returns a comma-separated string containing the unique, alphabetically
    /// sorted list of states derived from CsvRows.
    /// Composes directly on GetUniqueSortedListOfStatesGivenCsvRows to avoid
    /// duplicating logic, and uses string.Join(string, string[]) for simple,
    /// efficient joining.
    /// </summary>
    public string GetAggregateSortedListOfStatesUsingCsvRows()
        => string.Join(", ", GetUniqueSortedListOfStatesGivenCsvRows());

    /// <summary>
    /// Projects every CSV row into a Person with a populated Address,
    /// then returns the sequence sorted by State, City, and Zip.
    /// Sorting is performed with case-insensitive comparers for consistent, 
    /// deterministic ordering across platforms and datasets. Returning IEnumerable
    /// keeps the sequence composable.
    /// </summary>
    public IEnumerable<IPerson> People => 
        CsvRows.Select(ParseColumns)
        .Select (cols =>
        {
            var addr = new Address(
                streetAddress: cols[4],
                city: cols[5],
                state: cols[6],
                zip: cols[7]
            );

            return (IPerson)new Person(
                firstName: cols[1],
                lastName: cols[2],
                address: addr,
                emailAddress: cols[3]
                );
        })
        .OrderBy(p => p.Address.State, StringComparer.OrdinalIgnoreCase)
        .ThenBy(p => p.Address.City, StringComparer.OrdinalIgnoreCase)
        .ThenBy(p => p.Address.Zip, StringComparer.OrdinalIgnoreCase)
        .ToArray();

    /// <summary>
    /// Filters People by applying the provided Predicate to each person's EmailAddress,
    /// returning a sequence of (FirstName, LastName) tuples for those that match.
    /// This keeps the public surface tightly scoped to only the required data.
    /// The tuple is lightweight and convenient for assertions.
    /// </summary>
    public IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(
        Predicate<string> filter) => People
        .Where(p => filter(p.EmailAddress))
        .Select(p => (p.FirstName, p.LastName))
        .ToArray();

    /// <summary>
    /// Returns a comma-separated list of unique states present in the provided
    /// people collection. 
    /// To keep output deterministic and human-friendly, we make the set case-insensitive,
    /// sort it, then aggregate into a single string. The sort also guarantees stable test
    /// assertions. 
    /// </summary>
    public string GetAggregateListOfStatesGivenPeopleCollection(
        IEnumerable<IPerson> people)
    {
        var orderedDistinctStates = people
            .Select(p => p.Address.State)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(s => s, StringComparer.OrdinalIgnoreCase);

        return orderedDistinctStates.Aggregate(
            seed: "",
            func: (acc, s) => string.IsNullOrEmpty(acc) ? s : $"{acc}, {s}");

    }
}
