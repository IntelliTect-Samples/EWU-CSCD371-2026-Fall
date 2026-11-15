using Assignment;

namespace Resilience;



public sealed class Importer : IDisposable
{
    private readonly TextReader _textReader;
    private bool _disposed = false;

    public Importer(string fileName)
        : this(new StreamReader(fileName))
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

            int id = int.Parse(fields[0]);

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

    private IEnumerable<Person> ImportCsvRows(ImportMode importMode,
        Action<InvalidPersonRecordException>? onInvalidRecord = null)
    {
        ThrowIfDisposed();
        int rowCount = 0;
        string? line;
        _ = _textReader.ReadLine(); //skips first line
        while ((line = _textReader.ReadLine()) != null)
        {
            Person person;
            ++rowCount;
            HashSet<int> ids = new HashSet<int>();
            try
            {
                person = ParsePerson(line, rowCount, ids);
                
            }
            catch (InvalidPersonRecordException ex)
            {
                if (importMode == ImportMode.FailFast)
                {
                    throw new InvalidPersonRecordException($"Error parsing row {rowCount}: [{line}]", ex);
                }
                else if (importMode == ImportMode.SkipInvalid)
                {
                    onInvalidRecord?.Invoke(ex);
                    continue;
                }

                throw;
            }
            yield return person;
        }
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