namespace CanHazFunny;

class Program
{
    static void Main(string[] args)
    {
        //tell a joke
        Jester jester = new(new JokeService(), new JokeConsoleOutput());
        jester.TellJoke();
    }
}
