namespace Assignment;

public static class PeopleZipReport
{
    public const string Header = "FirstName,LastName,EmailAddress,ZipCode,City,State";

    // Downloads the people for a user, looks up each ZIP code to get the City and State,
    // and returns the combined six-column rows (header included).
    public static async Task<(DownloadStatus Status, List<string> Rows)> BuildAsync(
        HttpClientWrapper wrapper,
        string user,
        CancellationToken cancellationToken = default)
    {
        var rows = new List<string> { Header };

        DownloadResult people = await wrapper.DownloadPeopleCsvAsync(user, cancellationToken);
        if (people.Status != DownloadStatus.Success)
        {
            return (people.Status, rows);   // report why it failed; header only
        }

        var cityStateByZip = new Dictionary<string, string>();   // "99223" -> "Spokane,WA"

        string[] lines = people.Csv!.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines.Skip(1))   // Skip(1) = header row
        {
            string[] cols = line.Trim().Split(',');
            if (cols.Length < 4) continue;       // skip malformed rows

            string zip = cols[3];

            if (!cityStateByZip.ContainsKey(zip))
            {
                DownloadResult place = await wrapper.DownloadZipAsync(zip, cancellationToken);

                cityStateByZip[zip] = place.Status == DownloadStatus.Success
                    ? place.Csv!.Trim().Replace("\"", "")
                    : ",";   // blank City,State if the lookup failed
            }

            rows.Add($"{line.Trim()},{cityStateByZip[zip]}");
        }

        return (DownloadStatus.Success, rows);
    }
}