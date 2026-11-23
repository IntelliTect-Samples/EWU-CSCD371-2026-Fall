namespace ForceConnector.Core.Services;

/// <summary>
/// Simple retry helper.
/// </summary>
public sealed class RetryPolicy
{
    private const int MaxAttempts = 3;

    public static async Task<T> ExecuteAsync<T>(
        Func<Task<T>> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);

        Exception? lastException = null;

        for (int attempt = 1;
            attempt <= MaxAttempts;
            attempt++)
        {
            try
            {
                return await operation();
            }
            catch (HttpRequestException ex)
            {
                lastException = ex;
            }
            catch (TaskCanceledException ex)
            {
                lastException = ex;
            }
            catch (InvalidDataException ex)
            {
                lastException = ex;
            }
        }

        throw lastException ??
              new InvalidOperationException(
                  "Retry operation failed.");
    }
}