using System.Diagnostics;
using Bogus;
using CsvHelper;
using ShereSoft;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);
builder.AddTelemetry();

builder.WebHost.ConfigureKestrel(x =>
{
    x.AllowSynchronousIO = true;
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddOpenApi();

builder.Services.AddScoped(sp =>
{
    var faker = new Faker<Person>();
    faker.RuleFor(p => p.FirstName, f => f.Name.FirstName());
    faker.RuleFor(p => p.LastName, f => f.Name.LastName());
    faker.RuleFor(p => p.EmailAddress, f => f.Internet.Email());
    faker.RuleFor(p => p.ZipCode, f =>
    {
        Random random = new(f.Random.Number(int.MaxValue));
        return Zip2City.GetRandomCityStateZip(random)[2];
    });
    return faker;
});

var app = builder.Build();
app.MapOpenApi();

var peopleGroup = app.MapGroup("/people");

/*
 * Failure cases:
 * 1. Invalid zip code
 * 2. Invalid CSV data?
 * 3. Timeout
 * 4. 500
 */

peopleGroup.MapGet("/{user}", async (string user, ChaosModes? code, Faker<Person> faker, ILogger<Program> logger) =>
{
    int seed = HashCode.Combine(user);

    // Attempts per user/seed. Counted up front so chaos failures and timeouts are included.
    Telemetry.PeopleRequests.Add(1, new KeyValuePair<string, object?>("user", user));
    Activity.Current?.SetTag("marmot.user", user);

    return Results.Stream(async stream =>
    {
        faker.UseSeed(seed);
        using CsvWriter csvWriter = CsvWriter.Create(stream);
        var people = faker.Generate(100);

        var mode = GetEffectiveChaosMode(code);
        RecordChaos(mode, "/people/{user}", logger);
        switch (mode)
        {
            case ChaosModes.InvalidData:
                for(int i = 0; i < people.Count; i+= Random.Shared.Next(1, 7))
                {
                    people[i].ZipCode = Random.Shared.Next(10000, 99999).ToString();
                }
                break;
            case ChaosModes.Timeout:
                await Task.Delay(TimeSpan.FromSeconds(30));
                break;
            case ChaosModes.Exception:
                throw new InvalidOperationException("Chaos Monkey broke it");
        }

        await csvWriter.WriteRecordsAsync(people);
        Telemetry.PeopleGenerated.Add(people.Count);
        logger.PeopleGenerated(people.Count, user);
    });
})
.WithName("GetPeople");

/*
 * Failure cases:
 * 1. 404 Not Found
 * 2. Timeout
 * 3. 418 - For fun, go get tea.
 */
peopleGroup.MapGet("zip/{zip}", async (string zip, ChaosModes? code, Faker<Person> faker, ILogger<Program> logger) =>
{
    byte[] csvData;
    var mode = GetEffectiveChaosMode(code);
    RecordChaos(mode, "/people/zip/{zip}", logger);
    switch (mode)
    {
        case ChaosModes.InvalidData:
            csvData = Encoding.ASCII.GetBytes("BAD_DATA");
            break;
        case ChaosModes.Timeout:
            await Task.Delay(TimeSpan.FromSeconds(30));
            goto default;
        case ChaosModes.Exception:
            return Results.StatusCode(418);
        default:
            var cityAndState = Zip2City.GetDefaultCityState(zip);
            Telemetry.ZipLookups.Add(1, new KeyValuePair<string, object?>("outcome", cityAndState is null ? "miss" : "hit"));
            logger.ZipLookup(zip, cityAndState is not null);
            if (cityAndState is null) return Results.NotFound();

            string city = cityAndState[0];
            string state = cityAndState[1];
            csvData = Encoding.ASCII.GetBytes($"\"{city}\",\"{state}\"");
            break;
    }


    return Results.Bytes(csvData, "text/csv; charset=utf-8");
})
.WithName("GetZip");

app.Run();

static void RecordChaos(ChaosModes mode, string endpoint, ILogger logger)
{
    if (mode == ChaosModes.None) return;

    // Tag the current request span so failures are filterable by mode in the trace view.
    Activity.Current?.SetTag("chaos.mode", mode.ToString());
    Telemetry.ChaosInjected.Add(1, new KeyValuePair<string, object?>("mode", mode.ToString()));
    logger.ChaosApplied(mode, endpoint);
}

static ChaosModes GetEffectiveChaosMode(ChaosModes? code)
{
    if (code is not null) return code.Value;

    // Weighted random
    /*
     * None: 70%
     * InvalidData: 10%
     * Timeout: 10%
     * Exception: 10%
     */
    return Random.Shared.Next(10) switch
    {
        7 => ChaosModes.InvalidData,
        8 => ChaosModes.Timeout,
        9 => ChaosModes.Exception,
        _ => ChaosModes.None,
    };
}

[CsvRecord]
public sealed partial class Person
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string EmailAddress { get; init; }
    public required string ZipCode { get; set; }
}

[JsonSerializable(typeof(ChaosModes))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}

[Flags]
public enum ChaosModes
{
    None = 0b000,

    InvalidData = 0b0001,
    Timeout = 0b0010,
    Exception = 0b0100,

    All = InvalidData | Timeout | Exception
}