using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculate.Tests;

[TestClass]
public class ProgramTests
{
    [TestMethod]
    public void DefaultConstructorDefaultsToConsoleDelegates()
    {
        Program program = new();

        Assert.IsNotNull(program.WriteLine);
        Assert.IsNotNull(program.ReadLine);
    }

    [TestMethod]
    public void WriteLineAndReadLineSetAtConstructionAreInvoked()
    {
        List<string?> written = [];
        Program program = new()
        {
            WriteLine = written.Add,
            ReadLine = () => "typed input"
        };

        program.WriteLine("hello");
        string? read = program.ReadLine();

        Assert.AreEqual<int>(1, written.Count);
        Assert.AreEqual<string?>("hello", written[0]);
        Assert.AreEqual<string?>("typed input", read);
    }

    [TestMethod]
    public void RunCalculatesEachLineUntilBlank()
    {
        Queue<string?> input = new(["3 + 4", "10 / 2", "bogus", "1 / 0", ""]);
        List<string?> output = [];
        Program program = new()
        {
            WriteLine = output.Add,
            ReadLine = input.Dequeue
        };

        program.Run();

        string?[] expected =
        [
            Program.Prompt,
            "7",
            "5",
            "Invalid calculation: 'bogus'",
            "Invalid calculation: '1 / 0'"
        ];
        Assert.AreEqual<int>(expected.Length, output.Count);
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.AreEqual<string?>(expected[i], output[i]);
        }
    }

    [TestMethod]
    public void RunStopsWhenReadLineReturnsNull()
    {
        List<string?> output = [];
        Program program = new()
        {
            WriteLine = output.Add,
            ReadLine = () => null
        };

        program.Run();

        Assert.AreEqual<int>(1, output.Count);
        Assert.AreEqual<string?>(Program.Prompt, output[0]);
    }
}
