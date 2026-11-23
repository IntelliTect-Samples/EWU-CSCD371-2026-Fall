using System.Text;

namespace Assignment9;

/// <summary>Minimal RFC 4180 style parsing and formatting of a single CSV line.</summary>
public static class CsvLine
{
    private enum State
    {
        FieldStart,
        Unquoted,
        Quoted,
        AfterClosingQuote,
    }

    /// <summary>
    /// Splits one CSV line into fields. Fields may be quoted; a doubled quote inside a quoted
    /// field is an escaped quote. Returns <see langword="false"/> for malformed input such as an
    /// unterminated quote, a stray quote inside an unquoted field, or text after a closing quote.
    /// </summary>
    public static bool TryParse(string line, out IReadOnlyList<string> fields)
    {
        ArgumentNullException.ThrowIfNull(line);

        var result = new List<string>();
        var current = new StringBuilder();
        State state = State.FieldStart;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            switch (state)
            {
                case State.FieldStart when c == '"':
                    state = State.Quoted;
                    break;
                case State.FieldStart or State.Unquoted when c == ',':
                    result.Add(current.ToString());
                    current.Clear();
                    state = State.FieldStart;
                    break;
                case State.FieldStart or State.Unquoted when c == '"':
                    fields = [];
                    return false;
                case State.FieldStart or State.Unquoted:
                    current.Append(c);
                    state = State.Unquoted;
                    break;
                case State.Quoted when c == '"':
                    if (i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++;
                    }
                    else
                    {
                        state = State.AfterClosingQuote;
                    }

                    break;
                case State.Quoted:
                    current.Append(c);
                    break;
                case State.AfterClosingQuote when c == ',':
                    result.Add(current.ToString());
                    current.Clear();
                    state = State.FieldStart;
                    break;
                default:
                    fields = [];
                    return false;
            }
        }

        if (state == State.Quoted)
        {
            fields = [];
            return false;
        }

        result.Add(current.ToString());
        fields = result;
        return true;
    }

    /// <summary>Joins fields into a CSV line, quoting only the fields that need it.</summary>
    public static string Format(params string[] fields)
    {
        ArgumentNullException.ThrowIfNull(fields);
        return string.Join(',', fields.Select(Escape));
    }

    public static string Escape(string field)
    {
        ArgumentNullException.ThrowIfNull(field);
        return field.AsSpan().IndexOfAny(",\"\r\n") >= 0
            ? $"\"{field.Replace("\"", "\"\"", StringComparison.Ordinal)}\""
            : field;
    }
}
