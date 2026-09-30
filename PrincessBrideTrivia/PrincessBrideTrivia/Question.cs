namespace PrincessBrideTrivia;

public class Question
{
    public string Text { get; set; }
    public string[] Answers { get; set; }
    public string CorrectAnswerIndex { get; set; }

    public bool IsCorrectAnswer(string userGuess)
    {
        return !string.IsNullOrWhiteSpace(userGuess)
            && userGuess.Trim() == CorrectAnswerIndex;
    }
}
