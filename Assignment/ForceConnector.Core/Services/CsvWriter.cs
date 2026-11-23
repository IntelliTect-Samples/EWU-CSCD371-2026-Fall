using System.Text;
using ForceConnector.Core.Models;

namespace ForceConnector.Core.Services;

public sealed class CsvWriter
{
    private const string Header =
        "FirstName,LastName,EmailAddress,ZipCode,City,State";

    public static async Task WriteAsync(
        string outputFile,
        IEnumerable<EnrichedPerson> people,
        CancellationToken cancellationToken)
    {
        string tempFile =
            Path.GetTempFileName();

        try
        {
            List<string> lines =
            [
                Header,

                .. people.Select(p =>
                    $"{p.FirstName}," +
                    $"{p.LastName}," +
                    $"{p.EmailAddress}," +
                    $"{p.ZipCode}," +
                    $"{p.City}," +
                    $"{p.State}")
            ];

            await File.WriteAllLinesAsync(
                tempFile,
                lines,
                Encoding.UTF8,
                cancellationToken);

            File.Move(
                tempFile,
                outputFile,
                true);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }
}