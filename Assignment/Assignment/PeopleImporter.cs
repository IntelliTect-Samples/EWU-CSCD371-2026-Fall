namespace Assignment;

/// <remarks>
/// Ownership: the importer takes ownership of the reader passed to its constructor and
/// disposes it when the importer is disposed. Callers must not use or dispose the reader themselves.
/// </remarks>
public sealed class PeopleImporter : IDisposable
{
    private readonly TextReader _reader;
    private bool _disposed;

    public PeopleImporter(TextReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);
        _reader = reader;
    }

    public static IReadOnlyList<Person> ImportFromFile(
        string path,
        ImportMode mode,
        Action<InvalidPersonRecordException>? onInvalidRecord = null,
        Func<Person, bool>? validator = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        using var importer = new PeopleImporter(new StreamReader(path));
        return importer.Import(mode, onInvalidRecord, validator);
    }

    public IReadOnlyList<Person> Import(
        ImportMode mode,
        Action<InvalidPersonRecordException>? onInvalidRecord = null,
        Func<Person, bool>? validator = null)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!Enum.IsDefined(mode))
        {
            throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unknown import mode.");
        }
        if (mode == ImportMode.SkipInvalid)
        {
            ArgumentNullException.ThrowIfNull(onInvalidRecord);
        }

        List<Person> people = [];
        int lineNumber = 1;
        ValidateHeader(_reader.ReadLine());

        string? line;
        while ((line = _reader.ReadLine()) is not null)
        {
            lineNumber++;
            Person person;
            try
            {
                person = PersonRecordParser.Parse(line);
            }
            catch (Exception ex) when (ex is FormatException or OverflowException)
            {
                Reject(mode, onInvalidRecord, new InvalidPersonRecordException(lineNumber, line, ex));
                continue;
            }

            if (validator is not null && !validator(person))
            {
                var inner = new FormatException("Person failed the custom validation rule.");
                Reject(mode, onInvalidRecord, new InvalidPersonRecordException(lineNumber, line, inner));
                continue;
            }

            people.Add(person);
        }

        return people;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        _reader.Dispose();
    }

    /// <summary>
    /// Line 1 must exactly match <see cref="PersonRecordParser.Header"/>. A missing or wrong header
    /// means the column layout is unknown, so it always throws regardless of the import mode.
    /// </summary>
    private static void ValidateHeader(string? header)
    {
        if (header is null)
        {
            throw new InvalidPersonRecordException(1, string.Empty,
                new FormatException("Input is empty; expected a CSV header."));
        }
        if (!string.Equals(header, PersonRecordParser.Header, StringComparison.Ordinal))
        {
            throw new InvalidPersonRecordException(1, header,
                new FormatException($"Expected header '{PersonRecordParser.Header}'."));
        }
    }

    private static void Reject(
        ImportMode mode,
        Action<InvalidPersonRecordException>? onInvalidRecord,
        InvalidPersonRecordException error)
    {
        if (mode == ImportMode.FailFast)
        {
            throw error;
        }
        onInvalidRecord!(error);
    }
}
