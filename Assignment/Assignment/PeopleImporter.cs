namespace Assignment;

public sealed class PeopleImporter
{
    private readonly TextReader _reader;

    public PeopleImporter(TextReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);
        _reader = reader;
    }

    public IReadOnlyList<Person> Import(ImportMode mode, Action<InvalidPersonRecordException>? onInvalidRecord = null)
    {
        if (!Enum.IsDefined(mode))
        {
            throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unknown import mode.");
        }
        if (mode == ImportMode.SkipInvalid)
        {
            ArgumentNullException.ThrowIfNull(onInvalidRecord);
        }

        List<Person> people = [];
        _reader.ReadLine(); // skip the header (line 1)
        int lineNumber = 1;

        string? line;
        while ((line = _reader.ReadLine()) is not null)
        {
            lineNumber++;
            try
            {
                people.Add(PersonRecordParser.Parse(line));
            }
            catch (Exception ex) when (ex is FormatException or OverflowException)
            {
                var error = new InvalidPersonRecordException(lineNumber, line, ex);
                if (mode == ImportMode.FailFast)
                {
                    throw error;
                }
                onInvalidRecord!(error);
            }
        }

        return people;
    }
}
