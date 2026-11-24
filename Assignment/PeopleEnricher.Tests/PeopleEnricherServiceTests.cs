using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeopleEnricher.Services;
using PeopleEnricher.Models;

namespace PeopleEnricher.Tests;

[TestClass]
public sealed class PeopleEnricherServiceTests
{
    [TestMethod]
    public async Task DownloadPeopleCsvAsync_RetriesTemporaryFailure()
    {
        const string expectedCsv =
            "FirstName,LastName,EmailAddress,ZipCode\n" +
            "John,Smith,john@example.com,99201";

        int attempts = 0;

        using HttpClient httpClient = new(
            new FakeHttpMessageHandler((_, _) =>
            {
                attempts++;

                HttpResponseMessage response;

                if (attempts == 1)
                {
                    response = new HttpResponseMessage(
                        HttpStatusCode.ServiceUnavailable);
                }
                else
                {
                    response = new HttpResponseMessage(
                        HttpStatusCode.OK)
                    {
                        Content = new StringContent(expectedCsv)
                    };
                }

                return Task.FromResult(response);
            }));

        PeopleEnricherService service = new(httpClient);

        string actualCsv = await service.DownloadPeopleCsvAsync(
            "Mavrick",
            CancellationToken.None);

        Assert.AreEqual(expectedCsv, actualCsv);
        Assert.AreEqual(2, attempts);
    }
    [TestMethod]
    public async Task DownloadPeopleCsvAsync_DoesNotRetryNotFound()
    {
        int attempts = 0;

        using HttpClient httpClient = new(
            new FakeHttpMessageHandler((_, _) =>
            {
                attempts++;

                return Task.FromResult(
                    new HttpResponseMessage(HttpStatusCode.NotFound));
            }));

        PeopleEnricherService service = new(httpClient);

        HttpRequestException exception =
            await Assert.ThrowsExactlyAsync<HttpRequestException>(
                async () =>
                {
                    await service.DownloadPeopleCsvAsync(
                        "Mavrick",
                        CancellationToken.None);
                });

        Assert.AreEqual(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.AreEqual(1, attempts);
    }

    [TestMethod]
    public async Task DownloadPeopleCsvAsync_StopsWhenCanceled()
    {
        int attempts = 0;

        using CancellationTokenSource cancellation = new();

        using HttpClient httpClient = new(
            new FakeHttpMessageHandler((_, _) =>
            {
                attempts++;
                cancellation.Cancel();

                return Task.FromResult(
                    new HttpResponseMessage(
                        HttpStatusCode.ServiceUnavailable));
            }));

        PeopleEnricherService service = new(httpClient);

        bool wasCanceled = false;

        try
        {
            await service.DownloadPeopleCsvAsync(
                "Mavrick",
                cancellation.Token);
        }
        catch (OperationCanceledException)
        {
            wasCanceled = true;
        }

        Assert.IsTrue(wasCanceled);
        Assert.AreEqual(1, attempts);
    }

    [TestMethod]
    public async Task LookupLocationAsync_RejectsInvalidState()
    {
        int attempts = 0;

        using HttpClient httpClient = new(
            new FakeHttpMessageHandler((_, _) =>
            {
                attempts++;

                return Task.FromResult(
                    new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(
                            "\"Spokane\",\"Washington\"")
                    });
            }));

        PeopleEnricherService service = new(httpClient);

        await Assert.ThrowsExactlyAsync<FormatException>(
            async () =>
            {
                await service.LookupLocationAsync(
                    "99201",
                    CancellationToken.None);
            });

        Assert.AreEqual(5, attempts);
    }

    [TestMethod]
    public void ParsePeopleCsv_RejectsInvalidRows()
    {
        const string csv =
            "FirstName,LastName,EmailAddress,ZipCode\n" +
            "John,Smith,john@example.com,99201\n" +
            "Jane,Doe,jane@example.com";

        var result = PeopleEnricherService.ParsePeopleCsv(csv);

        Assert.HasCount(1, result.People);
        Assert.AreEqual("John", result.People[0].Person.FirstName);

        Assert.HasCount(1, result.RejectedPeople);
        Assert.AreEqual(3, result.RejectedPeople[0].RowNumber);
        Assert.AreEqual(
            "Incorrect column count",
            result.RejectedPeople[0].Reason);
    }

