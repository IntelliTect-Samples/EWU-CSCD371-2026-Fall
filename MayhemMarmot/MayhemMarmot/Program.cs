using Bogus;
using CsvHelper;
using ShereSoft;
using System.Text;

var builder = WebApplication.CreateSlimBuilder(args);

builder.WebHost.ConfigureKestrel(x =>
{
    x.AllowSynchronousIO = true;
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

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var peopleGroup = app.MapGroup("/people");

peopleGroup.MapGet("/{user}", (string user, Faker<Person> faker) =>
{
    int seed = HashCode.Combine(user);

    return Results.Stream(async stream =>
    {
        faker.UseSeed(seed);
        using CsvWriter csvWriter = CsvWriter.Create(stream);
        var people = faker.Generate(100);
        await csvWriter.WriteRecordsAsync(people);
    });
})
.WithName("GetPeople");

peopleGroup.MapGet("zip/{zip}", (string zip, Faker<Person> faker) =>
{
    var cityAndState = Zip2City.GetDefaultCityState(zip);
    if (cityAndState is null) return Results.NotFound();

    string city = cityAndState[0];
    string state = cityAndState[1];

    byte[] csvData = Encoding.ASCII.GetBytes($"\"{city}\",\"{state}\"");
    return Results.Bytes(csvData, "text/csv; charset=utf-8");
})
.WithName("GetZip");

app.Run();

[CsvRecord]
public sealed partial class Person
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string EmailAddress { get; init; }
    public required string ZipCode { get; init; }
}
