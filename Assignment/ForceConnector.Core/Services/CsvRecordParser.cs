using System.Text;

namespace ForceConnector.Core.Services;

internal static class CsvRecordParser
{
    public static IReadOnlyList<IReadOnlyList<string>> Parse(string csv)
    {
        List<IReadOnlyList<string>> records = [];
        List<string> fields = [];
        StringBuilder field = new();
        bool inQuotes = false;
        bool hasContent = false;

        for (int index = 0; index < csv.Length; index++)
        {
            char character = csv[index];

            if (inQuotes)
            {
                if (character == '"')
                {
                    if (index + 1 < csv.Length && csv[index + 1] == '"')
                    {
                        field.Append('"');
                        index++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    field.Append(character);
                }

                continue;
            }

            if (character == '"' && field.Length == 0)
            {
                inQuotes = true;
                hasContent = true;
            }
            else if (character == ',')
            {
                fields.Add(field.ToString());
                field.Clear();
                hasContent = true;
            }
            else if (character == '\r' || character == '\n')
            {
                fields.Add(field.ToString());
                field.Clear();

                if (hasContent || fields.Any(value => value.Length > 0))
                {
                    records.Add(fields.ToArray());
                }

                fields.Clear();
                hasContent = false;

                if (character == '\r' && index + 1 < csv.Length && csv[index + 1] == '\n')
                {
                    index++;
                }
            }
            else
            {
                field.Append(character);
                hasContent = true;
            }
        }

        if (inQuotes)
        {
            throw new FormatException("CSV contains an unterminated quoted field.");
        }

        if (hasContent || field.Length > 0 || fields.Count > 0)
        {
            fields.Add(field.ToString());
            records.Add(fields.ToArray());
        }

        return records;
    }
}
