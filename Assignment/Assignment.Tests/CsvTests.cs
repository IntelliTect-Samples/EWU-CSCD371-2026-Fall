using Assignment.Interfaces;

namespace Assignment.Tests;

public sealed class CsvTests
{
    [Fact]
    public void ParseReturnsNoRowsForEmptyOrBlankInput()
    {
        Assert.Empty(Csv.Parse(string.Empty));
        Assert.Empty(Csv.Parse("\n\r\n"));
    }

    [Fact]
    public void ParseHandlesQuotedFieldsEscapedQuotesLineEndingsAndEmptyFields()
    {
        var rows = Csv.Parse(
            "a,b,\r\n" +
            "\"comma,field\",\"escaped \"\"quote\"\"\"\n\n" +
            "\"multi\nline\",end");

        Assert.Collection(
            rows,
            row => Assert.Equal(["a", "b", ""], row),
            row => Assert.Equal(["comma,field", "escaped \"quote\""], row),
            row => Assert.Equal(["multi\nline", "end"], row));
    }

    [Fact]
    public void ParseThrowsForUnterminatedQuotedField()
    {
        Assert.Throws<InvalidDataException>(() => Csv.Parse("first,\"unfinished"));
    }

    [Theory]
    [InlineData("plain", "plain")]
    [InlineData("with,comma", "\"with,comma\"")]
    [InlineData("with\"quote", "\"with\"\"quote\"")]
    [InlineData("with\nnewline", "\"with\nnewline\"")]
    [InlineData("with\rreturn", "\"with\rreturn\"")]
    public void EscapeQuotesFieldsOnlyWhenRequired(string value, string expected)
    {
        Assert.Equal(expected, Csv.Escape(value));
    }

    [Fact]
    public void FormatHeaderAndRowEscapeEveryField()
    {
        Assert.Equal("\"First,Name\",Last", Csv.FormatHeader(["First,Name", "Last"]));
        Assert.Equal("Ada,\"said \"\"hello\"\"\"", Csv.FormatRow(["Ada", "said \"hello\""]));
    }

    [Theory]
    [InlineData("Ada", "Lovelace", "ada@example.com", "99201", true)]
    [InlineData("", "Lovelace", "ada@example.com", "99201", false)]
    [InlineData("Ada", " ", "ada@example.com", "99201", false)]
    [InlineData("Ada", "Lovelace", "ada.example.com", "99201", false)]
    [InlineData("Ada", "Lovelace", "ada@@example.com", "99201", false)]
    [InlineData("Ada", "Lovelace", "ada @example.com", "99201", false)]
    [InlineData("Ada", "Lovelace", "ada@example", "99201", false)]
    [InlineData("Ada", "Lovelace", "ada@example.com", "9920", false)]
    [InlineData("Ada", "Lovelace", "ada@example.com", "99a01", false)]
    public void IsValidPersonChecksNamesEmailAndZipCode(
        string firstName,
        string lastName,
        string emailAddress,
        string zipCode,
        bool expected)
    {
        Assert.Equal(expected, Csv.IsValidPerson(new Person(firstName, lastName, emailAddress, zipCode)));
    }

    [Theory]
    [InlineData("", "Lovelace", "bad-email", "bad-zip", "Missing name")]
    [InlineData("Ada", "Lovelace", "bad-email", "99201", "Invalid email")]
    [InlineData("Ada", "Lovelace", "ada@example.com", "99a01", "Invalid ZIP code")]
    [InlineData("Ada", "Lovelace", "ada@example.com", "99201", "Invalid person")]
    public void PersonInvalidReasonReportsTheFirstApplicableReason(
        string firstName,
        string lastName,
        string emailAddress,
        string zipCode,
        string expected)
    {
        Assert.Equal(
            expected,
            Csv.PersonInvalidReason(new Person(firstName, lastName, emailAddress, zipCode)));
    }

    [Theory]
    [InlineData("Spokane", "WA", true)]
    [InlineData("", "WA", false)]
    [InlineData("Spokane", "wa", false)]
    [InlineData("Spokane", "W1", false)]
    [InlineData("Spokane", "WAS", false)]
    public void IsValidLocationRequiresCityAndUppercaseTwoLetterState(
        string city,
        string state,
        bool expected)
    {
        Assert.Equal(expected, Csv.IsValidLocation(city, state));
    }
}
