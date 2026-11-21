using System.Diagnostics;
using System.Net;
using System.Text;

namespace Assignment.Tests;

internal enum Endpoint { People, Zip }

internal sealed record Call(Endpoint Endpoint, int Number, string Path);

internal sealed class FakeHandler(Func<Call, HttpResponseMessage?> respond) : HttpMessageHandler
{
    private int _peopleCalls;
    private int _zipCalls;

    public int PeopleCalls => Volatile.Read(ref _peopleCalls);
    public int ZipCalls => Volatile.Read(ref _zipCalls);

    public static string PeopleCsv(params string[] zips) =>
        string.Join('\n', new[] { PeopleEnricherService.PeopleHeader }
            .Concat(zips.Select((zip, i) => $"First{i},Last{i},p{i}@example.com,{zip}")));

    public static HttpResponseMessage Ok(string body) =>
        new(HttpStatusCode.OK) { Content = new StringContent(body, Encoding.UTF8) };

    public static HttpResponseMessage Status(HttpStatusCode code) => new(code);

    /// <summary>A 200 whose body fails partway through, like the people endpoint's Exception mode.</summary>
    public static HttpResponseMessage ResetMidBody() =>
        new(HttpStatusCode.OK) { Content = new StreamContent(new ResettingStream()) };

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string path = request.RequestUri!.AbsolutePath;
        Call call = path.StartsWith("/people/zip/", StringComparison.Ordinal)
            ? new(Endpoint.Zip, Interlocked.Increment(ref _zipCalls), path)
            : new(Endpoint.People, Interlocked.Increment(ref _peopleCalls), path);

        return respond(call) ?? await StallAsync(cancellationToken);
    }

    private static async Task<HttpResponseMessage> StallAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(Timeout.Infinite, cancellationToken);
        throw new UnreachableException();
    }

    private sealed class ResettingStream : Stream
    {
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public override int Read(byte[] buffer, int offset, int count) => throw new IOException("Connection reset.");
        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
