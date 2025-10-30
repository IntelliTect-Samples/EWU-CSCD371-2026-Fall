namespace GenericsHomeWork.Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenericsHomeWork;
using System.Collections.Generic;
using System.Linq;

[TestClass]
public class VennDiagramTests
{
    private Circle<string> _circleRed = null!;
    private Circle<string> _circleGreen = null!;
    private Circle<string> _circleRound = null!;
    private VennDiagram<string> _diagram = null!;

    [TestInitialize]
    public void Setup()
    {
        _circleRed = new Circle<string>("RedItems");
        _circleRed.Add("Apple");
        _circleRed.Add("Strawberry");
        _circleRed.Add("Grapes");
        _circleRed.Add("Tomato");
        _circleRed.Add("Cherry");
        _circleRed.Add("Bell Pepper");
        _circleRed.Add("Wagon");

        _circleGreen = new Circle<string>("GreenItems");
        _circleGreen.Add("Kiwi");
        _circleGreen.Add("Apple");
        _circleGreen.Add("Limes");
        _circleGreen.Add("Grapes");
        _circleGreen.Add("Tomato");
        _circleGreen.Add("Bell Pepper");
        _circleGreen.Add("Moss");

        _circleRound = new Circle<string>("RoundItems");
        _circleRound.Add("Tomato");
        _circleRound.Add("Cherry");
        _circleRound.Add("Kiwi");
        _circleRound.Add("Grapes");
        _circleRound.Add("Limes");
        _circleRound.Add("Orange");
        _circleRound.Add("Watermelon");

        _diagram = new VennDiagram<string>();
        _diagram.AddCircle(_circleRed);
        _diagram.AddCircle(_circleGreen);
        _diagram.AddCircle(_circleRound);
    }

    [TestMethod]
    public void Circle_AddAndContains_WorksCorrectly()
    {
        var circle = new Circle<string>("TestCircle");
        circle.Add("Item1");
        circle.Add("Item2");
        Assert.IsTrue(circle.Contains("Item1"), "Circle should contain 'Item1'.");
        Assert.IsTrue(circle.Contains("Item2"), "Circle should contain 'Item2'.");
        Assert.IsFalse(circle.Contains("Item3"), "Circle should not contain 'Item3'.");
    }

    [TestMethod]
    public void Circle_Add_PreventDuplicateItems()
    {
        var circle = new Circle<string>("TestCircle");
        circle.Add("Orange");
        circle.Add("Orange");
        Assert.AreEqual(1, circle.GetItems().Count(), "Circle should prevent duplicates due to internal HashSet.");
    }

    [TestMethod]
    public void FindIntersection_RedAndGreenCircles_ReturnsCommonItems()
    {
        var expected = new List<string> { "Grapes", "Bell Pepper", "Apple", "Tomato" };
        var actual = _diagram.FindIntersection(_circleRed, _circleGreen).ToList();
        CollectionAssert.AreEquivalent(expected, actual, "Intersection of Red and Green Items should be Grapes, Bell Pepper, Apple, and Tomato.");
    }

    [TestMethod]
    public void FindIntersection_RedAndRoundCircles_ReturnsCommonItems()
    {
        var expected = new List<string> { "Tomato", "Cherry", "Grapes" };
        var actual = _diagram.FindIntersection(_circleRed, _circleRound).ToList();
        CollectionAssert.AreEquivalent(expected, actual, "Intersection of Red and Round Items should be Tomato, Cherry, and Grapes.");
    }

    [TestMethod]
    public void FindIntersection_GreenAndRoundCircles_ReturnsCommonItems()
    {
        var expected = new List<string> { "Tomato", "Kiwi", "Limes", "Grapes" };
        var actual = _diagram.FindIntersection(_circleGreen, _circleRound).ToList();
        CollectionAssert.AreEquivalent(expected, actual, "Intersection of Green and Round Items should be Tomato, Kiwi, Limes, and Grapes.");
    }

    [TestMethod]
    public void FindIntersection_ThreeCircles_ReturnsCommonElements()
    {
        var expected = new List<string> { "Tomato", "Grapes" };
        var actual = _diagram.FindIntersection(_circleRed, _circleGreen, _circleRound).ToList();
        CollectionAssert.AreEquivalent(expected, actual, "Intersection of Red, Green, and Round Items should be Tomato and Grapes.");
    }

    [TestMethod]
    public void FindUnion_AllCircles_ReturnsAllUniqueItems()
    {
        var expected = new List<string>
        {
            "Apple", "Strawberry", "Grapes", "Tomato", "Cherry", "Bell Pepper", "Wagon",
            "Kiwi", "Limes", "Moss", "Orange", "Watermelon"
        };
        var actual = _diagram.FindUnion().ToList();
        CollectionAssert.AreEquivalent(expected, actual, "Union of all circles should contain all unique items.");
    }

    [TestMethod]
    public void FindUniqueToCircle_RedCircle_ReturnsUniqueItems()
    {
        var expected = new List<string> {"Wagon", "Strawberry"};
        var actual = _diagram.FindUniqueToCircle(_circleRed).ToList();
        CollectionAssert.AreEquivalent(expected, actual, "Unique items to Red Circle should be Wagon, and Strawberry.");
    }

    [TestMethod]
    public void FindUniqueToCircle_GreenCircle_ReturnsUniqueItems()
    {
        var expected = new List<string> {"Moss"};
        var actual = _diagram.FindUniqueToCircle(_circleGreen).ToList();
        CollectionAssert.AreEquivalent(expected, actual, "Unique items to Green Circle should be Moss.");
    }

    [TestMethod]
    public void FindUniqueToCircle_RoundCircle_ReturnsUniqueItems()
    {
        var expected = new List<string> { "Orange", "Watermelon" };
        var actual = _diagram.FindUniqueToCircle(_circleRound).ToList();
        CollectionAssert.AreEquivalent(expected, actual, "Unique items to Round Circle should be Orange and Watermelon.");
    }

    [TestMethod]
    public void FindIntersection_NoCircles_ReturnsEmpty()
    {
        var actual = _diagram.FindIntersection().ToList();
        Assert.AreEqual(0, actual.Count, "Intersection with no circles should return empty.");
    }
}

