namespace CanHazFunny;

public interface IJokeService
{
    /// <summary>
    /// Gets a random joke from the joke service.
    /// </summary>
    /// <returns>A string containing a random joke. If no jokes are available, returns an empty string.</returns>
    string GetJoke();
}