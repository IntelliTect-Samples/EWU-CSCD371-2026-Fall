using System;

namespace CanHazFunny;

internal sealed class Program
{
    public static void Main(string[] args)
    {
        new Jester(new ConsoleOutput(), new JokeService()).TellJoke();
    }
}