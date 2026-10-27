using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dag.Tests;

[TestClass]
public class NodeEqualityTests
{
    [TestMethod]
    public void AddDependency_ReferenceEquality_KeepsEqualLookingInstancesDistinct()
    {
        Dag<Course> graph = new();
        Course first = new(211);
        Course second = new(211);

        Assert.AreEqual(first.Number, second.Number);
        Assert.IsTrue(graph.AddNode(first));
        Assert.IsTrue(graph.AddNode(second));
        Assert.IsFalse(graph.AddNode(first));
        Assert.IsTrue(graph.AddDependency(first, second));

        Assert.AreSame(first, Assert.ContainsSingle(graph.GetDependencies(second)));
        Assert.AreSame(second, Assert.ContainsSingle(graph.GetDependents(first)));
        Assert.IsTrue(graph.RemoveNode(first));
        Assert.IsEmpty(graph.GetDependencies(second));
        Assert.IsEmpty(graph.GetDependents(second));
    }

    [TestMethod]
    public void AddDependency_ReadonlyRecordStruct_UsesValueEqualityIncludingDefaultValue()
    {
        Dag<CourseId> graph = new();
        CourseId prerequisite = default;
        CourseId course = new(211);
        graph.AddNode(prerequisite);
        graph.AddNode(course);

        Assert.IsFalse(graph.AddNode(new CourseId(0)));
        Assert.IsTrue(graph.AddDependency(new CourseId(0), new CourseId(211)));
        CollectionAssert.AreEquivalent(new[] { prerequisite }, graph.GetDependencies(course));
        CollectionAssert.AreEquivalent(new[] { course }, graph.GetDependents(prerequisite));
        Assert.ThrowsExactly<InvalidOperationException>(
            () => graph.AddDependency(new CourseId(211), new CourseId(0)));

        Assert.IsTrue(graph.RemoveNode(new CourseId(0)));
        Assert.IsEmpty(graph.GetDependencies(course));
    }

    [TestMethod]
    public void AddDependency_HashCollisions_PreservesDistinctNodesAndDetectsCycle()
    {
        Dag<CollidingCourse> graph = new();
        CollidingCourse first = new(211);
        CollidingCourse second = new(300);
        CollidingCourse third = new(371);
        graph.AddNode(first);
        graph.AddNode(second);
        graph.AddNode(third);
        graph.AddDependency(first, second);
        graph.AddDependency(second, third);

        Assert.IsFalse(graph.AddNode(new CollidingCourse(211)));
        Assert.IsFalse(graph.AddDependency(new CollidingCourse(211), new CollidingCourse(300)));
        CollectionAssert.AreEquivalent(new[] { second }, graph.GetDependencies(third));
        CollectionAssert.AreEquivalent(new[] { second }, graph.GetDependents(first));
        Assert.ThrowsExactly<InvalidOperationException>(() => graph.AddDependency(third, first));

        Assert.IsTrue(graph.RemoveNode(new CollidingCourse(300)));
        Assert.IsTrue(graph.AddDependency(third, first));
    }

    private sealed class Course(int number)
    {
        public int Number { get; } = number;
    }

    private readonly record struct CourseId(int Number);

    private sealed record class CollidingCourse(int Number)
    {
        public override int GetHashCode() => 0;
    }
}
