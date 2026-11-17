using System;
using System.Collections.Generic;
using System.IO;

namespace Assignment;

public sealed class PeopleImporter : IDisposable
{
    private readonly TextReader _reader;
    private bool _disposed;

    // The importer owns and disposes the reader.
    public PeopleImporter(TextReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);
        _reader = reader;
    }

    public IReadOnlyList<Person> Import(
        ImportMode mode,
        Action<InvalidPersonRecordException>? onInvalidRecord = null,
        Func<Person, bool>? validator = null)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (mode != ImportMode.FailFast &&
            mode != ImportMode.SkipInvalid)
        {
            throw new ArgumentOutOfRangeException(nameof(mode));
        }

        if (mode == ImportMode.SkipInvalid &&
            onInvalidRecord is null)
        {
            throw new ArgumentNullException(nameof(onInvalidRecord));
        }

        List<Person> people = new();
        HashSet<int> ids = new();

        string? header = _reader.ReadLine();

        if (header != PersonRecordParser.Header)
        {
            throw new InvalidPersonRecordException(
                1,
                header ?? string.Empty,
                "Missing or incorrect CSV header.",
                new FormatException("Invalid CSV header."));
        }

        int lineNumber = 1;
        string? rawLine;

        while ((rawLine = _reader.ReadLine()) != null)
        {
            lineNumber++;
            Person person;

            try
            {
                person = PersonRecordParser.Parse(rawLine);

                if (ids.Contains(person.Id))
                {
                    throw new FormatException(
                        $"Duplicate ID: {person.Id}");
                }
            }
            catch (Exception ex) when (
                ex is FormatException or OverflowException)
            {
                var invalidRecord = new InvalidPersonRecordException(
                    lineNumber,
                    rawLine,
                    ex.Message,
                    ex);

                if (mode == ImportMode.FailFast)
                {
                    throw invalidRecord;
                }

                onInvalidRecord!(invalidRecord);
                continue;
            }

            // Extra Credit: Caller-supplied validation
            if (validator?.Invoke(person) == false)
            {
                var invalidRecord = new InvalidPersonRecordException(
                    lineNumber,
                    rawLine,
                    "Record failed custom validation.",
                    new FormatException(
                        "Custom validation rule was not satisfied."));

                if (mode == ImportMode.FailFast)
                {
                    throw invalidRecord;
                }

                onInvalidRecord!(invalidRecord);
                continue;
            }

            // Only accepted records count toward duplicate IDs
            ids.Add(person.Id);
            people.Add(person);
        }

        return people;
    }

    public static IReadOnlyList<Person> ImportFromFile(
        string path,
        ImportMode mode,
        Action<InvalidPersonRecordException>? onInvalidRecord = null,
        Func<Person, bool>? validator = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        using var importer = new PeopleImporter(File.OpenText(path));

        return importer.Import(mode, onInvalidRecord, validator);
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
}