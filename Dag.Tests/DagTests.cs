namespace Dag.Tests;

[TestClass]
public sealed class DagTests
{
    private const string Cscd211 = "CSCD 211";
    private const string Cscd300 = "CSCD 300";
    private const string Cscd320 = "CSCD 320";
    private const string Cscd371 = "CSCD 371";

    // Fan-out/fan-in: 211 -> {300, 320} -> 371
    private static Dag<string> CreateCourseGraph()
    {
        var dag = new Dag<string>();
        foreach (string course in new[] { Cscd211, Cscd300, Cscd320, Cscd371 })
        {
            dag.AddNode(course);
        }

        dag.AddDependency(Cscd211, Cscd300);
        dag.AddDependency(Cscd211, Cscd320);
        dag.AddDependency(Cscd300, Cscd371);
        dag.AddDependency(Cscd320, Cscd371);
        return dag;
    }

    // Order of a node's neighbours is not part of the contract, so compare as sets.
    private static void AssertNodes(IReadOnlyCollection<string> actual, params string[] expected)
    {
        CollectionAssert.AreEquivalent(expected, actual.ToArray());
    }

    // ----- Demonstration -----

    [TestMethod]
    public void CourseGraphFansOutAndFansIn()
    {
        Dag<string> dag = CreateCourseGraph();

        AssertNodes(dag.GetDependents(Cscd211), Cscd300, Cscd320);
        Assert.IsEmpty(dag.GetDependencies(Cscd211));
        AssertNodes(dag.GetDependencies(Cscd371), Cscd300, Cscd320);
        Assert.IsEmpty(dag.GetDependents(Cscd371));
    }

    [TestMethod]
    public void CourseGraphRejectsPrerequisiteCycle()
    {
        Dag<string> dag = CreateCourseGraph();

        InvalidOperationException ex = Assert.ThrowsExactly<InvalidOperationException>(
            () => dag.AddDependency(Cscd371, Cscd211));

        Assert.AreEqual("Adding this dependency would create a cycle.", ex.Message);
    }

    // ----- AddNode -----

    [TestMethod]
    public void AddNodeNewNodeHasNoEdges()
    {
        var dag = new Dag<string>();

        dag.AddNode("A");

        Assert.IsEmpty(dag.GetDependencies("A"));
        Assert.IsEmpty(dag.GetDependents("A"));
    }

    [TestMethod]
    public void AddNodeDuplicateDoesNotThrowOrResetEdges()
    {
        var dag = new Dag<string>();
        dag.AddNode("A");
        dag.AddNode("B");
        dag.AddDependency("A", "B");

        dag.AddNode("A");
        dag.AddNode("B");

        AssertNodes(dag.GetDependencies("B"), "A");
        AssertNodes(dag.GetDependents("A"), "B");
    }

    // ----- AddDependency -----

    [TestMethod]
    public void AddDependencyAddsEdgeInBothDirections()
    {
        var dag = new Dag<string>();
        dag.AddNode("A");
        dag.AddNode("B");

        dag.AddDependency("A", "B");

        AssertNodes(dag.GetDependencies("B"), "A");
        AssertNodes(dag.GetDependents("A"), "B");
        Assert.IsEmpty(dag.GetDependencies("A"));
        Assert.IsEmpty(dag.GetDependents("B"));
    }

    [TestMethod]
    public void AddDependencyDuplicateEdgeIsIgnored()
    {
        var dag = new Dag<string>();
        dag.AddNode("A");
        dag.AddNode("B");

        dag.AddDependency("A", "B");
        dag.AddDependency("A", "B");

        Assert.HasCount(1, dag.GetDependencies("B"));
        Assert.HasCount(1, dag.GetDependents("A"));
    }

    [TestMethod]
    public void AddDependencySelfEdgeThrowsCycle()
    {
        var dag = new Dag<string>();
        dag.AddNode("A");

        InvalidOperationException ex = Assert.ThrowsExactly<InvalidOperationException>(
            () => dag.AddDependency("A", "A"));

        Assert.AreEqual("Adding this dependency would create a cycle.", ex.Message);
    }

