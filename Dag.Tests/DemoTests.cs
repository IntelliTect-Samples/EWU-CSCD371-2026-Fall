using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dag.Tests;

[TestClass]
public class DemoTests
{
    [TestMethod]
    [ResourceLock(WellKnownResources.Console)]
    public void Main_CoursePrerequisites_PrintsPrerequisitesRejectedCycleAndFlowchart()
    {
        TextWriter originalOutput = Console.Out;
        using StringWriter output = new();
        try
        {
            Console.SetOut(output);

            Demo.Program.Main();
        }
        finally
        {
            Console.SetOut(originalOutput);
        }

        string[] lines = output.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.Contains("CSCD 211 prerequisites: None", lines);
        Assert.Contains("CSCD 300 prerequisites: CSCD 211", lines);
        Assert.Contains("CSCD 320 prerequisites: CSCD 211", lines);
        string finalCourse = Assert.ContainsSingle(lines.Where(
            line => line.StartsWith("CSCD 371 prerequisites: ", StringComparison.Ordinal)));
        string[] prerequisites = finalCourse["CSCD 371 prerequisites: ".Length..].Split(", ");
        CollectionAssert.AreEquivalent(new[] { "CSCD 300", "CSCD 320" }, prerequisites);
        Assert.Contains("CSCD 371 → CSCD 211 rejected: Adding this dependency would create a cycle.", lines);
        Assert.Contains("flowchart TD", lines);
        Assert.HasCount(4, lines.Where(line => line.Contains("[\"CSCD ", StringComparison.Ordinal)));
        Assert.HasCount(4, lines.Where(line => line.Contains(" --> ", StringComparison.Ordinal)));
    }
}
