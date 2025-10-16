namespace CanHazFunny;

public interface IOutputService
{
    /// <summary>
    /// Writes a joke to the output destination.
    /// </summary>
    /// <param name="joke">The joke text to write. Can be empty string.</param>
    void WriteJoke(string joke);
}