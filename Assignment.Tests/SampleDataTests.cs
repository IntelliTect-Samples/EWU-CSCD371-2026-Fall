using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class SampleDataTests
{
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task CsvRows_LoadsPeopleWithoutHeaderOrBlankLines(bool asynchronous)
    {
        SampleData data = new();
        string[] rows = asynchronous
            ? await new SampleDataAsync().CsvRows.ToArrayAsync()
            : data.CsvRows.ToArray();

        Assert.HasCount(50, rows);
        Assert.AreEqual<string>(
            "1,Priscilla,Jenyns,pjenyns0@state.gov,7884 Corry Way,Helena,MT,70577", rows[0]);
        Assert.IsTrue(rows.All(row => !string.IsNullOrWhiteSpace(row)));
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task States_FromRepeatedAddresses_ReturnEachStateOnceInAlphabeticalOrder(bool asynchronous)
    {
        SampleData data = CreateSampleData();

        string[] states = asynchronous
            ? await AsAsync(data).GetUniqueSortedListOfStatesGivenCsvRows().ToArrayAsync()
            : data.GetUniqueSortedListOfStatesGivenCsvRows().ToArray();
        string[] expected = ["OR", "WA"];

        Assert.HasCount(expected.Length, states);
        Assert.IsTrue(states.Zip(expected, (actual, state) => actual == state).All(match => match));
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task States_FromCsv_AreAlphabeticallySorted(bool asynchronous)
    {
        SampleData data = new();

        string[] states = asynchronous
            ? await AsAsync(data).GetUniqueSortedListOfStatesGivenCsvRows().ToArrayAsync()
            : data.GetUniqueSortedListOfStatesGivenCsvRows().ToArray();

        Assert.IsTrue(states.SequenceEqual(states.Order()));
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task AggregateStates_FromCsv_SeparatesUniqueSortedStatesWithCommas(bool asynchronous)
    {
        SampleData data = CreateSampleData();

        string states = asynchronous
            ? await AsAsync(data).GetAggregateSortedListOfStatesUsingCsvRows()
            : data.GetAggregateSortedListOfStatesUsingCsvRows();

        Assert.AreEqual<string>("OR, WA", states);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task People_FromCsv_PopulatesNamesEmailAndAddress(bool asynchronous)
    {
        SampleData data = new();

        IPerson[] people = asynchronous ? await new SampleDataAsync().People.ToArrayAsync() : data.People.ToArray();
        IPerson person = people.Single(person => person.EmailAddress == "pjenyns0@state.gov");

        Assert.AreEqual<int>(data.CsvRows.Count(), people.Length);
        Assert.AreEqual<string>("Priscilla", person.FirstName);
        Assert.AreEqual<string>("Jenyns", person.LastName);
        Assert.AreEqual<string>("pjenyns0@state.gov", person.EmailAddress);
        Assert.AreEqual<string>("7884 Corry Way", person.Address.StreetAddress);
        Assert.AreEqual<string>("Helena", person.Address.City);
        Assert.AreEqual<string>("MT", person.Address.State);
        Assert.AreEqual<string>("70577", person.Address.Zip);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task People_AreSortedByStateThenCityThenZip(bool asynchronous)
    {
        SampleData data = CreateSampleData();

        IPerson[] people = asynchronous ? await AsAsync(data).People.ToArrayAsync() : data.People.ToArray();
        string[] names = people.Select(person => person.FirstName).ToArray();

        Assert.IsTrue(names.SequenceEqual(new[] { "Grace", "Katherine", "Ada", "Alan" }));
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task FilterByEmailAddress_ReturnsOnlyMatchingNames(bool asynchronous)
    {
        SampleData data = CreateSampleData();

        Predicate<string> filter = email => email.EndsWith("@example.com", StringComparison.Ordinal);
        (string FirstName, string LastName)[] names = asynchronous
            ? await AsAsync(data).FilterByEmailAddress(filter).ToArrayAsync()
            : data.FilterByEmailAddress(filter).ToArray();

        Assert.IsTrue(names.SequenceEqual(new[] { ("Ada", "Lovelace"), ("Alan", "Turing") }));
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task AggregateStates_UsesSuppliedPeopleAndRemovesDuplicates(bool asynchronous)
    {
        SampleData data = new();
        SampleData supplied = CreateSampleData();

        string states = asynchronous
            ? await new SampleDataAsync().GetAggregateListOfStatesGivenPeopleCollection(supplied.People.ToAsyncEnumerable())
            : data.GetAggregateListOfStatesGivenPeopleCollection(supplied.People);

        Assert.AreEqual<string>(string.Join(", ", supplied.GetUniqueSortedListOfStatesGivenCsvRows()), states);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task AggregateStates_WithNoPeople_ReturnsEmptyString(bool asynchronous)
    {
        SampleData data = new();

        string states = asynchronous
            ? await new SampleDataAsync().GetAggregateListOfStatesGivenPeopleCollection(Array.Empty<IPerson>().ToAsyncEnumerable())
            : data.GetAggregateListOfStatesGivenPeopleCollection([]);

        Assert.AreEqual<string>(string.Empty, states);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task FilterByEmailAddress_WithNullFilter_ThrowsArgumentNullException(bool asynchronous)
    {
        SampleData data = new();

        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            _ = asynchronous
                ? await new SampleDataAsync().FilterByEmailAddress(null!).ToArrayAsync()
                : data.FilterByEmailAddress(null!).ToArray();
        });
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task AggregateStates_WithNullPeople_ThrowsArgumentNullException(bool asynchronous)
    {
        SampleData data = new();

        await Assert.ThrowsExactlyAsync<ArgumentNullException>(async () =>
        {
            _ = asynchronous
                ? await new SampleDataAsync().GetAggregateListOfStatesGivenPeopleCollection(null!)
                : data.GetAggregateListOfStatesGivenPeopleCollection(null!);
        });
    }

    private static SampleDataAsync AsAsync(SampleData data) => new() { CsvRows = data.CsvRows.ToAsyncEnumerable() };

    private static SampleData CreateSampleData() => new()
    {
        CsvRows =
        [
            "1,Ada,Lovelace,ada@example.com,1 Main St,Seattle,WA,98109",
            "2,Grace,Hopper,grace@navy.mil,2 Main St,Portland,OR,97201",
            "3,Alan,Turing,alan@example.com,3 Main St,Spokane,WA,99201",
            "4,Katherine,Johnson,katherine@nasa.gov,4 Main St,Seattle,WA,98101"
        ]
    };
}
