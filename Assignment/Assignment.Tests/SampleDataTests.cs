using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class SampleDataTests
{
    private const string Header = "Id,FirstName,LastName,Email,StreetAddress,City,State,Zip";

    private readonly List<string> _tempFiles = [];

    [TestCleanup]
    public void Cleanup()
    {
        foreach (string file in _tempFiles)
        {
            File.Delete(file);
        }
    }

    // Writes a small CSV (with header) so tests can use a hardcoded list of addresses.
    private SampleData CreateSampleData(params string[] rows)
    {
        string path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.csv");
        File.WriteAllLines(path, rows.Prepend(Header));
        _tempFiles.Add(path);
        return new SampleData(path);
    }

    // ---- Item 1: CsvRows ----

    [TestMethod]
    public void CsvRows_SkipsHeaderRow()
    {
        List<string> rows = new SampleData().CsvRows.ToList();

        Assert.AreEqual<int>(50, rows.Count);
        Assert.IsFalse(rows.Contains(Header));
        Assert.IsTrue(rows[0].StartsWith("1,Priscilla,Jenyns", StringComparison.Ordinal));
    }

    [TestMethod]
    public void CsvRows_CountIsFileLinesMinusHeader()
    {
        int fileLines = File.ReadLines(Path.Combine(AppContext.BaseDirectory, "People.csv")).Count();

        Assert.AreEqual<int>(fileLines - 1, new SampleData().CsvRows.Count());
    }

    [TestMethod]
    public void CsvRows_MissingFile_ThrowsFileNotFoundException()
    {
        SampleData data = new(Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.csv"));

        Assert.Throws<FileNotFoundException>(() => data.CsvRows.ToList());
    }

    [TestMethod]
    public void Constructor_NullOrBlankPath_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new SampleData(null!));
        Assert.Throws<ArgumentException>(() => new SampleData("  "));
    }

    [TestMethod]
    public void CsvRows_MalformedRow_ThrowsFormatExceptionWhenParsed()
    {
        SampleData data = CreateSampleData("1,Only,Three");

        Assert.Throws<FormatException>(() => data.GetUniqueSortedListOfStatesGivenCsvRows().ToList());
    }

    // ---- Item 2: unique sorted states ----

    [TestMethod]
    public void GetUniqueSortedStates_HardcodedAddresses_ReturnsSortedUniqueList()
    {
        SampleData data = CreateSampleData(
            "1,A,One,a@x.com,1 Main St,Dallas,TX,75001",
            "2,B,Two,b@x.com,2 Main St,Phoenix,AZ,85001",
            "3,C,Three,c@x.com,3 Main St,Houston,TX,77001",
            "4,D,Four,d@x.com,4 Main St,Fresno,CA,93650",
            "5,E,Five,e@x.com,5 Main St,Tucson,AZ,85701");

        string[] expected = ["AZ", "CA", "TX"];
        List<string> actual = data.GetUniqueSortedListOfStatesGivenCsvRows().ToList();

        Assert.AreEqual<int>(expected.Length, actual.Count);
        Assert.IsTrue(expected.Zip(actual, (e, a) => e == a).All(match => match));
    }

    [TestMethod]
    public void GetUniqueSortedStates_RealData_IsSortedAndUnique()
    {
        SampleData data = new();
        List<string> states = data.GetUniqueSortedListOfStatesGivenCsvRows().ToList();

        bool isSorted = states
            .Zip(states.Skip(1), (previous, next) => string.CompareOrdinal(previous, next) < 0)
            .All(ordered => ordered);
        Assert.IsTrue(isSorted, "States must be in strictly ascending order (which also implies unique).");

        // Independent check: every state in the CSV appears, and nothing else does.
        HashSet<string> statesInCsv = data.CsvRows.Select(row => row.Split(',')[6]).ToHashSet();
        Assert.AreEqual<int>(statesInCsv.Count, states.Count);
        Assert.IsTrue(states.All(statesInCsv.Contains));
    }

    // ---- Item 3: aggregate sorted states ----

    [TestMethod]
    public void GetAggregateSortedListOfStatesUsingCsvRows_IsCommaSeparatedUniqueSortedStates()
    {
        SampleData data = new();
        string[] states = data.GetUniqueSortedListOfStatesGivenCsvRows().ToArray();

        string actual = data.GetAggregateSortedListOfStatesUsingCsvRows();

        Assert.AreEqual<string>(string.Join(",", states), actual);
        Assert.AreEqual<int>(states.Length, actual.Split(',').Length);
    }

    [TestMethod]
    public void GetAggregateSortedListOfStatesUsingCsvRows_HardcodedAddresses()
    {
        SampleData data = CreateSampleData(
            "1,A,One,a@x.com,1 Main St,Dallas,TX,75001",
            "2,B,Two,b@x.com,2 Main St,Phoenix,AZ,85001",
            "3,C,Three,c@x.com,3 Main St,Houston,TX,77001");

        Assert.AreEqual<string>("AZ,TX", data.GetAggregateSortedListOfStatesUsingCsvRows());
    }

    // ---- Item 4: People ----

    [TestMethod]
    public void People_ReturnsEveryCsvRowWithAddressPopulated()
    {
        SampleData data = new();
        List<IPerson> people = data.People.ToList();
        List<string> rows = data.CsvRows.ToList();

        Assert.AreEqual<int>(rows.Count, people.Count);

        // Compare against the raw CSV: the same people exist, matched on e-mail address.
        Dictionary<string, string[]> columnsByEmail = rows
            .Select(row => row.Split(','))
            .ToDictionary(columns => columns[3]);

        foreach (IPerson person in people)
        {
            string[] columns = columnsByEmail[person.EmailAddress];
            Assert.AreEqual<string>(columns[1], person.FirstName);
            Assert.AreEqual<string>(columns[2], person.LastName);
            Assert.IsNotNull(person.Address);
            Assert.AreEqual<string>(columns[4], person.Address.StreetAddress);
            Assert.AreEqual<string>(columns[5], person.Address.City);
            Assert.AreEqual<string>(columns[6], person.Address.State);
            Assert.AreEqual<string>(columns[7], person.Address.Zip);
        }
    }

    [TestMethod]
    public void People_AreSortedByStateThenCityThenZip()
    {
        List<IPerson> people = new SampleData().People.ToList();

        bool isSorted = people
            .Zip(people.Skip(1), (previous, next) => Compare(previous.Address, next.Address) <= 0)
            .All(ordered => ordered);

        Assert.IsTrue(isSorted);
        Assert.AreEqual<string>("AL", people.First().Address.State);
        Assert.AreEqual<string>("WV", people.Last().Address.State);

        static int Compare(IAddress a, IAddress b)
        {
            int result = string.CompareOrdinal(a.State, b.State);
            if (result == 0)
            {
                result = string.CompareOrdinal(a.City, b.City);
            }

            return result != 0 ? result : string.CompareOrdinal(a.Zip, b.Zip);
        }
    }

    [TestMethod]
    public void People_HardcodedAddresses_AreSortedByStateCityZip()
    {
        SampleData data = CreateSampleData(
            "1,A,One,a@x.com,1 Main St,Phoenix,AZ,85002",
            "2,B,Two,b@x.com,2 Main St,Phoenix,AZ,85001",
            "3,C,Three,c@x.com,3 Main St,Mesa,AZ,85201",
            "4,D,Four,d@x.com,4 Main St,Austin,TX,73301");

        string[] actual = data.People.Select(p => p.FirstName).ToArray();

        string[] expected = ["C", "B", "A", "D"]; // Mesa < Phoenix(85001) < Phoenix(85002) < TX
        Assert.IsTrue(expected.Zip(actual, (e, a) => e == a).All(match => match));
        Assert.AreEqual<int>(expected.Length, actual.Length);
    }

    // ---- Item 5: FilterByEmailAddress ----

    [TestMethod]
    public void FilterByEmailAddress_GovAddresses_ReturnsMatchingNames()
    {
        List<(string FirstName, string LastName)> actual =
            new SampleData().FilterByEmailAddress(email => email.EndsWith(".gov", StringComparison.Ordinal)).ToList();

        Assert.AreEqual<int>(4, actual.Count);
        Assert.IsTrue(actual.Contains(("Priscilla", "Jenyns")));
        Assert.IsTrue(actual.Contains(("Amelia", "Toal")));
        Assert.IsTrue(actual.Contains(("Ev", "Challace")));
        Assert.IsTrue(actual.Contains(("Marijn", "McKennan")));
    }

    [TestMethod]
    public void FilterByEmailAddress_NoMatches_ReturnsEmpty()
    {
        Assert.AreEqual<int>(0, new SampleData().FilterByEmailAddress(_ => false).Count());
    }

    [TestMethod]
    public void FilterByEmailAddress_MatchesEveryone_ReturnsAllPeople()
    {
        SampleData data = new();

        Assert.AreEqual<int>(data.People.Count(), data.FilterByEmailAddress(_ => true).Count());
    }

    [TestMethod]
    public void FilterByEmailAddress_NullFilter_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new SampleData().FilterByEmailAddress(null!));
    }

    // ---- Item 6: GetAggregateListOfStatesGivenPeopleCollection ----

    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_MatchesCsvRowsStates()
    {
        SampleData data = new();

        string actual = data.GetAggregateListOfStatesGivenPeopleCollection(data.People);
        string expected = string.Join(",", data.GetUniqueSortedListOfStatesGivenCsvRows());

        Assert.AreEqual<string>(expected, actual);
    }

    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_IsUniqueWhenPeopleShareStates()
    {
        SampleData data = CreateSampleData(
            "1,A,One,a@x.com,1 Main St,Dallas,TX,75001",
            "2,B,Two,b@x.com,2 Main St,Houston,TX,77001",
            "3,C,Three,c@x.com,3 Main St,Phoenix,AZ,85001");

        Assert.AreEqual<string>("AZ,TX", data.GetAggregateListOfStatesGivenPeopleCollection(data.People));
    }

    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_EmptyCollection_ReturnsEmptyString()
    {
        Assert.AreEqual<string>(string.Empty,
            new SampleData().GetAggregateListOfStatesGivenPeopleCollection([]));
    }

    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_NullPeople_Throws()
    {
        Assert.Throws<ArgumentNullException>(
            () => new SampleData().GetAggregateListOfStatesGivenPeopleCollection(null!));
    }
}
