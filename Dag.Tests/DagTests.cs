using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dag.Tests;

[TestClass]
public class DagTests
{
    [TestMethod]
    public void AddNode_NewValue_AddsNodeWithoutConnections()
    {
        Dag<string> graph = new();

        Assert.IsTrue(graph.AddNode("Course"));

        Assert.IsEmpty(graph.GetDependencies("Course"));
        Assert.IsEmpty(graph.GetDependents("Course"));
    }

    [TestMethod]
    public void AddNode_NullValue_ThrowsArgumentNullException()
    {
        Dag<string> graph = new();

        ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(() => graph.AddNode(null!));

        Assert.AreEqual("value", exception.ParamName);
    }

    [TestMethod]
    public void AddNode_DuplicateValue_ReturnsFalseAndPreservesConnections()
    {
        Dag<string> graph = new();
        graph.AddNode("Prerequisite");
        graph.AddNode("Course");
        graph.AddDependency("Prerequisite", "Course");

        Assert.IsFalse(graph.AddNode("Prerequisite"));
        Assert.IsFalse(graph.AddNode("Course"));

        CollectionAssert.AreEquivalent(new[] { "Prerequisite" }, graph.GetDependencies("Course"));
        CollectionAssert.AreEquivalent(new[] { "Course" }, graph.GetDependents("Prerequisite"));
    }

    [TestMethod]
    public void AddNode_DifferentCasing_StoresSeparateNodes()
    {
        Dag<string> graph = new();

        Assert.IsTrue(graph.AddNode("Course"));
        Assert.IsTrue(graph.AddNode("course"));
        Assert.IsTrue(graph.AddDependency("Course", "course"));

        CollectionAssert.AreEquivalent(new[] { "Course" }, graph.GetDependencies("course"));
        Assert.IsEmpty(graph.GetDependencies("Course"));
    }

    [TestMethod]
    public void AddDependency_ValueTypeNodes_SupportsZeroAndNegativeValues()
    {
        Dag<int> graph = new();
        graph.AddNode(0);
        graph.AddNode(-1);

        Assert.IsTrue(graph.AddDependency(0, -1));

        CollectionAssert.AreEquivalent(new[] { 0 }, graph.GetDependencies(-1));
        CollectionAssert.AreEquivalent(new[] { -1 }, graph.GetDependents(0));
    }

    [TestMethod]
    public void AddDependency_EqualRecordInstances_UsesValueEqualityThroughoutGraph()
    {
        Dag<Course> graph = new();
        graph.AddNode(new Course(211));
        graph.AddNode(new Course(371));

        Assert.IsFalse(graph.AddNode(new Course(211)));
        Assert.IsTrue(graph.AddDependency(new Course(211), new Course(371)));
        Assert.IsFalse(graph.AddDependency(new Course(211), new Course(371)));
        CollectionAssert.AreEquivalent(new[] { new Course(211) }, graph.GetDependencies(new Course(371)));
        CollectionAssert.AreEquivalent(new[] { new Course(371) }, graph.GetDependents(new Course(211)));

        InvalidOperationException exception = Assert.ThrowsExactly<InvalidOperationException>(
            () => graph.AddDependency(new Course(371), new Course(211)));

        Assert.AreEqual("Adding this dependency would create a cycle.", exception.Message);
        Assert.IsTrue(graph.RemoveNode(new Course(211)));
        Assert.IsEmpty(graph.GetDependencies(new Course(371)));
    }

    [TestMethod]
    [DataRow(null, "Course", "dependsOn")]
    [DataRow("Prerequisite", null, "dependent")]
    [DataRow(null, null, "dependsOn")]
    public void AddDependency_NullNode_ThrowsWithoutChangingGraph(
        string? dependsOn, string? dependent, string expectedParameter)
    {
        Dag<string> graph = new();
        graph.AddNode("Prerequisite");
        graph.AddNode("Course");

        ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(
            () => graph.AddDependency(dependsOn!, dependent!));

        Assert.AreEqual(expectedParameter, exception.ParamName);
        Assert.IsEmpty(graph.GetDependents("Prerequisite"));
        Assert.IsEmpty(graph.GetDependencies("Course"));
    }