    [TestMethod]
    public void GetPersonValidationError_ChecksPersonInformation()
    {
        Person validPerson = new(
            "John", "Smith", "john@example.com", "99201");

        Person missingFirstName = new(
            "", "Smith", "john@example.com", "99201");

        Person missingLastName = new(
            "John", "", "john@example.com", "99201");

        Person invalidEmail = new(
            "John", "Smith", "not-an-email", "99201");

        Person invalidZip = new(
            "John", "Smith", "john@example.com", "123");

        Assert.IsNull(
            PeopleEnricherService.GetPersonValidationError(validPerson));

        Assert.AreEqual(
            "Missing first name",
            PeopleEnricherService.GetPersonValidationError(missingFirstName));

        Assert.AreEqual(
            "Missing last name",
            PeopleEnricherService.GetPersonValidationError(missingLastName));

        Assert.AreEqual(
            "Invalid email address",
            PeopleEnricherService.GetPersonValidationError(invalidEmail));

        Assert.AreEqual(
            "Invalid ZIP code",
            PeopleEnricherService.GetPersonValidationError(invalidZip));
    }

    [TestMethod]
    public void FilterValidPeople_SeparatesValidAndInvalidPeople()
    {
        List<(int RowNumber, Person Person)> people =
        [
            (2, new Person(
            "John", "Smith", "john@example.com", "99201")),

        (3, new Person(
            "", "Doe", "jane@example.com", "99019")),

        (4, new Person(
            "Bob", "Jones", "bob@example.com", "123")),

        (5, new Person(
            "Amy", "Lee", "amy@example.com", "10001"))
        ];

        var result = PeopleEnricherService.FilterValidPeople(people);

        Assert.HasCount(2, result.ValidPeople);
        Assert.HasCount(2, result.RejectedPeople);

        Assert.AreEqual("John", result.ValidPeople[0].Person.FirstName);
        Assert.AreEqual("Amy", result.ValidPeople[1].Person.FirstName);

        Assert.AreEqual(3, result.RejectedPeople[0].RowNumber);
        Assert.AreEqual(
            "Missing first name",
            result.RejectedPeople[0].Reason);

        Assert.AreEqual(4, result.RejectedPeople[1].RowNumber);
        Assert.AreEqual(
            "Invalid ZIP code",
            result.RejectedPeople[1].Reason);
    }

    [TestMethod]
    public async Task ProcessPeopleAsync_LimitsConcurrentRequests()
    {
        int activeRequests = 0;
        int maxActiveRequests = 0;
        object counterLock = new();

        TaskCompletionSource<bool> fiveStarted = new(
            TaskCreationOptions.RunContinuationsAsynchronously);

        TaskCompletionSource<bool> releaseRequests = new(
            TaskCreationOptions.RunContinuationsAsynchronously);

        using HttpClient httpClient = new(
            new FakeHttpMessageHandler(async (_, token) =>
            {
                lock (counterLock)
                {
                    activeRequests++;

                    maxActiveRequests = Math.Max(
                        maxActiveRequests, activeRequests);

                    if (activeRequests == 5)
                    {
                        fiveStarted.TrySetResult(true);
                    }
                }

                try
                {
                    await releaseRequests.Task.WaitAsync(token);

                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("\"Spokane\",\"WA\"")
                    };
                }
                finally
                {
                    lock (counterLock)
                    {
                        activeRequests--;
                    }
                }
            }));

        PeopleEnricherService service = new(httpClient);

        List<(int RowNumber, Person Person)> people =
            Enumerable.Range(0, 10)
                .Select(index => (
                    RowNumber: index + 2,
                    Person: new Person(
                        "John", "Smith",
                        $"person{index}@example.com",
                        "99201")))
                .ToList();