    [TestMethod]
    public void AddDependencyDirectCycleThrows()
    {
        var dag = new Dag<string>();
        dag.AddNode("A");
        dag.AddNode("B");
        dag.AddDependency("A", "B");

        Assert.ThrowsExactly<InvalidOperationException>(() => dag.AddDependency("B", "A"));
    }

    [TestMethod]
    public void AddDependencyIndirectCycleThrows()
    {
        var dag = new Dag<string>();
        foreach (string node in new[] { "A", "B", "C", "D" })
        {
            dag.AddNode(node);
        }

        dag.AddDependency("A", "B");
        dag.AddDependency("B", "C");
        dag.AddDependency("C", "D");

        Assert.ThrowsExactly<InvalidOperationException>(() => dag.AddDependency("D", "A"));
    }

    [TestMethod]
    public void AddDependencyRejectedCycleLeavesGraphUnchanged()
    {
        var dag = new Dag<string>();
        dag.AddNode("A");
        dag.AddNode("B");
        dag.AddDependency("A", "B");

        Assert.ThrowsExactly<InvalidOperationException>(() => dag.AddDependency("B", "A"));

        Assert.IsEmpty(dag.GetDependencies("A"));
        Assert.IsEmpty(dag.GetDependents("B"));
    }

    [TestMethod]
    public void AddDependencyDiamondShapeWithoutCycleIsAllowed()
    {
        // A second path to the same node is not a cycle.
        var dag = new Dag<string>();
        foreach (string node in new[] { "A", "B", "C", "D" })
        {
            dag.AddNode(node);
        }

        dag.AddDependency("A", "B");
        dag.AddDependency("A", "C");
        dag.AddDependency("B", "D");
        dag.AddDependency("C", "D");
        dag.AddDependency("A", "D");

        AssertNodes(dag.GetDependencies("D"), "A", "B", "C");
    }

    [TestMethod]
    public void AddDependencyMissingDependsOnThrows()
    {
        var dag = new Dag<string>();
        dag.AddNode("B");

        Assert.ThrowsExactly<ArgumentException>(() => dag.AddDependency("A", "B"));
    }

    [TestMethod]
    public void AddDependencyMissingDependentThrows()
    {
        var dag = new Dag<string>();
        dag.AddNode("A");

        Assert.ThrowsExactly<ArgumentException>(() => dag.AddDependency("A", "B"));
    }

    [TestMethod]
    public void AddDependencyMissingNodeDoesNotAddIt()
    {
        var dag = new Dag<string>();
        dag.AddNode("A");

        Assert.ThrowsExactly<ArgumentException>(() => dag.AddDependency("A", "B"));
        Assert.ThrowsExactly<ArgumentException>(() => dag.GetDependents("B"));
    }

    // ----- Queries -----

    [TestMethod]
    public void GetDependenciesMissingNodeThrows()
    {
        var dag = new Dag<string>();

        Assert.ThrowsExactly<ArgumentException>(() => dag.GetDependencies("A"));
    }

    [TestMethod]
    public void GetDependentsMissingNodeThrows()
    {
        var dag = new Dag<string>();

        Assert.ThrowsExactly<ArgumentException>(() => dag.GetDependents("A"));
    }

    [TestMethod]
    public void QueriesReturnOnlyDirectNeighbours()
    {
        var dag = new Dag<string>();
        foreach (string node in new[] { "A", "B", "C" })
        {
            dag.AddNode(node);
        }

        dag.AddDependency("A", "B");
        dag.AddDependency("B", "C");

        AssertNodes(dag.GetDependents("A"), "B");
        AssertNodes(dag.GetDependencies("C"), "B");
    }

    [TestMethod]
    public void QueriesReturnCopiesSoGraphStaysEncapsulated()
    {
        var dag = new Dag<string>();
        dag.AddNode("A");
        dag.AddNode("B");
        dag.AddDependency("A", "B");

        IReadOnlyCollection<string> snapshot = dag.GetDependencies("B");
        dag.AddNode("C");
        dag.AddDependency("C", "B");

        Assert.HasCount(1, snapshot);
        Assert.HasCount(2, dag.GetDependencies("B"));
    }

    // ----- Equality strategy -----

    [TestMethod]
    public void DefaultEqualityTreatsEqualValuesAsSameNode()
    {
        var dag = new Dag<string>();
        dag.AddNode("A");
        dag.AddNode(new string('A', 1));
        dag.AddNode("B");

        dag.AddDependency(new string('A', 1), "B");

        AssertNodes(dag.GetDependencies("B"), "A");
    }