    [TestMethod]
    [DataRow(false, true, "dependsOn")]
    [DataRow(true, false, "dependent")]
    [DataRow(false, false, "dependsOn")]
    public void AddDependency_MissingNode_ThrowsWithoutAddingNodesOrEdges(
        bool addPrerequisite, bool addCourse, string expectedParameter)
    {
        Dag<string> graph = new();
        if (addPrerequisite)
        {
            graph.AddNode("Prerequisite");
        }

        if (addCourse)
        {
            graph.AddNode("Course");
        }

        ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(
            () => graph.AddDependency("Prerequisite", "Course"));

        Assert.AreEqual(expectedParameter, exception.ParamName);
        Assert.AreEqual(!addPrerequisite, graph.AddNode("Prerequisite"));
        Assert.AreEqual(!addCourse, graph.AddNode("Course"));
        Assert.IsEmpty(graph.GetDependents("Prerequisite"));
        Assert.IsEmpty(graph.GetDependencies("Course"));
    }

    [TestMethod]
    public void AddDependency_ExistingNodes_AddsEdgeInCorrectDirection()
    {
        Dag<string> graph = new();
        graph.AddNode("Prerequisite");
        graph.AddNode("Course");

        Assert.IsTrue(graph.AddDependency("Prerequisite", "Course"));

        CollectionAssert.AreEquivalent(new[] { "Prerequisite" }, graph.GetDependencies("Course"));
        CollectionAssert.AreEquivalent(new[] { "Course" }, graph.GetDependents("Prerequisite"));
        Assert.IsEmpty(graph.GetDependencies("Prerequisite"));
        Assert.IsEmpty(graph.GetDependents("Course"));
    }

    [TestMethod]
    public void AddDependency_DuplicateEdge_ReturnsFalseAndKeepsSingleEdge()
    {
        Dag<int> graph = new();
        graph.AddNode(1);
        graph.AddNode(2);
        graph.AddDependency(1, 2);

        Assert.IsFalse(graph.AddDependency(1, 2));

        CollectionAssert.AreEquivalent(new[] { 1 }, graph.GetDependencies(2));
        CollectionAssert.AreEquivalent(new[] { 2 }, graph.GetDependents(1));
    }

    [TestMethod]
    public void AddDependency_SelfDependency_ThrowsWithoutAddingEdge()
    {
        Dag<string> graph = new();
        graph.AddNode("Course");

        InvalidOperationException exception = Assert.ThrowsExactly<InvalidOperationException>(
            () => graph.AddDependency("Course", "Course"));

        Assert.AreEqual("Adding this dependency would create a cycle.", exception.Message);
        Assert.IsEmpty(graph.GetDependencies("Course"));
        Assert.IsEmpty(graph.GetDependents("Course"));
    }

    [TestMethod]
    public void AddDependency_ReverseEdge_ThrowsWithoutChangingExistingEdge()
    {
        Dag<int> graph = new();
        graph.AddNode(1);
        graph.AddNode(2);
        graph.AddDependency(1, 2);

        InvalidOperationException exception = Assert.ThrowsExactly<InvalidOperationException>(
            () => graph.AddDependency(2, 1));

        Assert.AreEqual("Adding this dependency would create a cycle.", exception.Message);
        CollectionAssert.AreEquivalent(new[] { 1 }, graph.GetDependencies(2));
        CollectionAssert.AreEquivalent(new[] { 2 }, graph.GetDependents(1));
        Assert.IsEmpty(graph.GetDependencies(1));
        Assert.IsEmpty(graph.GetDependents(2));
    }

