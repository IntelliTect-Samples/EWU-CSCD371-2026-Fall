namespace ForceConnector.Core.Services;

/// <summary>
/// Simple retry helper.
/// </summary>
public sealed class RetryPolicy
{
    private const int MaxAttempts = 3;
    private const int BaseDelayMilliseconds = 100;
    private const int MaxDelayMilliseconds = 2_000;

    public static async Task<T> ExecuteAsync<T>(
        Func<Task<T>> operation,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);

        for (int attempt = 1;
            attempt <= MaxAttempts;
            attempt++)
        {
            try
            {
                return await operation();
            }
            catch (HttpRequestException)
                when (attempt < MaxAttempts)
            {
                await DelayBeforeRetryAsync(
                    attempt,
                    cancellationToken);
            }
            catch (InvalidDataException)
                when (attempt < MaxAttempts)
            {
                await DelayBeforeRetryAsync(
                    attempt,
                    cancellationToken);
            }
            catch (OperationCanceledException)
                when (!cancellationToken.IsCancellationRequested &&
                      attempt < MaxAttempts)
            {
                await DelayBeforeRetryAsync(
                    attempt,
                    cancellationToken);
            }
        }

        throw new InvalidOperationException(
            "Retry operation failed.");
    }

    private static Task DelayBeforeRetryAsync(
        int attempt,
        CancellationToken cancellationToken)
    {
        int exponentialDelay =
            Math.Min(
                MaxDelayMilliseconds,
                BaseDelayMilliseconds * (1 << (attempt - 1)));

        int jitter = Random.Shared.Next(0, BaseDelayMilliseconds);

        return Task.Delay(
            TimeSpan.FromMilliseconds(exponentialDelay + jitter),
            cancellationToken);
    }
}