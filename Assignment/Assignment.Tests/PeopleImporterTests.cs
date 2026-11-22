using System.Net;
using static Assignment.Tests.FakeHandler;

namespace Assignment.Tests;

public class PeopleImporterTests
{
    private const string Header = "FirstName,LastName,EmailAddress,ZipCode";
    private const string OutputHeader = "FirstName,LastName,EmailAddress,ZipCode,City,State";
    private const string Spokane = "\"Spokane\",\"WA\"";
    private const string LookupFailed = "ZIP lookup failed or returned invalid data";

    private static CancellationToken Token => TestContext.Current.CancellationToken;

    [Fact]
    public async Task ValidPeopleAreEnriched()
    {
        string csv = PeopleCsv("John,Doe,john@example.com,99201", "Jane,Smith,jane@example.com,10001");

        await RunTestAsync((path, _, _) => path switch
        {
            "/people/testuser" => Ok(csv),
            "/people/zip/99201" => Ok(Spokane),
            "/people/zip/10001" => Ok("\"New York\",\"NY\""),
            _ => throw new InvalidOperationException(path),
        },
        async (importer, file, _) =>
        {
            ImportResult result = await importer.ImportAsync("testuser", file, Token);
            string[] lines = await File.ReadAllLinesAsync(file, Token);

            Assert.Equal(2, result.WrittenCount);
            Assert.Empty(result.Rejections);
            Assert.Equal(
                new[]
                {
                    OutputHeader,
                    "John,Doe,john@example.com,99201,Spokane,WA",
                    "Jane,Smith,jane@example.com,10001,New York,NY",
                },
                lines);
        });
    }

    [Fact]
    public async Task InvalidRowsAreRemovedReportedAndNeverLookedUp()
    {
        string csv = PeopleCsv(
            "John,Doe,john@example.com,99201",   // row 2: valid
            "Jane,Smith,invalid-email,10001",    // row 3
            "Bob,Ray,bob@example.com",           // row 4
            "Ed,Kim,ed@example.com,123");        // row 5

        await RunTestAsync(
            (path, _, _) => Ok(path.Contains("/zip/", StringComparison.Ordinal) ? Spokane : csv),
            async (importer, file, handler) =>
            {
                ImportResult result = await importer.ImportAsync("testuser", file, Token);

                Assert.Equal(1, result.WrittenCount);
                Assert.Equal(
                    new[]
                    {
                        new Rejection(3, "Invalid email"),
                        new Rejection(4, "Wrong number of columns"),
                        new Rejection(5, "Invalid ZIP"),
                    },
                    result.Rejections);
                Assert.Contains(("Invalid email", 1), result.CountsByReason);
                Assert.Equal(1, handler.CountFor("/people/zip/99201"));
                Assert.Equal(0, handler.CountFor("/people/zip/10001"));
            });
    }

    [Fact]
    public async Task FailedLookupRemovesOnlyThatPerson()
    {
        string csv = PeopleCsv("John,Doe,john@example.com,99201", "Jane,Smith,jane@example.com,11111");

        await RunTestAsync((path, _, _) => path switch
        {
            "/people/testuser" => Ok(csv),
            "/people/zip/11111" => Status(HttpStatusCode.InternalServerError),
            _ => Ok(Spokane),
        },
        async (importer, file, _) =>
        {
            ImportResult result = await importer.ImportAsync("testuser", file, Token);
            string[] lines = await File.ReadAllLinesAsync(file, Token);

            Assert.Equal(1, result.WrittenCount);
            Assert.Equal(new[] { new Rejection(3, LookupFailed) }, result.Rejections);
            Assert.Equal(new[] { OutputHeader, "John,Doe,john@example.com,99201,Spokane,WA" }, lines);
        });
    }

    [Fact]
    public async Task InvalidLocationIsRejected()
    {
        string csv = PeopleCsv("John,Doe,john@example.com,99201");

        await RunTestAsync(
            (path, _, _) => Ok(path.Contains("/zip/", StringComparison.Ordinal) ? "\"\",\"\"" : csv),
            async (importer, file, _) =>
            {
                ImportResult result = await importer.ImportAsync("testuser", file, Token);

                Assert.Equal(0, result.WrittenCount);
                Assert.Equal(new[] { new Rejection(2, LookupFailed) }, result.Rejections);
                Assert.Equal(new[] { OutputHeader }, await File.ReadAllLinesAsync(file, Token));
            });
    }

    [Fact]
    public async Task FailedPeopleDownloadThrowsAndWritesNothing()
    {
        await RunTestAsync(
            (_, _, _) => Status(HttpStatusCode.BadRequest),
            async (importer, file, _) =>
            {
                await Assert.ThrowsAsync<HttpRequestException>(
                    () => importer.ImportAsync("testuser", file, Token));

                Assert.False(File.Exists(file));
            });
    }

