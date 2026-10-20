namespace Dag.Tests;

[TestClass]
public class DagTests
{
    [TestMethod]
    public void AddNode_NewNode_ReturnsTrue()
    {
        Dag<string> dag = new();

        Assert.IsTrue(dag.AddNode("CSCD 211"));
    }

    [TestMethod]
    public void AddNode_ExistingNode_ReturnsFalseWithoutThrowing()
    {
        Dag<string> dag = new();
        dag.AddNode("CSCD 211");

        Assert.IsFalse(dag.AddNode("CSCD 211"));
    }

    [TestMethod]
    public void AddNode_CustomComparer_TreatsEquivalentValuesAsSameNode()
    {
        Dag<string> dag = new(StringComparer.OrdinalIgnoreCase);
        dag.AddNode("cscd 211");

        Assert.IsFalse(dag.AddNode("CSCD 211"));
    }

    [TestMethod]
    public void RemoveNode_ExistingNode_ReturnsTrueAndRemovesNode()
    {
        Dag<string> dag = new();
        dag.AddNode("CSCD 211");

        Assert.IsTrue(dag.RemoveNode("CSCD 211"));
        Assert.IsTrue(dag.AddNode("CSCD 211"));
    }

    [TestMethod]
    public void RemoveNode_MissingNode_ReturnsFalse()
    {
        Dag<string> dag = new();

        Assert.IsFalse(dag.RemoveNode("CSCD 211"));
    }

    [TestMethod]
    public void RemoveNode_OtherNodes_RemainInGraph()
    {
        Dag<string> dag = new();
        dag.AddNode("CSCD 211");
        dag.AddNode("CSCD 300");

        dag.RemoveNode("CSCD 211");

        Assert.IsFalse(dag.AddNode("CSCD 300"));
    }
}