    [TestMethod]
    public void CustomComparerIsAppliedConsistently()
    {
        var dag = new Dag<string>(StringComparer.OrdinalIgnoreCase);
        dag.AddNode("a");
        dag.AddNode("A");
        dag.AddNode("b");

        dag.AddDependency("A", "B");

        // "A" and "a" are the same node under this comparer, so the edge is found
        // whichever spelling is used. Only the casing of the returned value may differ.
        IReadOnlyCollection<string> dependencies = dag.GetDependencies("b");
        Assert.HasCount(1, dependencies);
        Assert.AreEqual("a", dependencies.Single(), ignoreCase: true);
        Assert.ThrowsExactly<InvalidOperationException>(() => dag.AddDependency("B", "a"));
    }

    // ----- RemoveNode (extra credit) -----

    [TestMethod]
    public void RemoveNodeRemovesIncomingAndOutgoingEdges()
    {
        Dag<string> dag = CreateCourseGraph();

        bool removed = dag.RemoveNode(Cscd300);

        Assert.IsTrue(removed);
        AssertNodes(dag.GetDependents(Cscd211), Cscd320);
        AssertNodes(dag.GetDependencies(Cscd371), Cscd320);
        Assert.ThrowsExactly<ArgumentException>(() => dag.GetDependencies(Cscd300));
    }

    [TestMethod]
    public void RemoveNodeLeavesUnrelatedNodesAndEdgesUnchanged()
    {
        Dag<string> dag = CreateCourseGraph();

        dag.RemoveNode(Cscd300);

        Assert.IsEmpty(dag.GetDependencies(Cscd211));
        AssertNodes(dag.GetDependencies(Cscd320), Cscd211);
        Assert.IsEmpty(dag.GetDependents(Cscd371));
    }

    [TestMethod]
    public void RemoveNodeMissingReturnsFalse()
    {
        var dag = new Dag<string>();

        Assert.IsFalse(dag.RemoveNode("A"));
    }

    [TestMethod]
    public void RemoveNodeAllowsEdgeThatWouldHaveBeenACycle()
    {
        Dag<string> dag = CreateCourseGraph();
        dag.RemoveNode(Cscd300);
        dag.RemoveNode(Cscd320);

        dag.AddDependency(Cscd371, Cscd211);

        AssertNodes(dag.GetDependencies(Cscd211), Cscd371);
    }

    // ----- ToMermaid (extra credit) -----

    [TestMethod]
    public void ToMermaidRendersNodesAndEdges()
    {
        var dag = new Dag<string>();
        dag.AddNode("A");
        dag.AddNode("B");
        dag.AddDependency("A", "B");

        string expected =
            "flowchart TD\n" +
            "    n0[\"A\"]\n" +
            "    n1[\"B\"]\n" +
            "    n0 --> n1\n";

        Assert.AreEqual(expected, dag.ToMermaid());
    }

    [TestMethod]
    public void ToMermaidCourseGraphContainsEveryEdge()
    {
        string mermaid = CreateCourseGraph().ToMermaid();

        Assert.IsTrue(mermaid.StartsWith("flowchart TD", StringComparison.Ordinal));
        Assert.IsTrue(mermaid.Contains("n0 --> n1", StringComparison.Ordinal));
        Assert.IsTrue(mermaid.Contains("n0 --> n2", StringComparison.Ordinal));
        Assert.IsTrue(mermaid.Contains("n1 --> n3", StringComparison.Ordinal));
        Assert.IsTrue(mermaid.Contains("n2 --> n3", StringComparison.Ordinal));
    }

    [TestMethod]
    public void ToMermaidEscapesQuotesInLabels()
    {
        var dag = new Dag<string>();
        dag.AddNode("say \"hi\"");

        Assert.IsTrue(dag.ToMermaid().Contains("n0[\"say #quot;hi#quot;\"]", StringComparison.Ordinal));
    }

    [TestMethod]
    public void ToMermaidEmptyGraphIsJustTheHeader()
    {
        Assert.AreEqual("flowchart TD\n", new Dag<string>().ToMermaid());
    }
}