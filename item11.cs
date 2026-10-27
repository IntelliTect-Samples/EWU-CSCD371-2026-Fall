object value = "Kevin";

string description = value switch
{
    string text when text.Length > 3 => "Long text",
    string => "Short text",
    _ => "Other"
};

Console.WriteLine(description);
