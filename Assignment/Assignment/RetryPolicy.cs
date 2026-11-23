using Microsoft.Kiota.Abstractions;

namespace Assignment9;

/// <summary>Settings for <see cref="RetryPolicy"/>.</summary>
public sealed record RetryOptions
{
    /// <summary>Total attempts, including the first one.</summary>
    public int MaxAttempts { get; init; } = 6;

    /// <summary>How long a single attempt may take before it is abandoned and retried.</summary>
    public TimeSpan AttemptTimeout { get; init; } = TimeSpan.FromSeconds(10);

    /// <summary>The backoff before the second attempt; it doubles on each later attempt.</summary>
    public TimeSpan BaseDelay { get; init; } = TimeSpan.FromMilliseconds(250);

    /// <summary>The upper bound on any single backoff.</summary>
    public TimeSpan MaxDelay { get; init; } = TimeSpan.FromSeconds(5);
}

/// <summary>
/// Retries an operation that fails with a recoverable error, using exponential backoff with
/// "full jitter" (a random delay between zero and the exponential cap).
/// <para>Recoverable errors are: <see cref="ApiException"/> (a non-success status code),
/// <see cref="HttpRequestException"/> (connection failures), <see cref="HttpIOException"/> (the body was cut off),
/// <see cref="InvalidResponseException"/> (a 2xx with bad data), and an attempt timing out.</para>
/// <para>Anything else propagates immediately, unchanged. When the caller's token is cancelled the
/// <see cref="OperationCanceledException"/> propagates immediately. When every attempt fails, the
/// last attempt's exception propagates unchanged, except that a final timeout becomes a
/// <see cref="TimeoutException"/> (with the cancellation as its inner exception) so that it is not
/// mistaken for caller cancellation.</para>
/// </summary>
public sealed class RetryPolicy
{
    private readonly RetryOptions options;
    private readonly Func<double> nextRandom;

    /// <param name="options">Retry settings.</param>
    /// <param name="nextRandom">Returns a value in [0, 1) used for jitter. Defaults to <see cref="Random.Shared"/>.</param>
    public RetryPolicy(RetryOptions options, Func<double>? nextRandom = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentOutOfRangeException.ThrowIfLessThan(options.MaxAttempts, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(options.BaseDelay, TimeSpan.Zero);
        ArgumentOutOfRangeException.ThrowIfLessThan(options.MaxDelay, TimeSpan.Zero);
        this.options = options;
        this.nextRandom = nextRandom ?? Random.Shared.NextDouble;
    }

    public static bool IsTransient(Exception exception) =>
        exception is ApiException or HttpRequestException or HttpIOException or InvalidResponseException;

    public async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(operation);

        for (int attempt = 1; ; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var attemptCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                attemptCancellation.CancelAfter(options.AttemptTimeout);
                try
                {
                    return await operation(attemptCancellation.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException exception) when (!cancellationToken.IsCancellationRequested)
                {
                    // The caller did not cancel, so this attempt timed out.
                    if (attempt >= options.MaxAttempts)
                    {
                        throw new TimeoutException(
                            $"The operation timed out on all {attempt} attempts ({options.AttemptTimeout.TotalSeconds:0.###}s each).",
                            exception);
                    }
                }
                catch (Exception exception) when (IsTransient(exception) && attempt < options.MaxAttempts)
                {
                    // Recoverable: fall through to the backoff and try again.
                }
            }

            await Task.Delay(GetDelay(attempt), cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>The backoff after a failed <paramref name="attempt"/> (1-based).</summary>
    public TimeSpan GetDelay(int attempt)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(attempt, 1);
        double exponentialMs = options.BaseDelay.TotalMilliseconds * Math.Pow(2, attempt - 1);
        double cappedMs = Math.Min(exponentialMs, options.MaxDelay.TotalMilliseconds);
        return TimeSpan.FromMilliseconds(cappedMs * nextRandom());
    }
}
