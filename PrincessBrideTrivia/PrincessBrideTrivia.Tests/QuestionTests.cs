namespace PrincessBrideTrivia.Tests;

[TestClass]
public class QuestionTests
{
    [TestMethod]
    [DataRow("1", true)]
    [DataRow(" 1 ", true)]
    [DataRow("\t1\t", true)]
    [DataRow("2", false)]
    [DataRow(" 2 ", false)]
    [DataRow("", false)]
    [DataRow("   ", false)]
    [DataRow(null, false)]
    public void IsCorrectAnswer_UserGuess_ReturnsExpectedResult(string userGuess, bool expectedResult)
    {
        // Arrange
        Question question = new() { CorrectAnswerIndex = "1" };

        // Act
        bool actualResult = question.IsCorrectAnswer(userGuess);

        // Assert
        Assert.AreEqual(expectedResult, actualResult);
    }
}
