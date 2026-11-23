namespace Assignment.Tests;

public class CsvParserTests
{
    [Fact]
    public void ValidPersonParses()
    {
        (Person? person, string? reason) = CsvParser.ParsePerson("John,Doe,john@example.com,99201");

        Assert.Equal(new Person("John", "Doe", "john@example.com", "99201"), person);
        Assert.Null(reason);
    }

    [Theory]
    [InlineData("", "Wrong number of columns")]
    [InlineData("John,Doe,john@example.com", "Wrong number of columns")]
    [InlineData("John,Doe,john@example.com,99201,extra", "Wrong number of columns")]
    [InlineData(",Doe,john@example.com,99201", "Missing name")]
    [InlineData("John,,john@example.com,99201", "Missing name")]
    [InlineData("John,Doe,invalid-email,99201", "Invalid email")]
    [InlineData("John,Doe,Bob <bob@x.com>,99201", "Invalid email")]
    [InlineData("John,Doe,john@example.com,123", "Invalid ZIP")]
    [InlineData("John,Doe,john@example.com,12A45", "Invalid ZIP")]
    [InlineData("John,Doe,john@example.com, 99201", "Invalid ZIP")]
    [InlineData(" John ,Doe,john@example.com,99201", "Missing name")]
    [InlineData("John,Doe, john@example.com,99201", "Invalid email")]
    public void InvalidPersonIsRejectedWithReason(string row, string expectedReason)
    {
        (Person? person, string? reason) = CsvParser.ParsePerson(row);

        Assert.Null(person);
        Assert.Equal(expectedReason, reason);
    }

    [Theory]
    [InlineData("\"Spokane\",\"WA\"", "Spokane", "WA")]
    [InlineData("\"New York\",\"NY\"", "New York", "NY")]
    [InlineData("\"Coeur d'Alene\",\"ID\"\n", "Coeur d'Alene", "ID")]
    [InlineData("\"Washington, D.C.\",\"DC\"", "Washington, D.C.", "DC")]
    public void ValidLocationParses(string line, string city, string state) =>
        Assert.Equal(new Location(city, state), CsvParser.ParseLocation(line));

    [Theory]
    [InlineData("")]
    [InlineData("BAD_DATA")]
    [InlineData("\"Spokane\"")]
    [InlineData("\"\",\"WA\"")]
    [InlineData("\"Spokane\",\"\"")]
    [InlineData("\"Spokane\",\"Washington\"")]
    [InlineData("\"Spokane\",\"wa\"")]
    [InlineData("\"Sp0kane\",\"WA\"")]
    public void InvalidLocationReturnsNull(string line) =>
        Assert.Null(CsvParser.ParseLocation(line));
}
