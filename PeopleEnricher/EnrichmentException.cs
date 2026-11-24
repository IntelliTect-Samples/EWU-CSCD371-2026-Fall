namespace PeopleEnricher;

/// <summary>
/// Represents a fatal enrichment failure that prevents processing from continuing.
/// </summary>
public sealed class EnrichmentException : Exception
{
    /// <summary>
    /// Identifies the category of enrichment failure that occured.
    /// </summary>
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

/// <summary>
/// Identifies the category of enrichment failure that occured.
/// </summary>
public enum EnrichmentErrorType
{
    /// <summary>
    /// The people CSV could not be downloaded due to communication failure with Endpoint.
    /// </summary>
    CsvDownloadFailed,
    
    /// <summary>
    /// The people CSV could not be downloaded within the configured Timeout allowance.
    /// </summary>
    CsvDownloadTimeout,
    
    /// <summary>
    /// The downloaded CSV did not contain the expected header.
    /// </summary>
    InvalidHeader
}