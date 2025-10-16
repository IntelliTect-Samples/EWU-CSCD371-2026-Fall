namespace CanHazFunny;

sealed class Program
{
    static void Main()
    {
        //Feel free to use your own setup here - this is just provided as an example
        new Jester(new JokeOutput(), new JokeService()).TellJoke();
    }
}
