using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenericsHomework.Tests;

[TestClass]
public class VennDiagramTests
{
    private static readonly string[] ExpectedUnion = { "x", "y", "z" };

    [TestMethod]
    public void Intersection_ReturnsOnlyCommonItems_Success()
    {
        var vd = new VennDiagram<string>();
        var a = vd.AddCircle("A");
        var b = vd.AddCircle("B");
        a.Add("x"); 
        a.Add("y");
        b.Add("y"); 
        b.Add("z");

        var intersection = vd.Intersection("A", "B").ToList();
        Assert.AreEqual<int>(1, intersection.Count);
        Assert.AreEqual<string>("y", intersection[0]);
    }

    [TestMethod]
    public void Union_ReturnsAllUniqueItems_Success()
    {
        var vd = new VennDiagram<string>();
        var a = vd.AddCircle("A");
        var b = vd.AddCircle("B");
        a.Add("x"); 
        a.Add("y");
        b.Add("y"); 
        b.Add("z");

        var union = vd.Union("A", "B").OrderBy(s => s).ToList();
        CollectionAssert.AreEqual(ExpectedUnion, union);
    }

    [TestMethod]
    public void Difference_ReturnsOnlyLeftMinusRightItems_Success()
    {
        var vd = new VennDiagram<string>();
        var a = vd.AddCircle("A");
        var b = vd.AddCircle("B");
        a.Add("x"); 
        a.Add("y");
        b.Add("y"); 
        b.Add("z");

        var diff = vd.Difference("A", "B").ToList();
        Assert.AreEqual<int>(1, diff.Count);
        Assert.AreEqual<string>("x", diff[0]);
    }
}
