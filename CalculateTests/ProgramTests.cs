namespace Calculate.Tests;
 
[TestClass]
public class ProgramTests
{
    [TestMethod]
    public void DefaultConstructorUsesConsoleDelegates()
    {
        Program program = new();
        Assert.IsNotNull(program.WriteLine);
        Assert.IsNotNull(program.ReadLine);
    }
 
    [TestMethod]
    public void InitPropertiesAreInvoked()
    {
        List<string?> written = [];
        Program program = new()
        {
            WriteLine = written.Add,
            ReadLine = static () => "hello",
        };
 
        program.WriteLine("output");
        string? read = program.ReadLine();

        Assert.HasCount(1, written);
        Assert.AreEqual<string?>("output", written[0]);
        Assert.AreEqual<string?>("hello", read);
    }
 
    [TestMethod]
    public void RunReportsDivisionByZeroAndContinues()
    {
        Queue<string?> input = new(["4 / 0", "1 + 1", ""]);
        List<string?> output = [];
        Program program = new() { WriteLine = output.Add, ReadLine = input.Dequeue };
 
        program.Run();
 
        Assert.HasCount(3, output);
        Assert.AreEqual<string?>("1 + 1 = 2", output[2]);
    }
 
    [TestMethod]
    public void RunCalculatesInputUntilBlankLine()
    {
        Queue<string?> input = new(["3 + 4", "oops", "10 / 2", ""]);
        List<string?> output = [];
        Program program = new() { WriteLine = output.Add, ReadLine = input.Dequeue };
 
        program.Run();
 
        Assert.HasCount(4, output);
        Assert.AreEqual<string?>("3 + 4 = 7", output[1]);
        Assert.AreEqual<string?>("Invalid calculation. Use: <integer> <+|-|*|/> <integer>", output[2]);
        Assert.AreEqual<string?>("10 / 2 = 5", output[3]);
    }
}
