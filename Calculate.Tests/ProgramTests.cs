using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;


namespace Calculate.Tests;

[TestClass]
public sealed class ProgramTests
{
    [TestMethod]
    public void Program_Constructor_DefinesNonNullDelegates()
    {
        var prog = new Program();

        // Using "IsNotNull" to avoid compiler warnings
        Assert.IsNotNull(prog.WriteLine);
        Assert.IsNotNull(prog.ReadLine);
    }

    [TestMethod]
    public void Run_DelegateInjection_FunctionsProperly()
    {
        var writeList = new List<string?>();
        var inputs = new Queue<string?>();
        inputs.Enqueue("6 + 7");
        inputs.Enqueue("");
        var prog = new Program(
            writeLine: s => writeList.Add(s),
            readLine: () => inputs.Count > 0 ? inputs.Dequeue() : null
        );
        var calc = new Calculator<int>();

        var exit = prog.Run(calc);

        Assert.AreEqual<int>(0, exit);
        Assert.AreEqual<string>("13", writeList[1]);
    }

    [TestMethod]
    public void Run_InvalidInput_AsksForInputAgain()
    {
        var writeList = new List<string?>();
        var inputs = new Queue<string?>();
        inputs.Enqueue("6+ 7");
        inputs.Enqueue("6 + 7");
        inputs.Enqueue("");
        var prog = new Program(
            writeLine: s => writeList.Add(s),
            readLine: () => inputs.Count > 0 ? inputs.Dequeue() : null
        );
        var calc = new Calculator<int>();

        var exit = prog.Run(calc);

        Assert.AreEqual<int>(0, exit);
        Assert.AreEqual<string>("Invalid input. Try again.", writeList[1]);
        Assert.AreEqual<string>("13", writeList[3]);
    }
    
    [TestMethod]
    public void Run_WhitespaceInput_ExitsLoop()
    {
        var writeList = new List<string?>();
        var inputs = new Queue<string?>();
        inputs.Enqueue("");
        var prog = new Program(
            writeLine: s => writeList.Add(s),
            readLine: () => inputs.Count > 0 ? inputs.Dequeue() : null
        );
        var calc = new Calculator<int>();

        var exit = prog.Run(calc);

        Assert.AreEqual<int>(0, exit);
        Assert.AreEqual<string>("Exiting Calculator.", writeList[1]);
    }
}
