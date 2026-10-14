namespace CanHazFunny;

internal static class Program
{
    private static void Main(string[] args)
    {
        using HttpClient client = new();

        new Jester(new JokeService(client), new JokeWrite()).TellJoke();
    }
}
