using System.Text;

namespace Assignment8.ResilientImporter.Tests;

/// <summary>
/// Tests importing of people from the csv from unstandard data. Tests data harmonization and edge cases...
/// </summary>
public sealed class PeopleImporterTests
{
    private const string Header =
        "Id,FirstName,LastName,Email,StreetAddress,City,State,Zip";

    /// <summary>
    /// Test method tests that a harmonized data point produces no errs
    /// </summary>
    [Fact]
    public void ImportSucceedsForValidStringReader()
    {
        using var importer = new PeopleImporter(CreateReader(
            $"{Header}{Environment.NewLine}" +
            "1,Ada,Lovelace,ada@example.com,12 Math St,London,WA,99001"));

        var people = importer.Import(ImportMode.FailFast);

        var person = Assert.Single(people);
        Assert.Equal(1, person.Id);
        Assert.Equal("Ada", person.FirstName);
        Assert.Equal("Lovelace", person.LastName);
        Assert.Equal("ada@example.com", person.Email);
        Assert.Equal("12 Math St", person.Address.StreetAddress);
        Assert.Equal("London", person.Address.City);
        Assert.Equal("WA", person.Address.State);
        Assert.Equal("99001", person.Address.Zip);
    }


    /// <summary>
    ///  tests import w/ fake data and rm after successful addition
    /// </summary>
    [Fact]
    public void ImportSucceedsForTemporaryFileAndDeletesIt()
    {
        var path = Path.Combine(Path.GetTempPath(), $"people-{Guid.NewGuid():N}.csv");
        try
        {
            File.WriteAllText(
                path,
                $"{Header}{Environment.NewLine}" +
                "2,Grace,Hopper,grace@example.com,1 Navy Way,Arlington,VA,22201",
                new UTF8Encoding(false));

            using var reader = new StreamReader(path);
            using var importer = new PeopleImporter(reader);

            var people = importer.Import(ImportMode.FailFast);

            Assert.Equal(2, Assert.Single(people).Id);
        }
        finally
        {
            File.Delete(path);
        }

        Assert.False(File.Exists(path));
    }

