namespace ForceConnector.Core.Models;

public sealed record CsvParseResult
{
    public IReadOnlyList<Person> People { get; init; } = [];
    public IReadOnlyList<Rejection> Rejections { get; init; } = [];
}
