using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment.Tests;

[TestClass]
public class SampleDataAsyncTests : BaseSampleDataTests<IAsyncSampleData>
{
    // 1.
    protected override IAsyncSampleData CreateService() => new SampleDataAsync();

    private static List<T> ResolveAsync<T>(IAsyncEnumerable<T> asyncEnumerable)
    {
        var resolvedList = new List<T>();

        var task = Task.Run(async () =>
        {
            await foreach (var item in asyncEnumerable)
            {
                resolvedList.Add(item);
            }
        });

        task.GetAwaiter().GetResult();

        return resolvedList;
    }

    private static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(IEnumerable<T> source)
    {
        foreach (var item in source)
        {
            yield return item;
        }
        await Task.CompletedTask;
    }

    // 2.
    protected override IEnumerable<string> GetCsvRows()
        => ResolveAsync(DataService.CsvRows);

    // 3.
    protected override IEnumerable<string> GetUniqueSortedListOfStates()
        => ResolveAsync(DataService.GetUniqueSortedListOfStatesGivenCsvRows());

    // 4.
    protected override IEnumerable<IPerson> GetPeople()
        => ResolveAsync(DataService.People);

    // 5.
    protected override string GetAggregateSortedListOfStates()
        => DataService.GetAggregateSortedListOfStatesUsingCsvRows();

    // 6.
    protected override IEnumerable<(string FirstName, string LastName)> FilterByEmail(Predicate<string> filter)
        => ResolveAsync(DataService.FilterByEmailAddress(filter));

    // 7.
    protected override string GetAggregateListOfStates(IEnumerable<IPerson> people)
    {
        var asyncPeople = ToAsyncEnumerable(people);
        return DataService.GetAggregateListOfStatesGivenPeopleCollection(asyncPeople);
    }
}