    [TestMethod]
    public void AddDependency_CoursePrerequisites_DemonstratesFanOutAndFanInAndRejectsCycle()
    {
        Dag<string> courses = new();
        string[] courseNames = ["CSCD 211", "CSCD 300", "CSCD 320", "CSCD 371"];
        foreach (string course in courseNames)
        {
            courses.AddNode(course);
        }

        courses.AddDependency("CSCD 211", "CSCD 300");
        courses.AddDependency("CSCD 211", "CSCD 320");
        courses.AddDependency("CSCD 300", "CSCD 371");
        courses.AddDependency("CSCD 320", "CSCD 371");

        InvalidOperationException exception = Assert.ThrowsExactly<InvalidOperationException>(
            () => courses.AddDependency("CSCD 371", "CSCD 211"));

        Assert.AreEqual("Adding this dependency would create a cycle.", exception.Message);
        Assert.IsEmpty(courses.GetDependencies("CSCD 211"));
        CollectionAssert.AreEquivalent(new[] { "CSCD 300", "CSCD 320" }, courses.GetDependents("CSCD 211"));
        CollectionAssert.AreEquivalent(new[] { "CSCD 211" }, courses.GetDependencies("CSCD 300"));
        CollectionAssert.AreEquivalent(new[] { "CSCD 211" }, courses.GetDependencies("CSCD 320"));
        CollectionAssert.AreEquivalent(new[] { "CSCD 371" }, courses.GetDependents("CSCD 300"));
        CollectionAssert.AreEquivalent(new[] { "CSCD 371" }, courses.GetDependents("CSCD 320"));
        CollectionAssert.AreEquivalent(new[] { "CSCD 300", "CSCD 320" }, courses.GetDependencies("CSCD 371"));
        Assert.IsEmpty(courses.GetDependents("CSCD 371"));
    }

    [TestMethod]
    public void AddDependency_CycleOnOneBranch_StillSearchesOtherBranches()
    {
        Dag<int> graph = new();
        for (int value = 0; value < 4; value++)
        {
            graph.AddNode(value);
        }

        graph.AddDependency(0, 1);
        graph.AddDependency(0, 2);
        graph.AddDependency(1, 3);

        Assert.ThrowsExactly<InvalidOperationException>(() => graph.AddDependency(3, 0));

        CollectionAssert.AreEquivalent(new[] { 1, 2 }, graph.GetDependents(0));
        CollectionAssert.AreEquivalent(new[] { 1 }, graph.GetDependencies(3));
        Assert.IsTrue(graph.AddDependency(2, 3));
    }

    [TestMethod]
    public void AddDependency_TransitiveShortcut_AddsValidDirectEdge()
    {
        Dag<int> graph = CreateChain();

        Assert.IsTrue(graph.AddDependency(1, 3));

        CollectionAssert.AreEquivalent(new[] { 1, 2 }, graph.GetDependencies(3));
        CollectionAssert.AreEquivalent(new[] { 2, 3 }, graph.GetDependents(1));
    }

    [TestMethod]
    public void AddDependency_DisconnectedComponents_ConnectsWithoutFalseCycle()
    {
        Dag<int> graph = CreateChain();
        graph.AddNode(4);
        graph.AddNode(5);
        graph.AddDependency(4, 5);

        Assert.IsTrue(graph.AddDependency(3, 4));

        CollectionAssert.AreEquivalent(new[] { 3 }, graph.GetDependencies(4));
        CollectionAssert.AreEquivalent(new[] { 5 }, graph.GetDependents(4));
        Assert.ThrowsExactly<InvalidOperationException>(() => graph.AddDependency(5, 1));
    }

    [TestMethod]
    public void AddDependency_LongCycle_RejectsWithoutRecursiveTraversal()
    {
        const int LastNode = 10000;
        Dag<int> graph = new();
        graph.AddNode(0);
        for (int value = 1; value <= LastNode; value++)
        {
            graph.AddNode(value);
            graph.AddDependency(value - 1, value);
        }

        InvalidOperationException exception = Assert.ThrowsExactly<InvalidOperationException>(
            () => graph.AddDependency(LastNode, 0));

        Assert.AreEqual("Adding this dependency would create a cycle.", exception.Message);
        Assert.IsEmpty(graph.GetDependencies(0));
        Assert.IsEmpty(graph.GetDependents(LastNode));
    }

