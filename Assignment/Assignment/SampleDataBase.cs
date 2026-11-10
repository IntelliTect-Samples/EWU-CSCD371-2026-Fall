using System;
using System.IO;

namespace Assignment;

public abstract class SampleDataBase
{
    protected static readonly string CsvFile =
        Path.Combine(AppContext.BaseDirectory
            , "People.csv");

    protected static string GetState(string row)
    {
        return row.Split(',')[6];
    }

    protected static IPerson ParsePerson(string row)
    {
        string[] columns = row.Split(',');

        return new Person(columns[1],
            columns[2],
            columns[3],
            new Address(columns[4],
            columns[5],
            columns[6],
            columns[7]));
    }

}