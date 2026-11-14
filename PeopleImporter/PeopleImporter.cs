using System;
using System.Collections.Generic;
using System.IO;

namespace PeopleImporter;

/// <summary>
/// Imports people from a CSV reader.
/// </summary>
/// <remarks>
/// The importer takes ownership of the supplied reader and disposes it
/// when an import completes or fails.
/// An importer instance can only be used for one import.
/// </remarks>
public sealed class PeopleImporter : IDisposable
{
    private const string ExpectedHeader =
        "Id,FirstName,LastName,Email,StreetAddress,City,State,Zip";

    private TextReader? _reader;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="PeopleImporter"/> class.
    /// </summary>
    /// <param name="reader">The reader whose ownership is transferred to this importer.</param>
    public PeopleImporter(TextReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        _reader = reader;
    }

    /// <summary>
    /// Imports people using the specified invalid-record handling mode.
    /// </summary>
    /// <param name="mode">The import mode.</param>
    /// <param name="onInvalidRecord">
    /// Callback invoked for each invalid record when using SkipInvalid mode.
    /// </param>
    /// <returns>The successfully imported people.</returns>
    public IReadOnlyList<Person> Import(
        ImportMode mode,
        Action<InvalidPersonRecordException>? onInvalidRecord = null)
    {
        
        ObjectDisposedException.ThrowIf(_disposed, this);

        try
        {
            if (!Enum.IsDefined(mode))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(mode),
                    mode,
                    "Invalid import mode.");
            }

            if (mode == ImportMode.SkipInvalid)
            {
                ArgumentNullException.ThrowIfNull(onInvalidRecord);
            }

            TextReader reader = _reader!;

            List<Person> people = [];
            HashSet<int> seenIds = [];

            int lineNumber = 1;

            string? header = reader.ReadLine();

            if (header is null)
            {
                throw new InvalidPersonRecordException(
                    "CSV input must contain a header.");
            }

            ValidateHeader(header);

            string? rawLine;

            while ((rawLine = reader.ReadLine()) is not null)
            {
                lineNumber++;

                try
                {
                    Person person = PersonRecordParser.Parse(
                        rawLine,
                        lineNumber);

                    if (!seenIds.Add(person.ID))
                    {
                        throw new InvalidPersonRecordException(
                            $"Person ID '{person.ID}' is duplicated.",
                            lineNumber,
                            rawLine);
                    }

                    people.Add(person);
                }
                catch (InvalidPersonRecordException exception)
                    when (mode == ImportMode.SkipInvalid)
                {
                    onInvalidRecord!(exception);
                }
            }

            return people.AsReadOnly();
        }
        finally
        {
            Dispose();
        }
    }

    private static void ValidateHeader(string header)
    {
        if (!string.Equals(
                header,
                ExpectedHeader,
                StringComparison.Ordinal))
        {
            throw new InvalidPersonRecordException(
                "CSV header does not match the expected columns.",
                1,
                header);
        }
    }

    /// <summary>
    /// Releases the reader owned by this importer.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        TextReader? reader = _reader;
        _reader = null;

        reader?.Dispose();
    }
}