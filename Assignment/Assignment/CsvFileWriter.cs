using System.Text;

namespace Assignment9;

/// <summary>Writes a file so that it either appears complete or not at all.</summary>
public static class CsvFileWriter
{
    /// <summary>
    /// Writes <paramref name="lines"/> to a temporary file next to <paramref name="path"/> and then
    /// moves it into place. If anything fails or is cancelled, the temporary file is deleted and any
    /// existing file at <paramref name="path"/> is left untouched.
    /// </summary>
    public static async Task WriteAtomicallyAsync(string path, IEnumerable<string> lines, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(lines);

        string fullPath = Path.GetFullPath(path);
        string tempPath = $"{fullPath}.{Guid.NewGuid():N}.tmp";
        bool moved = false;
        try
        {
            await WriteLinesAsync(tempPath, lines, cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            File.Move(tempPath, fullPath, overwrite: true);
            moved = true;
        }
        finally
        {
            if (!moved)
            {
                File.Delete(tempPath);
            }
        }
    }

    private static async Task WriteLinesAsync(string path, IEnumerable<string> lines, CancellationToken cancellationToken)
    {
        await using var stream = new FileStream(
            path, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
        await using var writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

        foreach (string line in lines)
        {
            await writer.WriteLineAsync(line.AsMemory(), cancellationToken).ConfigureAwait(false);
        }

        await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
    }
}
