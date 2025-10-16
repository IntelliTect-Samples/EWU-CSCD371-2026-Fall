using System.Text.Json.Serialization;

namespace CanHazFunny;

public record JokeResponse(
    [property: JsonPropertyName("joke")] string Joke
);