        using CancellationTokenSource timeout =
            new(TimeSpan.FromSeconds(10));

        Task<ProcessingResult> work = service.ProcessPeopleAsync(
            people, timeout.Token);

        try
        {
            await fiveStarted.Task.WaitAsync(timeout.Token);
        }
        finally
        {
            releaseRequests.TrySetResult(true);
        }

        ProcessingResult result = await work;

        Assert.HasCount(10, result.CompletedPeople);
        Assert.IsEmpty(result.RejectedPeople);
        Assert.AreEqual(5, maxActiveRequests);
    }

    [TestMethod]
    public async Task RunAsync_ProcessesAndRejectsPeopleCorrectly()
    {
        const string peopleCsv =
            "FirstName,LastName,EmailAddress,ZipCode\n" +
            "John,Smith,john@example.com,99201\n" +
            "Jane,Doe,not-an-email,99019\n" +
            "Bob,Jones,bob@example.com";

        int peopleRequests = 0;
        int locationRequests = 0;

        using HttpClient httpClient = new(
            new FakeHttpMessageHandler((request, _) =>
            {
                string path = request.RequestUri!.AbsolutePath;

                if (path == "/people/Mavrick")
                {
                    peopleRequests++;

                    return Task.FromResult(
                        new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent(peopleCsv)
                        });
                }

                if (path == "/people/zip/99201")
                {
                    locationRequests++;

                    return Task.FromResult(
                        new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent(
                                "\"Spokane\",\"WA\"")
                        });
                }

                return Task.FromResult(
                    new HttpResponseMessage(HttpStatusCode.NotFound));
            }));

        PeopleEnricherService service = new(httpClient);

        ProcessingResult result = await service.RunAsync(
            "Mavrick",
            CancellationToken.None,
            expectedPeopleCount: 3);

        Assert.HasCount(1, result.CompletedPeople);
        Assert.HasCount(2, result.RejectedPeople);

        Assert.AreEqual(
            "John",
            result.CompletedPeople[0].FirstName);

        Assert.AreEqual(
            "Spokane",
            result.CompletedPeople[0].City);

        Assert.AreEqual(
            "WA",
            result.CompletedPeople[0].State);

        Assert.AreEqual(3, result.RejectedPeople[0].RowNumber);
        Assert.AreEqual(
            "Invalid email address",
            result.RejectedPeople[0].Reason);

        Assert.AreEqual(4, result.RejectedPeople[1].RowNumber);
        Assert.AreEqual(
            "Incorrect column count",
            result.RejectedPeople[1].Reason);

        Assert.AreEqual(1, peopleRequests);
        Assert.AreEqual(1, locationRequests);
    }

    [TestMethod]
    public async Task DownloadPeopleCsvAsync_RetriesHttp418()
    {
        int attempts = 0;

        const string expectedCsv =
            "FirstName,LastName,EmailAddress,ZipCode\n" +
            "John,Smith,john@example.com,99201";

        using HttpClient httpClient = new(
            new FakeHttpMessageHandler((_, _) =>
            {
                attempts++;

                return Task.FromResult(
                    new HttpResponseMessage(
                        attempts == 1
                            ? (HttpStatusCode)418
                            : HttpStatusCode.OK)
                    {
                        Content = new StringContent(expectedCsv)
                    });
            }));

        PeopleEnricherService service = new(httpClient);

        string result = await service.DownloadPeopleCsvAsync(
            "Mavrick",
            CancellationToken.None);

        Assert.AreEqual(expectedCsv, result);
        Assert.AreEqual(2, attempts);
    }

    [TestMethod]
    public async Task LookupLocationAsync_RetriesInvalidData()
    {
        int attempts = 0;

        using HttpClient httpClient = new(
            new FakeHttpMessageHandler((_, _) =>
            {
                attempts++;

                string response = attempts == 1
                    ? "\"Spokane\",\"Washington\""
                    : "\"Spokane\",\"WA\"";

                return Task.FromResult(
                    new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(response)
                    });
            }));

        PeopleEnricherService service = new(httpClient);

        Location location = await service.LookupLocationAsync(
            "99201",
            CancellationToken.None);

        Assert.AreEqual("Spokane", location.City);
        Assert.AreEqual("WA", location.State);
        Assert.AreEqual(2, attempts);
    }

    [TestMethod]
    public async Task SaveCsvAsync_CancellationCleansUpFiles()
    {
        string testDirectory = Path.Combine(
            Path.GetTempPath(),
            $"PeopleEnricherTest_{Guid.NewGuid():N}");

        Directory.CreateDirectory(testDirectory);

        string outputPath = Path.Combine(
            testDirectory, "CompletedPeople.csv");

        const string originalContent = "Previous successful CSV";

        try
        {
            await File.WriteAllTextAsync(
                outputPath, originalContent);

            ProcessingResult result = new(
                [
                    new PersonWithLocation(
                    "John",
                    "Smith",
                    "john@example.com",
                    "99201",
                    "Spokane",
                    "WA")
                ],
                []);

            using CancellationTokenSource cancellation = new();
            cancellation.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(
                async () =>
                {
                    await PeopleEnricherService.SaveCsvAsync(
                        result, outputPath, cancellation.Token);
                });

            Assert.AreEqual(
                originalContent,
                await File.ReadAllTextAsync(outputPath));

            Assert.IsEmpty(
                Directory.GetFiles(testDirectory, "*.tmp"));
        }
        finally
        {
            Directory.Delete(testDirectory, recursive: true);
        }
    }

    [TestMethod]
    public async Task DownloadPeopleCsvAsync_RetriesInvalidHeader()
    {
        const string validCsv =
            "FirstName,LastName,EmailAddress,ZipCode\n" +
            "John,Smith,john@example.com,99201";

        int attempts = 0;

        using HttpClient httpClient = new(
            new FakeHttpMessageHandler((_, _) =>
            {
                attempts++;

                string response = attempts == 1
                    ? "InvalidHeader\nJohn,Smith,john@example.com,99201"
                    : validCsv;

                return Task.FromResult(
                    new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(response)
                    });
            }));

        PeopleEnricherService service = new(httpClient);

        string result = await service.DownloadPeopleCsvAsync(
            "Mavrick",
            CancellationToken.None);

        Assert.AreEqual(validCsv, result);
        Assert.AreEqual(2, attempts);
    }

    [TestMethod]
    public async Task DownloadPeopleCsvAsync_RetriesTimeout()
    {
        int attempts = 0;

        const string expectedCsv =
            "FirstName,LastName,EmailAddress,ZipCode\n" +
            "John,Smith,john@example.com,99201";

        using HttpClient httpClient = new(
            new FakeHttpMessageHandler((_, _) =>
            {
                attempts++;

                if (attempts == 1)
                {
                    return Task.FromException<HttpResponseMessage>(
                        new TaskCanceledException("Simulated timeout"));
                }

                return Task.FromResult(
                    new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(expectedCsv)
                    });
            }));

        PeopleEnricherService service = new(httpClient);

        string result = await service.DownloadPeopleCsvAsync(
            "Mavrick",
            CancellationToken.None);

        Assert.AreEqual(expectedCsv, result);
        Assert.AreEqual(2, attempts);
    }

    [TestMethod]
    public async Task LookupLocationAsync_RetriesTimeout()
    {
        int attempts = 0;

        using HttpClient httpClient = new(
            new FakeHttpMessageHandler((_, _) =>
            {
                attempts++;

                if (attempts == 1)
                {
                    return Task.FromException<HttpResponseMessage>(
                        new TaskCanceledException("Simulated timeout"));
                }

                return Task.FromResult(
                    new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(
                            "\"Spokane\",\"WA\"")
                    });
            }));

        PeopleEnricherService service = new(httpClient);

        Location location = await service.LookupLocationAsync(
            "99201",
            CancellationToken.None);

        Assert.AreEqual("Spokane", location.City);
        Assert.AreEqual("WA", location.State);
        Assert.AreEqual(2, attempts);
    }

    [TestMethod]
    public async Task DownloadPeopleCsvAsync_ThrowsAfterAllRetries()
    {
        int attempts = 0;

        using HttpClient httpClient = new(
            new FakeHttpMessageHandler((_, _) =>
            {
                attempts++;

                return Task.FromResult(
                    new HttpResponseMessage(
                        HttpStatusCode.ServiceUnavailable));
            }));

        PeopleEnricherService service = new(httpClient);

        HttpRequestException exception =
            await Assert.ThrowsExactlyAsync<HttpRequestException>(
                () => service.DownloadPeopleCsvAsync(
                    "Mavrick",
                    CancellationToken.None));

        Assert.AreEqual(
            HttpStatusCode.ServiceUnavailable,
            exception.StatusCode);

        Assert.AreEqual(5, attempts);
    }

    [TestMethod]
    public async Task LookupLocationAsync_PropagatesUnexpectedException()
    {
        int attempts = 0;

        using HttpClient httpClient = new(
            new FakeHttpMessageHandler((_, _) =>
            {
                attempts++;

                return Task.FromException<HttpResponseMessage>(
                    new InvalidOperationException(
                        "Unexpected test failure"));
            }));

        PeopleEnricherService service = new(httpClient);

        InvalidOperationException exception =
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                () => service.LookupLocationAsync(
                    "99201",
                    CancellationToken.None));

        Assert.AreEqual(
            "Unexpected test failure",
            exception.Message);

        Assert.AreEqual(1, attempts);
    }

    [TestMethod]
    public async Task SaveCsvAsync_FailedMoveCleansUpTemporaryFile()
    {
        string testDirectory = Path.Combine(
            Path.GetTempPath(),
            $"PeopleEnricherTest_{Guid.NewGuid():N}");

        Directory.CreateDirectory(testDirectory);

        string outputPath = Path.Combine(
            testDirectory, "CompletedPeople.csv");

        // Create a directory where the final CSV should go.
        // This will cause File.Move to fail.
        Directory.CreateDirectory(outputPath);

        try
        {
            ProcessingResult result = new(
                [
                    new PersonWithLocation(
                    "John",
                    "Smith",
                    "john@example.com",
                    "99201",
                    "Spokane",
                    "WA")
                ],
                []);

            Exception? failure = null;

            try
            {
                await PeopleEnricherService.SaveCsvAsync(
                    result,
                    outputPath,
                    CancellationToken.None);
            }
            catch (IOException ex)
            {
                failure = ex;
            }
            catch (UnauthorizedAccessException ex)
            {
                failure = ex;
            }

            Assert.IsNotNull(failure);

            Assert.IsTrue(Directory.Exists(outputPath));

            Assert.IsEmpty(
                Directory.GetFiles(testDirectory, "*.tmp"));
        }
        finally
        {
            Directory.Delete(testDirectory, recursive: true);
        }
    }

    [TestMethod]
    public async Task DownloadPeopleCsvAsync_RetriesIncompleteCsv()
    {
        const string header =
            "FirstName,LastName,EmailAddress,ZipCode\n";

        const string firstPerson =
            "John,Smith,john@example.com,99201\n";

        const string secondPerson =
            "Jane,Doe,jane@example.com,99019\n";

        const string thirdPerson =
            "Bob,Jones,bob@example.com,10001\n";

        string incompleteCsv =
            header + firstPerson + secondPerson;

        string completeCsv =
            header + firstPerson + secondPerson + thirdPerson;

        int attempts = 0;

        using HttpClient httpClient = new(
            new FakeHttpMessageHandler((_, _) =>
            {
                attempts++;

                string response = attempts == 1
                    ? incompleteCsv
                    : completeCsv;

                return Task.FromResult(
                    new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(response)
                    });
            }));

        PeopleEnricherService service = new(httpClient);

        string result = await service.DownloadPeopleCsvAsync(
            "Mavrick",
            CancellationToken.None,
            expectedPeopleCount: 3);

        Assert.AreEqual(completeCsv, result);
        Assert.AreEqual(2, attempts);
    }
}