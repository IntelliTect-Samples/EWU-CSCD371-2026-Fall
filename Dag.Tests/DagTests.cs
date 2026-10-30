namespace Dag.Tests;

[TestClass]
public sealed class DagTests
{
    [TestMethod]
    public void AddNode_DuplicateNode_DoesNotDuplicateButDoesNotThrow()
    {
        Dag<string> tester = new();
        tester.AddNode("A");
        tester.AddNode("A");
        Assert.IsEmpty(tester.GetDependents("A"));
    }
    [TestMethod]
    public void AddDependency_ConnectsProperly()
    {
        Dag<string> dag = new();
        dag.AddNode("A");
        dag.AddNode("B");

        dag.AddDependency("A", "B");

        CollectionAssert.Contains(dag.GetDependencies("A").ToList(), "B");  // changed
    }
    [TestMethod]
    public void Edge_Is_VisibleFromBothSides()
    {
        var dag = new Dag<string>();
        dag.AddNode("A");
        dag.AddNode("B");
        dag.AddDependency("B", "A");   // B depends on A

        Assert.Contains("A", dag.GetDependencies("B").ToList());   // changed
        Assert.Contains("B", dag.GetDependents("A").ToList());     // changed
    }
    [TestMethod]
    public void RemoveNode_MissingNode_ReturnsFalse()
    {
        var g = new Dag<string>();
        g.AddNode("A");

        Assert.IsFalse(g.RemoveNode("Z"));
    }

    private static readonly string[] expected = new[] { "C" };
    private static readonly string[] expectedArray = new[] { "A" };

    [TestMethod]
    public void RemoveNode_RemovesNodeAndEdges_LeavesOthersUnchanged()
    {
        var testDag = new Dag<string>();
        foreach (var n in new[] { "A", "B", "C" })
            testDag.AddNode(n);

        testDag.AddDependency("A", "B");   // incoming to B
        testDag.AddDependency("B", "C");   // outgoing from B
        testDag.AddDependency("A", "C");   // unrelated to B

        Assert.IsTrue(testDag.RemoveNode("B"));

        // B is gone from everyone's edges
        Assert.AreSequenceEqual(expected, testDag.GetDependencies("A").ToList(), Microsoft.VisualStudio.TestTools.UnitTesting.SequenceOrder.InAnyOrder);   // changed
        Assert.AreSequenceEqual(expectedArray, testDag.GetDependents("C").ToList(), Microsoft.VisualStudio.TestTools.UnitTesting.SequenceOrder.InAnyOrder);     // changed

        // Nothing else changed
        Assert.IsEmpty(testDag.GetDependents("A"));
        Assert.IsEmpty(testDag.GetDependencies("C"));
    }
    [TestMethod]
    public void AddDependency_CycleDetected_Throws()
    {
        Dag<string> dag = new();
        dag.AddNode("A");
        dag.AddNode("B");
        dag.AddNode("C");

        dag.AddDependency("B", "A");
        dag.AddDependency("C", "B");

        Assert.Throws<InvalidOperationException>(() => dag.AddDependency("A", "C"));
    }
    [TestMethod]
    public void AddDependency_DirectedGraph_DoesNotThrow()
    {
        Dag<int> graph = new();
        graph.AddNode(101);
        graph.AddNode(202);
        graph.AddNode(303);
        graph.AddNode(404);
        graph.AddNode(505);

        graph.AddDependency(101, 202);
        graph.AddDependency(101, 303);
        graph.AddDependency(202, 404);
        graph.AddDependency(303, 404);

        graph.AddDependency(505, 101);   // exhausts the dag, revisits a node but can not reach
        graph.AddDependency(505, 404);   // same, in the opposite direction 

        Assert.HasCount(2, graph.GetDependencies(505));
        Assert.Contains(101, graph.GetDependencies(505));
        Assert.Contains(404, graph.GetDependencies(505));
    }
    [TestMethod]
    public void RemoveNode_RemovesIncomingAndOutgoingEdges()
    {
        Dag<string> dag = new();

        dag.AddNode("A");
        dag.AddNode("B");
        dag.AddNode("C");
        dag.AddNode("D");

        dag.AddDependency("A", "B");
        dag.AddDependency("A", "C");
        dag.AddDependency("B", "D");
        dag.AddDependency("C", "D");

        dag.RemoveNode("B");

        Assert.DoesNotContain(
            "B", dag.GetDependencies("A").ToList());

        Assert.Contains(
            "C", dag.GetDependencies("A").ToList());

        Assert.Contains(
            "D", dag.GetDependencies("C").ToList());
    }
    [TestMethod]
    public void GetDependencies_ReturnsDirectDependencies()   // renamed
    {
        Dag<string> dag = new();

        dag.AddNode("A");
        dag.AddNode("B");
        dag.AddNode("C");

        dag.AddDependency("A", "B");
        dag.AddDependency("A", "C");

        IReadOnlyCollection<string> result =
            dag.GetDependencies("A");   // changed

        Assert.HasCount(2, result);
        Assert.Contains("B", result);
        Assert.Contains("C", result);
    }
    [TestMethod]
    public void AddDependent_UnknownDependent_Throws()
    {
        Dag<string> dag = new();
        dag.AddNode("A");

        Assert.Throws<ArgumentException>(() => dag.AddDependency("B", "A"));
    }


    //Extra Credit Test 2:
    [TestMethod]
    public void ToMermaid_ReturnsExpectedFlowchart()
    {
        Dag<string> dag = new();

        dag.AddNode("A");
        dag.AddNode("B");

        dag.AddDependency("A", "B");

        string mermaid = dag.ToMermaid();

        StringAssert.Contains(
            mermaid,
            "Mermaid flowchart");

        StringAssert.Contains(
            mermaid,
            "B --> A");
    }
    [TestMethod]
    public void NonExistentNodeThrowsArgumentException()
    {
        Dag<int> dag = new();

        dag.AddNode(100);

        Assert.Throws<ArgumentException>(() => dag.GetDependents(101)); // nonexistent value throws argument exception
        Assert.Throws<ArgumentException>(() => dag.GetDependencies(101)); // nonexistent value throws argument exception
    }
}
