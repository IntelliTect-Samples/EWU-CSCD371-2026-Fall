using Assignment;

namespace Resilience;



public sealed class Importer : IDisposable
{
    private readonly TextReader _textReader;
    private bool _disposed = false;

    public Importer(string fileName)
        : this(new StreamReader(
            fileName ?? throw new ArgumentNullException(nameof(fileName))))
    { }

    /// <summary>
    /// Make tests easier to implement and setup
    /// </summary>
    /// <param name="textReader"></param>
    internal Importer(TextReader textReader)
    {
        ArgumentNullException.ThrowIfNull(textReader);

        _textReader = textReader;
    }

    private static Person ParsePerson(string line, int lineNumber,
        HashSet<int> ids)
    {
        try
        {
            string[] fields = line.Split(',');

            if (fields.Length != 8)
            {
                throw new FormatException(
                    "Expected exactly 8 fields.");
            }

            if (!int.TryParse(fields[0], out int id))
            {
                throw new FormatException("Id must be a number.");
            }

            if (!ids.Add(id))
            {
                throw new ArgumentException(
                    "Duplicate ID.");
            }

            Address address =
                new(
                    fields[4],
                    fields[5],
                    fields[6],
                    fields[7]);

            return new Person(
                id,
                fields[1],
                fields[2],
                fields[3],
                address);
        }
        catch (Exception ex)
            when (
                ex is FormatException ||
                ex is ArgumentException ||
                ex is ArgumentOutOfRangeException)
        {
            throw new InvalidPersonRecordException(
                $"Invalid record on line {lineNumber}.",
                lineNumber,
                line,
                ex);
        }
    }

    public IReadOnlyList<Person> Import(ImportMode importMode,
        Action<InvalidPersonRecordException>? onInvalidRecord = null)
    {
        ThrowIfDisposed();
        if (importMode == ImportMode.SkipInvalid)
        {
            ArgumentNullException.ThrowIfNull(onInvalidRecord);
        }

        int lineNumber = 1;

        _ = _textReader.ReadLine(); //skips first line

        HashSet<int> ids = [];
        List<Person> people = [];

        string? line;
        while ((line = _textReader.ReadLine()) != null)
        {
            ++lineNumber;
            try
            {
                people.Add(ParsePerson(line, lineNumber, ids));
            }
            catch (InvalidPersonRecordException ex) when (importMode == ImportMode.SkipInvalid)
            {
                onInvalidRecord?.Invoke(ex);
                continue;
            }
        }

        return people;
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _textReader.Dispose();

        _disposed = true;
    }

}
public sealed class ThrowingTextReader : TextReader
{
    public override string? ReadLine()
    {
        throw new IOException(
            "Simulated I/O failure.");
    }
}