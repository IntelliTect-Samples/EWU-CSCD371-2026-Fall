namespace Calculate.Tests;

[TestClass]
public sealed class ProgramTests
{
    [TestMethod]
    public void Program_WriteLineAndReadLine_WorkCorrectly()
    {
        // Arrange
        string expectedOutput = "Hello, World!";
        string expectedInput = "6 + 7";
        string actualOutput = string.Empty;
        string? actualInput = string.Empty;

        Program program = new()
        {
            WriteLine = (output) => actualOutput = output,
            ReadLine = () => expectedInput
        };

        // Act
        program.WriteLine(expectedOutput);
        actualInput = program.ReadLine();

        // Assert
        Assert.AreEqual<string>(expectedOutput, actualOutput);
        Assert.AreEqual<string>(expectedInput, actualInput);

    }

    [TestMethod]
    public void Program_DifferentOutput_SuccessfullyCapturesOutput()
    {
        // Arrange
        string expectedOutput = "6 + 7";
        string actualOutput = string.Empty;

        Program program = new()
        {
            WriteLine = (output) => actualOutput = output
        };

        // Act
        program.WriteLine(expectedOutput);

        // Assert
        Assert.AreEqual<string>(expectedOutput, actualOutput);
    }

    [TestMethod]
    public void Program_DifferentInput_SuccessfullyProvidesInput()
    {
        // Arrange
        string expectedInput = "Goodbye";
        string? actualInput = string.Empty;

        Program program = new()
        {
            ReadLine = () => expectedInput
        };

        // Act
        actualInput = program.ReadLine();

        // Assert
        Assert.AreEqual<string>(expectedInput, actualInput);
    }
}
