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
        ArgumentException.ThrowIfNullOrWhiteSpace(outputFile);
        ArgumentNullException.ThrowIfNull(people);

        string fullOutputPath = Path.GetFullPath(outputFile);
        string? directory = Path.GetDirectoryName(fullOutputPath);

        if (directory is null)
        {
            throw new InvalidOperationException(
                "The output path does not specify a directory.");
        }

        Directory.CreateDirectory(directory);

        string tempFile = Path.Combine(
            directory,
            $".{Path.GetFileName(fullOutputPath)}.{Guid.NewGuid():N}.tmp");

        try
        {
            List<string> lines =
            [
                Header,

                .. people.Select(p =>
                    string.Join(",",
                    [
                        Escape(p.FirstName),
                        Escape(p.LastName),
                        Escape(p.EmailAddress),
                        Escape(p.ZipCode),
                        Escape(p.City),
                        Escape(p.State)
                    ]))
            ];

            await File.WriteAllLinesAsync(
                tempFile,
                lines,
                Encoding.UTF8,
                cancellationToken);

            File.Move(tempFile, fullOutputPath, true);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    private static string Escape(string value)
    {
        if (!value.Contains('"') &&
            !value.Contains(',') &&
            !value.Contains('\r') &&
            !value.Contains('\n'))
        {
            return value;
        }

        return $"\"{value.Replace("\"", "\"\"")}\"";
    }
}