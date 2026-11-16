using System.Globalization;

namespace PeopleImport;

public sealed class PeopleImporter : IDisposable
{
    private readonly TextReader _Reader;
    private bool _Disposed;

    public PeopleImporter(TextReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        _Reader = reader;
    }

    public IReadOnlyList<Person> Import(ImportMode mode = ImportMode.FailFast,
        Action<InvalidPersonRecordException>? reportError = null, Predicate<Person>? validate = null)
    {
        ObjectDisposedException.ThrowIf(_Disposed, this);
        if (mode is not (ImportMode.FailFast or ImportMode.SkipInvalid))
        {
            throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unsupported import mode.");
        }
        if (mode == ImportMode.SkipInvalid)
        {
            ArgumentNullException.ThrowIfNull(reportError);
        }

        List<Person> people = [];
        _Reader.ReadLine();
        int lineNumber = 1;
        while (_Reader.ReadLine() is { } line)
        {
            lineNumber++;
            Person person;
            try
            {
                person = ParseRecord(line, lineNumber);
            }
            catch (InvalidPersonRecordException exception) when (mode == ImportMode.SkipInvalid)
            {
                reportError!(exception);
                continue;
            }

            if (validate is not null && !validate(person))
            {
                InvalidPersonRecordException exception = new(lineNumber, line,
                    $"Person record at line {lineNumber} does not satisfy the supplied validation rule.");
                if (mode == ImportMode.FailFast)
                {
                    throw exception;
                }
                reportError!(exception);
                continue;
            }

            people.Add(person);
        }

        return people;
    }

    public void Dispose()
    {
        if (_Disposed)
        {
            return;
        }

        _Disposed = true;
        _Reader.Dispose();
    }

    private static Person ParseRecord(string line, int lineNumber)
    {
        try
        {
            string[] fields = line.Split(',', StringSplitOptions.TrimEntries);
            if (fields.Length != 8)
            {
                throw new FormatException("A person record must contain exactly eight fields.");
            }

            return new Person(int.Parse(fields[0], CultureInfo.InvariantCulture), fields[1], fields[2], fields[3],
                new Address(fields[4], fields[5], fields[6], fields[7]));
        }
        catch (Exception exception) when (exception is FormatException or OverflowException or ArgumentException)
        {
            throw new InvalidPersonRecordException(lineNumber, line,
                $"Invalid person record at line {lineNumber}: {exception.Message}", exception);
        }
    }
}
