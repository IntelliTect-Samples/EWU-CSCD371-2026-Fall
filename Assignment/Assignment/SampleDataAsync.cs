namespace Assignment;

using Assignment.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Assignment.SampleData;


/// <summary>
/// This class implements the IAsyncSampleData interface and provides asynchronous methods for reading and processing CSV data from a file named "People.csv".
/// It includes methods to retrieve CSV rows, get unique sorted lists of states, aggregate sorted lists of states, and retrieve people data asynchronously.
/// </summary>
public class SampleDataAsync : IAsyncSampleData
{
    // 1.
    public IAsyncEnumerable<string> CsvRows
    {
        get
        {
            return ReadCsvRows();
        }
    }


    /// <summary>
    /// Asynchronously reads CSV rows from the "People.csv" file, skipping the header line, and yields each row as a string.
    /// </summary>
    /// <returns></returns>
    private static async IAsyncEnumerable<string> ReadCsvRows()
    {
        using var reader = new StreamReader("People.csv");
        await reader.ReadLineAsync(); // Skip header line

        while (true)
        {
            var line = await reader.ReadLineAsync();
            if (line == null)
            {
                yield break;
            }

            yield return line;
        }
    }

    
    /// <summary>
    /// Asynchronously retrieves a unique sorted list of states from the CSV rows by reading each row, extracting the state information, and yielding the unique states in sorted order.
    /// </summary>
    /// <returns></returns>
    public async IAsyncEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows()
    {
        var states = new HashSet<string>();

        await foreach (var row in CsvRows)
        {
            var state = row.Split(',')[6];
            states.Add(state);
        }

        foreach (var state in states.OrderBy(s => s))
        {
            yield return state;
        }
    }

    
    /// <summary>
    /// Asynchronously retrieves an aggregate sorted list of states from the CSV rows by calling the GetUniqueSortedListOfStatesGivenCsvRows method, collecting the unique states, and returning them as a comma-separated string.
    /// </summary>
    /// <returns></returns>
    public string GetAggregateSortedListOfStatesUsingCsvRows()
    {
        var states = new List<string>();
        var enumerator = GetUniqueSortedListOfStatesGivenCsvRows().GetAsyncEnumerator();

        try
        {
            while (enumerator.MoveNextAsync().AsTask().GetAwaiter().GetResult())
            {
                states.Add(enumerator.Current);
            }
        }
        finally
        {
            enumerator.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        return string.Join(",", states);
    }


    /// <summary>
    /// Asynchronously filters the people data based on the provided email address filter predicate and returns an asynchronous enumerable of tuples containing the first and last names of the filtered people.
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(Predicate<string> filter)
    {
        await foreach (var person in People)
        {
            if (filter(person.EmailAddress))
            {
                yield return (person.FirstName, person.LastName);
            }
        }
    }


    /// <summary>
    /// Asynchronously retrieves an aggregate list of states from the provided asynchronous enumerable of people by extracting the state information from each person's address and returning them as a comma-separated string.
    /// </summary>
    /// <param name="people"></param>
    /// <returns></returns>
    private async static Task<string> GetAggregateListOfStatesGivenPeopleCollectionAsync(IAsyncEnumerable<IPerson> people)
    {
        var states = new HashSet<string>();
        await foreach (var person in people)
        {
            if (person.Address != null && !string.IsNullOrEmpty(person.Address.State))
            {
                states.Add(person.Address.State);
            }
        }

        return string.Join(",", states.OrderBy(s => s));
    }

    

    /// <summary>
    /// Asynchronously retrieves an asynchronous enumerable of IPerson objects by reading the CSV rows, parsing each row into a Person object, and yielding the resulting IPerson instances.
    /// The people are ordered by state, city, and zip code.
    /// </summary>
    public IAsyncEnumerable<IPerson> People
    {
        get
        {
            return ReadPeople();
        }
    }


    /// <summary>
    /// Asynchronously reads the CSV rows, parses each row into a Person object, and yields the resulting IPerson instances.
    /// The people are ordered by state, city, and zip code.
    /// </summary>
    /// <returns></returns>
    private async IAsyncEnumerable<IPerson> ReadPeople()
    {
        await foreach (var row in CsvRows)
        {
            var columns = row.Split(',');
            yield return new Person
            {
                FirstName = columns[1],
                LastName = columns[2],
                EmailAddress = columns[3],
                Address = new Address
                {
                    StreetAddress = columns[4],
                    City = columns[5],
                    State = columns[6],
                    Zip = columns[7]
                }
            };
        }
    }


    /// <summary>
    /// Asynchronously retrieves an aggregate list of states from the provided asynchronous enumerable of people by calling the GetAggregateListOfStatesGivenPeopleCollectionAsync method and returning the result synchronously.
    /// </summary>
    /// <param name="people"></param>
    /// <returns></returns>
    string IAsyncSampleData.GetAggregateListOfStatesGivenPeopleCollection(IAsyncEnumerable<IPerson> people)
    {
        // Synchronously wait for the async method to complete and return the result
        return GetAggregateListOfStatesGivenPeopleCollectionAsync(people).GetAwaiter().GetResult();
    }
}