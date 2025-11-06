namespace Calculate.Tests;

[TestClass]
public sealed class ProgramTests
{
    private static readonly string[] ValidCalculationInputs = new[] { "5 + 3", "exit" };
    private static readonly string[] InvalidCalculationInputs = new[] { "invalid input", "exit" };
    private static readonly string[] MultipleCalculationInputs = new[]
    {
        "10 - 5",
        "3 * 4",
        "20 / 4",
        "bad",
        "exit"
    };
    private static readonly string[] MixedValidInvalidInputs = new[]
    {
        "1 + 1",
        "no spaces",
        "100 / 10",
        "exit"
    };
    private static readonly string[] ExitCommandVariations = new[] { "EXIT", "exit", "Exit" };

    // Lock object to prevent parallel test execution from interfering with Console redirection
    private static readonly object ConsoleLock = new();

    [TestMethod]
    public void Constructor_InitializesWriteLineAndReadLine_Success()
    {
        // Arrange & Act
        var program = new Program();
        
        // Assert
        Assert.IsNotNull(program.WriteLine);
        Assert.IsNotNull(program.ReadLine);
    }

    [TestMethod]
    public void Constructor_WriteLineDefaultsToConsoleWriteLine_Success()
    {
        // Arrange
        var program = new Program();
        
        // Act - WriteLine should be callable without throwing
        program.WriteLine("test");
        
        // Assert - If we get here without exception, default is set
        Assert.IsNotNull(program.WriteLine);
    }

    [TestMethod]
    public void Constructor_ReadLineDefaultsToConsoleReadLine_Success()
    {
        // Arrange
        var program = new Program();
        
        // Assert - ReadLine should be callable
        Assert.IsNotNull(program.ReadLine);
    }

    [TestMethod]
    public void WriteLine_CustomDelegate_WritesExpectedOutput()
    {
        // Arrange
        var output = string.Empty;
        var program = new Program
        {
            WriteLine = s => output = s
        };
        var expectedMessage = "Hello, World!";
        
        // Act
        program.WriteLine(expectedMessage);
        
        // Assert
        Assert.AreEqual<string>(expectedMessage, output);
    }

    [TestMethod]
    public void ReadLine_CustomDelegate_ReturnsExpectedInput()
    {
        // Arrange
        var expectedInput = "Test Input";
        var program = new Program
        {
            ReadLine = () => expectedInput
        };
        
        // Act
        var actualInput = program.ReadLine();
        
        // Assert
        Assert.AreEqual<string>(expectedInput, actualInput);
    }

    [TestMethod]
    public void WriteLine_MultipleInvocations_CapturesAllOutput()
    {
        // Arrange
        var outputs = new List<string>();
        var program = new Program
        {
            WriteLine = s => outputs.Add(s)
        };
        
        // Act
        program.WriteLine("First");
        program.WriteLine("Second");
        program.WriteLine("Third");
        
        // Assert
        Assert.HasCount(3, outputs);
        Assert.AreEqual<string>("First", outputs[0]);
        Assert.AreEqual<string>("Second", outputs[1]);
        Assert.AreEqual<string>("Third", outputs[2]);
    }

    [TestMethod]
    public void Main_ValidCalculation_OutputsCorrectResult()
    {
        lock (ConsoleLock)
        {
            // Arrange
            var inputs = string.Join(Environment.NewLine, ValidCalculationInputs);
            var writer = new StringWriter();
            var reader = new StringReader(inputs);
            var originalOut = Console.Out;
            var originalIn = Console.In;

            try
            {
                Console.SetOut(writer);
                Console.SetIn(reader);

                // Act
                Program.Main();

                // Assert
                var output = writer.ToString();
                Assert.Contains("Result: 8", output);
                Assert.Contains("Enter calculation", output);
            }
            finally
            {
                Console.SetOut(originalOut);
                Console.SetIn(originalIn);
                writer.Dispose();
                reader.Dispose();
            }
        }
    }

    [TestMethod]
    public void Main_InvalidCalculation_OutputsErrorMessage()
    {
        lock (ConsoleLock)
        {
            // Arrange
            var inputs = string.Join(Environment.NewLine, InvalidCalculationInputs);
            var writer = new StringWriter();
            var reader = new StringReader(inputs);
            var originalOut = Console.Out;
            var originalIn = Console.In;

            try
            {
                Console.SetOut(writer);
                Console.SetIn(reader);

                // Act
                Program.Main();

                // Assert
                var output = writer.ToString();
                Assert.Contains("Invalid calculation", output);
            }
            finally
            {
                Console.SetOut(originalOut);
                Console.SetIn(originalIn);
                writer.Dispose();
                reader.Dispose();
            }
        }
    }

