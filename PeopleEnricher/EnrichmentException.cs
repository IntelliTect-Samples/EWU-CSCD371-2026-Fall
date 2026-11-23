namespace PeopleEnricher;

public sealed class EnrichmentException : Exception
{
    public EnrichmentErrorType ErrorType { get; }

    public EnrichmentException(EnrichmentErrorType errorType, string message)
        : base(message)
    {
        ErrorType = errorType;
    }

    public EnrichmentException(EnrichmentErrorType errorType, string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorType = errorType;
    }
}

public enum EnrichmentErrorType
{
    CsvDownloadFailed,
    CsvDownloadTimeout,
    InvalidHeader
}