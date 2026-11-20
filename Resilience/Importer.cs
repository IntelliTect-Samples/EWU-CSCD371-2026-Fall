namespace Resilience;

/// <summary>
/// Imports Person records from a CSV source.
/// </summary>
/// <remarks>
/// Validation performed during import includes:
/// <list type="bullet">
/// <item>
/// Record must contain exactly 8 fields.
/// </item>
/// <item>
/// Person IDs must be valid, positive integers.
/// </item>
/// <item>
/// Person IDs must be unique within the import.
/// </item>
/// </list>
/// </remarks>
public sealed class Importer : IDisposable
{
    private readonly TextReader _textReader;
    private bool _disposed;
    private bool _imported;

    public Importer(string fileName)
        : this(CreateReader(fileName))
    { }

    /// <summary>
    /// Creates a new importer, making tests easier to implement and setup
    /// </summary>
    /// <param name="textReader"></param>
    /// Ownership of the supplied reader is transferred
    /// to the importer. The reader will be disposed
    /// when this importer is disposed.
    /// </param>
    internal Importer(TextReader textReader)
    {
        ArgumentNullException.ThrowIfNull(textReader);

        _textReader = textReader;
    }
    private static StreamReader CreateReader(string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            fileName,
            nameof(fileName));

        return new StreamReader(fileName);
    }
    /// <summary>
    /// Parses a CSV line into a <see cref="Person"/> instance.
    /// </summary>
    /// <param name="line">
    /// The CSV record to parse.
    /// </param>
    /// <param name="lineNumber">
    /// The source line number used for error reporting.
    /// </param>
    /// <param name="ids">
    /// Tracks previously imported IDs so duplicate IDs can
    /// be detected within a single import operation.
    /// </param>
    /// <returns>
    /// A validated <see cref="Person"/> instance.
    /// </returns>
    /// <remarks>
    /// Validation rules:
    /// <list type="number">
    /// <item>
    /// <description>
    /// Each record must contain exactly 8 comma-separated fields.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// The Id field must be a valid integer greater than zero.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Duplicate Id values are not allowed within the same import.
    /// </description>
    /// </item>
    /// </list>
    /// Additional validation is performed by the
    /// <see cref="Person"/> and <see cref="Address"/> constructors.
    /// </remarks>
    /// <exception cref="FormatException">
    /// Thrown when the record format is invalid.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when a duplicate ID or invalid field value is detected.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the ID is less than or equal to zero.
    /// </exception>
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
            for (int i = 0; i < fields.Length; i++)
            {
                fields[i] = fields[i].Trim();
                if (string.IsNullOrWhiteSpace(fields[i]))
                {
                    throw new FormatException(
                        $"Field {i + 1} is required.");
                }
            }
            if (!int.TryParse(fields[0], out int id))
            {
                throw new FormatException("Id must be a valid 32-bit integer.");
            }

            

            Address address =
                new(
                    fields[4],
                    fields[5],
                    fields[6],
                    fields[7]);

            Person person = new(
                id,
                fields[1],
                fields[2],
                fields[3],
                address);

            if (!ids.Add(id))
            {
                throw new ArgumentException(
                    "Duplicate ID.");
            }

            return person;
        }
        catch (Exception ex)
            when (
                ex is FormatException ||
                ex is ArgumentException)
        {
            throw new InvalidPersonRecordException(
                $"Invalid record on line {lineNumber}.",
                lineNumber,
                line,
                ex);
        }
    }

    public IReadOnlyList<Person> Import(ImportMode importMode, Func<Person, (bool, string)>? validate,
        Action<InvalidPersonRecordException>? onInvalidRecord = null)
    {
        ThrowIfDisposed();

        if (_imported)
        {
            throw new InvalidOperationException(
                "Import can only be called once.");
        }

        _imported = true;

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
                Person person = ParsePerson(line, lineNumber, ids);

                var (isValid, message) = validate?.Invoke(person) ?? (true, string.Empty);

                if (!isValid)
                {
                    throw new InvalidPersonRecordException(
                        $"Error validating person on line {lineNumber}: {message}"
                        , lineNumber, line, new ArgumentException(message));
                }

                people.Add(person);
            }
            catch (InvalidPersonRecordException ex) when (importMode == ImportMode.SkipInvalid)
            {
                onInvalidRecord!.Invoke(ex); //Can't be null, null exception already caught
                continue;
            }
        }

        return people;
    }

    public IReadOnlyList<Person> Import(ImportMode importMode,
        Action<InvalidPersonRecordException>? onInvalidRecord = null)
    {
        return Import(importMode, null, onInvalidRecord);
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