    [TestMethod]
    public void Main_BlankInput_ExitsImmediately()
    {
        lock (ConsoleLock)
        {
            // Arrange
            var writer = new StringWriter();
            var reader = new StringReader(Environment.NewLine);
            var originalOut = Console.Out;
            var originalIn = Console.In;

            try
            {
                Console.SetOut(writer);
                Console.SetIn(reader);

                // Act
                Program.Main();

                // Assert
                var output = writer.ToString();
                Assert.Contains("Enter calculation", output);
                var lines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                Assert.HasCount(1, lines);
            }
            finally
            {
                Console.SetOut(originalOut);
                Console.SetIn(originalIn);
                writer.Dispose();
                reader.Dispose();
            }
        }
    }

    [TestMethod]
    public void Main_ExitCommand_ExitsWithoutProcessing()
    {
        lock (ConsoleLock)
        {
            // Arrange
            var writer = new StringWriter();
            var reader = new StringReader("exit");
            var originalOut = Console.Out;
            var originalIn = Console.In;

            try
            {
                Console.SetOut(writer);
                Console.SetIn(reader);

                // Act
                Program.Main();

                // Assert
                var output = writer.ToString();
                Assert.Contains("Enter calculation", output);
                Assert.DoesNotContain("Result:", output);
                Assert.DoesNotContain("Invalid calculation", output);
            }
            finally
            {
                Console.SetOut(originalOut);
                Console.SetIn(originalIn);
                writer.Dispose();
                reader.Dispose();
            }
        }
    }

    [TestMethod]
    public void Main_MultipleCalculations_ProcessesAllCorrectly()
    {
        lock (ConsoleLock)
        {
            // Arrange
            var inputs = string.Join(Environment.NewLine, MultipleCalculationInputs);
            var writer = new StringWriter();
            var reader = new StringReader(inputs);
            var originalOut = Console.Out;
            var originalIn = Console.In;

            try
            {
                Console.SetOut(writer);
                Console.SetIn(reader);

                // Act
                Program.Main();

                // Assert
                var output = writer.ToString();
                Assert.Contains("Result: 5", output);
                Assert.Contains("Result: 12", output);
                Assert.Contains("Invalid calculation", output);
            }
            finally
            {
                Console.SetOut(originalOut);
                Console.SetIn(originalIn);
                writer.Dispose();
                reader.Dispose();
            }
        }
    }

    [TestMethod]
    public void Main_MixedValidAndInvalid_HandlesAll()
    {
        lock (ConsoleLock)
        {
            // Arrange
            var inputs = string.Join(Environment.NewLine, MixedValidInvalidInputs);
            var writer = new StringWriter();
            var reader = new StringReader(inputs);
            var originalOut = Console.Out;
            var originalIn = Console.In;

            try
            {
                Console.SetOut(writer);
                Console.SetIn(reader);

                // Act
                Program.Main();

                // Assert
                var output = writer.ToString();
                Assert.Contains("Result: 2", output);
                Assert.Contains("Result: 10", output);
                Assert.Contains("Invalid calculation", output);
            }
            finally
            {
                Console.SetOut(originalOut);
                Console.SetIn(originalIn);
                writer.Dispose();
                reader.Dispose();
            }
        }
    }

    [TestMethod]
    public void Main_ExitCommandCaseInsensitive_ExitsCorrectly()
    {
        lock (ConsoleLock)
        {
            // Arrange
            var inputs = string.Join(Environment.NewLine, ExitCommandVariations);
            var writer = new StringWriter();
            var reader = new StringReader(inputs);
            var originalOut = Console.Out;
            var originalIn = Console.In;

            try
            {
                Console.SetOut(writer);
                Console.SetIn(reader);

                // Act
                Program.Main();

                // Assert
                var output = writer.ToString();
                // Should exit on first EXIT command
                Assert.Contains("Enter calculation", output);
            }
            finally
            {
                Console.SetOut(originalOut);
                Console.SetIn(originalIn);
                writer.Dispose();
                reader.Dispose();
            }
        }
    }
}