using Assignment.Interfaces;

namespace Assignment;

public class PersonEnricher
{
    private static string ParseValue(string value)
    {
        return value.Length <= 1 ? value : string.Concat(char.ToUpperInvariant(value[0]), value[1..].ToLowerInvariant());
    }

    public static async Task<string> GetData()

    public static async Task<EnrichedPerson> EnrichAsync(IPerson person, SemaphoreSlim limiter, CancellationToken cancellationToken)
    {
        await limiter.WaitAsync(cancellationToken);

        try
        {
            string response = await HttpClientWrapper.Client.GetStringAsync($"/people/zip/{person.ZipCode}", cancellationToken = default);
            string[] cols = response.Split(',', StringSplitOptions.TrimEntries);
            string city = ParseValue(cols[0]);
            string state = ParseValue(cols[1]);
            return new(person, city, state);
        }
        finally
        {
            limiter.Release();
        }
    }

    public static async Task<EnrichedPerson[]> EnrichPeople(IReadOnlyList<Person> people
        , int concurrentRequestLimit
        , CancellationToken cancellationToken = default)
    {
        using SemaphoreSlim limiter = new SemaphoreSlim(initialCount: concurrentRequestLimit);
        Task<EnrichedPerson>[] tasks = people.Select(async (Person person) =>
        foreach (Person person in people)
        {

        }
        var enrichedPeople = await Task.WhenAll(tasks);
        return enrichedPeople;
    }
}
