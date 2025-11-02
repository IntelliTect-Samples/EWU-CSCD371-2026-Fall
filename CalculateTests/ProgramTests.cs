using Calculate;

namespace CalculateTests;

[TestClass]
public class ProgramTests
{
    [TestMethod]
    public void ProgramConstructor_DefaultsToConsole_Success()
    {
        //Arrange
        Program p = new();
        //Assert
        Assert.AreEqual(p.WriteLine, Console.WriteLine);
        Assert.AreEqual(p.ReadLine, Console.ReadLine);
    }
    [TestMethod]
    public void Write_Custom_Success()
    {
        //Arrange
        string text = "ABC";
        string output = null;
        Program p = new()
        {
            WriteLine = (string input) => { output = input; }
        };
        //Act
        p.WriteLine(text);

        //Assert
        Assert.AreEqual(output, text);
    }
    [TestMethod]
    public void ReadLine_Custom_Success()
    {
        //Arrange
        string text = "ABC";
        Program p = new()
        {
            ReadLine = () => { return text; }
        };
        //Act
        string output = p.ReadLine()!;

        //Assert
        Assert.AreEqual(output, text);
    }
}
