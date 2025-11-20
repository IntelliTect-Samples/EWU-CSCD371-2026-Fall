using Assignment;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assignment.Tests;

[TestClass]
public class NodeTests
{
    [TestMethod]
    public void NewNode_HasSelfLoopInNext()
    {
        Node<int> n = new Node<int>(42);
        Assert.IsNotNull(n.Next);
        Assert.AreSame(n, n.Next);
    }

    [TestMethod]
    public void ToString_DelegatesToValueToString_ForValueTypes()
    {
        Node<int> n = new Node<int>(123);
        if (n == null) throw new ArgumentNullException(nameof(n));
        Assert.AreEqual<string>("123", n.ToString());
    }

    [TestMethod]
    public void ToString_HandlesNullableReferenceValue()
    {
        Node<string?> n = new Node<string?>(null);
        Assert.AreEqual<string>(string.Empty, n.ToString());
    }

    [TestMethod]
    public void Next_HasPrivateSetter()
    {
        Node<int> n = new Node<int>(1);
        Assert.AreSame(n, n.Next);
    }

    [TestMethod]
    public void Append_AddsNewNodeAfterCurrentNode_Success()
    {
        Node<string> n1 = new Node<string>("first");
        n1.Append("second");
        Assert.AreEqual("second", n1.Next.ToString());
        Assert.AreSame(n1, n1.Next.Next);
    }

    [TestMethod]
    public void Append_DuplicateOnHead_ThrowsInvalidOperationException()
    {
        // Arrange
        Node<int> n1 = new Node<int>(10);

        // Act
        void Act() { n1.Append(10); }

        // Assert
        Assert.ThrowsExactly<InvalidOperationException>(Act);
    }

    [TestMethod]
    public void Append_DuplicateInMiddle_ThrowsInvalidOperationException()
    {
        // Arrange
        Node<int> n1 = new Node<int>(10);
        n1.Append(20);
        n1.Append(30);

        // Act
        void Act() { n1.Append(20); }

        // Assert
        Assert.ThrowsExactly<InvalidOperationException>(Act);
    }

    [TestMethod]
    public void Append_DuplicateNullReference_ThrowsInvalidOperationException()
    {
        // Arrange
        Node<string?> n1 = new Node<string?>(null);
        n1.Append("x");

        // Act
        void Act() { n1.Append(null); }

        // Assert
        Assert.ThrowsExactly<InvalidOperationException>(Act);
    }

    [TestMethod]
    public void Clear_RemovesAllButCurrentNode_Success()
    {
        // arrange
        Node<int> n1 = new Node<int>(1);
        n1.Append(2);
        n1.Append(3);
        // act
        n1.Clear();
        // assert
        Assert.AreSame(n1, n1.Next);
    }

    [TestMethod]
    public void Clear_OnSingleNodeList_DoesNothing()
    {
        // arrange
        Node<int> n1 = new Node<int>(1);
        // act
        n1.Clear();
        // assert
        Assert.AreSame(n1, n1.Next);
    }

    [TestMethod]
    public void Exists_FindsHeadValue_ReturnsTrue()
    {
        // Arrange
        Node<int> n1 = new Node<int>(10);
        n1.Append(20);
        n1.Append(30);

        // Act
        bool found = n1.Exists(10);

        // Assert
        Assert.IsTrue(found);
    }

    [TestMethod]
    public void Exists_FindsMiddleValue_ReturnsTrue()
    {
        // Arrange
        Node<int> n1 = new Node<int>(10);
        n1.Append(20);
        n1.Append(30);

        // Act
        bool found = n1.Exists(20);

        // Assert
        Assert.IsTrue(found);
    }

    [TestMethod]
    public void Exists_FindsTailValue_ReturnsTrue()
    {
        // Arrange
        Node<int> n1 = new Node<int>(10);
        n1.Append(20);
        n1.Append(30);

        // Act
        bool found = n1.Exists(30);

        // Assert
        Assert.IsTrue(found);
    }

    [TestMethod]
    public void Exists_MissingValue_ReturnsFalse()
    {
        // Arrange
        Node<int> n1 = new Node<int>(10);
        n1.Append(20);
        n1.Append(30);

        // Act
        bool found = n1.Exists(40);

        // Assert
        Assert.IsFalse(found);
    }

    [TestMethod]
    public void Exists_SingleNodePresent_ReturnsTrue()
    {
        // Arrange
        Node<int> n = new Node<int>(7);

        // Act
        bool found = n.Exists(7);

        // Assert
        Assert.IsTrue(found);
    }

