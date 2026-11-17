using System.Globalization;

namespace Assignment;

/// <summary>
/// Imports people from CSV text.
/// </summary>
/// <remarks>
/// <b>Ownership:</b> the importer owns its <see cref="TextReader"/>, whether it opened the reader
/// itself (from a file path) or the caller passed one in. The reader is disposed when an import
/// finishes (successfully or not) or when <see cref="Dispose"/> is called, whichever comes first.
/// Callers must not use a reader after passing it to the importer.
/// An importer can import only once. Any use after that, or after disposal, throws
/// <see cref="ObjectDisposedException"/>.
/// </remarks>
public sealed class PeopleCSVImporter : IDisposable
{
    private readonly TextReader _reader;
    private bool _disposed;

    /// <summary>Creates an importer that reads the file at <paramref name="filePath"/>.</summary>
    public PeopleCSVImporter(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _reader = File.OpenText(filePath);
    }

    /// <summary>Creates an importer that takes ownership of <paramref name="reader"/>.</summary>
    public PeopleCSVImporter(TextReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);
        _reader = reader;
    }

    /// <summary>
    /// Gets or sets an optional extra validation rule, applied to each person after the built-in
    /// rules pass. Return <see langword="null"/> if the person is valid, or a message describing the
    /// violation. Violations are handled by the selected import mode like any other invalid record.
    /// Exceptions thrown by the delegate are not caught: they propagate unchanged.
    /// </summary>
    public Func<IPerson, string?>? Validator { get; init; }

    public IReadOnlyList<IPerson> Import() => ImportRecords(null);

    public IReadOnlyList<IPerson> Import(Action<Exception> onInvalidRecord)
    {
        ArgumentNullException.ThrowIfNull(onInvalidRecord);
        return ImportRecords(onInvalidRecord);
    }

    private IReadOnlyList<IPerson> ImportRecords(Action<Exception>? onInvalidRecord)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var people = new List<IPerson>();
        try
        {
            _reader.ReadLine();

            var lineNumber = 1;
            string? line;
            while ((line = _reader.ReadLine()) is not null)
            {
                lineNumber++;

                IPerson person;
                try
                {
                    var fields = line.Split(',');
                    Validate(fields);

                    person = new Person(
                        fields[1],
                        fields[2],
                        fields[3],
                        new Address(fields[4], fields[5], fields[6], fields[7]));
                }
                catch (Exception exception) when (exception is FormatException or ArgumentException)
                {
                    Reject(
                        new InvalidPersonRecordException(
                            $"Invalid person record at line {lineNumber}: {exception.Message}",
                            lineNumber,
                            line,
                            exception),
                        onInvalidRecord);
                    continue;
                }

                // Called outside the try block above, so exceptions thrown by the caller's
                // delegate are never wrapped or swallowed.
                var violation = Validator?.Invoke(person);
                if (violation is not null)
                {
                    Reject(
                        new InvalidPersonRecordException(
                            $"Invalid person record at line {lineNumber}: {violation}",
                            lineNumber,
                            line),
                        onInvalidRecord);
                    continue;
                }

                people.Add(person);
            }

            return people;
        }
        finally
        {
            Dispose();
        }
    }

    /// <summary>
    /// Fail-fast mode (no callback) throws the record exception; skip-invalid mode reports it.
    /// </summary>
    private static void Reject(InvalidPersonRecordException exception, Action<Exception>? onInvalidRecord)
    {
        if (onInvalidRecord is null)
        {
            throw exception;
        }

        onInvalidRecord(exception);
    }

    private static void Validate(string[] fields)
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

    /// <summary>Disposes the owned reader. Safe to call more than once.</summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _reader.Dispose();
    }
}
