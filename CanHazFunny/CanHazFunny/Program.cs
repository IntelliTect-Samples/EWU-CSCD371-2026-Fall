namespace CanHazFunny;

static class Program
{
    static void Main()
    {
        new Jester(new JokeService(), new CJokeOutput()).TellJoke();
    }
}