    [TestMethod]
    public void Exists_SingleNodeMissing_ReturnsFalse()
    {
        // Arrange
        Node<int> n = new Node<int>(7);

        // Act
        bool found = n.Exists(8);

        // Assert
        Assert.IsFalse(found);
    }

    [TestMethod]
    public void Exists_NullReferenceValue_ReturnsTrue()
    {
        // Arrange
        Node<string?> n = new Node<string?>(null);
        n.Append("x");

        // Act
        bool found = n.Exists(null);

        // Assert
        Assert.IsTrue(found);
    }

    [TestMethod]
    public void ChildItems_MaxGreaterThanChildCount_ReturnsAllChildItems()
    {
        // Arrange
        Node<int> n1 = new Node<int>(1);
        n1.Append(2);
        n1.Append(3);

        // Act
        List<int> result = n1.ChildItems(5).ToList();

        // Assert
        int[] expected = [3, 2];
        Assert.HasCount(expected.Length, result);
        Assert.IsTrue(result.Zip(expected, (a, b) => a == b).All(match => match));
    }

    [TestMethod]
    public void ChildItems_MaxLessThanChildCount_ReturnsUpToMaximum()
    {
        // Arrange
        Node<int> n1 = new Node<int>(1);
        n1.Append(2);
        n1.Append(3);
        n1.Append(4);
        n1.Append(5);

        // Act
        List<int> result = n1.ChildItems(2).ToList();

        // Assert
        int[] expected = [5, 4];
        Assert.HasCount(expected.Length, result);
        Assert.IsTrue(result.Zip(expected, (a, b) => a == b).All(match => match));
    }

    [TestMethod]
    public void ChildItems_MaximumZero_ReturnsEmptyCollection()
    {
        // Arrange
        Node<int> n1 = new Node<int>(1);
        n1.Append(2);
        n1.Append(3);
        
        // Act
        List<int> result = n1.ChildItems(0).ToList();

        // Assert
        Assert.IsEmpty(result);
    }

    [TestMethod]
    public void Count_SingleNode_ReturnsOne()
    {
        // Arrange
        Node<int> n = new Node<int>(1);
        if (n == null) throw new ArgumentNullException(nameof(n));

        // Act
        int count = n.Count;

        // Assert
        Assert.AreEqual(1, count);
    }

    [TestMethod]
    public void Count_MultipleNodes_ReturnsNumberOfNodes()
    {
        // Arrange
        Node<int> n = new Node<int>(1);
        n.Append(2);
        n.Append(3);

        // Act
        int count = n.Count;

        // Assert
        Assert.AreEqual(3, count);
    }

    [TestMethod]
    public void Count_AfterClear_ReturnsZero()
    {
        // Arrange
        Node<int> n = new Node<int>(1);
        n.Append(2);
        n.Append(3);
        n.Clear();

        // Act
        int count = n.Count;

        // Assert
        Assert.AreEqual(0, count);
    }

    [TestMethod]
    public void IsReadOnly_Always_ReturnsFalse()
    {
        // Arrange
        Node<int> n = new Node<int>(1);
        if (n == null) throw new ArgumentNullException(nameof(n));

        // Act
        bool isReadOnly = n.IsReadOnly;

        // Assert
        Assert.IsFalse(isReadOnly);
    }

    [TestMethod]
    public void ICollectionAdd_AddsItem_AppendsAfterHead()
    {
        // Arrange
        Node<int> n = new Node<int>(1);
        ICollection<int> collection = n;

        // Act
        collection.Add(2);

        // Assert
        int[] result = [.. n];
        int[] expected = [1, 2];
        Assert.HasCount(expected.Length, result);
        Assert.IsTrue(result.Zip(expected, (a, b) => a == b).All(match => match));
    }

    [TestMethod]
    public void Contains_ValuePresent_ReturnsTrue()
    {
        // Arrange
        Node<int> n = new Node<int>(1);
        n.Append(2);
        n.Append(3);

        // Act
        bool contains = n.Contains(2);

        // Assert
        Assert.IsTrue(contains);
    }

    [TestMethod]
    public void Contains_ValueMissing_ReturnsFalse()
    {
        // Arrange
        Node<int> n = new Node<int>(1);
        n.Append(2);
        n.Append(3);

        // Act
        bool contains = n.Contains(4);

        // Assert
        Assert.IsFalse(contains);
    }

