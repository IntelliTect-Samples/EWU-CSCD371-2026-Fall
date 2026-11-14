namespace PeopleImporter;

/// <summary>
/// Specifies how the importer handles invalid person records.
/// </summary>
public enum ImportMode
{
    /// <summary>
    /// Stops importing and throws when an invalid record is encountered.
    /// </summary>
    FailFast,

    /// <summary>
    /// Skips invalid records and reports them through a callback.
    /// </summary>
    SkipInvalid
}