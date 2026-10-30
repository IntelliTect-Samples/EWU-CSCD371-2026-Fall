// Demo: Dag<T> fan-out/fan-in dependency graph using these course prerequisites: `CSCD 211 → CSCD 300`, `CSCD 211 → CSCD 320`, `CSCD 300 → CSCD 371`, and `CSCD 320 → CSCD 371`. CSCD 211 is a prerequisite for both intermediate courses, and both intermediate courses are prerequisites for CSCD 371.
//Demo mermaid flow chart added for course prerequisite graph.
//Attempt to add `CSCD 371 → CSCD 211' throws an `InvalidOperationException.'
// Run with: dotnet run --project Dag
using Dag;

Dag<string> dag = new();

string[] courses = ["CSCD 211", "CSCD 300", "CSCD 320", "CSCD 371"];
foreach (string course in courses)
    dag.AddNode(course);

dag.AddDependency("CSCD 300", "CSCD 211");   // 211 -> 300
dag.AddDependency("CSCD 320", "CSCD 211");   // 211 -> 320
dag.AddDependency("CSCD 371", "CSCD 300");   // 300 -> 371
dag.AddDependency("CSCD 371", "CSCD 320");   // 320 -> 371

Console.WriteLine("\n---------------------------------------------");
Console.WriteLine("Fan-out/Fan-in CS Prerequisite course graph");
Console.WriteLine("---------------------------------------------");
foreach (string course in courses)
{
    Console.WriteLine(course);
    Console.WriteLine($"  Prerequisite(s): {Format(dag.GetDependencies(course))}");
    Console.WriteLine($"  Dependent course(s): {Format(dag.GetDependents(course))}");
}

Console.WriteLine("-------------------------------------------");
Console.WriteLine("Prerequisite CS Course Mermaid Flow Chart");
Console.WriteLine("-------------------------------------------");
Console.WriteLine(dag.ToMermaid());

Console.WriteLine();
Console.WriteLine("Attempting to add CSCD 371 -> CSCD 211...");
try
{
    dag.AddDependency("CSCD 211", "CSCD 371");
    Console.WriteLine("ERROR: no exception was thrown. Cycle not detected.");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"InvalidOperationException caught: Attempt to add 'CSCD 371 → CSCD 211' is not permitted. ({ex.Message})");
}

static string Format(IReadOnlyCollection<string> items) =>
    items.Count == 0 ? "(none)" : string.Join(", ", items);