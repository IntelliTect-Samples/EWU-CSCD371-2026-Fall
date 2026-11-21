using PeopleEnricher;

namespace PeopleEnricher.Tests;

[TestClass]
public sealed class PeopleEnrichmentServiceTests
{
    [TestMethod]
    public async Task EnrichAsyncReturnsEnrichedPeople()
    {
        Person[] people =
        [
            CreatePerson(1),
            CreatePerson(2)
        ];

        FakePeopleApiClient api = CreateFake(people);

        PeopleEnrichmentService service = new(api);

        EnrichmentResult result = await service.EnrichAsync("test-user");

        Assert.HasCount(2, result.People);
        Assert.IsEmpty(result.Rejections);

        Assert.AreEqual("SPOKANE", result.People[0].City);
        Assert.AreEqual("WA", result.People[0].State);
    }

    [TestMethod]
    public async Task EnrichAsyncPreservesPeopleFeedRejections()
    {
        RejectedPerson existingRejection = new(
            RejectionStage.PeopleCsv,
            4,
            "Invalid original CSV row",
            "EmailAddress is invalid.");

        FakePeopleApiClient api = CreateFake(
            [CreatePerson(1)],
            [existingRejection]);

        PeopleEnrichmentService service = new(api);

        EnrichmentResult result = await service.EnrichAsync("test-user");

        Assert.HasCount(1, result.People);
        Assert.HasCount(1, result.Rejections);
        Assert.AreSame(existingRejection, result.Rejections[0]);
    }

    [TestMethod]
    public async Task EnrichAsyncRejectsOnlyPersonWithFailedZipLookup()
    {
        Person successfulPerson = CreatePerson(1);
        Person failedPerson = CreatePerson(2);

        FakePeopleApiClient api = CreateFake(
            [successfulPerson, failedPerson],
            (zipCode, _) =>
            {
                if (zipCode == failedPerson.ZipCode)
                {
                    return Task.FromException<ZipLocation>(
                        new HttpRequestException("Lookup failed."));
                }

                return Task.FromResult(
                    new ZipLocation("SPOKANE", "WA"));
            });

        PeopleEnrichmentService service = new(api);

        EnrichmentResult result = await service.EnrichAsync("test-user");

        Assert.HasCount(1, result.People);
        Assert.HasCount(1, result.Rejections);

        Assert.AreEqual(
            successfulPerson.FirstName,
            result.People[0].FirstName);

        Assert.AreEqual(
            RejectionStage.ZipLookup,
            result.Rejections[0].Stage);

        Assert.Contains("Lookup failed.", result.Rejections[0].Reason);
    }

    [TestMethod]
    public async Task EnrichAsyncRejectsPersonWithInvalidZipResponse()
    {
        Person person = CreatePerson(1);

        FakePeopleApiClient api = CreateFake(
            [person],
            (_, _) => Task.FromException<ZipLocation>(
                new InvalidDataException("BAD_DATA")));

        PeopleEnrichmentService service = new(api);

        EnrichmentResult result = await service.EnrichAsync("test-user");

        Assert.IsEmpty(result.People);
        Assert.HasCount(1, result.Rejections);
        Assert.AreEqual("BAD_DATA", result.Rejections[0].Reason);
    }

    [TestMethod]
    public async Task EnrichAsyncRejectsPersonAfterZipTimeout()
    {
        Person person = CreatePerson(1);

        FakePeopleApiClient api = CreateFake(
            [person],
            (_, _) => Task.FromException<ZipLocation>(
                new OperationCanceledException("Simulated timeout.")));

        PeopleEnrichmentService service = new(api);

        EnrichmentResult result = await service.EnrichAsync("test-user");

        Assert.IsEmpty(result.People);
        Assert.HasCount(1, result.Rejections);

        Assert.Contains("timed out", result.Rejections[0].Reason);
    }

    [TestMethod]
    public async Task EnrichAsyncPropagatesCallerCancellation()
    {
        TaskCompletionSource<bool> lookupStarted = new(
            TaskCreationOptions.RunContinuationsAsynchronously);

        FakePeopleApiClient api = CreateFake(
            [CreatePerson(1)],
            async (_, cancellationToken) =>
            {
                lookupStarted.TrySetResult(true);

                await Task.Delay(
                    Timeout.InfiniteTimeSpan,
                    cancellationToken);

                return new ZipLocation("SPOKANE", "WA");
            });

        PeopleEnrichmentService service = new(api);

        using CancellationTokenSource cancellationSource = new();

        Task<EnrichmentResult> operation = service.EnrichAsync(
            "test-user",
            cancellationSource.Token);

        await lookupStarted.Task.WaitAsync(TimeSpan.FromSeconds(2));

        cancellationSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            async () => await operation);

