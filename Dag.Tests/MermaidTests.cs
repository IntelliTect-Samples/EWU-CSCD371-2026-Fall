using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dag.Tests;

[TestClass]
public class MermaidTests
{
    [TestMethod]
    public void ToMermaid_EmptyGraph_ReturnsFlowchartHeader()
    {
        Dag<string> graph = new();

        Assert.AreEqual($"flowchart TD{Environment.NewLine}", graph.ToMermaid());
    }

    [TestMethod]
    public void ToMermaid_ConnectedAndIsolatedNodes_IncludesEveryNodeAndDirectedEdgeOnce()
    {
        Dag<string> graph = new();
        graph.AddNode("CSCD 211");
        graph.AddNode("CSCD 371");
        graph.AddNode("Isolated");
        graph.AddDependency("CSCD 211", "CSCD 371");
        graph.AddDependency("CSCD 211", "CSCD 371");

        string[] lines = graph.ToMermaid().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        string prerequisiteId = GetNodeId(lines, "CSCD 211");
        string courseId = GetNodeId(lines, "CSCD 371");
        string isolatedId = GetNodeId(lines, "Isolated");

        Assert.AreEqual("flowchart TD", lines[0]);
        Assert.HasCount(5, lines);
        Assert.Contains($"    {prerequisiteId} --> {courseId}", lines);
        Assert.IsFalse(lines.Any(line => line.Contains($"--> {isolatedId}", StringComparison.Ordinal)));
    }

    [TestMethod]
    [DataRow("end")]
    [DataRow("x")]
    [DataRow("A[0] (course)")]
    [DataRow("课程")]
    public void ToMermaid_SpecialNodeNames_UsesGeneratedIdsAndQuotedLabels(string value)
    {
        Dag<string> graph = new();
        graph.AddNode(value);

        string[] lines = graph.ToMermaid().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.HasCount(2, lines);
        Assert.AreEqual($"    node0[\"{value}\"]", lines[1]);
    }

    [TestMethod]
    [DataRow("Say \"hello\"", "Say #quot;hello#quot;")]
    [DataRow("A & B", "A #38; B")]
    [DataRow("<course>", "#60;course#62;")]
    [DataRow("#quot;", "#35;quot;")]
    [DataRow("`course`", "#96;course#96;")]
    [DataRow("first\r\nsecond", "first<br/>second")]
    [DataRow("first\nsecond", "first<br/>second")]
    [DataRow("first\rsecond", "first<br/>second")]
    [DataRow("first\u0085second", "first<br/>second")]
    [DataRow("first\u2028second", "first<br/>second")]
    [DataRow("first\u2029second", "first<br/>second")]
    [DataRow("first\fsecond", "first<br/>second")]
    [DataRow("\"]\nnode99 --> node0", "#quot;]<br/>node99 --#62; node0")]
    public void ToMermaid_CharactersWithSpecialMeaning_EncodesLabelWithoutAddingSyntax(
        string value, string expectedLabel)
    {
        Dag<string> graph = new();
        graph.AddNode(value);

        string[] lines = graph.ToMermaid().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        CollectionAssert.AreEqual(new[] { "flowchart TD", $"    node0[\"{expectedLabel}\"]" }, lines);
    }

    [TestMethod]
    public void ToMermaid_DistinctNodesWithIdenticalLabels_KeepsSeparateIdsAndEdge()
    {
        Dag<Course> graph = new();
        graph.AddNode(new Course(211));
        graph.AddNode(new Course(371));
        graph.AddDependency(new Course(211), new Course(371));

        string[] lines = graph.ToMermaid().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        string[] nodes = lines.Where(line => line.Contains("[\"Course\"]", StringComparison.Ordinal)).ToArray();

        Assert.HasCount(4, lines);
        Assert.HasCount(2, nodes);
        string firstId = nodes[0].Trim().Split('[')[0];
        string secondId = nodes[1].Trim().Split('[')[0];
        Assert.AreNotEqual(firstId, secondId);
        Assert.IsTrue(lines.Contains($"    {firstId} --> {secondId}")
            || lines.Contains($"    {secondId} --> {firstId}"));
    }

    [TestMethod]
    public void ToMermaid_RemovedNode_DoesNotIncludeStaleNodeOrEdges()
    {
        Dag<int> graph = new();
        graph.AddNode(1);
        graph.AddNode(2);
        graph.AddDependency(1, 2);
        graph.RemoveNode(1);

        string[] lines = graph.ToMermaid().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.HasCount(2, lines);
        Assert.AreEqual("    node0[\"2\"]", lines[1]);
    }

    [TestMethod]
    public void ToMermaid_EmptyStringNode_UsesValidBlankLabelAndPreservesEdge()
    {
        Dag<string> graph = new();
        graph.AddNode(string.Empty);
        graph.AddNode("Course");
        graph.AddDependency(string.Empty, "Course");

        string[] lines = graph.ToMermaid().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        string blankId = GetNodeId(lines, " ");
        string courseId = GetNodeId(lines, "Course");

        Assert.HasCount(4, lines);
        Assert.Contains($"    {blankId} --> {courseId}", lines);
    }

    [TestMethod]
    public void ToMermaid_NullStringRepresentation_UsesValidBlankLabel()
    {
        Dag<NodeWithNullLabel> graph = new();
        graph.AddNode(new NodeWithNullLabel());

        string[] lines = graph.ToMermaid().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        CollectionAssert.AreEqual(new[] { "flowchart TD", "    node0[\" \"]" }, lines);
    }

    private static string GetNodeId(string[] lines, string label) =>
        lines.Single(line => line.EndsWith($"[\"{label}\"]", StringComparison.Ordinal)).Trim().Split('[')[0];

    private sealed record class Course(int Number)
    {
        public override string ToString() => "Course";
    }

    private sealed class NodeWithNullLabel
    {
        public override string? ToString() => null;
    }
}