    /// <summary>
    /// Import data from assn 7 csv. This was auto-gen by AI so that I didn't have to manually mv data from assn 7...
    /// </summary>
    [Fact]
    public void ImportSucceedsForAssignment7PeopleCsv()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "People.csv");
        using var reader = new StreamReader(path);
        using var importer = new PeopleImporter(reader);

        var people = importer.Import(ImportMode.FailFast);

        Assert.Equal(50, people.Count);
        Assert.Equal(1, people[0].Id);
        Assert.Equal(50, people[49].Id);
    }


    /// <summary>
    /// Fast fail testing (enum switch test). I want to see what happens when fast fail is enabled.
    /// Expected behavior should make this test pass 
    /// </summary>
    [Fact]
    public void FailFastThrowsAtFirstBadLineAndWrapsInnerException()
    {
        using var importer = new PeopleImporter(CreateReader(
            $"{Header}{Environment.NewLine}" +
            $"bad,Bad,Record,bad@example.com,1 Main St,City,WA,98001{Environment.NewLine}" +
            "3,Good,Record,good@example.com,2 Main St,City,OR,97001"));

        var exception = Assert.Throws<InvalidPersonRecordException>(
            () => importer.Import(ImportMode.FailFast));

        Assert.Equal(2, exception.LineNumber);
        Assert.StartsWith("bad,", exception.RawLine, StringComparison.Ordinal);
        Assert.IsType<FormatException>(exception.InnerException);
    }


    /// <summary>
    /// Other enum switch test to see if it produces expected behavior of rejecting a malformed record.
    /// </summary>
    [Fact]
    public void SkipInvalidReturnsValidPeopleAndReportsEveryRejectedRecord()
    {
        var records = new List<InvalidPersonRecordException>();
        using var importer = new PeopleImporter(CreateReader(
            $"{Header}{Environment.NewLine}" +
            $"1,Ada,Lovelace,ada@example.com,12 Math St,London,WA,99001{Environment.NewLine}" +
            $"1,Duplicate,Lovelace,dup@example.com,1 Street,City,WA,99001{Environment.NewLine}" +
            $"not-an-id,Missing,Email,bad,1 Street,City,WA,99001{Environment.NewLine}" +
            $"too,few,fields{Environment.NewLine}" +
            "4,,Hopper,grace@example.com,1 Navy Way,Arlington,VA,22201"));

        var people = importer.Import(
            ImportMode.SkipInvalid,
            exception => records.Add(exception));

        Assert.Equal(1, Assert.Single(people).Id);
        Assert.Equal(4, records.Count);
        Assert.Equal([3, 4, 5, 6], records.Select(record => record.LineNumber));
        Assert.All(records, record => Assert.NotNull(record.InnerException));
    }


    /// <summary>
    /// Reject duplicate data entry. This test proved to be the bane of my existence...
    /// </summary>
    [Fact]
    public void DuplicateIdIsRejectedAsInvalidOperationException()
    {
        using var importer = new PeopleImporter(CreateReader(
            $"{Header}{Environment.NewLine}" +
            $"7,One,Person,one@example.com,1 Main St,City,WA,98001{Environment.NewLine}" +
            "7,Two,Person,two@example.com,2 Main St,City,WA,98001"));

        var exception = Assert.Throws<InvalidPersonRecordException>(
            () => importer.Import(ImportMode.FailFast));

        Assert.Equal(3, exception.LineNumber);
        Assert.IsType<InvalidOperationException>(exception.InnerException);
    }


    /// <summary>
    /// Reject record doesn't get an ID (ideally...)
    /// </summary>
    [Fact]
    public void RejectedRecordDoesNotReserveItsIdForSkipInvalidImport()
    {
        var records = new List<InvalidPersonRecordException>();
        using var importer = new PeopleImporter(CreateReader(
            $"{Header}{Environment.NewLine}" +
            $"12,Missing,Email,bad,1 Street,City,WA,99001{Environment.NewLine}" +
            "12,Valid,Person,valid@example.com,2 Street,City,WA,99002"));

        var people = importer.Import(
            ImportMode.SkipInvalid,
            records.Add);

        Assert.Equal(12, Assert.Single(people).Id);
        Assert.Single(records);
    }


    /// <summary>
    /// Missing value in data produces except
    /// </summary>
    [Fact]
    public void MissingRequiredValuePreservesArgumentException()
    {
        using var importer = new PeopleImporter(CreateReader(
            $"{Header}{Environment.NewLine}" +
            "8, ,Lovelace,ada@example.com,12 Math St,London,WA,99001"));

        var exception = Assert.Throws<InvalidPersonRecordException>(
            () => importer.Import(ImportMode.FailFast));

        Assert.IsType<ArgumentException>(exception.InnerException);
    }

    /// <summary>
    /// Constructor enforces correct name in record 
    /// </summary>
    [Fact]
    public void ConstructorAndMethodArgumentsHaveCorrectNames()
    {
        Assert.Equal(
            "reader",
            Assert.Throws<ArgumentNullException>(() => new PeopleImporter(null!)).ParamName);

        using var importer = new PeopleImporter(CreateReader(Header));

        Assert.Equal(
            "invalidRecordCallback",
            Assert.Throws<ArgumentNullException>(
                () => importer.Import(ImportMode.SkipInvalid)).ParamName);

        Assert.Equal(
            "mode",
            Assert.Throws<ArgumentOutOfRangeException>(
                () => importer.Import((ImportMode)999)).ParamName);

        Assert.Equal(
            "firstName",
            Assert.Throws<ArgumentException>(
                () => new Person(1, " ", "Lovelace", "ada@example.com", new Address("Main", "London", "WA", "99001"))).ParamName);

        Assert.Equal(
            "address",
            Assert.Throws<ArgumentNullException>(
                () => new Person(1, "Ada", "Lovelace", "ada@example.com", null!)).ParamName);
    }

    /// <summary>
    /// Intentionally get the reader to fail to see if it produces an except like its supposed to...
    /// </summary>
    [Fact]
    public void ReaderIOExceptionPropagatesAsSameInstance()
    {
        var ioException = new IOException("reader failed");
        using var reader = new ThrowingTextReader(ioException);
        using var importer = new PeopleImporter(reader);

        var thrown = Assert.Throws<IOException>(
            () => importer.Import(ImportMode.FailFast));

        Assert.Same(ioException, thrown);
    }

    /// <summary>
    /// Test callback exception. Read method for more details
    /// </summary>
    [Fact]
    public void CallbackExceptionPropagatesAsSameInstance()
    {
        var callbackException = new InvalidOperationException("callback failed");
        using var importer = new PeopleImporter(CreateReader(
            $"{Header}{Environment.NewLine}" +
            "bad,Bad,Record,bad,1 Street,City,WA,99001"));

        var thrown = Assert.Throws<InvalidOperationException>(
            () => importer.Import(
                ImportMode.SkipInvalid,
                (Action<InvalidPersonRecordException>)(_ => throw callbackException)));

        Assert.Same(callbackException, thrown);
    }

    /// <summary>
    /// Dispose reader after import for mem efficiency test
    /// </summary>
    [Fact]
    public void DisposesReaderAfterSuccessfulImport()
    {
        using var reader = new TrackingTextReader(Header);
        var importer = new PeopleImporter(reader);

        importer.Import(ImportMode.FailFast);
        importer.Dispose();

        Assert.True(reader.IsDisposed);
        importer.Dispose();
        Assert.True(reader.IsDisposed);
    }

    /// <summary>
    /// Failed import should also produce same behavior 
    /// </summary>
    [Fact]
    public void DisposesReaderAfterFailedImport()
    {
        using var reader = new TrackingTextReader(
            $"{Header}{Environment.NewLine}bad,Person");
        var importer = new PeopleImporter(reader);

        Assert.Throws<InvalidPersonRecordException>(
            () => importer.Import(ImportMode.FailFast));

        importer.Dispose();

        Assert.True(reader.IsDisposed);
    }


    /// <summary>
    /// Ensure disposal isnt invoked again after JUST being disposed 
    /// A safeguard to ensure this opens after re-execution
    /// </summary>
    [Fact]
    public void UseAfterDisposalThrowsObjectDisposedException()
    {
        using var reader = new TrackingTextReader(Header);
        var importer = new PeopleImporter(reader);
        importer.Dispose();

        Assert.Throws<ObjectDisposedException>(
            () => importer.Import(ImportMode.FailFast));
    }


    /// <summary>
    ///  Custom delegation is handled as expected...
    /// </summary>
    [Fact]
    public void CustomValidationDelegateIsHandledInSelectedMode()
    {
        using var importer = new PeopleImporter(CreateReader(
            $"{Header}{Environment.NewLine}" +
            "9,Good,Person,good@example.com,1 Main St,City,WA,98001"));

        var exception = Assert.Throws<InvalidPersonRecordException>(
            () => importer.Import(
                ImportMode.FailFast,
                validationDelegate: _ => "Rejected by caller"));

        Assert.Equal(2, exception.LineNumber);
        Assert.IsType<InvalidOperationException>(exception.InnerException);
    }

    /// <summary>
    /// Same thing just different mode
    /// </summary>
    [Fact]
    public void CustomValidationDelegateIsHandledInSkipInvalidMode()
    {
        var records = new List<InvalidPersonRecordException>();
        using var importer = new PeopleImporter(CreateReader(
            $"{Header}{Environment.NewLine}" +
            "13,Rejected,Person,rejected@example.com,1 Main St,City,WA,98001"));

        var people = importer.Import(
            ImportMode.SkipInvalid,
            records.Add,
            _ => "Rejected by caller");

        Assert.Empty(people);
        Assert.Equal(2, Assert.Single(records).LineNumber);
    }

    /// <summary>
    /// Validation exception props unwrapped data 
    /// </summary>
    [Fact]
    public void UnexpectedValidationExceptionPropagatesUnwrapped()
    {
        var unexpected = new UnexpectedValidationException("unexpected");
        using var importer = new PeopleImporter(CreateReader(
            $"{Header}{Environment.NewLine}" +
            "10,Good,Person,good@example.com,1 Main St,City,WA,98001"));

        var thrown = Assert.Throws<UnexpectedValidationException>(
            () => importer.Import(
                ImportMode.FailFast,
                validationDelegate: _ => throw unexpected));

        Assert.Same(unexpected, thrown);
    }

    /// <summary>
    /// Filter failed exception for non-valid record. (DNE, Malformed, etc...)
    /// </summary>
    [Fact]
    public void WhenFilterDoesNotCatchUnexpectedRecordException()
    {
        var unexpected = new UnexpectedValidationException("not a record error");
        using var importer = new PeopleImporter(CreateReader(
            $"{Header}{Environment.NewLine}" +
            "11,Good,Person,good@example.com,1 Main St,City,WA,98001"));

        var thrown = Assert.Throws<UnexpectedValidationException>(
            () => importer.Import(
                ImportMode.FailFast,
                validationDelegate: _ => throw unexpected));

        Assert.Same(unexpected, thrown);
    }

    /// <summary>
    /// String writer test to see if data is mk correct
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private static StringReader CreateReader(string text)
        => new StringReader(text);

    private sealed class UnexpectedValidationException : Exception
    {
        public UnexpectedValidationException()
        {
        }

        public UnexpectedValidationException(string message)
            : base(message)
        {
        }

        public UnexpectedValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Tests that exception data is printed and override functions
    /// </summary>
    /// <param name="exception"></param>
    private sealed class ThrowingTextReader(Exception exception) : TextReader
    {
        public override string? ReadLine()
            => throw exception;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Tests to ensure the text reader is correctly tracking and disposing when approp.
    /// </summary>
    /// <param name="text"></param>
    private sealed class TrackingTextReader(string text) : TextReader
    {
        private readonly Queue<string?> lines =
            new(text.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n'));

        public bool IsDisposed { get; private set; }

        public override string? ReadLine()
        {
            if (lines.Count == 0)
            {
                return null;
            }

            return lines.Dequeue();
        }


        /// <summary>
        /// Disposal override test. I need to make sure this overrides even outside of test data
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            IsDisposed = true;
            base.Dispose(disposing);
        }
    }
    /// <summary>
    /// Tests the that the Default constructor of InvalidPersonRecordException sets the correct and expected default values. 
    /// </summary>
    [Fact]
    public void DefaultConstructorSetsExpectedDefaults()
    {
        var exception = new InvalidPersonRecordException();

        Assert.Equal("The person record is invalid.", exception.Message);
        Assert.Equal(0, exception.LineNumber);
        Assert.Equal(string.Empty, exception.RawLine);
        Assert.Null(exception.InnerException);
    }
    /// <summary>
    /// Tests that the Message Constructor of InvalidPersonRecordException sets message correctly.
    /// </summary>
    [Fact]
    public void MessageConstructorSetsMessage()
    {
        var exception = new InvalidPersonRecordException("Custom message");

        Assert.Equal("Custom message", exception.Message);
        Assert.Equal(0, exception.LineNumber);
        Assert.Equal(string.Empty, exception.RawLine);
        Assert.Null(exception.InnerException);
    }
    /// <summary>
    /// Tests that the Message and InnerException Constructor of InvalidPersonRecordException sets message and inner exception correctly.
    /// </summary>
    [Fact]
    public void MessageAndInnerExceptionConstructorSetsProperties()
    {
        var innerException = new FormatException("Inner exception");
        var exception = new InvalidPersonRecordException("Custom message", innerException);

        Assert.Equal("Custom message", exception.Message);
        Assert.Equal(0, exception.LineNumber);
        Assert.Equal(string.Empty, exception.RawLine);
        Assert.Same(innerException, exception.InnerException);
    }
    /// <summary>
    /// Tests that the Constructor throws when Inner exception is null. This is to ensure that the exception is always wrapped with a valid inner exception.
    /// </summary>
    [Fact]
    public void ConstructorThrowsWhenInnerExceptionIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new InvalidPersonRecordException("Custom message", null!));
    }
    /// <summary>
    /// Tests that the ToString method of InvalidPersonRecordException contains LineNumber and RawLine properties. This is to ensure that the exception provides useful information for debugging.
    /// </summary>
    [Fact]
    public void ToStringContainsLineNumberAndRawLine()
    {
         var exception = new InvalidPersonRecordException(
        "Bad Record",
        42,
        "bad,data");

        var toStringOutput = exception.ToString();

        Assert.Contains("LineNumber: 42", toStringOutput, StringComparison.Ordinal);
        Assert.Contains("RawLine: bad,data", toStringOutput, StringComparison.Ordinal);
    }
    /// <summary>
    /// PeopleImporter tests that tests for Missing header
    /// </summary>
    [Fact]
    public void MissingHeaderThrowsInvalidPersonRecordException()
    {
    using var importer = new PeopleImporter(
        new StringReader(string.Empty));

    var exception = Assert.Throws<InvalidPersonRecordException>(
        () => importer.Import(ImportMode.FailFast));

    Assert.Equal(1, exception.LineNumber);
    Assert.IsType<FormatException>(exception.InnerException);
    }
    /// <summary>
    /// Checks if the header of PeopleImporter Has the correct number of columns. If not, it throws an InvalidPersonRecordException.
    /// </summary>
    [Fact]
    public void HeaderWithIncorrectNumberOfColumnsThrowsInvalidPersonRecordException()
    {
        using var importer = new PeopleImporter(
            new StringReader("Id,FirstName,LastName,Email,StreetAddress,City,State"));

        var exception = Assert.Throws<InvalidPersonRecordException>(
            () => importer.Import(ImportMode.FailFast));

        Assert.Equal(1, exception.LineNumber);
        Assert.IsType<FormatException>(exception.InnerException);
    }
    /// <summary>
    /// Checks if the Header has the correct name. If not, it throws an InvalidPersonRecordException.
    /// </summary>
    [Fact]
    public void HeaderWithIncorrectNameThrowsInvalidPersonRecordException()
    {
        using var importer = new PeopleImporter(
            new StringReader("Id,FirstName,LastName,Email,StreetAddress,City,State,ZipCode"));

        var exception = Assert.Throws<InvalidPersonRecordException>(
            () => importer.Import(ImportMode.FailFast));

        Assert.Equal(1, exception.LineNumber);
        Assert.IsType<FormatException>(exception.InnerException);
    }
    /// <summary>
    /// Makes sure zeroId is rejected as invalid. This is to ensure that the Id property of Person is always a positive integer.
    /// </summary>
    [Fact]
    public void ZeroIdIsRejectedAsInvalid()
    {
    using var importer = new PeopleImporter(CreateReader(
        $"{Header}{Environment.NewLine}" +
        "0,Ada,Lovelace,ada@example.com,Street,City,WA,99001"));

    var exception = Assert.Throws<InvalidPersonRecordException>(
        () => importer.Import(ImportMode.FailFast));

    Assert.IsType<ArgumentOutOfRangeException>(
        exception.InnerException);
    }
    /// <summary>
    /// Makes sure negativeId is rejected as invalid. This is to ensure that the Id property of Person is always a positive integer.
    /// </summary>
    [Fact]
    public void NegativeIdIsRejectedAsInvalid()
    {
    using var importer = new PeopleImporter(CreateReader(
        $"{Header}{Environment.NewLine}" +
        "-1,Ada,Lovelace,ada@example.com,Street,City,WA,99001"));

    var exception = Assert.Throws<InvalidPersonRecordException>(
        () => importer.Import(ImportMode.FailFast));

    Assert.IsType<ArgumentOutOfRangeException>(
        exception.InnerException);
    }
    /// <summary>
    /// Ensures that the State must consist of two letters.
    /// </summary>
    [Fact]
    public void StateMustBeTwoLetters()
    {
        using var importer = new PeopleImporter(CreateReader(
            $"{Header}{Environment.NewLine}" +
            "1,Ada,Lovelace,ada@example.com,Street,City,Washington,99001"));

        var exception = Assert.Throws<InvalidPersonRecordException>(
            () => importer.Import(ImportMode.FailFast));

        Assert.Equal(2, exception.LineNumber);
        Assert.IsType<ArgumentException>(
            exception.InnerException);
    
    }
    /// <summary>
    /// Ensures that the Email must be in a valid format.
    /// </summary>
    [Fact]
    public void EmailMustBeValid()
    {
        using var importer = new PeopleImporter(CreateReader(
            $"{Header}{Environment.NewLine}" +
            "1,Ada,Lovelace,invalid-email,Street,City,WA,99001"));

        var exception = Assert.Throws<InvalidPersonRecordException>(
            () => importer.Import(ImportMode.FailFast));

        Assert.Equal(2, exception.LineNumber);
        Assert.IsType<ArgumentException>(
            exception.InnerException);
    
    }
    /// <summary>
    /// Ensures that the FirstName and LastName must not be empty or whitespace.
    /// </summary>
    [Fact]
    public void FirstNameAndLastNameMustNotBeEmptyOrWhitespace()
    {
        using var importer = new PeopleImporter(CreateReader(
            $"{Header}{Environment.NewLine}" +
            "1, , ,ada@example.com,Street,City,WA,99001"));

        var exception = Assert.Throws<InvalidPersonRecordException>(
            () => importer.Import(ImportMode.FailFast));

        Assert.Equal(2, exception.LineNumber);
        Assert.IsType<ArgumentException>(
            exception.InnerException);
    
    }
    ///
    /// <summary>
    /// Ensures that the StreetAddress and City must not be empty or whitespace.
    /// </summary>
    [Fact]
    public void StreetAddressAndCityMustNotBeEmptyOrWhitespace()
    {
        using var importer = new PeopleImporter(CreateReader(
            $"{Header}{Environment.NewLine}" +
            "1,Ada,Lovelace,ada@example.com, , ,WA,99001"));

        var exception = Assert.Throws<InvalidPersonRecordException>(
            () => importer.Import(ImportMode.FailFast));

        Assert.Equal(2, exception.LineNumber);
        Assert.IsType<ArgumentException>(
            exception.InnerException);
    
    }
    /// <summary>
    /// Ensures that the ValidationDelegate returning null is treated as a valid record.
    /// </summary>
    [Fact]
    public void ValidationDelegateReturningNullIsValid()
    {
        using var importer = new PeopleImporter(CreateReader(
            $"{Header}{Environment.NewLine}" +
            "1,Ada,Lovelace,ada@example.com,123 Main St,Anytown,WA,99001"));

        var people = importer.Import(
        ImportMode.FailFast,
        validationDelegate: _ => null);

        Assert.Single(people);  
    }
    /// <summary>
    /// Ensures that the ValidationDelegate returning an empty string is treated as an invalid record
    /// </summary>
    [Fact]
    public void ValidationDelegateReturningEmptyStringIsInvalid()
    {
        using var importer = new PeopleImporter(CreateReader(
            $"{Header}{Environment.NewLine}" +
            "1,Ada,Lovelace,ada@example.com,123 Main St,Anytown,WA,99001"));

        var exception = Assert.Throws<InvalidPersonRecordException>(
            () => importer.Import(
                ImportMode.FailFast,
                validationDelegate: _ => string.Empty));

        Assert.IsType<InvalidOperationException>(
            exception.InnerException);
    }
}