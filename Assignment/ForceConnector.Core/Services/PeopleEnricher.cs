using ForceConnector.Core.Interfaces;
using ForceConnector.Core.Models;

namespace ForceConnector.Core.Services;

public sealed class PeopleEnricher : IPeopleEnricher
{
    private readonly IPeopleClient _peopleClient;
    private readonly ILocationClient _locationClient;
    private readonly PersonValidator _personValidator;
    private readonly LocationValidator _locationValidator;
    private readonly ICsvParser _csvParser;
    private const int MaxConcurrentLookups = 10;
    public PeopleEnricher(
    IPeopleClient peopleClient,
    ILocationClient locationClient,
    ICsvParser csvParser)
    {
        _peopleClient = peopleClient;
        _locationClient = locationClient;

        _personValidator = new PersonValidator();
        _locationValidator = new LocationValidator();
        _csvParser = csvParser;
    }

    public async Task<EnrichmentResult> EnrichAsync(
        string user,
        CancellationToken cancellationToken)
    {
        string csv =
            await _peopleClient.DownloadPeopleCsvAsync(
                user,
                cancellationToken);

        CsvParseResult parseResult =
            _csvParser.ParsePeopleWithRejections(csv);

        IReadOnlyList<Person> people = parseResult.People;

        List<Person> validPeople = [];

        List<Rejection> rejections = [.. parseResult.Rejections];

        foreach (Person person in people)
        {
            IReadOnlyList<string> errors =
                _personValidator.GetErrors(person);

            if (errors.Count == 0)
            {
                validPeople.Add(person);
            }
            else
            {
                rejections.Add(
                    new Rejection
                    {
                        EmailAddress =
                            person.EmailAddress,
                        Reason = string.Join(" ", errors)
                    });
            }
        }

        SemaphoreSlim semaphore =
            new(MaxConcurrentLookups);

        try
        {
            List<Task<EnrichedPerson?>> tasks =
                validPeople
                    .Select(person =>
                        EnrichPersonAsync(
                            person,
                            semaphore,
                            rejections,
                            cancellationToken))
                    .ToList();

            EnrichedPerson?[] results =
                await Task.WhenAll(tasks);

            List<EnrichedPerson> enrichedPeople =
                results
                    .Where(p => p is not null)
                    .Cast<EnrichedPerson>()
                    .ToList();

            return new EnrichmentResult
            {
                People = enrichedPeople,
                Rejections = rejections
            };
        }
        finally
        {
            semaphore.Dispose();
        }
    }

    private async Task<EnrichedPerson?> EnrichPersonAsync(
        Person person,
        SemaphoreSlim semaphore,
        List<Rejection> rejections,
        CancellationToken cancellationToken)
    {
        await semaphore.WaitAsync(
            cancellationToken);

        try
        {
            Location location =
                await RetryPolicy.ExecuteAsync(
                    () =>
                        _locationClient.LookupAsync(
                            person.ZipCode,
                            cancellationToken),
                    cancellationToken);

            IReadOnlyList<string> errors =
                _locationValidator.GetErrors(location);

            if (errors.Count > 0)
            {
                lock (rejections)
                {
                    rejections.Add(
                        new Rejection
                        {
                            EmailAddress =
                                person.EmailAddress,
                            Reason = string.Join(" ", errors)
                        });
                }

                return null;
            }

            return new EnrichedPerson
            {
                FirstName = person.FirstName,
                LastName = person.LastName,
                EmailAddress = person.EmailAddress,
                ZipCode = person.ZipCode,
                City = location.City,
                State = location.State
            };
        }
        catch (HttpRequestException)
        {
            lock (rejections)
            {
                rejections.Add(
                    new Rejection
                    {
                        EmailAddress =
                            person.EmailAddress,
                        Reason =
                            "Location request failed"
                    });
            }

            return null;
        }
        catch (OperationCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            lock (rejections)
            {
                rejections.Add(
                    new Rejection
                    {
                        EmailAddress =
                            person.EmailAddress,
                        Reason =
                            "Location request timed out"
                    });
            }

            return null;
        }
        catch (InvalidDataException)
        {
            lock (rejections)
            {
                rejections.Add(
                    new Rejection
                    {
                        EmailAddress =
                            person.EmailAddress,
                        Reason =
                            "Invalid location response"
                    });
            }

            return null;
        }
        finally
        {
            semaphore.Release();
        }
    }
}