using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text;
using Assignment9.Generated;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace Assignment9.Tests;

/// <summary>A response body stream that records whether it was disposed.</summary>
internal sealed class TrackingStream(byte[] bytes) : MemoryStream(bytes, writable: false)
{
    private int disposed;

    public bool IsDisposed => Volatile.Read(ref disposed) == 1;

    protected override void Dispose(bool disposing)
    {
        Interlocked.Exchange(ref disposed, 1);
        base.Dispose(disposing);
    }
}

/// <summary>Response content whose body streams record whether they were disposed.</summary>
internal sealed class TrackingContent(string body) : HttpContent
{
    private readonly byte[] bytes = Encoding.UTF8.GetBytes(body);
    private readonly ConcurrentQueue<TrackingStream> streams = new();

    public IReadOnlyCollection<TrackingStream> Streams => streams;

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context) =>
        stream.WriteAsync(bytes).AsTask();

    protected override bool TryComputeLength(out long length)
    {
        length = bytes.Length;
        return true;
    }

    protected override Task<Stream> CreateContentReadStreamAsync() => Task.FromResult<Stream>(OpenStream());

    protected override Task<Stream> CreateContentReadStreamAsync(CancellationToken cancellationToken) =>
        Task.FromResult<Stream>(OpenStream());

    protected override Stream CreateContentReadStream(CancellationToken cancellationToken) => OpenStream();

    private TrackingStream OpenStream()
    {
        var stream = new TrackingStream(bytes);
        streams.Enqueue(stream);
        return stream;
    }
}

/// <summary>
/// Wires a <see cref="FakeMayhemApi"/> through the real Kiota request adapter and the generated
/// client, exactly as the console app does with the live endpoint. Disposes what it creates.
/// </summary>
internal sealed class FakeConnection : IDisposable
{
    private readonly HttpClient httpClient;
    private readonly HttpClientRequestAdapter requestAdapter;

    public FakeConnection(FakeMayhemApi api)
    {
        httpClient = new HttpClient(api, disposeHandler: false);
        requestAdapter = new HttpClientRequestAdapter(new AnonymousAuthenticationProvider(), httpClient: httpClient)
        {
            BaseUrl = "https://fake.test",
        };
        Client = new MayhemMarmotClient(new MayhemMarmotApiClient(requestAdapter));
    }

    public MayhemMarmotClient Client { get; }

    public void Dispose()
    {
        requestAdapter.Dispose();
        httpClient.Dispose();
    }
}

/// <summary>Builders for each behavior the real endpoints can exhibit.</summary>
internal static class Responses
{
    public static Task<HttpResponseMessage> Ok(string body) => Task.FromResult(Create(HttpStatusCode.OK, body));

    /// <summary>The "Exception" failure mode: the server answers with an error status.</summary>
    public static Task<HttpResponseMessage> ServerError() =>
        Task.FromResult(Create(HttpStatusCode.InternalServerError, "Internal Server Error"));

    /// <summary>The "InvalidData" failure mode: a 200 with a body that is not valid data.</summary>
    public static Task<HttpResponseMessage> InvalidData() => Ok("BAD_DATA");

    /// <summary>The "Timeout" failure mode: the server never answers.</summary>
    public static async Task<HttpResponseMessage> Hang(CancellationToken cancellationToken)
    {
        await Task.Delay(Timeout.Infinite, cancellationToken);
        throw new UnreachableException();
    }

    private static HttpResponseMessage Create(HttpStatusCode statusCode, string body) =>
        new(statusCode) { Content = new TrackingContent(body) };
}

/// <summary>
/// An in-memory stand-in for the Mayhem Marmot API. Behavior is scripted per endpoint and per
/// attempt number. It records requests, concurrency, and every response it hands out.
/// </summary>
internal sealed class FakeMayhemApi : HttpMessageHandler
{
    private const string zipPrefix = "/people/zip/";

    private readonly ConcurrentDictionary<string, int> attempts = new(StringComparer.Ordinal);
    private readonly ConcurrentQueue<HttpResponseMessage> responses = new();
    private readonly ConcurrentQueue<Uri> requestUris = new();
    private int inFlight;
    private int maxInFlight;

