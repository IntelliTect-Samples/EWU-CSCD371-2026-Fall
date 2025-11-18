using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Assignment;

public abstract class SampleDataServiceBase
{
    protected const string SampleDirectory = "People.csv";

    // 1.
    protected abstract IEnumerable<string> GetRawCsvRowsSync();

    // 2.
    public static IPerson StringToPerson(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        string[] split = input.Split(',');

        if (split.Length <= 7) throw new InvalidDataException($"{nameof(input)} : {input}");

        Address address = new Address(split[4], split[5], split[6], split[7]);
        return new Person(split[1], split[2], address, split[3]);
    }

    protected static IEnumerable<IPerson> GetOrderedPeopleFromListSync(IEnumerable<IPerson> people)
    {
        return people
            .OrderBy(person => person.Address.State)
            .ThenBy(person => person.Address.City)
            .ThenBy(person => person.Address.Zip);
    }

    protected static string AggregateStatesSync(IEnumerable<IPerson> people)
    {
        return people
            .Select(p => p.Address.State)
            .Distinct()
            .Aggregate((a, b) => a + $",{b}");
    }
}