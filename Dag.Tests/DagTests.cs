namespace Dag.Tests;

[TestClass]
public class DagTests
{
    // constants used to avoid string typing errors
    private const string CSCD211 = "CSCD 211";
    private const string CSCD300 = "CSCD 300";
    private const string CSCD320 = "CSCD 320";
    private const string CSCD371 = "CSCD 371";

    #region AddNode Tests (REQ 2)

    // REQ: 2.1: AddNode adds a node to the graph
    [TestMethod]
    public void AddNode_NewNode_ReturnsTrue()
    {
        Dag<string> graph = new();
        
        bool result = graph.AddNode(CSCD211);
        
        Assert.IsTrue(result);
    }

    // REQ: 2.2 : Adding an existing node must not throw or add a duplicate
    [TestMethod]
    public void AddNode_ExistingNode_ReturnsFalse()
    {
        Dag<string> graph = new();
        
        graph.AddNode(CSCD211);
        
        bool result = graph.AddNode(CSCD211);
        
        Assert.IsFalse(result);
    }

    #endregion
    
    #region AddDependency Tests (REQ 3)
    
    // REQ: 3.1: both nodes must already exist in the graph
    [TestMethod]
    public void AddDependency_MissingNode_ReturnsFalse()
    {
        Dag<string> graph = new();
        
        graph.AddNode(CSCD211);
        
        bool result = graph.AddDependency(CSCD211, CSCD300);
        
        Assert.IsFalse(result);
    }

    // REQ: 3.2: Adds the directed edge dependsOn -> dependent
    [TestMethod]
    public void AddDependency_ValidDependency_ReturnsTrue()
    {
        Dag<string> graph = new();

        graph.AddNode(CSCD211);
        graph.AddNode(CSCD300);

        bool result = graph.AddDependency(CSCD211, CSCD300);

        Assert.IsTrue(result);
    }
    
    // REQ: 3.3: Must not add duplicate edges
    [TestMethod]
    public void AddDependency_DuplicateDependency_ReturnsFalse()
    {
        Dag<string> graph = new();
        
        graph.AddNode(CSCD211);
        graph.AddNode(CSCD300);
        
        graph.AddDependency(CSCD211, CSCD300);
        
        bool result = graph.AddDependency(CSCD211, CSCD300);
        
        Assert.IsFalse(result);
    }

    // REQ: 3.4: Throw InvalidOperationException if the edge would create a cycle.
    // Use the message: "Adding this dependency would create a cycle."
    [TestMethod]
    public void AddDependency_CreatingCycle_ThrowsInvalidOperationException()
    {
        Dag<string> graph = new();
        
        graph.AddNode(CSCD211);
        graph.AddNode(CSCD300);
        
        graph.AddDependency(CSCD211, CSCD300);
        
        InvalidOperationException exception = 
            Assert.ThrowsExactly<InvalidOperationException>(
                () => graph.AddDependency(CSCD300, CSCD211));
        
        Assert.AreEqual("Adding this dependency would create a cycle.", exception.Message);
    }

    [TestMethod]
    public void AddDependency_SelfDependency_ThrowsInvalidOperationException()
    {
        Dag<string> graph = new();
        
        graph.AddNode(CSCD211);
        
        InvalidOperationException exception = 
            Assert.ThrowsExactly<InvalidOperationException>(
                () => graph.AddDependency(CSCD211, CSCD211));
        
        Assert.AreEqual("Adding this dependency would create a cycle.", exception.Message);
    }

    #endregion

    #region GetDependencies Tests (REQ 4)
    
    [TestMethod]
    public void GetDependencies_ValidNodeWithDependency_ReturnsDependency()
    {
        Dag<string> graph = new();
        
        graph.AddNode(CSCD211);
        graph.AddNode(CSCD300);
        
        graph.AddDependency(CSCD211, CSCD300);
        
        Assert.AreSequenceEqual([CSCD211], graph.GetDependencies(CSCD300));
    }
    
    [TestMethod]
    public void GetDependencies_NodeNotInGraph_ReturnsEmptyCollection()
    {
        Dag<string> graph = new();
        
        Assert.AreSequenceEqual([], graph.GetDependencies("Not a node"));
    }
    
    [TestMethod]
    public void GetDependencies_ValidNodeNoDependency_ReturnsEmptyCollection()
    {
        Dag<string> graph = new();
        
        graph.AddNode(CSCD211);
        
        Assert.AreSequenceEqual([], graph.GetDependencies(CSCD211));
    }

    #endregion

    #region GetDependents Tests (Req 5)

    [TestMethod]
    public void GetDependents_ValidNodeWithDependent_ReturnsDependent()
    {
        Dag<string> graph = new();
        
        graph.AddNode(CSCD211);
        graph.AddNode(CSCD300);
        
        graph.AddDependency(CSCD211, CSCD300);
        
        Assert.AreSequenceEqual([CSCD300], graph.GetDependents(CSCD211));
    }

    [TestMethod]
    public void GetDependents_NodeNotInGraph_ReturnsEmptyCollection()
    {
        Dag<string> graph = new();
        
        Assert.AreSequenceEqual([], graph.GetDependents("Not a node"));
    }
    
    [TestMethod]
    public void GetDependents_ValidNodeNoDependents_ReturnsEmptyCollection()
    {
        Dag<string> graph = new();
        
        graph.AddNode(CSCD211);
        
        Assert.AreSequenceEqual([], graph.GetDependents(CSCD211));
    }

    #endregion

    #region Dependency Relationship Proof (REQ 6)

