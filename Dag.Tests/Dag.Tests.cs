using Dag;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dag.Tests;

[TestClass]
public class DagTests
{
    [TestMethod]
    public void AddNode_DoesNotAddDuplicates()
    {
        Dag<string> dag = new();

        dag.AddNode("A");
        dag.AddNode("A");

        Assert.AreEqual(0, dag.GetDependents("A").Count);
    }

    [TestMethod]
    public void AddDependency_ReturnsCorrectDependenciesAndDependents()
    {
        Dag<string> dag = new();

        dag.AddNode("CSCD 211");
        dag.AddNode("CSCD 300");
        dag.AddNode("CSCD 320");
        dag.AddNode("CSCD 371");

        dag.AddDependency("CSCD 211", "CSCD 300");
        dag.AddDependency("CSCD 211", "CSCD 320");
        dag.AddDependency("CSCD 300", "CSCD 371");
        dag.AddDependency("CSCD 320", "CSCD 371");

        Assert.AreEqual(2, dag.GetDependents("CSCD 211").Count);
        Assert.IsTrue(dag.GetDependents("CSCD 211").Contains("CSCD 300"));
        Assert.IsTrue(dag.GetDependents("CSCD 211").Contains("CSCD 320"));

        Assert.AreEqual(2, dag.GetDependencies("CSCD 371").Count);
        Assert.IsTrue(dag.GetDependencies("CSCD 371").Contains("CSCD 300"));
        Assert.IsTrue(dag.GetDependencies("CSCD 371").Contains("CSCD 320"));

        Assert.IsTrue(dag.GetDependencies("CSCD 300").Contains("CSCD 211"));
        Assert.IsTrue(dag.GetDependents("CSCD 300").Contains("CSCD 371"));
    }

    [TestMethod]
    public void AddDependency_DoesNotAddDuplicates()
    {
        Dag<string> dag = new();

        dag.AddNode("A");
        dag.AddNode("B");

        dag.AddDependency("A", "B");
        dag.AddDependency("A", "B");

        Assert.AreEqual(1, dag.GetDependents("A").Count);
        Assert.AreEqual(1, dag.GetDependencies("B").Count);
    }

    [TestMethod]
    public void AddDependency_ThrowsWhenNodeDoesNotExist()
    {
        Dag<string> dag = new();
        dag.AddNode("A");

        Assert.ThrowsExactly<ArgumentException>(
            () => dag.AddDependency("Missing", "A"));

        Assert.ThrowsExactly<ArgumentException>(
            () => dag.AddDependency("A", "Missing"));
    }

    [TestMethod]
    public void AddDependency_ThrowsWhenItCreatesCycle()
    {
        Dag<string> dag = new();

        dag.AddNode("CSCD 211");
        dag.AddNode("CSCD 300");
        dag.AddNode("CSCD 320");
        dag.AddNode("CSCD 371");

        dag.AddDependency("CSCD 211", "CSCD 300");
        dag.AddDependency("CSCD 211", "CSCD 320");
        dag.AddDependency("CSCD 300", "CSCD 371");
        dag.AddDependency("CSCD 320", "CSCD 371");

        InvalidOperationException exception =
            Assert.ThrowsExactly<InvalidOperationException>(
                () => dag.AddDependency("CSCD 371", "CSCD 211"));

        Assert.AreEqual(
            "Adding this dependency would create a cycle.",
            exception.Message);
    }

    [TestMethod]
    public void GetDependenciesAndGetDependents_ThrowForMissingNode()
    {
        Dag<string> dag = new();

        Assert.ThrowsExactly<ArgumentException>(
            () => dag.GetDependencies("Missing"));

        Assert.ThrowsExactly<ArgumentException>(
            () => dag.GetDependents("Missing"));
    }

    [TestMethod]
    public void RemoveNode_RemovesItsEdgesAndKeepsUnrelatedEdges()
    {
        Dag<string> dag = new();

        dag.AddNode("A");
        dag.AddNode("B");
        dag.AddNode("C");
        dag.AddNode("D");
        dag.AddNode("E");

        dag.AddDependency("A", "B");
        dag.AddDependency("B", "C");
        dag.AddDependency("D", "E");

        dag.RemoveNode("B");

        Assert.ThrowsExactly<ArgumentException>(
            () => dag.GetDependents("B"));

        Assert.AreEqual(0, dag.GetDependents("A").Count);
        Assert.AreEqual(0, dag.GetDependencies("C").Count);

        Assert.IsTrue(dag.GetDependents("D").Contains("E"));
        Assert.IsTrue(dag.GetDependencies("E").Contains("D"));
    }

    [TestMethod]
    public void ToMermaid_ReturnsFlowchartWithNodesAndEdges()
    {
        Dag<string> dag = new();

        dag.AddNode("A");
        dag.AddNode("B");
        dag.AddDependency("A", "B");

        string result = dag.ToMermaid();

        Assert.IsTrue(result.Contains("flowchart TD"));
        Assert.IsTrue(result.Contains("[\"A\"]"));
        Assert.IsTrue(result.Contains("[\"B\"]"));
        Assert.IsTrue(result.Contains("node0 --> node1"));
    }
}