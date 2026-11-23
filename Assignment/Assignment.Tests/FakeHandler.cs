using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace Assignment.Tests;

/// <summary>Fake server. Counts requests per path so tests can say "fail the first call, then succeed".</summary>
internal sealed class FakeHandler(Func<string, int, CancellationToken, Task<HttpResponseMessage>> respond)
    : HttpMessageHandler
{
    private readonly ConcurrentDictionary<string, int> _counts = new();
    private int _total;

    public int RequestCount => Volatile.Read(ref _total);

    public int CountFor(string path) => _counts.GetValueOrDefault(path);

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string path = request.RequestUri!.AbsolutePath;
        Interlocked.Increment(ref _total);
        int number = _counts.AddOrUpdate(path, 1, (_, n) => n + 1);
        return respond(path, number, cancellationToken);
    }

    public static Task<HttpResponseMessage> Ok(string body) =>
        Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(body, Encoding.UTF8)
        });

    public static Task<HttpResponseMessage> Status(HttpStatusCode code) =>
        Task.FromResult(new HttpResponseMessage(code));

    /// <summary>The three ways the live API misbehaves.</summary>
    public static Task<HttpResponseMessage> Fail(string mode, CancellationToken ct) => mode switch
    {
        "Exception" => Status((HttpStatusCode)418),
        "InvalidData" => Ok("BAD_DATA"),
        "Timeout" => StallAsync(ct),
        _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
    };

    /// <summary>Never answers; only HttpClient.Timeout or cancellation ends it.</summary>
    public static async Task<HttpResponseMessage> StallAsync(CancellationToken ct)
    {
        await Task.Delay(Timeout.Infinite, ct);
        throw new UnreachableException();
    }

    public static HttpClient NewHttp(FakeHandler handler) =>
        new(handler, disposeHandler: false) { Timeout = TimeSpan.FromMilliseconds(200) };
}
