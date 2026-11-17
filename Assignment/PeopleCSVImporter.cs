using System.Globalization;

namespace Assignment;

public sealed class PeopleCSVImporter : IDisposable
{
    private readonly TextReader _reader;

    public PeopleCSVImporter(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _reader = File.OpenText(filePath);
    }

    public IReadOnlyList<IPerson> Import() => ImportRecords(null);

    public IReadOnlyList<IPerson> Import(Action<Exception> onInvalidRecord)
    {
        ArgumentNullException.ThrowIfNull(onInvalidRecord);
        return ImportRecords(onInvalidRecord);
    }

    private IReadOnlyList<IPerson> ImportRecords(Action<Exception>? onInvalidRecord)
    {
        var people = new List<IPerson>();
        try
        {
            _reader.ReadLine();

            var lineNumber = 1;
            string? line;
            while ((line = _reader.ReadLine()) is not null)
            {
                lineNumber++;
                try
                {
                    var fields = line.Split(',');
                    Validate(fields);

                    people.Add(new Person(
                        fields[1],
                        fields[2],
                        fields[3],
                        new Address(fields[4], fields[5], fields[6], fields[7])));
                }
                catch (Exception exception) when (
                    exception is FormatException or ArgumentException or IndexOutOfRangeException)
                {
                    var recordException = new InvalidPersonRecordException(
                        $"Invalid person record at line {lineNumber}: {exception.Message}",
                        lineNumber,
                        line,
                        exception);

                    if (onInvalidRecord is null)
                    {
                        throw recordException;
                    }

                    onInvalidRecord(recordException);
                }
            }

            return people;
        }
        finally
        {
            _reader.Dispose();
        }
    }

    private void Validate(string[] fields)
    {
        if (fields.Length != 8)
        {
            throw new FormatException("A person record must contain exactly eight fields.");
        }

        if (!int.TryParse(fields[0], NumberStyles.None, CultureInfo.InvariantCulture, out var id) || id <= 0)
        {
            throw new FormatException("The Id must be a positive integer.");
        }

        for (var index = 1; index < fields.Length; index++)
        {
            if (string.IsNullOrWhiteSpace(fields[index]))
            {
                throw new FormatException("Person and address fields cannot be empty.");
            }
        }
    }

    public void Dispose()
    {
        _reader.Dispose();
    }
}