    /// <summary>Behavior of GET /people/{user}, given the 1-based attempt number.</summary>
    public Func<int, CancellationToken, Task<HttpResponseMessage>> People { get; set; } =
        (_, _) => Responses.Ok(SampleData.PeopleCsv);

    /// <summary>Behavior of GET /people/zip/{zip}, given the ZIP and the 1-based attempt number for it.</summary>
    public Func<string, int, CancellationToken, Task<HttpResponseMessage>> Zip { get; set; } =
        (zip, _, _) => Responses.Ok(SampleData.LocationBodyFor(zip));

    public int MaxInFlight => Volatile.Read(ref maxInFlight);

    public IReadOnlyCollection<Uri> RequestUris => requestUris;

    public int ResponseCount => responses.Count;

    private IEnumerable<TrackingStream> BodyStreams =>
        responses.SelectMany(response => ((TrackingContent)response.Content).Streams);

    /// <summary>How many response body streams were opened (by Kiota, on our behalf).</summary>
    public int BodyStreamCount => BodyStreams.Count();

    /// <summary>True when every response body stream that was opened has been disposed.</summary>
    public bool AllBodyStreamsDisposed => BodyStreams.All(stream => stream.IsDisposed);

    public int AttemptsFor(string path) => attempts.TryGetValue(path, out int count) ? count : 0;

    public FakeConnection Connect() => new(this);

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Uri uri = request.RequestUri ?? throw new InvalidOperationException("Request had no URI.");
        requestUris.Enqueue(uri);
        string path = uri.AbsolutePath;
        int attempt = attempts.AddOrUpdate(path, 1, (_, count) => count + 1);

        int now = Interlocked.Increment(ref inFlight);
        int seen;
        while (now > (seen = Volatile.Read(ref maxInFlight))
            && Interlocked.CompareExchange(ref maxInFlight, now, seen) != seen)
        {
        }

        try
        {
            HttpResponseMessage response = path.StartsWith(zipPrefix, StringComparison.Ordinal)
                ? await Zip(Uri.UnescapeDataString(path[zipPrefix.Length..]), attempt, cancellationToken)
                : await People(attempt, cancellationToken);
            responses.Enqueue(response);
            return response;
        }
        finally
        {
            Interlocked.Decrement(ref inFlight);
        }
    }
}

internal static class SampleData
{
    public const string User = "trever";
    public const string PeoplePath = "/people/trever";

    public static string PeopleCsv => Csv(
        "Ada,Lovelace,ada@example.com,99201",
        "Grace,Hopper,grace@example.com,98101",
        "Alan,Turing,alan@example.com,97201");

    public static string Csv(params string[] rows) =>
        string.Join("\n", rows.Prepend(global::Assignment9.PeopleCsv.ExpectedHeader)) + "\n";

    public static string ManyPeopleCsv(int count) =>
        Csv([.. Enumerable.Range(0, count).Select(i => string.Create(
            CultureInfo.InvariantCulture, $"First,Last,person{i}@example.com,{10000 + i:D5}"))]);

    public static string LocationBodyFor(string zip) => zip switch
    {
        "99201" => "\"SPOKANE\",\"WA\"",
        "98101" => "\"SEATTLE\",\"WA\"",
        "97201" => "\"PORTLAND\",\"OR\"",
        _ => "\"SOMEWHERE\",\"WA\"",
    };

    public static string ZipPath(string zip) => $"/people/zip/{zip}";
}

internal static class TestRetry
{
    public static RetryPolicy Create(int maxAttempts = 3, TimeSpan? attemptTimeout = null) => new(new RetryOptions
    {
        MaxAttempts = maxAttempts,
        AttemptTimeout = attemptTimeout ?? TimeSpan.FromMilliseconds(250),
        BaseDelay = TimeSpan.Zero,
        MaxDelay = TimeSpan.Zero,
    });
}

/// <summary>A unique temporary directory that is deleted on dispose.</summary>
internal sealed class TempDirectory : IDisposable
{
    public string FullPath { get; } = Directory.CreateTempSubdirectory("assignment9-").FullName;

    public string PathOf(string fileName) => Path.Combine(FullPath, fileName);

    public string[] Files() => Directory.GetFiles(FullPath);

    public void Dispose() => Directory.Delete(FullPath, recursive: true);
}
