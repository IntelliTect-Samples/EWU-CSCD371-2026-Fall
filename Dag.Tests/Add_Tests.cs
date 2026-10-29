namespace Dag.Tests;

[TestClass]
public class AddTests
{
    private static readonly string[] ExpectedFanBranches = ["CSCD 300", "CSCD 320"];
    private static readonly string[] ExpectedCourse211 = ["CSCD 211"];
    private static readonly string[] ExpectedCourse300 = ["CSCD 300"];
    private static readonly int[] ExpectedNode0Dependency = [4];
    private static readonly int[] ExpectedNode4Dependent = [0];

    [TestMethod]
    public void AddNodeAddsNodeToGraph()
    {
        var graph = new Dag<string>();

        graph.AddNode("CSCD 211");

        Assert.AreEqual(0, graph.GetDependencies("CSCD 211").ToArray().Length);
        Assert.AreEqual(0, graph.GetDependents("CSCD 211").ToArray().Length);
    }

    [TestMethod]
    public void AddNodeDuplicateNodeDoesNotThrowOrDuplicateEdges()
    {
        var graph = new Dag<string>();
        graph.AddNode("CSCD 211");
        graph.AddNode("CSCD 300");

        graph.AddNode("CSCD 211");
        graph.AddDependency("CSCD 211", "CSCD 300");
        graph.AddDependency("CSCD 211", "CSCD 300");

        CollectionAssert.AreEqual(
            ExpectedCourse211,
            graph.GetDependencies("CSCD 300").ToArray());
        CollectionAssert.AreEqual(
            ExpectedCourse300,
            graph.GetDependents("CSCD 211").ToArray());
    }

    [TestMethod]
    public void AddDependencyMissingNodesThrows()
    {
        var graph = new Dag<int>();

        var ex = Assert.Throws<InvalidOperationException>(() => graph.AddDependency(1, 2));
        Assert.AreEqual("Both nodes must exist in the graph.", ex.Message);
    }

    [TestMethod]
    public void AddDependencySkipsAlreadyVisitedNodesDuringCycleCheck()
    {
        var graph = new Dag<int>();

        for (var node = 0; node <= 4; node++)
        {
            graph.AddNode(node);
        }

        graph.AddDependency(0, 1);
        graph.AddDependency(0, 2);
        graph.AddDependency(1, 3);
        graph.AddDependency(2, 3);

        graph.AddDependency(4, 0);

        CollectionAssert.AreEqual(ExpectedNode0Dependency, graph.GetDependencies(0).ToArray());
        CollectionAssert.AreEqual(ExpectedNode4Dependent, graph.GetDependents(4).ToArray());
    }

    private static Dag<string> BuildFan()
    {
        var graph = new Dag<string>();

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
    public void FanInOutDemonstration()
    {
        var fan = BuildFan();

        CollectionAssert.AreEquivalent(ExpectedFanBranches, fan.GetDependents("CSCD 211").ToArray());
        CollectionAssert.AreEquivalent(ExpectedFanBranches, fan.GetDependencies("CSCD 371").ToArray());

        Assert.Contains("CSCD 211", fan.GetDependencies("CSCD 300"));
        Assert.Contains("CSCD 211", fan.GetDependencies("CSCD 320"));
        Assert.Contains("CSCD 371", fan.GetDependents("CSCD 300"));
        Assert.Contains("CSCD 371", fan.GetDependents("CSCD 320"));
    }

    [TestMethod]
    public void AddDependencyCreatesCycleThrows()
    {
        var fan = BuildFan();

        var ex = Assert.Throws<InvalidOperationException>(
            () => fan.AddDependency("CSCD 371", "CSCD 320"));

        Assert.AreEqual("Adding this dependency would create a cycle.", ex.Message);
    }

}