    [TestMethod]
    public void CopyTo_NullArray_ThrowsArgumentNullException()
    {
        // Arrange
        Node<int> n = new Node<int>(1);
        if (n == null) throw new ArgumentNullException(nameof(n));

        // Act
        void Act() => n.CopyTo(null!, 0);

        // Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);
    }

    [TestMethod]
    public void CopyTo_NegativeIndex_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        Node<int> n = new Node<int>(1);
        if (n == null) throw new ArgumentNullException(nameof(n));
        int[] array = new int[3];

        // Act
        void Act() => n.CopyTo(array, -1);

        // Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(Act);
    }

    [TestMethod]
    public void CopyTo_InsufficientSpace_ThrowsArgumentException()
    {
        // Arrange
        Node<int> n = new Node<int>(1);
        n.Append(2);
        n.Append(3);
        int[] array = new int[2]; 

        // Act
        void Act() => n.CopyTo(array, 0);

        // Assert
        Assert.ThrowsExactly<ArgumentException>(Act);
    }

    [TestMethod]
    public void CopyTo_ValidArray_CopiesAllElementsInLogicalOrder()
    {
        // Arrange
        Node<int> n = new Node<int>(1);
        n.Append(2);
        n.Append(3);
        int[] array = new int[3];

        // Act
        n.CopyTo(array, 0);

        // Assert
        int[] expected = [1, 2, 3];
        Assert.HasCount(expected.Length, array);
        Assert.IsTrue(array.Zip(expected, (a, b) => a == b).All(match => match));
    }

    [TestMethod]
    public void Remove_SingleNode_RemovesAndMarksEmpty()
    {
        // Arrange
        Node<int> n = new Node<int>(1);

        // Act
        bool removed = n.Remove(1);

        // Assert
        Assert.IsTrue(removed);
        Assert.AreEqual(0, n.Count);
        Assert.AreEqual(string.Empty, n.ToString()); 
    }

    [TestMethod]
    public void Remove_ExistingMiddleNode_ReturnsTrueAndRemovesNode()
    {
        // Arrange
        Node<int> n = new Node<int>(1);
        n.Append(2);
        n.Append(3); 

        // Act
        bool removed = n.Remove(2);

        // Assert
        Assert.IsTrue(removed);
        int[] result = [.. n];
        int[] expected = [1, 3];
        Assert.HasCount(expected.Length, result);
        Assert.IsTrue(result.Zip(expected, (a, b) => a == b).All(match => match));
    }

    [TestMethod]
    public void Remove_MissingValue_ReturnsFalseAndListUnchanged()
    {
        // Arrange
        Node<int> n = new Node<int>(1);
        n.Append(2);
        n.Append(3);
        int[] original = [.. n];

        // Act
        bool removed = n.Remove(4);

        // Assert
        Assert.IsFalse(removed);
        int[] result = [.. n];
        Assert.HasCount(original.Length, result);
        Assert.IsTrue(result.Zip(original, (a, b) => a == b).All(match => match));
    }

    [TestMethod]
    public void GetEnumerator_MultipleNodes_EnumeratesAllValuesInOrder()
    {
        // Arrange
        Node<int> n = new Node<int>(1);
        n.Append(2);
        n.Append(3);

        // Act
        List<int> result = n.ToList();

        // Assert
        int[] expected = [1, 2, 3];
        Assert.HasCount(expected.Length, result);
        Assert.IsTrue(result.Zip(expected, (a, b) => a == b).All(match => match));
    }

    [TestMethod]
    public void GetEnumerator_AfterClear_YieldsNoElements()
    {
        // Arrange
        Node<int> n = new Node<int>(1);
        n.Append(2);
        n.Append(3);
        n.Clear();

        // Act
        List<int> result = n.ToList();

        // Assert
        Assert.IsEmpty(result);
    }

    [TestMethod]
    public void GetEnumerator_IteratingThroughNode_ReturnsAllItemsInOrder()
    {
        // Arrange
        Node<int> n1 = new Node<int>(1);
        n1.Append(2);
        n1.Append(3);

        // Act
        List<int> result = n1.ToList();

        // Assert
        int[] expected = [1, 2, 3];
        Assert.HasCount(expected.Length, result);
        Assert.IsTrue(result.Zip(expected, (a, b) => a == b).All(match => match));
    }

    [TestMethod]
    public void ChildItems_EmptyNode_ReturnsEmptyCollection()
    {
        // Arrange
        Node<int> n1 = new Node<int>(1);
        n1.Append(2);
        n1.Clear(); 

        // Act
        List<int> result = n1.ChildItems(5).ToList();

        // Assert
        Assert.IsEmpty(result);
    }
}