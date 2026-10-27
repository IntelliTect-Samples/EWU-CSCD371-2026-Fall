int score = 85;

string grade = score switch
{
    >= 90 and <= 100 => "A",
    >= 80 and < 90 => "B",
    _ => "Other"
};

Console.WriteLine(grade);
