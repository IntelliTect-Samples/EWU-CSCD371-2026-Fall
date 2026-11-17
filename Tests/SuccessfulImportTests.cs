using Assignment;

namespace Tests;

public class SuccessfulImportTests
{
    [Fact]
    public void Import_ValidRecords_ReturnsEveryPersonWithAllFields()
    {
        using var importer = new PeopleCSVImporter(
            new FakeTextReader(TestCsv.WithHeader(TestCsv.Alice, TestCsv.Bob)));

        var people = importer.Import();

        Assert.Collection(
            people,
            alice =>
            {
                Assert.Equal("Alice", alice.FirstName);
                Assert.Equal("Smith", alice.LastName);
                Assert.Equal("alice@example.com", alice.EmailAddress);
                Assert.Equal("100 Main Street", alice.Address.StreetAddress);
                Assert.Equal("Seattle", alice.Address.City);
                Assert.Equal("WA", alice.Address.State);
                Assert.Equal("98101", alice.Address.Zip);
            },
            bob => Assert.Equal("Bob", bob.FirstName));
    }

    [Fact]
    public void Import_HeaderOnly_ReturnsEmptyList()
    {
        using var importer = new PeopleCSVImporter(new FakeTextReader(TestCsv.WithHeader()));

        var people = importer.Import();

        Assert.Empty(people);
    }

    [Fact]
    public void Import_FromTemporaryFile_ReturnsPeople()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllLines(path, TestCsv.WithHeader(TestCsv.Alice, TestCsv.Bob, TestCsv.Carol));
            using var importer = new PeopleCSVImporter(path);

            var people = importer.Import();

            string[] expected = ["Alice", "Bob", "Carol"];
            Assert.Equal(expected, people.Select(person => person.FirstName));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Import_PeopleCsvTestDataFile_ReturnsBothPeople()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "TestData", "People.csv");
        using var importer = new PeopleCSVImporter(path);

        var people = importer.Import();

        string[] expected = ["Alice", "Bob"];
        Assert.Equal(expected, people.Select(person => person.FirstName));
    }

    [Fact]
    public void Import_SkipInvalidModeWithOnlyValidRecords_NeverCallsCallback()
    {
        var callbackCount = 0;
        using var importer = new PeopleCSVImporter(
            new FakeTextReader(TestCsv.WithHeader(TestCsv.Alice, TestCsv.Bob)));

        var people = importer.Import(_ => callbackCount++);

        Assert.Equal(2, people.Count);
        Assert.Equal(0, callbackCount);
    }
}
