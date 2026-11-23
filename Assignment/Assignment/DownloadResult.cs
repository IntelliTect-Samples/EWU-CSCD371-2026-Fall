namespace Assignment;

public enum DownloadStatus
{
    Success, //default
    Cancelled,
    Timeout, 
    InvalidData,
    Exception
}

public sealed record DownloadResult(
    DownloadStatus Status,
    string? Csv = null,
    Exception? Error = null);