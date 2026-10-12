namespace CanHazFunny;

public class Jester
{
    private IJokeService JokeService { get; }

    private IJokeWrite JokeWrite { get; }

    public Jester(IJokeService jokeService, IJokeWrite jokeWrite)
    {
        ArgumentNullException.ThrowIfNull(jokeService);
        ArgumentNullException.ThrowIfNull(jokeWrite);

        JokeService = jokeService;
        JokeWrite = jokeWrite;
    }

    public void TellJoke()
    {
        string joke;
        do
        {
            joke = JokeService.GetJoke();
        }
        while (joke.Contains("Chuck Norris"));

        JokeWrite.WriteJoke(joke);
    }
}