        Assert.AreEqual(1, api.LookupCalls);
    }

    [TestMethod]
    public async Task EnrichAsyncPropagatesUnexpectedExceptions()
    {
        FakePeopleApiClient api = CreateFake(
            [CreatePerson(1)],
            (_, _) => Task.FromException<ZipLocation>(
                new InvalidOperationException("Unexpected failure.")));

        PeopleEnrichmentService service = new(api);

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            async () => await service.EnrichAsync("test-user"));
    }

    [TestMethod]
    public async Task EnrichAsyncLimitsConcurrentLookups()
    {
        const int personCount = 12;
        const int concurrencyLimit = 3;

        int activeRequests = 0;
        int maximumObservedConcurrency = 0;

        Person[] people = Enumerable
            .Range(1, personCount)
            .Select(CreatePerson)
            .ToArray();

        FakePeopleApiClient api = CreateFake(
            people,
            async (_, cancellationToken) =>
            {
                int current = Interlocked.Increment(
                    ref activeRequests);

                int observed;

                while ((observed = Volatile.Read(
                           ref maximumObservedConcurrency)) < current)
                {
                    if (Interlocked.CompareExchange(
                            ref maximumObservedConcurrency,
                            current,
                            observed) == observed)
                    {
                        break;
                    }
                }

                try
                {
                    await Task.Delay(
                        50,
                        cancellationToken);

                    return new ZipLocation("SPOKANE", "WA");
                }
                finally
                {
                    Interlocked.Decrement(ref activeRequests);
                }
            });

        PeopleEnrichmentService service = new(
            api,
            concurrencyLimit);

        EnrichmentResult result = await service.EnrichAsync("test-user");

        Assert.HasCount(personCount, result.People);
        Assert.AreEqual(personCount, api.LookupCalls);

        Assert.IsGreaterThan(
            1,
            maximumObservedConcurrency,
            "ZIP requests should execute concurrently.");

        Assert.IsLessThanOrEqualTo(
            maximumObservedConcurrency,
            concurrencyLimit,
            "The concurrency limit was exceeded.");
    }

    [TestMethod]
    public async Task EnrichAsyncPreservesOriginalPeopleOrder()
    {
        Person[] people =
        [
            CreatePerson(1),
            CreatePerson(2),
            CreatePerson(3)
        ];

        FakePeopleApiClient api = CreateFake(
            people,
            async (zipCode, cancellationToken) =>
            {
                // Deliberately complete requests in a different order.
                int delay = zipCode == people[0].ZipCode ? 60 :
                    zipCode == people[1].ZipCode ? 30 : 5;

                await Task.Delay(delay, cancellationToken);

                return new ZipLocation("SPOKANE", "WA");
            });

        PeopleEnrichmentService service = new(api);

        EnrichmentResult result = await service.EnrichAsync("test-user");

        for (int index = 0; index < people.Length; index++)
        {
            Assert.AreEqual(
                people[index].FirstName,
                result.People[index].FirstName);
        }
    }

    [TestMethod]
    public async Task EnrichAsyncPropagatesPeopleFeedFailure()
    {
        FakePeopleApiClient api = CreateFake(
            (_, _) => Task.FromException<ZipLocation>(
                new HttpRequestException("Should not be called.")),
            _ => Task.FromException<PeopleCsvParseResult>(
                new HttpRequestException("People feed unavailable.")));

        PeopleEnrichmentService service = new(api);

        await Assert.ThrowsExactlyAsync<HttpRequestException>(
            async () => await service.EnrichAsync("test-user"));

        Assert.AreEqual(0, api.LookupCalls);
    }

    [TestMethod]
    public void ConstructorRejectsZeroConcurrency()
    {
        FakePeopleApiClient api = CreateFake([]);

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new PeopleEnrichmentService(api, 0));
    }

    private static Person CreatePerson(int number)
    {
        return new Person(
            $"First{number}",
            $"Last{number}",
            $"person{number}@example.com",
            number.ToString("D5"));
    }

    private static FakePeopleApiClient CreateFake(
        Person[] people,
        RejectedPerson[]? rejections = null)
    {
        PeopleCsvParseResult feed = new(
            Array.AsReadOnly(people),
            Array.AsReadOnly(rejections ?? []));

        return new FakePeopleApiClient(
            _ => Task.FromResult(feed),
            (_, _) => Task.FromResult(
                new ZipLocation("SPOKANE", "WA")));
    }

    private static FakePeopleApiClient CreateFake(
        Person[] people,
        Func<string, CancellationToken, Task<ZipLocation>> lookup)
    {
        PeopleCsvParseResult feed = new(
            Array.AsReadOnly(people),
            Array.Empty<RejectedPerson>());

        return new FakePeopleApiClient(
            _ => Task.FromResult(feed),
            lookup);
    }

    private static FakePeopleApiClient CreateFake(
        Func<string, CancellationToken, Task<ZipLocation>> lookup,
        Func<CancellationToken, Task<PeopleCsvParseResult>> getPeople)
    {
        return new FakePeopleApiClient(getPeople, lookup);
    }

    private sealed class FakePeopleApiClient(
        Func<CancellationToken, Task<PeopleCsvParseResult>> getPeople,
        Func<string, CancellationToken, Task<ZipLocation>> getLocation)
        : IPeopleApiClient
    {
        private int _lookupCalls;

        public int LookupCalls => Volatile.Read(ref _lookupCalls);

        public Task<PeopleCsvParseResult> GetPeopleAsync(
            string user,
            CancellationToken cancellationToken = default)
        {
            return getPeople(cancellationToken);
        }

        public Task<ZipLocation> GetZipLocationAsync(
            string zipCode,
            CancellationToken cancellationToken = default)
        {
            Interlocked.Increment(ref _lookupCalls);

            return getLocation(zipCode, cancellationToken);
        }
    }
}