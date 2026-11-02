using ConsoleUtilities;

namespace Calculate.Tests;

[TestClass]
public class ProgramTests
{
    [TestMethod]
    public void Program_InheritsFromProgramBase()
    {
        Program program = new();

        Assert.AreEqual<bool>(true, program is ProgramBase);
    }

    [TestMethod]
    public void Run_ValidThenInvalidInput_WritesResultThenError()
    {
        Queue<string?> inputs = new(["3 + 4", "3+4", ""]);
        List<string> outputs = [];
        Program program = new() { ReadLine = inputs.Dequeue, WriteLine = outputs.Add };

        program.Run();

        Assert.AreEqual<int>(3, outputs.Count);
        Assert.AreEqual<string>("3 + 4 = 7", outputs[1]);
        Assert.AreEqual<string>("Invalid calculation: '3+4'", outputs[2]);
    }

    [TestMethod]
    public void Run_EndOfInput_StopsAfterPrompt()
    {
        List<string> outputs = [];
        Program program = new() { ReadLine = () => null, WriteLine = outputs.Add };

        program.Run();

        Assert.AreEqual<int>(1, outputs.Count);
    }
}