namespace Calculate.Tests;

[TestClass]
public sealed class ProgramTests
{
    [TestMethod]
    public void WriteLine_WithProvidedDelegate_CapturesProvidedValue()
    {
        const string expectedValue = "Foo";
        
        // captures the value of the param passed to the delegate
        string? receivedValue = null;

        Program program = new()
        {
            WriteLine = value => receivedValue = value,
            ReadLine = () => string.Empty
        };
        
        program.WriteLine(expectedValue);
        
        Assert.AreEqual<string?>(expectedValue, receivedValue);
    }

    [TestMethod]
    public void ReadLine_WithProvidedDelegate_ReturnsProvidedValue()
    {
        const string expectedValue = "Foo";
        
        Program program = new()
        {
            WriteLine = _ => { },
            ReadLine = () => expectedValue
        };
        
        string? actualValue = program.ReadLine();
        
        Assert.AreEqual<string?>(expectedValue, actualValue);
    }

    [TestMethod]
    public void WriteLine_WithDefaultConstructor_WritesToConsole()
    {
        Program program = new();
        TextWriter originalOutput = Console.Out;
        using StringWriter output = new();

        try
        {
            Console.SetOut(output);

            program.WriteLine("Foo");

            Assert.AreEqual<string?>($"Foo{Environment.NewLine}", output.ToString());
        }
        finally
        {
            Console.SetOut(originalOutput);
        }
    }

    [TestMethod]
    public void ReadLine_WithDefaultConstructor_ReadsFromConsole()
    {
        Program program = new();
        TextReader originalInput = Console.In;

        try
        {
            Console.SetIn(new StringReader("Foo"));

            string? actualValue = program.ReadLine();

            Assert.AreEqual<string?>("Foo", actualValue);
        }
        finally
        {
            Console.SetIn(originalInput);
        }
    }

    [TestMethod]
    public void Run_WithValidCalculation_WritesResult()
    {
        List<string?> actualOutput = [];
        Program program = new()
        {
            WriteLine = value => actualOutput.Add(value),
            ReadLine = () => "3 + 4"
        };

        program.Run();

        Assert.AreEqual(2, actualOutput.Count);
        Assert.AreEqual<string?>("Enter a calculation (e.g., 3 + 4):", actualOutput[0]);
        Assert.AreEqual<string?>("7", actualOutput[1]);
    }

    [TestMethod]
    public void Run_WithInvalidCalculation_WritesErrorMessage()
    {
        List<string?> actualOutput = [];
        Program program = new()
        {
            WriteLine = value => actualOutput.Add(value),
            ReadLine = () => "invalid"
        };

        program.Run();

        Assert.AreEqual(2, actualOutput.Count);
        Assert.AreEqual<string?>("Enter a calculation (e.g., 3 + 4):", actualOutput[0]);
        Assert.AreEqual<string?>("Invalid calculation.", actualOutput[1]);
    }
}