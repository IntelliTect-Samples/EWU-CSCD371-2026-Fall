namespace CanHazFunny;

class Program
{
    static void Main(string[] args)
    {
        IJokeOutput output = new ConsoleJokeOutput();
        IJokeService jokeService = new JokeService();

        Jester jester = new Jester(output, jokeService);

        jester.TellJoke();
    }
}