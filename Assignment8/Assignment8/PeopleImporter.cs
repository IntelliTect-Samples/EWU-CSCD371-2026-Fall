using System.Globalization;

namespace Assignment8;

public sealed class ImportFailedException(IReadOnlyList<InvalidPersonException> failures)
    : Exception($"Import failed with {failures.Count} invalid record(s).")
{
    public IReadOnlyList<InvalidPersonException> Failures { get; } =
        failures ?? throw new ArgumentNullException(nameof(failures));
}

public sealed class PeopleImporter : IDisposable
{
    private readonly TextReader _reader;
    private bool _disposed;
    private bool _imported;

    public PeopleImporter(TextReader reader)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    public PeopleImporter(string filePath)
        : this(new StreamReader(filePath ?? throw new ArgumentNullException(nameof(filePath))))
    { }
    public IReadOnlyList<Person> Import(ImportMode mode = ImportMode.FailFast,
    Action<InvalidPersonException>? onInvalid = null,
    Func<Person, string?>? validate = null)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_imported)
        {
            throw new InvalidOperationException("This importer can only be read once.");
        }
        if (!Enum.IsDefined(mode))
        {
            throw new ArgumentOutOfRangeException(nameof(mode));
        }

        _imported = true;
        var result = new List<Person>();
        string? line;
        int lineNumber = 0;
        while ((line = _reader.ReadLine()) is not null)
        {
            lineNumber++;

            // Remove a byte-order mark from the first line, then skip blank lines.
            if (lineNumber == 1)
            {
                line = line.TrimStart('\uFEFF');
            }
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            // Skip the header row.
            if (lineNumber == 1 && line.Equals(
                "Id,FirstName,LastName,Email,StreetAddress,City,State,Zip",
                StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            // Parse the line into a Person. Only parse errors are caught here.
            Person person;
            try
            {
                person = ParsePerson(line);
            }
            catch (Exception ex) when (ex is FormatException or OverflowException or ArgumentException)
            {
                var failure = new InvalidPersonException(
                    $"Invalid person at CSV line {lineNumber}: {ex.Message}", lineNumber, line, ex);
                HandleInvalid(failure, mode, onInvalid);
                continue;
            }

            // If the delegate itself throws, nothing here catches it and it propagates.
            string? problem = validate?.Invoke(person);
            if (problem is not null)
            {
                var failure = new InvalidPersonException(
                    $"Invalid person at CSV line {lineNumber}: {problem}", lineNumber, line);
                HandleInvalid(failure, mode, onInvalid);
                continue;
            }

            // The person is valid, so add it to the result.
            result.Add(person);
        }
        return result;
    }


    public IReadOnlyList<Person> ImportPeople(ImportMode mode = ImportMode.FailFast,
    Action<InvalidPersonException>? onInvalid = null,
    Func<Person, string?>? validate = null) => Import(mode, onInvalid, validate);

    private static bool TryParsePerson(string line, out Person person, out IReadOnlyList<string> errors)
    {
        try
        {
            person = ParsePerson(line);
            errors = Array.Empty<string>();
            return true;
        }
        catch (Exception ex) when (ex is FormatException or OverflowException)
        {
            person = default!;
            errors = new[] { ex.Message };
            return false;
        }
    }
    // Fail-fast throws invalid records; skip-invalid reports through the callback and continues.
    private static void HandleInvalid(InvalidPersonException failure, ImportMode mode,
        Action<InvalidPersonException>? onInvalid)
    {
        if (mode == ImportMode.FailFast)
        {
            throw failure;
        }
        onInvalid?.Invoke(failure);
    }

    private static Person ParsePerson(string line)
    {
        string[] values = line.Split(',');
        if (values.Length != 8)
        {
            throw new FormatException($"Expected 8 columns but found {values.Length}.");
        }

        for (int i = 0; i < values.Length; i++)
        {
            values[i] = values[i].Trim();
            if (values[i].Length == 0)
            {
                throw new FormatException($"Column {i + 1} is required.");
            }
        }

        int id = int.Parse(values[0], NumberStyles.None, CultureInfo.InvariantCulture);
        if (id <= 0)
        {
            throw new FormatException("Id must be positive.");
        }

        if (!values[3].Contains('@', StringComparison.Ordinal) ||
            values[3].StartsWith('@') || values[3].EndsWith('@'))
        {
            throw new FormatException("Email must contain a local part and domain.");
        }

        if (values[6].Length != 2 || !values[6].All(char.IsLetter))
        {
            throw new FormatException("State must be a two-letter abbreviation.");
        }

        if (values[7].Length != 5 || !values[7].All(char.IsDigit))
        {
            throw new FormatException("Zip must contain five digits.");
        }

        return new Person
        {
            Id = id,
            FirstName = values[1],
            LastName = values[2],
            Email = values[3],
            Address = new Address
            {
                StreetAddress = values[4],
                City = values[5],
                State = values[6],
                Zip = values[7]
            }
        };
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _reader.Dispose();
    }
}
