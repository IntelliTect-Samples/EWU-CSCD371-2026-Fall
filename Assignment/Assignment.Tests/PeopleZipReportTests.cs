using System.Net;

namespace Assignment.Tests;

[TestClass]
public class PeopleZipReportTests
{
    private static readonly string PeopleCsv =
        "FirstName,LastName,EmailAddress,ZipCode\n" +
        "John,Doe,john@gmail.com,99223\n" +
        "Mary,Smith,mary@gmail.com,99223\n" +
        "Jane,Doe,jane@gmail.com,98101\n";
    private static readonly string[] expected =
        {
            "FirstName,LastName,EmailAddress,ZipCode,City,State",
            "John,Doe,john@gmail.com,99223,Spokane,WA",
            "Mary,Smith,mary@gmail.com,99223,Spokane,WA",
            "Jane,Doe,jane@gmail.com,98101,Seattle,WA"
        };

    private static FakeHandler Routed(Func<string, HttpResponseMessage> route) =>
        new((req, _) => Task.FromResult(route(req.RequestUri!.AbsolutePath)));

    [TestMethod]
    public async Task BuildAsyncSuccessReturnsHeaderAndSixColumnRows()
    {
        var handler = Routed(path => path switch
        {
            "/people/Ben" => Http.Ok(PeopleCsv),
            "/people/zip/99223" => Http.Ok("\"Spokane\",\"WA\""),
            "/people/zip/98101" => Http.Ok("\"Seattle\",\"WA\""),
            _ => Http.Status(HttpStatusCode.NotFound)
        });

        var (status, rows) = await PeopleZipReport.BuildAsync(Http.Wrapper(handler), "Ben");

        Assert.AreEqual(DownloadStatus.Success, status);
        Assert.AreSequenceEqual(expected, rows);
    }

    [TestMethod]
    public async Task BuildAsyncRepeatedZipIsLookedUpOnlyOnce()
    {
        var handler = Routed(path => path.StartsWith("/people/zip", StringComparison.InvariantCulture)
            ? Http.Ok("\"Spokane\",\"WA\"")
            : Http.Ok(PeopleCsv));

        await PeopleZipReport.BuildAsync(Http.Wrapper(handler), "Ben");

        Assert.HasCount(1, handler.Paths.Where(p => p == "/people/zip/99223"));
        Assert.HasCount(1, handler.Paths.Where(p => p == "/people/zip/98101"));
    }

    [TestMethod]
    [DataRow(HttpStatusCode.NotFound)]
    [DataRow(HttpStatusCode.InternalServerError)]
    public async Task BuildAsyncPeopleDownloadFailsReturnsStatusAndHeaderOnly(HttpStatusCode code)
    {
        var handler = Routed(_ => Http.Status(code));

        var (status, rows) = await PeopleZipReport.BuildAsync(Http.Wrapper(handler), "Ben");

        Assert.AreEqual(DownloadStatus.Exception, status);
        CollectionAssert.AreEqual(new[] { PeopleZipReport.Header }, rows);
        Assert.HasCount(1, handler.Paths);   // never tried any ZIP
    }

    [TestMethod]
    public async Task BuildAsyncPeopleBodyBlankReturnsInvalidData()
    {
        var handler = Routed(_ => Http.Ok("  "));

        var (status, _) = await PeopleZipReport.BuildAsync(Http.Wrapper(handler), "Ben");

        Assert.AreEqual(DownloadStatus.InvalidData, status);
    }

    [TestMethod]
    public async Task BuildAsyncPeopleTimeoutReturnsTimeout()
    {
        var handler = new FakeHandler((_, _) => throw new TaskCanceledException());

        var (status, _) = await PeopleZipReport.BuildAsync(Http.Wrapper(handler), "Ben");

        Assert.AreEqual(DownloadStatus.Timeout, status);
    }

    [TestMethod]
    public async Task BuildAsyncPeopleCancelledReturnsCancelled()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var handler = Routed(_ => Http.Ok(PeopleCsv));

        var (status, _) = await PeopleZipReport.BuildAsync(Http.Wrapper(handler), "Ben", cts.Token);

        Assert.AreEqual(DownloadStatus.Cancelled, status);
    }

    [TestMethod]
    public async Task BuildAsyncZipLookupFailsRowGetsBlankCityAndState()
    {
        var handler = Routed(path => path.StartsWith("/people/zip", StringComparison.InvariantCulture)
            ? Http.Status(HttpStatusCode.NotFound)
            : Http.Ok("FirstName,LastName,EmailAddress,ZipCode\nJohn,Doe,john@gmail.com,99223\n"));

        var (status, rows) = await PeopleZipReport.BuildAsync(Http.Wrapper(handler), "Ben");

        Assert.AreEqual(DownloadStatus.Success, status);   // partial failure is silent today
        Assert.AreEqual("John,Doe,john@gmail.com,99223,,", rows[1]);
    }

    [TestMethod]
    public async Task BuildAsyncMalformedRowIsSkipped()
    {
        var handler = Routed(path => path.StartsWith("/people/zip", StringComparison.InvariantCulture)
            ? Http.Ok("\"Spokane\",\"WA\"")
            : Http.Ok("FirstName,LastName,EmailAddress,ZipCode\nbroken,row\nJohn,Doe,john@gmail.com,99223\n"));

        var (_, rows) = await PeopleZipReport.BuildAsync(Http.Wrapper(handler), "Ben");

        Assert.HasCount(2, rows);   // header + the one good row
    }

    [TestMethod]
    public async Task BuildAsyncWindowsLineEndingsAreTrimmed()
    {
        var handler = Routed(path => path.StartsWith("/people/zip", StringComparison.InvariantCulture)
            ? Http.Ok("\"Spokane\",\"WA\"")
            : Http.Ok("FirstName,LastName,EmailAddress,ZipCode\r\nJohn,Doe,john@gmail.com,99223\r\n"));

        var (_, rows) = await PeopleZipReport.BuildAsync(Http.Wrapper(handler), "Ben");

        Assert.AreEqual("John,Doe,john@gmail.com,99223,Spokane,WA", rows[1]);
    }
}