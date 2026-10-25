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

    [TestMethod]
    public void AddDependency_NewEdge_ReturnsTrue()
    {
        Dag<string> dag = new();
        dag.AddNode("CSCD 211");
        dag.AddNode("CSCD 300");

        Assert.IsTrue(dag.AddDependency("CSCD 211", "CSCD 300"));
    }

    [TestMethod]
    public void AddDependency_DuplicateEdge_ReturnsFalse()
    {
        Dag<string> dag = new();
        dag.AddNode("CSCD 211");
        dag.AddNode("CSCD 300");
        dag.AddDependency("CSCD 211", "CSCD 300");

        Assert.IsFalse(dag.AddDependency("CSCD 211", "CSCD 300"));
    }

    [TestMethod]
    public void AddDependency_MissingDependsOn_ThrowsArgumentException()
    {
        Dag<string> dag = new();
        dag.AddNode("CSCD 300");

        ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(
            () => dag.AddDependency("CSCD 211", "CSCD 300"));
        Assert.AreEqual("dependsOn", exception.ParamName);
    }

    [TestMethod]
    public void AddDependency_MissingDependent_ThrowsArgumentException()
    {
        Dag<string> dag = new();
        dag.AddNode("CSCD 211");

        ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(
            () => dag.AddDependency("CSCD 211", "CSCD 300"));
        Assert.AreEqual("dependent", exception.ParamName);
    }

    [TestMethod]
    public void AddDependency_SelfEdge_ThrowsInvalidOperationException()
    {
        Dag<string> dag = new();
        dag.AddNode("CSCD 211");

        InvalidOperationException exception = Assert.ThrowsExactly<InvalidOperationException>(
            () => dag.AddDependency("CSCD 211", "CSCD 211"));
        Assert.AreEqual("Adding this dependency would create a cycle.", exception.Message);
    }

    [TestMethod]
    public void AddDependency_DirectCycle_ThrowsInvalidOperationException()
    {
        Dag<string> dag = new();
        dag.AddNode("CSCD 211");
        dag.AddNode("CSCD 300");
        dag.AddDependency("CSCD 211", "CSCD 300");

        InvalidOperationException exception = Assert.ThrowsExactly<InvalidOperationException>(
            () => dag.AddDependency("CSCD 300", "CSCD 211"));
        Assert.AreEqual("Adding this dependency would create a cycle.", exception.Message);
    }

    [TestMethod]
    public void AddDependency_CoursePrerequisites_RejectsCycle()
    {
        Dag<string> dag = new();
        foreach (string course in new[] { "CSCD 211", "CSCD 300", "CSCD 320", "CSCD 371" })
        {
            dag.AddNode(course);
        }

        Assert.IsTrue(dag.AddDependency("CSCD 211", "CSCD 300"));
        Assert.IsTrue(dag.AddDependency("CSCD 211", "CSCD 320"));
        Assert.IsTrue(dag.AddDependency("CSCD 300", "CSCD 371"));
        Assert.IsTrue(dag.AddDependency("CSCD 320", "CSCD 371"));

        InvalidOperationException exception = Assert.ThrowsExactly<InvalidOperationException>(
            () => dag.AddDependency("CSCD 371", "CSCD 211"));
        Assert.AreEqual("Adding this dependency would create a cycle.", exception.Message);
    }

    [TestMethod]
    public void AddDependency_CustomComparer_TreatsEquivalentEdgesAsDuplicate()
    {
        Dag<string> dag = new(StringComparer.OrdinalIgnoreCase);
        dag.AddNode("CSCD 211");
        dag.AddNode("CSCD 300");
        dag.AddDependency("cscd 211", "cscd 300");

        Assert.IsFalse(dag.AddDependency("CSCD 211", "CSCD 300"));
    }

    [TestMethod]
    public void GetDependencies_NodeWithoutDependencies_ReturnsEmpty()
    {
        Dag<string> dag = new();
        dag.AddNode("CSCD 211");

        Assert.IsEmpty(dag.GetDependencies("CSCD 211"));
    }

    [TestMethod]
    public void GetDependencies_MissingNode_ThrowsArgumentException()
    {
        Dag<string> dag = new();

        ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(
            () => dag.GetDependencies("CSCD 211"));
        Assert.AreEqual("value", exception.ParamName);
    }

    [TestMethod]
    public void GetDependencies_CoursePrerequisites_ReturnsOnlyDirectDependencies()
    {
        Dag<string> dag = new();
        foreach (string course in new[] { "CSCD 211", "CSCD 300", "CSCD 320", "CSCD 371" })
        {
            dag.AddNode(course);
        }
        dag.AddDependency("CSCD 211", "CSCD 300");
        dag.AddDependency("CSCD 211", "CSCD 320");
        dag.AddDependency("CSCD 300", "CSCD 371");
        dag.AddDependency("CSCD 320", "CSCD 371");

        CollectionAssert.AreEquivalent(new[] { "CSCD 300", "CSCD 320" }, dag.GetDependencies("CSCD 371").ToArray());
        CollectionAssert.AreEquivalent(new[] { "CSCD 211" }, dag.GetDependencies("CSCD 300").ToArray());
        Assert.IsEmpty(dag.GetDependencies("CSCD 211"));
    }

    [TestMethod]
    public void GetDependencies_DuplicateEdge_ReturnsDependencyOnce()
    {
        Dag<string> dag = new();
        dag.AddNode("CSCD 211");
        dag.AddNode("CSCD 300");
        dag.AddDependency("CSCD 211", "CSCD 300");
        dag.AddDependency("CSCD 211", "CSCD 300");

        CollectionAssert.AreEqual(new[] { "CSCD 211" }, dag.GetDependencies("CSCD 300").ToArray());
    }

    [TestMethod]
    public void GetDependencies_ReturnedCollection_IsNotLiveView()
    {
        Dag<string> dag = new();
        dag.AddNode("CSCD 211");
        dag.AddNode("CSCD 300");
        IReadOnlyCollection<string> dependencies = dag.GetDependencies("CSCD 300");

        dag.AddDependency("CSCD 211", "CSCD 300");

        Assert.IsEmpty(dependencies);
    }

    [TestMethod]
    public void GetDependencies_AfterRemovingDependency_ExcludesRemovedNode()
    {
        Dag<string> dag = new();
        dag.AddNode("CSCD 211");
        dag.AddNode("CSCD 300");
        dag.AddDependency("CSCD 211", "CSCD 300");

        dag.RemoveNode("CSCD 211");

        Assert.IsEmpty(dag.GetDependencies("CSCD 300"));
    }

    [TestMethod]
    public void GetDependents_NodeWithoutDependents_ReturnsEmpty()
    {
        Dag<string> dag = new();
        dag.AddNode("CSCD 371");

        Assert.IsEmpty(dag.GetDependents("CSCD 371"));
    }

    [TestMethod]
    public void GetDependents_MissingNode_ThrowsArgumentException()
    {
        Dag<string> dag = new();

        ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(
            () => dag.GetDependents("CSCD 211"));
        Assert.AreEqual("value", exception.ParamName);
    }

    [TestMethod]
    public void GetDependents_CoursePrerequisites_ReturnsOnlyDirectDependents()
    {
        Dag<string> dag = new();
        foreach (string course in new[] { "CSCD 211", "CSCD 300", "CSCD 320", "CSCD 371" })
        {
            dag.AddNode(course);
        }
        dag.AddDependency("CSCD 211", "CSCD 300");
        dag.AddDependency("CSCD 211", "CSCD 320");
        dag.AddDependency("CSCD 300", "CSCD 371");
        dag.AddDependency("CSCD 320", "CSCD 371");

        CollectionAssert.AreEquivalent(new[] { "CSCD 300", "CSCD 320" }, dag.GetDependents("CSCD 211").ToArray());
        CollectionAssert.AreEquivalent(new[] { "CSCD 371" }, dag.GetDependents("CSCD 300").ToArray());
        Assert.IsEmpty(dag.GetDependents("CSCD 371"));
    }

    [TestMethod]
    public void GetDependents_AfterRemovingDependent_ExcludesRemovedNode()
    {
        Dag<string> dag = new();
        dag.AddNode("CSCD 211");
        dag.AddNode("CSCD 300");
        dag.AddDependency("CSCD 211", "CSCD 300");

        dag.RemoveNode("CSCD 300");

        Assert.IsEmpty(dag.GetDependents("CSCD 211"));
    }

    [TestMethod]
    public void AddDependency_NewEdge_AppearsInBothDirections()
    {
        Dag<string> dag = new();
        dag.AddNode("CSCD 211");
        dag.AddNode("CSCD 300");

        dag.AddDependency("CSCD 211", "CSCD 300");

        CollectionAssert.AreEqual(new[] { "CSCD 211" }, dag.GetDependencies("CSCD 300").ToArray());
        CollectionAssert.AreEqual(new[] { "CSCD 300" }, dag.GetDependents("CSCD 211").ToArray());
    }

    [TestMethod]
    public void AddNode_DefaultComparer_IsCaseSensitive()
    {
        Dag<string> dag = new();
        dag.AddNode("CSCD 211");

        Assert.IsTrue(dag.AddNode("cscd 211"));
    }

    [TestMethod]
    public void AddDependency_CustomComparer_DetectsCycleAcrossEquivalentValues()
    {
        Dag<string> dag = new(StringComparer.OrdinalIgnoreCase);
        dag.AddNode("CSCD 211");
        dag.AddNode("CSCD 300");
        dag.AddDependency("CSCD 211", "CSCD 300");

        Assert.ThrowsExactly<InvalidOperationException>(() => dag.AddDependency("cscd 300", "cscd 211"));
    }

    [TestMethod]
    public void GetDependenciesAndDependents_CustomComparer_FindEquivalentValues()
    {
        Dag<string> dag = new(StringComparer.OrdinalIgnoreCase);
        dag.AddNode("CSCD 211");
        dag.AddNode("CSCD 300");
        dag.AddDependency("CSCD 211", "CSCD 300");

        Assert.HasCount(1, dag.GetDependencies("cscd 300"));
        Assert.HasCount(1, dag.GetDependents("cscd 211"));
    }

    [TestMethod]
    public void GetDependencies_ReturnedCollection_CannotBeModified()
    {
        Dag<string> dag = new();
        dag.AddNode("CSCD 211");
        dag.AddNode("CSCD 300");
        dag.AddDependency("CSCD 211", "CSCD 300");

        ICollection<string> dependencies = (ICollection<string>)dag.GetDependencies("CSCD 300");

        Assert.IsTrue(dependencies.IsReadOnly);
        Assert.ThrowsExactly<NotSupportedException>(() => dependencies.Add("CSCD 999"));
        CollectionAssert.AreEqual(new[] { "CSCD 211" }, dag.GetDependencies("CSCD 300").ToArray());
    }

    [TestMethod]
    public void GetDependents_ReturnedCollection_CannotBeModified()
    {
        Dag<string> dag = new();
        dag.AddNode("CSCD 211");
        dag.AddNode("CSCD 300");
        dag.AddDependency("CSCD 211", "CSCD 300");

        ICollection<string> dependents = (ICollection<string>)dag.GetDependents("CSCD 211");

        Assert.IsTrue(dependents.IsReadOnly);
        Assert.ThrowsExactly<NotSupportedException>(() => dependents.Add("CSCD 999"));
        CollectionAssert.AreEqual(new[] { "CSCD 300" }, dag.GetDependents("CSCD 211").ToArray());
    }

    [TestMethod]
    public void ToMermaid_Nodes_AreDeclaredWithLabels()
    {
        Dag<string> dag = new();
        dag.AddNode("CSCD 211");

        string result = dag.ToMermaid();

        StringAssert.Contains(result, "flowchart TD");
        StringAssert.Contains(result, "n0[\"CSCD 211\"]");
    }

    [TestMethod]
    public void ToMermaid_NodeNameWithQuote_IsEscaped()
    {
        Dag<string> dag = new();
        dag.AddNode("Say \"hi\"");

        string result = dag.ToMermaid();

        StringAssert.Contains(result, "n0[\"Say #quot;hi#quot;\"]");
    }

    [TestMethod]
    public void ToMermaid_CoursePrerequisites_DrawsEdgesFromPrerequisiteToDependent()
    {
    Dag<string> dag = new();
        foreach (string course in new[] { "CSCD 211", "CSCD 300", "CSCD 320", "CSCD 371" })
        {
            dag.AddNode(course);
        }
        dag.AddDependency("CSCD 211", "CSCD 300");
        dag.AddDependency("CSCD 211", "CSCD 320");
        dag.AddDependency("CSCD 300", "CSCD 371");
        dag.AddDependency("CSCD 320", "CSCD 371");

        string result = dag.ToMermaid();

        StringAssert.Contains(result, "n0 --> n1");
        StringAssert.Contains(result, "n0 --> n2");
        StringAssert.Contains(result, "n1 --> n3");
        StringAssert.Contains(result, "n2 --> n3");
    }

}
