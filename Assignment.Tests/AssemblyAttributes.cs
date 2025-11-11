using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

[assembly: Parallelize(Scope = ExecutionScope.ClassLevel)]
namespace Assignment.Tests;
[TestClass]
public class PlaceholderTest
{
    [TestMethod]
    public void PlaceholderUsingLinqCount()
    {
        // Arrange
        var items = new List<int> { 1, 2, 3};
        // Act & Assert
        Assert.HasCount(3, items, "The list count check failed.");
    }
}
