using System.Text;

namespace PeopleEnricher;

/// <summary>
/// Writes formatted CSV data to a destination file atomically.
/// </summary>
public sealed class AtomicCsvFileWriter
{
    private readonly IPeopleCsvFormatter _formatter;

    /// <summary>
    /// Creates a writer using the supplied CSV formatter.
    /// </summary>
    public AtomicCsvFileWriter(IPeopleCsvFormatter formatter)
    {
        ArgumentNullException.ThrowIfNull(formatter);
        _formatter = formatter;
    }

    /// <summary>
    /// Writes enriched people to a CSV file.
    /// </summary>
    /// <param name="destinationPath">The output file path.</param>
    /// <param name="people">The successfully enriched people.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task WriteAsync(
        string destinationPath,
        IEnumerable<EnrichedPerson> people,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationPath);
        ArgumentNullException.ThrowIfNull(people);

        cancellationToken.ThrowIfCancellationRequested();

        // Formatting happens before creating any files.
        string csv = _formatter.Format(people);

        ArgumentNullException.ThrowIfNull(csv);

        string fullPath = Path.GetFullPath(destinationPath);
        string? directory = Path.GetDirectoryName(fullPath);

        if (directory is null)
        {
            throw new ArgumentException(
                "The destination must include a directory.",
                nameof(destinationPath));
        }

        // The temporary file is in the destination directory so that
        // the final move occurs on the same volume.
        string temporaryPath = Path.Combine(
            directory,
            $".{Path.GetFileName(fullPath)}.{Guid.NewGuid():N}.tmp");

        bool temporaryFileCreated = false;

        try
        {
            await using (FileStream fileStream = new(
                temporaryPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                4096,
                FileOptions.Asynchronous))
            {
                temporaryFileCreated = true;

                await using (StreamWriter writer = new(
                    fileStream,
                    new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
                    4096,
                    leaveOpen: true))
                {
                    await writer.WriteAsync(
                        csv.AsMemory(),
                        cancellationToken);

                    await writer.FlushAsync(cancellationToken);
                }

                await fileStream.FlushAsync(cancellationToken);
            }

            // Do not replace the destination after cancellation.
            cancellationToken.ThrowIfCancellationRequested();

            // Replace the destination only after the complete file
            // has been successfully written and closed.
            File.Move(
                temporaryPath,
                fullPath,
                overwrite: true);

            temporaryFileCreated = false;
        }
        finally
        {
            // Remove incomplete temporary output on failure or cancellation.
            if (temporaryFileCreated)
            {
                File.Delete(temporaryPath);
            }
        }
    }
}