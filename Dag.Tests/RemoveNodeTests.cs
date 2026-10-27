using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dag.Tests;

[TestClass]
public class RemoveNodeTests
{
    [TestMethod]
    public void RemoveNode_NullValue_ThrowsArgumentNullException()
    {
        Dag<string> graph = new();

        Assert.ThrowsExactly<ArgumentNullException>(() => graph.RemoveNode(null!));
    }

    [TestMethod]
    public void RemoveNode_EmptyGraph_ReturnsFalse()
    {
        Dag<int> graph = new();

        Assert.IsFalse(graph.RemoveNode(1));
    }

    [TestMethod]
    public void RemoveNode_MissingNode_PreservesOtherNodesAndEdges()
    {
        Dag<int> graph = CreateGraph();

        Assert.IsFalse(graph.RemoveNode(6));

        CollectionAssert.AreEquivalent(new[] { 1, 2 }, graph.GetDependencies(3));
        CollectionAssert.AreEquivalent(new[] { 4, 5 }, graph.GetDependents(3));
    }

    [TestMethod]
    public void RemoveNode_IsolatedNode_PreservesOtherNodesAndEdges()
    {
        Dag<int> graph = CreateGraph();
        graph.AddNode(6);

        Assert.IsTrue(graph.RemoveNode(6));

        Assert.ThrowsExactly<ArgumentException>(() => graph.GetDependencies(6));
        CollectionAssert.AreEquivalent(new[] { 1, 2 }, graph.GetDependencies(3));
        CollectionAssert.AreEquivalent(new[] { 4, 5 }, graph.GetDependents(3));
    }

    [TestMethod]
    public void RemoveNode_ConnectedNode_RemovesAllItsEdgesAndPreservesUnrelatedEdge()
    {
        Dag<int> graph = CreateGraph();

        Assert.IsTrue(graph.RemoveNode(3));

        Assert.ThrowsExactly<ArgumentException>(() => graph.GetDependencies(3));
        Assert.ThrowsExactly<ArgumentException>(() => graph.GetDependents(3));
        CollectionAssert.AreEquivalent(new[] { 2 }, graph.GetDependents(1));
        CollectionAssert.AreEquivalent(new[] { 1 }, graph.GetDependencies(2));
        Assert.IsEmpty(graph.GetDependents(2));
        Assert.IsEmpty(graph.GetDependencies(4));
        Assert.IsEmpty(graph.GetDependencies(5));
        Assert.IsEmpty(graph.GetDependents(4));
        Assert.IsEmpty(graph.GetDependents(5));
        Assert.IsFalse(graph.RemoveNode(3));
    }

    [TestMethod]
    public void RemoveNode_Root_RemovesOutgoingEdges()
    {
        Dag<int> graph = CreateGraph();

        Assert.IsTrue(graph.RemoveNode(1));

        Assert.IsEmpty(graph.GetDependencies(2));
        CollectionAssert.AreEquivalent(new[] { 2 }, graph.GetDependencies(3));
        CollectionAssert.AreEquivalent(new[] { 4, 5 }, graph.GetDependents(3));
    }

    [TestMethod]
    public void RemoveNode_Leaf_RemovesIncomingEdges()
    {
        Dag<int> graph = CreateGraph();

        Assert.IsTrue(graph.RemoveNode(4));

        CollectionAssert.AreEquivalent(new[] { 5 }, graph.GetDependents(3));
        CollectionAssert.AreEquivalent(new[] { 1, 2 }, graph.GetDependencies(3));
    }

    [TestMethod]
    public void RemoveNode_ThenReadd_StartsWithoutOldEdges()
    {
        Dag<int> graph = CreateGraph();
        graph.RemoveNode(3);

        Assert.IsTrue(graph.AddNode(3));
        Assert.IsEmpty(graph.GetDependencies(3));
        Assert.IsEmpty(graph.GetDependents(3));
        Assert.IsTrue(graph.AddDependency(4, 3));
        Assert.IsTrue(graph.AddDependency(3, 1));

        CollectionAssert.AreEquivalent(new[] { 4 }, graph.GetDependencies(3));
        CollectionAssert.AreEquivalent(new[] { 1 }, graph.GetDependents(3));
    }

    [TestMethod]
    public void RemoveNode_AllNodes_LeavesReusableEmptyGraph()
    {
        Dag<int> graph = CreateGraph();
        for (int value = 1; value <= 5; value++)
        {
            Assert.IsTrue(graph.RemoveNode(value));
        }

        Assert.AreEqual($"flowchart TD{Environment.NewLine}", graph.ToMermaid());
        Assert.IsTrue(graph.AddNode(1));
        Assert.IsTrue(graph.AddNode(2));
        Assert.IsTrue(graph.AddDependency(2, 1));
    }

    private static Dag<int> CreateGraph()
    {
        Dag<int> graph = new();
        for (int value = 1; value <= 5; value++)
        {
            graph.AddNode(value);
        }

        graph.AddDependency(1, 2);
        graph.AddDependency(1, 3);
        graph.AddDependency(2, 3);
        graph.AddDependency(3, 4);
        graph.AddDependency(3, 5);
        return graph;
    }
}
