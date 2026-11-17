namespace Tests;

/// <summary>
/// Shared CSV text for tests, so every test class uses the same header and sample records.
/// </summary>
internal static class TestCsv
{
    public const string Header = "Id,FirstName,LastName,Email,StreetAddress,City,State,Zip";

    public const string Alice = "1,Alice,Smith,alice@example.com,100 Main Street,Seattle,WA,98101";
    public const string Bob = "2,Bob,Jones,bob@example.com,200 Oak Avenue,Spokane,WA,99201";
    public const string Carol = "3,Carol,Diaz,carol@example.com,300 Pine Road,Tacoma,WA,98402";

    /// <summary>Returns the header followed by <paramref name="records"/>, one per line.</summary>
    public static string[] WithHeader(params string[] records) => [Header, .. records];
}
