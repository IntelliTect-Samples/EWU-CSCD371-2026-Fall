namespace Assignment9;

/// <summary>
/// Thrown when an endpoint answers successfully (HTTP 2xx) but the body is not valid data.
/// Like <see cref="HttpRequestException"/>, this is treated as a transient failure and retried.
/// </summary>
public sealed class InvalidResponseException : Exception
{
    public InvalidResponseException()
    {
    }

    public InvalidResponseException(string message)
        : base(message)
    {
    }

    public InvalidResponseException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
