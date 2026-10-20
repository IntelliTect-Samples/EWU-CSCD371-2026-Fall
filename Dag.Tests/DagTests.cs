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
}
