namespace Calculate.Tests;

[TestClass]
public sealed class ProgramTests
{
    [TestMethod]
    public void Run_ValidAndInvalidInput_WritesResultsUntilBlankLine()
    {
        Queue<string?> inputs = new(["3 + 4", "bad", "10 / 2", ""]);
        List<string?> output = [];
        Program program = new()
        {
            WriteLine = output.Add,
            ReadLine = inputs.Dequeue
        };

        program.Run();

        CollectionAssert.AreEqual(new string?[] { "7", "Invalid calculation.", "5" }, output.Skip(1).ToArray());
    }

    [TestMethod]
    public void Run_EndOfInput_Exits()
    {
        List<string?> output = [];
        Program program = new()
        {
            WriteLine = output.Add,
            ReadLine = () => null
        };

        program.Run();

        Assert.AreEqual<int>(1, output.Count);
    }
}
