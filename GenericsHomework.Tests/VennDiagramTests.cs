namespace GenericsHomework.Tests;

[TestClass]
public class VennDiagramTests
{
    [TestMethod]
    public void Add_And_Contains_WorkCorrectly()
    {
        var circle = new Circle<string>("A");
        circle.Add("1");
        circle.Add("2");
        circle.Add("3");

        Assert.IsTrue(circle.Contains("1"));
        Assert.IsTrue(circle.Contains("2"));
        Assert.IsTrue(circle.Contains("3"));
    }

    [TestMethod]
    public void GetIntersection_MultipleCircles_ReturnsSharedItems()
    {
        var diagram = new VennDiagram<string>();

        var circleA = new Circle<string>("A");
        circleA.Add("apple");
        circleA.Add("banana");
        circleA.Add("cherry");

        var circleB = new Circle<string>("B");
        circleB.Add("apple");
        circleB.Add("coconut");
        circleB.Add("cherry");
        circleB.Add("mango");

        var circleC = new Circle<string>("C");
        circleC.Add("apple");
        circleC.Add("banana");
        circleC.Add("cherry");
        circleC.Add("carrot");

        diagram.AddCircle(circleA);
        diagram.AddCircle(circleB);
        diagram.AddCircle(circleC);

        var result = diagram.GetIntersection("A", "B", "C").ToList();

        CollectionAssert.AreEquivalent(new List<string> { "apple", "cherry" }, result);
    }

    [TestMethod]
    public void GetUnion_MultipleCircles_ReturnsAllItems()
    {
        var diagram = new VennDiagram<string>();

        var circleA = new Circle<string>("A");
        circleA.Add("apple");
        circleA.Add("banana");
        circleA.Add("cherry");

        var circleB = new Circle<string>("B");
        circleB.Add("apple");
        circleB.Add("coconut");
        circleB.Add("cherry");
        circleB.Add("mango");

        var circleC = new Circle<string>("C");
        circleC.Add("apple");
        circleC.Add("banana");
        circleC.Add("cherry");
        circleC.Add("carrot");

        diagram.AddCircle(circleA);
        diagram.AddCircle(circleB);
        diagram.AddCircle(circleC);

        var result = diagram.GetUnion("A", "B", "C").ToList();

        CollectionAssert.AreEquivalent(new List<string> { "apple", "cherry", "banana", "coconut", "mango", "carrot" }, result);
    }
}