    [TestMethod]
    public void DependencyRelationship_ExistingEdge_DependenciesAndDependentsCorrect()
    {
        Dag<string> graph = new();
        
        graph.AddNode(CSCD211);
        graph.AddNode(CSCD300);
        
        graph.AddDependency(CSCD211, CSCD300);
        
        Assert.Contains(CSCD211, graph.GetDependencies(CSCD300));

        Assert.Contains(CSCD300, graph.GetDependents(CSCD211));
    }

    #endregion

    #region ContainsNode Tests

    [TestMethod]
    public void ContainsNode_ValidNode_ReturnsTrue()
    {
        Dag<string> graph = new();
        
        graph.AddNode(CSCD211);
        
        Assert.IsTrue(graph.ContainsNode(CSCD211));
    }

    [TestMethod]
    public void ContainsNode_InvalidNode_ReturnsFalse()
    {
        Dag<string> graph = new();
        
        Assert.IsFalse(graph.ContainsNode(CSCD211));
    }

    #endregion

    #region CourseGraph Demo Tests
    
    [TestMethod]
    public void CourseGraph_FanInFanOut_DependencyRelationshipsCorrect()
    {
        Dag<string> graph = CreateCourseGraph();
        
        // 211 fans out to 300 and 320
        Assert.AreSequenceEqual([CSCD300, CSCD320], graph.GetDependents(CSCD211), SequenceOrder.InAnyOrder);
        
        // 300 directly depends on 211
        Assert.AreSequenceEqual([CSCD211], graph.GetDependencies(CSCD300));
        
        // 320 directly depends on 211
        Assert.AreSequenceEqual([CSCD211], graph.GetDependencies(CSCD320));
        
        // 300 is a prerequisite of 371
        Assert.AreSequenceEqual([CSCD371], graph.GetDependents(CSCD300));
        
        // 320 is a prerequisite of 371
        Assert.AreSequenceEqual([CSCD371], graph.GetDependents(CSCD320));
        
        // 371 fans in from 300 and 320
        Assert.AreSequenceEqual([CSCD300, CSCD320], graph.GetDependencies(CSCD371), SequenceOrder.InAnyOrder);
    }

    [TestMethod]
    public void CourseGraph_AddDependency371To211_ThrowsInvalidOperationExceptionDueToCycle()
    {
        Dag<string> graph = CreateCourseGraph();

        InvalidOperationException exception =
            Assert.ThrowsExactly<InvalidOperationException>(
                () => graph.AddDependency(CSCD371, CSCD211));
        
        Assert.AreEqual("Adding this dependency would create a cycle.", exception.Message);
    }

    private static Dag<string> CreateCourseGraph()
    {
        Dag<string> graph = new();
        
        graph.AddNode(CSCD211);
        graph.AddNode(CSCD300);
        graph.AddNode(CSCD320);
        graph.AddNode(CSCD371);
        
        graph.AddDependency(CSCD211, CSCD300);
        graph.AddDependency(CSCD211, CSCD320);
        graph.AddDependency(CSCD300, CSCD371);
        graph.AddDependency(CSCD320, CSCD371);
        
        return graph;
    }
    
    #endregion

    #region ExtraCredit Mermaid Flowchart Tests

    [TestMethod]
    public void ToMermaidFlowchart_CourseGraph_RendersNodesAndDependencies()
    {
        Dag<string> graph = CreateCourseGraph();

        string result = graph.ToMermaidFlowchart();

        Assert.AreEqual(
            string.Join(
                Environment.NewLine,
                "flowchart TD",
                "node0[\"CSCD 211\"]",
                "node1[\"CSCD 300\"]",
                "node2[\"CSCD 320\"]",
                "node3[\"CSCD 371\"]",
                "node0 --> node1",
                "node0 --> node2",
                "node1 --> node3",
                "node2 --> node3"),
            result);
    }

    [TestMethod]
    public void ToMermaidFlowchart_EmptyGraph_RendersFlowchartHeader()
    {
        Dag<string> graph = new();

        string result = graph.ToMermaidFlowchart();

        Assert.AreEqual("flowchart TD", result);
    }

    [TestMethod]
    public void ToMermaidFlowchart_SpecialCharactersInLabel_AreEscaped()
    {
        Dag<string> graph = new();
        graph.AddNode("A #1 & \"B\" <C> \\ D");

        string result = graph.ToMermaidFlowchart();

        Assert.AreEqual(
            string.Join(
                Environment.NewLine,
                "flowchart TD",
                "node0[\"A #35;1 #38; #34;B#34; #60;C#62; #92; D\"]"),
            result);
    }

    #endregion

    #region ExtraCredit RemoveNode Tests

    [TestMethod]
    public void RemoveNode_ExistingNode_ReturnsTrue()
    {
        Dag<string> graph = new();
        
        graph.AddNode(CSCD211);
        
        bool result = graph.RemoveNode(CSCD211);
        
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void RemoveNode_InvalidNode_ReturnsFalse()
    {
        Dag<string> graph = new();
        
        bool result = graph.RemoveNode(CSCD211);
        
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void RemoveNode_ExistingNode_RemovesAssociatedEdges()
    {
        Dag<string> graph = CreateCourseGraph();
        
        graph.RemoveNode(CSCD300);
        
        Assert.IsFalse(graph.ContainsNode(CSCD300));
        
        // 211 now only leads to 320
        Assert.AreSequenceEqual([CSCD320], graph.GetDependents(CSCD211));
        
        // 371 now only requires 320
        Assert.AreSequenceEqual([CSCD320], graph.GetDependencies(CSCD371));
    }

    #endregion
}