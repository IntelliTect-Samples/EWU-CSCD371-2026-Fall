using Dag;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Dag.Tests;

[TestClass]
public class GetTests
{
    [TestMethod]
    public void GetDependenciesThrowsExceptionOnMissingElement()
    {
        var dag = new Dag<int>();
        var ex = Assert.Throws<InvalidOperationException>(() => dag.GetDependencies(5));

        Assert.AreEqual("The node does not exist in the graph.", ex.Message);
    }

    [TestMethod]
    public void GetDependentsThrowsExceptionOnMissingElement()
    {
        var dag = new Dag<int>();
        var ex = Assert.Throws<InvalidOperationException>(() => dag.GetDependents(5));

        Assert.AreEqual("The node does not exist in the graph.", ex.Message);
    }
}
