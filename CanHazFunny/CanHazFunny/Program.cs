namespace CanHazFunny;

/// <summary>
/// The main entry point for the application.
/// </summary>
/// <remarks>
/// The <see cref="Program"/> class contains the <see cref="Main(string[])"/> method, which is the entry point of the application. It creates an instance of the <see cref="Jester"/> class and calls its <see cref="Jester.TellJoke"/> method to tell a joke.
/// </remarks>
class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    /// <remarks>
    /// The <see cref="Main(string[])"/> method creates an instance of the <see cref="Jester"/> class, passing in instances of the <see cref="JokeService"/> and <see cref="JokeOutput"/> classes. It then calls the <see cref="Jester.TellJoke"/> method to tell a joke.
    /// </remarks>
    static void Main(string[] args)
    {
        Jester jester = new Jester(
            new JokeService(), 
            new JokeOutput());

        jester.TellJoke();
    }
}