    [TestMethod]
    public void AddDependency_RepeatedFanIn_SearchesSharedDescendantsWithoutFalseCycle()
    {
        Dag<int> graph = new();
        graph.AddNode(0);
        int[] parents = [0];
        for (int level = 0; level < 32; level++)
        {
            int[] children = [level * 2 + 1, level * 2 + 2];
            foreach (int child in children)
            {
                graph.AddNode(child);
                foreach (int parent in parents)
                {
                    graph.AddDependency(parent, child);
                }
            }

            parents = children;
        }

        graph.AddNode(-1);

        Assert.IsTrue(graph.AddDependency(-1, 0));

        CollectionAssert.AreEquivalent(new[] { -1 }, graph.GetDependencies(0));
    }

    [TestMethod]
    public void GetDependencies_Chain_ReturnsOnlyDirectPrerequisite()
    {
        Dag<int> graph = CreateChain();

        CollectionAssert.AreEquivalent(new[] { 2 }, graph.GetDependencies(3));
    }

    [TestMethod]
    public void GetDependents_Chain_ReturnsOnlyDirectDependent()
    {
        Dag<int> graph = CreateChain();

        CollectionAssert.AreEquivalent(new[] { 2 }, graph.GetDependents(1));
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void GetDependencies_MissingNode_ThrowsArgumentException(bool populateGraph)
    {
        Dag<int> graph = populateGraph ? CreateChain() : new();

        ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(() => graph.GetDependencies(4));

        Assert.AreEqual("value", exception.ParamName);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void GetDependents_MissingNode_ThrowsArgumentException(bool populateGraph)
    {
        Dag<int> graph = populateGraph ? CreateChain() : new();

        ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(() => graph.GetDependents(4));

        Assert.AreEqual("value", exception.ParamName);
    }

    [TestMethod]
    public void GetDependencies_NullValue_ThrowsArgumentNullException()
    {
        Dag<string> graph = new();

        Assert.ThrowsExactly<ArgumentNullException>(() => graph.GetDependencies(null!));
    }

    [TestMethod]
    public void GetDependents_NullValue_ThrowsArgumentNullException()
    {
        Dag<string> graph = new();

        Assert.ThrowsExactly<ArgumentNullException>(() => graph.GetDependents(null!));
    }

    [TestMethod]
    public void GetConnections_ModifyingReturnedArrays_DoesNotChangeGraph()
    {
        Dag<int> graph = CreateChain();
        int[] dependencies = graph.GetDependencies(2);
        int[] dependents = graph.GetDependents(2);

        dependencies[0] = 3;
        dependents[0] = 1;

        CollectionAssert.AreEquivalent(new[] { 1 }, graph.GetDependencies(2));
        CollectionAssert.AreEquivalent(new[] { 3 }, graph.GetDependents(2));
        Assert.ThrowsExactly<InvalidOperationException>(() => graph.AddDependency(3, 1));
    }

    [TestMethod]
    public void GetConnections_GraphChanges_PreservesEarlierSnapshots()
    {
        Dag<int> graph = CreateChain();
        int[] dependencies = graph.GetDependencies(3);
        int[] dependents = graph.GetDependents(1);

        graph.AddDependency(1, 3);

        CollectionAssert.AreEquivalent(new[] { 2 }, dependencies);
        CollectionAssert.AreEquivalent(new[] { 2 }, dependents);
        Assert.HasCount(2, graph.GetDependencies(3));
        Assert.HasCount(2, graph.GetDependents(1));
    }

    private static Dag<int> CreateChain()
    {
        Dag<int> graph = new();
        graph.AddNode(1);
        graph.AddNode(2);
        graph.AddNode(3);
        graph.AddDependency(1, 2);
        graph.AddDependency(2, 3);
        return graph;
    }

    private sealed record class Course(int Number);
}
