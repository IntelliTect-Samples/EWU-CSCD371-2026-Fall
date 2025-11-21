using Assignment;

namespace Assignment;

public static class CsvParser
{
    /// <summary>
    /// Extracts the State field (index 5) from a CSV row string.
    /// </summary>
    public static string ParseStateFromCsvRow(string row)
    {
        var parts = row.Split(',');
        // State is the 6th field (index 5)
        return parts.Length > 5 ? parts[5].Trim() : string.Empty;
    }

    /// <summary>
    /// Parses a CSV row into a complete IPerson object.
    /// </summary>
    public static IPerson ParsePersonFromCsvRow(string row)
    {
        var parts = row.Split(',');
        
        // Parts indices: 0:FirstName, 1:LastName, 2:Email, 3:Street, 4:City, 5:State, 6:Zip
        var address = new Address(
            parts.Length > 3 ? parts[3].Trim() : "",
            parts.Length > 4 ? parts[4].Trim() : "",
            parts.Length > 5 ? parts[5].Trim() : "",
            parts.Length > 6 ? parts[6].Trim() : ""
        );

        return new Person(
            parts.Length > 0 ? parts[0].Trim() : "",
            parts.Length > 1 ? parts[1].Trim() : "",
            address,                                  
            parts.Length > 2 ? parts[2].Trim() : ""   
        );
    }
}