namespace PrincessBrideTrivia;

public class Program
{
    public static void Main(string[] args)
    {
        int userChoice = GetQuizInputFromUser();
        
        string filePath = GetFilePathForChoice(userChoice);
        Question[] questions = LoadQuestions(filePath);

        int numberCorrect = 0;
        for (int i = 0; i < questions.Length; i++)
        {
            bool result = AskQuestion(questions[i]);
            if (result)
            {
                numberCorrect++;
            }
        }
        Console.WriteLine("You got " + GetPercentCorrect(numberCorrect, questions.Length) + " correct");
    }

    /// <summary>
    /// Prompts the user to Select quiz 1 or 2.
    /// Continues prompting until valid input is received.
    /// </summary>
    /// <returns>
    /// the int value representing the selected quiz.
    /// </returns>
    public static int GetQuizInputFromUser()
    {
        while (true)
        {
            Console.WriteLine("Would you like to take quiz one, or quiz two?");
            Console.Write("Enter 1 or 2: ");
            
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    return 1;
                
                case "2":
                    return 2;
                
                default:
                    Console.WriteLine("Invalid input. Please enter a 1 or a 2.");
                    break;
            }
        }
    }
    
    /// <summary>
    /// Returns the file path for the associated quiz.
    /// Use <see cref="GetQuizInputFromUser"/> to obtain this int value. 
    /// </summary>
    /// <param name="choice">
    /// The int value representing selected quiz.
    /// </param>
    /// <returns>
    /// The file path for the selected quiz.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="choice"/> does not match a valid quiz option.
    /// </exception>
    public static string GetFilePathForChoice(int choice)
    {
        return choice switch
        {
            1 => "Trivia.txt",
            2 => "Trivia2.txt",
            _ => throw new ArgumentException("Invalid choice.")
        };
    }

    public static string GetPercentCorrect(int numberCorrectAnswers, int numberOfQuestions)
    {
        return Math.Round((double)numberCorrectAnswers / numberOfQuestions * 100, 2) + "%";
    }

    public static bool AskQuestion(Question question)
    {
        DisplayQuestion(question);

        string userGuess = GetGuessFromUser();
        return DisplayResult(userGuess, question);
    }

    public static string GetGuessFromUser()
    {
        return Console.ReadLine();
    }

    public static bool DisplayResult(string userGuess, Question question)
    {
        if (userGuess == question.CorrectAnswerIndex)
        {
            Console.WriteLine("Correct");
            return true;
        }

        Console.WriteLine("Incorrect");
        return false;
    }

    public static void DisplayQuestion(Question question)
    {
        Console.WriteLine("Question: " + question.Text);
        for (int i = 0; i < question.Answers.Length; i++)
        {
            Console.WriteLine((i + 1) + ": " + question.Answers[i]);
        }
    }

    public static Question[] LoadQuestions(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);

        Question[] questions = new Question[lines.Length / 5];
        for (int i = 0; i < questions.Length; i++)
        {
            int lineIndex = i * 5;
            string questionText = lines[lineIndex];

            string answer1 = lines[lineIndex + 1];
            string answer2 = lines[lineIndex + 2];
            string answer3 = lines[lineIndex + 3];

            string correctAnswerIndex = lines[lineIndex + 4];

            Question question = new();
            question.Text = questionText;
            question.Answers = new string[3];
            question.Answers[0] = answer1;
            question.Answers[1] = answer2;
            question.Answers[2] = answer3;
            question.CorrectAnswerIndex = correctAnswerIndex;

            questions[i] = question;
        }
        return questions;
    }
}
