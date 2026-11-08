using System;
using System.IO;

namespace Assignment;

/// <summary>
/// Parsing and formatting logic shared by <see cref="SampleData"/> and <see cref="SampleDataAsync"/>.
/// </summary>
internal static class PeopleCsv
{
    public static string FilePath { get; } = Path.Combine(AppContext.BaseDirectory, "People.csv");

    public static string GetState(string row) => row.Split(',')[6];

    public static IPerson ParsePerson(string row)
    {
        string[] columns = row.Split(',');
        if (columns.Length != 8)
        {
            throw new FormatException("A People.csv row must contain exactly 8 columns.");
        }

        return new Person(
            columns[1],
            columns[2],
            columns[3],
            new Address(columns[4], columns[5], columns[6], columns[7]));
    }

    public static string AppendState(string aggregate, string state)
        => aggregate.Length == 0 ? state : $"{aggregate},{state}";
}