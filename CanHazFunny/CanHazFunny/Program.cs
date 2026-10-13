namespace CanHazFunny;

internal sealed class Program
{
    static void Main(string[] args)
    {
        //tell a joke
        Jester jester = new(new JokeService(), new JokeConsoleOutput());
        jester.TellJoke();
    }
}
