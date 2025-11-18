using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assignment.Tests;

[TestClass]
public class SampleDataTests : BaseSampleDataTests<ISampleData>
{
    // 1.
    protected override ISampleData CreateService() => new SampleData();

    // 2.
    protected override IEnumerable<string> GetCsvRows() => DataService.CsvRows;

    protected override IEnumerable<string> GetUniqueSortedListOfStates() => DataService.GetUniqueSortedListOfStatesGivenCsvRows();

    protected override IEnumerable<IPerson> GetPeople() => DataService.People;

    protected override string GetAggregateSortedListOfStates() => DataService.GetAggregateSortedListOfStatesUsingCsvRows();

    protected override IEnumerable<(string FirstName, string LastName)> FilterByEmail(Predicate<string> filter) => DataService.FilterByEmailAddress(filter);

    protected override string GetAggregateListOfStates(IEnumerable<IPerson> people) => DataService.GetAggregateListOfStatesGivenPeopleCollection(people);
}