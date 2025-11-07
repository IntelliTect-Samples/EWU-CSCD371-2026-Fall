using System.Reflection;

namespace Calculate;

public class ProgramTests
{
    // Here because checks were failing due to xUnit2006 warning
    #pragma warning disable xUnit2006
    [Fact]
    public void ReadLine_NonDefaultValueSet_ReturnsExpectedValue()
    {
        Program program = new Program(() => { return "Test Value"; }, (value) => { });
        Assert.Equal<string>("Test Value", program.ReadLine());
    }

    [Fact]
    public void WriteLine_DefaultImplementation_WritesToConsole()
    {
        Program program = new Program();
        StringWriter consoleOutput = new StringWriter();
        TextWriter originalOutput = Console.Out;

        Console.SetOut(consoleOutput);
        program.WriteLine("Test");
        Console.SetOut(originalOutput);
        Assert.Equal<string>("Test" + Environment.NewLine, consoleOutput.ToString());
    }

    [Fact]
    public void WriteLine_NonDefaultImplementation_CallsProvidedAction()
    {
        string? capturedValue = null;
        Program program = new Program(() => { return ""; }, (value) => { capturedValue = value; });

        program.WriteLine("Test Value");

        Assert.Equal<string>("Test Value", capturedValue);
    }
    #pragma warning restore xUnit2006
}