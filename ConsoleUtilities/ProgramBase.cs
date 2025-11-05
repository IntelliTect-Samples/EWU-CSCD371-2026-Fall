namespace ConsoleUtilities;

public class ProgramBase
{
    /// <summary>
    /// Delegate for writing a line of text. Defaults to Console.WriteLine.
    /// Exposing WriteLine as an Action allows us to redirect output for testing.
    /// Rather than writing directly to the console, we can capture output in memory, 
    /// enabling automated verification.
    /// </summary>
    public Action<string?> WriteLine { get; init; }

    /// <summary>
    /// Delegate for reading a line of text. Defaults to Console.ReadLine.
    /// Using a Func for input lets us replace Console.ReadLine with a stub or mock sequence in tests.
    /// This removes the need for user input during execution.
    /// </summary>
    public Func<string?> ReadLine { get; init; }

    /// <summary>
    /// Default constructor.
    /// Initializes delegates to Console methods.
    /// This ensures the application works normally when run from the command line,
    /// but still allows the delegates to be overridden for testing.
    /// </summary>
    public ProgramBase()
    {
        WriteLine = Console.WriteLine;
        ReadLine = Console.ReadLine;
    }

    /// <summary>
    /// Constructor allowing custom delegates for input and output.
    /// This enables dependency injection at construction time.
    /// This pattern supports unit testing and reuse in other contexts
    /// where you may want to replace console input/output with alternative mechanisms.
    /// </summary>
    /// <param name="writeLine">Delegate for output; must not be null.</param>
    /// <param name="readLine">Delegate for input; must not be null.</param>
    public ProgramBase(Action<string?> writeLine, Func<string?> readLine)
    {
        WriteLine = writeLine ?? throw new ArgumentNullException(nameof(writeLine));
        ReadLine = readLine ?? throw new ArgumentNullException(nameof(readLine));
    }

}
