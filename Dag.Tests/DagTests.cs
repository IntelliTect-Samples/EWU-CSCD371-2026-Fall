using Microsoft.VisualStudio.TestTools.UnitTesting;
using Graph = global::Dag.Dag<string>;

namespace Dag.Tests;

[TestClass]
public class DagTests
{
    // Create the course graph required by the assignment
    private static global::Dag.Dag<string> CreateCourseGraph()
    {
        var graph = new global::Dag.Dag<string>();

        graph.AddNode("CSCD 211");
        graph.AddNode("CSCD 300");
        graph.AddNode("CSCD 320");
        graph.AddNode("CSCD 371");

        graph.AddDependency("CSCD 211", "CSCD 300");
        graph.AddDependency("CSCD 211", "CSCD 320");
        graph.AddDependency("CSCD 300", "CSCD 371");
        graph.AddDependency("CSCD 320", "CSCD 371");

        return graph;
    }

    [TestMethod]
    public void AddNode_DoesNotCreateDuplicates()
    {
        var graph = new global::Dag.Dag<string>();

        graph.AddNode("A");
        graph.AddNode("B");
        graph.AddDependency("A", "B");

        // Adding A again should not change the graph
        graph.AddNode("A");

        Assert.HasCount(1, graph.GetDependents("A"));
        Assert.HasCount(1, graph.GetDependencies("B"));
    }

    [TestMethod]
    public void CourseGraph_HasCorrectDependencies()
    {
        var graph = CreateCourseGraph();

        Assert.AreSequenceEqual(
            new[] { "CSCD 211" }, graph.GetDependencies("CSCD 300").ToArray(), Microsoft.VisualStudio.TestTools.UnitTesting.SequenceOrder.InAnyOrder);

        Assert.AreSequenceEqual(
            new[] { "CSCD 300", "CSCD 320" }, graph.GetDependencies("CSCD 371").ToArray(), Microsoft.VisualStudio.TestTools.UnitTesting.SequenceOrder.InAnyOrder);

        Assert.AreSequenceEqual(
            new[] { "CSCD 300", "CSCD 320" }, graph.GetDependents("CSCD 211").ToArray(), Microsoft.VisualStudio.TestTools.UnitTesting.SequenceOrder.InAnyOrder);

        Assert.AreSequenceEqual(
            new[] { "CSCD 371" }, graph.GetDependents("CSCD 320").ToArray(), Microsoft.VisualStudio.TestTools.UnitTesting.SequenceOrder.InAnyOrder);
    }

    [TestMethod]
    public void AddDependency_DoesNotCreateDuplicateEdges()
    {
        var graph = new global::Dag.Dag<string>();

        graph.AddNode("A");
        graph.AddNode("B");

        graph.AddDependency("A", "B");
        graph.AddDependency("A", "B");

        Assert.HasCount(1, graph.GetDependents("A"));
        Assert.HasCount(1, graph.GetDependencies("B"));
    }

    [TestMethod]
    public void AddDependency_RejectsIndirectCycle()
    {
        var graph = CreateCourseGraph();

        var exception = Assert.ThrowsExactly<InvalidOperationException>(
            () => graph.AddDependency("CSCD 371", "CSCD 211"));

        Assert.AreEqual(
            "Adding this dependency would create a cycle.",
            exception.Message);

        // Rejected edge should not have been added
        Assert.IsEmpty(graph.GetDependents("CSCD 371"));
    }

    [TestMethod]
    public void AddDependency_RejectsSelfCycle()
    {
        var graph = new global::Dag.Dag<string>();
        graph.AddNode("A");

        Assert.ThrowsExactly<InvalidOperationException>(
            () => graph.AddDependency("A", "A"));
    }

    [TestMethod]
    public void AddDependency_RequiresExistingNodes()
    {
        var graph = new global::Dag.Dag<string>();
        graph.AddNode("A");

        Assert.ThrowsExactly<ArgumentException>(
            () => graph.AddDependency("A", "Missing"));

        Assert.ThrowsExactly<ArgumentException>(
            () => graph.AddDependency("Missing", "A"));
    }

    [TestMethod]
    public void Query_RejectsMissingNodes()
    {
        var graph = new global::Dag.Dag<string>();

        Assert.ThrowsExactly<ArgumentException>(
            () => graph.GetDependencies("Missing"));

        Assert.ThrowsExactly<ArgumentException>(
            () => graph.GetDependents("Missing"));
    }

    [TestMethod]
    public void Queries_ReturnOnlyDirectConnections()
    {
        var graph = new global::Dag.Dag<string>();

        graph.AddNode("A");
        graph.AddNode("B");
        graph.AddNode("C");

        graph.AddDependency("A", "B");
        graph.AddDependency("B", "C");

        string[] expected = ["B"];

        Assert.AreSequenceEqual(
            expected,
            graph.GetDependencies("C"),
            SequenceOrder.InAnyOrder);

        Assert.AreSequenceEqual(
            expected,
            graph.GetDependents("A"),
            SequenceOrder.InAnyOrder);
    }
}
