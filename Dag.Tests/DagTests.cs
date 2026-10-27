using Microsoft.VisualStudio.TestTools.UnitTesting;
using Graph = global::Dag.Dag<string>;

namespace Dag.Tests;

[TestClass]
public class DagTests
{
    // Reusable arrays for expected test results
    private static readonly string[] Course211 = ["CSCD 211"];
    private static readonly string[] Courses300And320 =
        ["CSCD 300", "CSCD 320"];
    private static readonly string[] Course371 = ["CSCD 371"];
    private static readonly string[] NodeB = ["B"];

    // Create the required course prerequisite graph
    private static Graph CreateCourseGraph()
    {
        Graph graph = new();

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
        Graph graph = new();

        graph.AddNode("A");
        graph.AddNode("B");
        graph.AddDependency("A", "B");
        graph.AddNode("A");

        Assert.HasCount(1, graph.GetDependents("A"));
        Assert.HasCount(1, graph.GetDependencies("B"));
    }

    [TestMethod]
    public void CourseGraph_HasCorrectDependencies()
    {
        Graph graph = CreateCourseGraph();

        Assert.AreSequenceEqual(
            Course211,
            graph.GetDependencies("CSCD 300"),
            SequenceOrder.InAnyOrder);

        Assert.AreSequenceEqual(
            Courses300And320,
            graph.GetDependencies("CSCD 371"),
            SequenceOrder.InAnyOrder);

        Assert.AreSequenceEqual(
            Courses300And320,
            graph.GetDependents("CSCD 211"),
            SequenceOrder.InAnyOrder);

        Assert.AreSequenceEqual(
            Course371,
            graph.GetDependents("CSCD 320"),
            SequenceOrder.InAnyOrder);
    }

    [TestMethod]
    public void AddDependency_DoesNotCreateDuplicateEdges()
    {
        Graph graph = new();

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
        Graph graph = CreateCourseGraph();

        var exception = Assert.ThrowsExactly<InvalidOperationException>(
            () => graph.AddDependency("CSCD 371", "CSCD 211"));

        Assert.AreEqual(
            "Adding this dependency would create a cycle.",
            exception.Message);

        // Verify the rejected edge was not added
        Assert.IsEmpty(graph.GetDependents("CSCD 371"));
    }

    [TestMethod]
    public void AddDependency_RejectsSelfCycle()
    {
        Graph graph = new();

        graph.AddNode("A");

        Assert.ThrowsExactly<InvalidOperationException>(
            () => graph.AddDependency("A", "A"));
    }

    [TestMethod]
    public void AddDependency_RequiresExistingNodes()
    {
        Graph graph = new();

        graph.AddNode("A");

        Assert.ThrowsExactly<ArgumentException>(
            () => graph.AddDependency("A", "Missing"));

        Assert.ThrowsExactly<ArgumentException>(
            () => graph.AddDependency("Missing", "A"));
    }

    [TestMethod]
    public void Query_RejectsMissingNodes()
    {
        Graph graph = new();

        Assert.ThrowsExactly<ArgumentException>(
            () => graph.GetDependencies("Missing"));

        Assert.ThrowsExactly<ArgumentException>(
            () => graph.GetDependents("Missing"));
    }

    [TestMethod]
    public void Queries_ReturnOnlyDirectConnections()
    {
        Graph graph = new();

        graph.AddNode("A");
        graph.AddNode("B");
        graph.AddNode("C");

        graph.AddDependency("A", "B");
        graph.AddDependency("B", "C");

        Assert.AreSequenceEqual(
            NodeB,
            graph.GetDependencies("C"),
            SequenceOrder.InAnyOrder);

        Assert.AreSequenceEqual(
            NodeB,
            graph.GetDependents("A"),
            SequenceOrder.InAnyOrder);
    }
}