using System;

namespace CanHazFunny;

sealed class Program
{
    static void Main(string[] args)
    {
        new Jester(new ConsoleOutputService(), new JokeService()).TellJoke();
    }
}
