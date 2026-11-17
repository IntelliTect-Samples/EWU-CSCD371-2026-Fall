using System;
using System.Globalization;

namespace Assignment;

public static class PersonRecordParser
{
    public const string Header =
        "Id,FirstName,LastName,Email,StreetAddress,City,State,Zip";

    public static Person Parse(string rawLine)
    {
        ArgumentNullException.ThrowIfNull(rawLine);

        string[] fields = rawLine.Split(',');

        // Rule 1: Each record must contain exactly 8 fields.
        if (fields.Length != 8)
        {
            throw new FormatException(
                "Each record must contain exactly 8 fields.");
        }

        // Rule 2: ID must be a positive integer.
        int id = int.Parse(fields[0].Trim(), CultureInfo.InvariantCulture);

        if (id <= 0)
        {
            throw new FormatException("ID must be greater than zero.");
        }

        string firstName = fields[1].Trim();
        string lastName = fields[2].Trim();
        string email = fields[3].Trim();
        string streetAddress = fields[4].Trim();
        string city = fields[5].Trim();
        string state = fields[6].Trim();
        string zip = fields[7].Trim();

        // Rule 3: All required fields must have values.
        if (string.IsNullOrWhiteSpace(firstName) ||
            string.IsNullOrWhiteSpace(lastName) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(streetAddress) ||
            string.IsNullOrWhiteSpace(city) ||
            string.IsNullOrWhiteSpace(state) ||
            string.IsNullOrWhiteSpace(zip))
        {
            throw new FormatException(
                "Required fields cannot be empty.");
        }

        Address address = new Address(
            streetAddress, city, state, zip);

        return new Person(
            id, firstName, lastName, email, address);
    }
}
