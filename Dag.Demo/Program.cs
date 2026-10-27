namespace Dag.Demo;

public static class Program
{
    public static void Main()
    {
        Dag<string> courses = new();
        string[] courseNames = ["CSCD 211", "CSCD 300", "CSCD 320", "CSCD 371"];

        foreach (string course in courseNames)
        {
            courses.AddNode(course);
        }

        courses.AddDependency("CSCD 211", "CSCD 300");
        courses.AddDependency("CSCD 211", "CSCD 320");
        courses.AddDependency("CSCD 300", "CSCD 371");
        courses.AddDependency("CSCD 320", "CSCD 371");

        foreach (string course in courseNames)
        {
            string[] prerequisites = courses.GetDependencies(course);
            string description = prerequisites.Length == 0 ? "None" : string.Join(", ", prerequisites);
            Console.WriteLine($"{course} prerequisites: {description}");
        }

        try
        {
            courses.AddDependency("CSCD 371", "CSCD 211");
        }
        catch (InvalidOperationException exception)
        {
            Console.WriteLine($"CSCD 371 → CSCD 211 rejected: {exception.Message}");
        }

        Console.WriteLine();
        Console.Write(courses.ToMermaid());
    }
}