    [Fact]
    public async Task InvalidPeopleCsvDoesNotReplaceExistingFile()
    {
        await RunTestAsync(
            (_, _, _) => Ok("Invalid CSV"),
            async (importer, file, handler) =>
            {
                await File.WriteAllTextAsync(file, "Existing file", Token);

                await Assert.ThrowsAsync<HttpRequestException>(
                    () => importer.ImportAsync("testuser", file, Token));

                Assert.Equal("Existing file", await File.ReadAllTextAsync(file, Token));
                Assert.Equal(PeopleApiClient.MaxAttempts, handler.RequestCount);
            });
    }

    [Fact]
    public async Task UnexpectedExceptionPropagatesAndWritesNothing()
    {
        string csv = PeopleCsv("John,Doe,john@example.com,99201");

        await RunTestAsync(
            (path, _, _) => path.Contains("/zip/", StringComparison.Ordinal)
                ? throw new FormatException("bug")
                : Ok(csv),
            async (importer, file, _) =>
            {
                await Assert.ThrowsAsync<FormatException>(
                    () => importer.ImportAsync("testuser", file, Token));

                Assert.False(File.Exists(file));
            });
    }

    [Fact]
    public async Task FailedWriteLeavesNoTempFile()
    {
        string csv = PeopleCsv("John,Doe,john@example.com,99201");

        await RunTestAsync(
            (path, _, _) => Ok(path.Contains("/zip/", StringComparison.Ordinal) ? Spokane : csv),
            async (importer, file, _) =>
            {
                Directory.CreateDirectory(file);   // a directory in the way makes the final move fail
                try
                {
                    await Assert.ThrowsAnyAsync<Exception>(
                        () => importer.ImportAsync("testuser", file, Token));

                    Assert.Empty(Directory.GetFiles(Path.GetTempPath(), Path.GetFileName(file) + ".*.tmp"));
                }
                finally
                {
                    Directory.Delete(file);
                }
            });
    }

    [Fact]
    public async Task ConcurrentLookupsStayWithinLimit()
    {
        string csv = PeopleCsv(Enumerable.Range(1, 20)
            .Select(i => $"John{i},Doe,john{i}@example.com,99201")
            .ToArray());
        int active = 0;
        int maximum = 0;

        await RunTestAsync(
            async (path, _, token) =>
            {
                if (!path.Contains("/zip/", StringComparison.Ordinal))
                    return await Ok(csv);

                int current = Interlocked.Increment(ref active);
                UpdateMaximum(ref maximum, current);
                try
                {
                    await Task.Delay(50, token);
                    return await Ok(Spokane);
                }
                finally
                {
                    Interlocked.Decrement(ref active);
                }
            },
            async (importer, file, _) =>
            {
                ImportResult result = await importer.ImportAsync("testuser", file, Token);

                Assert.Equal(20, result.WrittenCount);
                Assert.InRange(maximum, 2, 8);   // concurrent, but never more than the limit
            });
    }

    [Fact]
    public async Task CancellationDuringLookupStopsImport()
    {
        string csv = PeopleCsv("John,Doe,john@example.com,99201");
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        await RunTestAsync(
            (path, _, token) =>
            {
                if (!path.Contains("/zip/", StringComparison.Ordinal))
                    return Ok(csv);

                started.TrySetResult();
                return StallAsync(token);
            },
            async (importer, file, _) =>
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(Token);
                Task<ImportResult> task = importer.ImportAsync("testuser", file, cts.Token);

                await started.Task.WaitAsync(Token);
                await cts.CancelAsync();

                await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
                Assert.False(File.Exists(file));
            });
    }

    [Fact]
    public async Task ImporterDoesNotDisposeTheHttpClientItWasGiven()
    {
        string csv = PeopleCsv("John,Doe,john@example.com,99201");

        await RunTestAsync(
            (path, _, _) => Ok(path.Contains("/zip/", StringComparison.Ordinal) ? Spokane : csv),
            async (importer, file, _) =>
            {
                await importer.ImportAsync("testuser", file, Token);

                // A second run on the same client would throw ObjectDisposedException if the first had disposed it.
                ImportResult second = await importer.ImportAsync("testuser", file, Token);
                Assert.Equal(1, second.WrittenCount);
            });
    }

    // ---------- helpers ----------

    private static string PeopleCsv(params string[] rows) => Header + "\n" + string.Join("\n", rows);

    private static async Task RunTestAsync(
        Func<string, int, CancellationToken, Task<HttpResponseMessage>> respond,
        Func<PeopleImporter, string, FakeHandler, Task> test)
    {
        using var handler = new FakeHandler(respond);
        using HttpClient http = NewHttp(handler);
        var importer = new PeopleImporter(new PeopleApiClient(http));
        string file = Path.Combine(Path.GetTempPath(), $"assignment9-{Guid.NewGuid():N}.csv");

        try
        {
            await test(importer, file, handler);
        }
        finally
        {
            if (File.Exists(file))
                File.Delete(file);
        }
    }

    private static void UpdateMaximum(ref int target, int value)
    {
        int previous;
        do
        {
            previous = Volatile.Read(ref target);
            if (value <= previous)
                return;
        }
        while (Interlocked.CompareExchange(ref target, value, previous) != previous);
    }


}
