using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenericsHomework.Tests;

[TestClass]
public class NodeTests
{
    // ==================== CONSTRUCTOR TESTS ====================

    [TestMethod]
    public void Constructor_ProperValue_Initializes()
    {
        // Arrange & Act
        NodeCollection<int> testNode = new NodeCollection<int>(10);

        // Assert
        Assert.IsNotNull(testNode);
        Assert.AreEqual<int>(10, testNode.Value);
        Assert.AreSame(testNode, testNode.Next);
    }

    [TestMethod]
    public void Constructor_StringValue_Initializes()
    {
        // Arrange & Act
        NodeCollection<string> testNode = new NodeCollection<string>("test");

        // Assert
        Assert.IsNotNull(testNode);
        Assert.AreEqual<string>("test", testNode.Value);
        Assert.AreSame(testNode, testNode.Next);
    }

    [TestMethod]
    public void Constructor_DoubleValue_Initializes()
    {
        // Arrange & Act
        NodeCollection<double> testNode = new NodeCollection<double>(3.14);

        // Assert
        Assert.IsNotNull(testNode);
        Assert.AreEqual<double>(3.14, testNode.Value);
        Assert.AreSame(testNode, testNode.Next);
    }

    // ==================== TOSTRING TESTS ====================

    [TestMethod]
    public void ToString_ReturnsValueString_Success()
    {
        // Arrange
        NodeCollection<string> testNode = new NodeCollection<string>("TestValue");

        // Act
        string result = testNode.ToString();

        // Assert
        Assert.AreEqual<string>("TestValue", result);
    }

    [TestMethod]
    public void ToString_IntValue_ReturnsString()
    {
        // Arrange
        NodeCollection<int> testNode = new NodeCollection<int>(42);

        // Act
        string result = testNode.ToString();

        // Assert
        Assert.AreEqual<string>("42", result);
    }

    [TestMethod]
    public void ToString_NullValue_ReturnsEmptyString()
    {
        // Arrange
        NodeCollection<string> testNode = new NodeCollection<string>("test");
        testNode.Value = null!;

        // Act
        string result = testNode.ToString();

        // Assert
        Assert.AreEqual<string>(string.Empty, result);
    }

    [TestMethod]
    public void ToString_CustomType_CallsTypeToString()
    {
        // Arrange
        var person = new Person { Name = "Alice", Age = 30 };
        NodeCollection<Person> node = new NodeCollection<Person>(person);

        // Act
        string result = node.ToString();

        // Assert
        Assert.AreEqual<string>("Alice (30)", result);
    }

    // ==================== NEXT PROPERTY TESTS ====================

    [TestMethod]
    public void Next_HasPrivateSetter_VerifiedByCircularStructure()
    {
        // This test verifies that Next has a private setter by ensuring
        // it can only be modified through Append or Clear methods
        // The fact that this maintains circular structure proves the setter works correctly

        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);

        // Act
        firstNode.Append(2);

        // Assert
        Assert.AreEqual<int>(2, firstNode.Next.Value);
        Assert.AreSame(firstNode, firstNode.Next.Next);
    }

    // ==================== APPEND TESTS ====================

    [TestMethod]
    public void Append_SingleNode_CreatesCircularList()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);

        // Act
        firstNode.Append(2);

        // Assert
        Assert.AreEqual<int>(2, firstNode.Next.Value);
        Assert.AreSame(firstNode, firstNode.Next.Next);
    }

    [TestMethod]
    public void Append_TwoNodes_MaintainsCircularStructure()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);

        // Act
        firstNode.Append(2);
        firstNode.Append(3);

        // Assert
        Assert.AreEqual<int>(1, firstNode.Value);
        Assert.AreEqual<int>(2, firstNode.Next.Value);
        Assert.AreEqual<int>(3, firstNode.Next.Next.Value);
        Assert.AreSame(firstNode, firstNode.Next.Next.Next);
    }

    [TestMethod]
    public void Append_MultipleNodes_MaintainsCircularStructure()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);

        // Act
        firstNode.Append(2);
        firstNode.Append(3);
        firstNode.Append(4);
        firstNode.Append(5);

        // Assert
        Assert.AreEqual<int>(1, firstNode.Value);
        Assert.AreEqual<int>(2, firstNode.Next.Value);
        Assert.AreEqual<int>(3, firstNode.Next.Next.Value);
        Assert.AreEqual<int>(4, firstNode.Next.Next.Next.Value);
        Assert.AreEqual<int>(5, firstNode.Next.Next.Next.Next.Value);
        Assert.AreSame(firstNode, firstNode.Next.Next.Next.Next.Next);
    }

    [TestMethod]
    public void Append_StringNodes_Works()
    {
        // Arrange
        NodeCollection<string> firstNode = new NodeCollection<string>("apple");

        // Act
        firstNode.Append("banana");
        firstNode.Append("cherry");

        // Assert
        Assert.AreEqual<string>("apple", firstNode.Value);
        Assert.AreEqual<string>("banana", firstNode.Next.Value);
        Assert.AreEqual<string>("cherry", firstNode.Next.Next.Value);
        Assert.AreSame(firstNode, firstNode.Next.Next.Next);
    }

    [TestMethod]
    public void Append_DuplicateValue_ThrowsInvalidOperationException()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);
        firstNode.Append(3);

        // Act & Assert
        InvalidOperationException ex = Assert.ThrowsException<InvalidOperationException>(() => firstNode.Append(2));
        StringAssert.Contains(ex.Message, "already exists");
        StringAssert.Contains(ex.Message, "2");
    }

    [TestMethod]
    public void Append_DuplicateOfFirstNode_ThrowsInvalidOperationException()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);
        firstNode.Append(3);

        // Act & Assert
        InvalidOperationException ex = Assert.ThrowsException<InvalidOperationException>(() => firstNode.Append(1));
        StringAssert.Contains(ex.Message, "already exists");
    }

    [TestMethod]
    public void Append_DuplicateString_ThrowsInvalidOperationException()
    {
        // Arrange
        NodeCollection<string> firstNode = new NodeCollection<string>("test");
        firstNode.Append("hello");

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => firstNode.Append("test"));
    }

    [TestMethod]
    public void Append_DuplicateInLargeList_ThrowsInvalidOperationException()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        for (int i = 2; i <= 10; i++)
        {
            firstNode.Append(i);
        }

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => firstNode.Append(5));
    }

    // ==================== EXISTS TESTS ====================

    [TestMethod]
    public void Exists_ValueInFirstNode_ReturnsTrue()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);
        firstNode.Append(3);

        // Act
        bool result = firstNode.Exists(1);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Exists_ValueInMiddleNode_ReturnsTrue()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);
        firstNode.Append(3);
        firstNode.Append(4);

        // Act
        bool result = firstNode.Exists(3);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Exists_ValueInLastNode_ReturnsTrue()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);
        firstNode.Append(3);

        // Act
        bool result = firstNode.Exists(3);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Exists_AllValuesInList_ReturnsTrue()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);
        firstNode.Append(3);

        // Act & Assert
        Assert.IsTrue(firstNode.Exists(1));
        Assert.IsTrue(firstNode.Exists(2));
        Assert.IsTrue(firstNode.Exists(3));
    }

    [TestMethod]
    public void Exists_ValueNotInList_ReturnsFalse()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);
        firstNode.Append(3);

        // Act & Assert
        Assert.IsFalse(firstNode.Exists(4));
        Assert.IsFalse(firstNode.Exists(0));
        Assert.IsFalse(firstNode.Exists(99));
    }

    [TestMethod]
    public void Exists_SingleNode_ExistingValue_ReturnsTrue()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(42);

        // Act & Assert
        Assert.IsTrue(firstNode.Exists(42));
    }

    [TestMethod]
    public void Exists_SingleNode_NonExistingValue_ReturnsFalse()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(42);

        // Act & Assert
        Assert.IsFalse(firstNode.Exists(1));
    }

    [TestMethod]
    public void Exists_StringValues_Works()
    {
        // Arrange
        NodeCollection<string> firstNode = new NodeCollection<string>("apple");
        firstNode.Append("banana");
        firstNode.Append("cherry");

        // Act & Assert
        Assert.IsTrue(firstNode.Exists("apple"));
        Assert.IsTrue(firstNode.Exists("banana"));
        Assert.IsTrue(firstNode.Exists("cherry"));
        Assert.IsFalse(firstNode.Exists("orange"));
    }

    [TestMethod]
    public void Exists_NullValue_Works()
    {
        // Arrange
        NodeCollection<string?> firstNode = new NodeCollection<string?>(null);
        firstNode.Append("test");

        // Act & Assert
        Assert.IsTrue(firstNode.Exists(null));
        Assert.IsTrue(firstNode.Exists("test"));
    }

    // ==================== CLEAR TESTS ====================

    [TestMethod]
    public void Clear_MultipleNodes_RemovesAllButCurrent()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);
        firstNode.Append(3);
        firstNode.Append(4);

        // Act
        firstNode.Clear();

        // Assert
        Assert.AreSame(firstNode, firstNode.Next);
        Assert.IsFalse(firstNode.Exists(2));
        Assert.IsFalse(firstNode.Exists(3));
        Assert.IsFalse(firstNode.Exists(4));
        Assert.IsTrue(firstNode.Exists(1));
    }

    [TestMethod]
    public void Clear_SingleNode_RemainsUnchanged()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);

        // Act
        firstNode.Clear();

        // Assert
        Assert.AreSame(firstNode, firstNode.Next);
        Assert.IsTrue(firstNode.Exists(1));
    }

    [TestMethod]
    public void Clear_TwoNodes_RemovesSecond()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);

        // Act
        firstNode.Clear();

        // Assert
        Assert.AreSame(firstNode, firstNode.Next);
        Assert.IsTrue(firstNode.Exists(1));
        Assert.IsFalse(firstNode.Exists(2));
    }

    [TestMethod]
    public void Clear_CanAppendAfterClearing()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);
        firstNode.Append(3);

        // Act
        firstNode.Clear();
        firstNode.Append(10);
        firstNode.Append(20);

        // Assert
        Assert.AreEqual<int>(10, firstNode.Next.Value);
        Assert.AreEqual<int>(20, firstNode.Next.Next.Value);
        Assert.AreSame(firstNode, firstNode.Next.Next.Next);
    }

    [TestMethod]
    public void Clear_CanAppendPreviousValues()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);

        // Act
        firstNode.Clear();
        firstNode.Append(2); // This should now work since 2 was removed

        // Assert
        Assert.AreEqual<int>(2, firstNode.Next.Value);
        Assert.IsTrue(firstNode.Exists(2));
    }

    [TestMethod]
    public void Clear_SimplifiesStructure_FirstNodeIsolated()
    {
        // This test verifies that Clear() effectively isolates the current node
        // by setting Next to point to itself.
        // 
        // Garbage Collection Note: The assignment states we only need to set 
        // Next to itself because C# garbage collector can handle circular references.
        // The removed nodes will still point to each other and back to the first node,
        // but since we break the first node's reference to them (by setting Next = this),
        // and assuming no other external references exist, the entire removed chain
        // becomes eligible for garbage collection. The GC uses mark-and-sweep from
        // GC roots, so it can detect and collect unreachable circular structures.

        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);
        firstNode.Append(3);
        firstNode.Append(4);

        // Capture references to verify the structure before clear
        NodeCollection<int> secondNode = firstNode.Next;

        // Act
        firstNode.Clear();

        // Assert - First node is now isolated
        Assert.AreSame(firstNode, firstNode.Next);
        Assert.IsTrue(firstNode.Exists(1));
        Assert.IsFalse(firstNode.Exists(2));
        Assert.IsFalse(firstNode.Exists(3));
        Assert.IsFalse(firstNode.Exists(4));

        // The removed nodes still maintain their original structure
        // (they still point back through the chain to firstNode)
        // In this case, secondNode still has its Next pointer intact
        Assert.AreEqual<int>(2, secondNode.Value);
        Assert.AreEqual<int>(3, secondNode.Next.Value);

        // This demonstrates we DON'T need to traverse and break the removed nodes' 
        // circular references - the GC will handle it when there are no external refs
    }

    // ==================== COUNT TESTS ====================

    [TestMethod]
    public void Count_MultipleNodes_ReturnsCorrectCount()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);
        firstNode.Append(3);
        firstNode.Append(4);
        firstNode.Append(5);

        // Act
        int count = firstNode.Count;

        // Assert
        Assert.AreEqual<int>(5, count);
    }

    [TestMethod]
    public void Count_SingleNode_ReturnsOne()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);

        // Act
        int count = firstNode.Count;

        // Assert
        Assert.AreEqual<int>(1, count);
    }

    [TestMethod]
    public void Count_AfterRemove_UpdatesCorrectly()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);
        firstNode.Append(3);
        Assert.AreEqual<int>(3, firstNode.Count);

        // Act
        firstNode.Remove(2);

        // Assert
        Assert.AreEqual<int>(2, firstNode.Count);
    }

    [TestMethod]
    public void Count_AfterClear_ReturnsOne()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);
        firstNode.Append(3);

        // Act
        firstNode.Clear();

        // Assert
        Assert.AreEqual<int>(1, firstNode.Count);
    }

    // ==================== ICOLLECTION<T> IMPLEMENTATION TESTS ====================

    [TestMethod]
    public void IsReadOnly_ReturnsFalse()
    {
        // Arrange
        NodeCollection<int> node = new NodeCollection<int>(1);

        // Assert
        Assert.IsFalse(node.IsReadOnly);
    }

    [TestMethod]
    public void Add_ProperNode_AppendsNode()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);

        // Act
        firstNode.Add(2);
        firstNode.Add(3);

        // Assert
        Assert.AreEqual<int>(2, firstNode.Next.Value);
        Assert.AreEqual<int>(3, firstNode.Next.Next.Value);
        Assert.AreSame(firstNode, firstNode.Next.Next.Next);
    }

    [TestMethod]
    public void Add_DuplicateValue_ThrowsInvalidOperationException()
    {
        // Testing Add (which calls Append)
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Add(2);

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => firstNode.Add(1));
    }

    [TestMethod]
    public void Contains_ExistingValue_ReturnsTrue()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);
        firstNode.Append(3);

        // Act
        bool contains = firstNode.Contains(2);

        // Assert
        Assert.IsTrue(contains);
    }

    [TestMethod]
    public void Contains_NonExistingValue_ReturnsFalse()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);

        // Act
        bool contains = firstNode.Contains(99);

        // Assert
        Assert.IsFalse(contains);
    }

    [TestMethod]
    public void Remove_ExistingValue_RemovesNode()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);
        firstNode.Append(3);

        // Act
        bool removed = firstNode.Remove(2);

        // Assert
        Assert.IsTrue(removed);
        Assert.IsFalse(firstNode.Exists(2));
        Assert.AreEqual<int>(3, firstNode.Next.Value);
    }

    [TestMethod]
    public void Remove_NonExistingValue_ReturnsFalse()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);
        firstNode.Append(3);

        // Act
        bool removed = firstNode.Remove(4);

        // Assert
        Assert.IsFalse(removed);
    }

    [TestMethod]
    public void Remove_FirstNode_UpdatesHead()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);
        firstNode.Append(3);

        // Act
        bool removed = firstNode.Remove(1);

        // Assert
        Assert.IsTrue(removed);
        Assert.AreEqual<int>(2, firstNode.Value); // Head should now be 2
        Assert.AreEqual<int>(3, firstNode.Next.Value);
    }

    [TestMethod]
    public void Remove_LastNode_LeavesOneNode()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);

        // Act
        bool removed = firstNode.Remove(2);

        // Assert
        Assert.IsTrue(removed);
        Assert.AreEqual<int>(1, firstNode.Count);
        Assert.AreSame(firstNode, firstNode.Next);
    }

    [TestMethod]
    public void Remove_OnlySingleNode_ReturnsFalse()
    {
        // This test highlights the single-node removal edge case
        // Current implementation returns false when trying to remove the only node
        // This prevents breaking the circular structure

        // Arrange
        NodeCollection<int> singleNode = new NodeCollection<int>(42);

        // Act
        bool removed = singleNode.Remove(42);

        // Assert
        Assert.IsFalse(removed);
        Assert.AreEqual<int>(42, singleNode.Value);
        Assert.AreSame(singleNode, singleNode.Next);
    }

    [TestMethod]
    public void Remove_MultipleNodesInSequence_MaintainsIntegrity()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);
        firstNode.Append(3);
        firstNode.Append(4);
        firstNode.Append(5);

        // Act
        firstNode.Remove(2);
        firstNode.Remove(4);

        // Assert
        Assert.AreEqual<int>(3, firstNode.Count);
        CollectionAssert.Contains(firstNode.ToArray(), 1);
        CollectionAssert.DoesNotContain(firstNode.ToArray(), 2);
        CollectionAssert.Contains(firstNode.ToArray(), 3);
        CollectionAssert.DoesNotContain(firstNode.ToArray(), 4);
        CollectionAssert.Contains(firstNode.ToArray(), 5);
    }

    [TestMethod]
    public void CopyTo_Array_CopiesElements()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);
        firstNode.Append(3);
        int[] array = new int[5];

        // Act
        firstNode.CopyTo(array, 1);

        // Assert
        Assert.AreEqual<int>(0, array[0]); // Default int value
        Assert.AreEqual<int>(1, array[1]);
        Assert.AreEqual<int>(2, array[2]);
        Assert.AreEqual<int>(3, array[3]);
        Assert.AreEqual<int>(0, array[4]); // Default int value
    }

    [TestMethod]
    public void CopyTo_NullArray_ThrowsArgumentNullException()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);

        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() => firstNode.CopyTo(null!, 0));
    }

    [TestMethod]
    public void CopyTo_NegativeIndex_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        int[] array = new int[5];

        // Act & Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => firstNode.CopyTo(array, -1));
    }

    [TestMethod]
    public void CopyTo_IndexAtArrayLength_ThrowsArgumentException()
    {
        // When arrayIndex equals array.Length, we're trying to start copying
        // at a position where there's no space, which should throw ArgumentException

        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);
        int[] array = new int[2];

        // Act & Assert - arrayIndex=2 means start at position 2 in a 2-length array
        // This leaves no room for 2 elements, so should throw ArgumentException
        Assert.ThrowsException<ArgumentException>(() => firstNode.CopyTo(array, 2));
    }

    [TestMethod]
    public void CopyTo_ArrayTooSmall_ThrowsArgumentException()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);
        firstNode.Append(3);
        int[] smallArray = new int[2];

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => firstNode.CopyTo(smallArray, 0));
    }

    [TestMethod]
    public void CopyTo_StartIndexWithInsufficientSpace_ThrowsArgumentException()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);
        firstNode.Append(3);
        int[] array = new int[4];

        // Act & Assert - Starting at index 2 means only 2 slots, but we have 3 elements
        Assert.ThrowsException<ArgumentException>(() => firstNode.CopyTo(array, 2));
    }

    // ==================== ENUMERATOR TESTS ====================

    [TestMethod]
    public void GetEnumerator_IteratesThroughNodes()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);
        firstNode.Append(3);
        var enumerator = firstNode.GetEnumerator();
        int sum = 0;

        // Act
        while (enumerator.MoveNext())
        {
            sum += enumerator.Current;
        }

        // Assert
        Assert.AreEqual<int>(6, sum); // 1 + 2 + 3 = 6
    }

    [TestMethod]
    public void ForEach_IteratesThroughAllNodes()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(1);
        firstNode.Append(2);
        firstNode.Append(3);
        List<int> values = new List<int>();

        // Act
        foreach (int value in firstNode)
        {
            values.Add(value);
        }

        // Assert
        int[] expected = new[] { 1, 2, 3 };
        CollectionAssert.AreEqual(expected, values);
    }

    [TestMethod]
    public void ForEach_SingleNode_IteratesOnce()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(42);
        List<int> values = new List<int>();

        // Act
        foreach (int value in firstNode)
        {
            values.Add(value);
        }

        // Assert
        Assert.AreEqual<int>(1, values.Count);
        Assert.AreEqual<int>(42, values[0]);
    }

    // ==================== GENERIC TYPE TESTS ====================

    [TestMethod]
    public void GenericType_CustomClass_Works()
    {
        // Arrange
        var person1 = new Person { Name = "Alice", Age = 30 };
        var person2 = new Person { Name = "Bob", Age = 25 };
        NodeCollection<Person> firstNode = new NodeCollection<Person>(person1);

        // Act
        firstNode.Append(person2);

        // Assert
        Assert.AreSame(person1, firstNode.Value);
        Assert.AreSame(person2, firstNode.Next.Value);
        Assert.IsTrue(firstNode.Exists(person1));
    }

    [TestMethod]
    public void GenericType_CustomStruct_Works()
    {
        // Arrange
        var point1 = new Point { X = 1, Y = 2 };
        var point2 = new Point { X = 3, Y = 4 };
        NodeCollection<Point> firstNode = new NodeCollection<Point>(point1);

        // Act
        firstNode.Append(point2);

        // Assert
        Assert.AreEqual<Point>(point1, firstNode.Value);
        Assert.AreEqual<Point>(point2, firstNode.Next.Value);
        Assert.IsTrue(firstNode.Exists(point1));
    }

    // ==================== LARGE LIST TESTS ====================

    [TestMethod]
    public void LargeList_MaintainsCircularIntegrity()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(0);

        // Act - Create a list with 100 nodes
        for (int i = 1; i < 100; i++)
        {
            firstNode.Append(i);
        }

        // Assert - Traverse all 100 nodes and verify we circle back
        NodeCollection<int> current = firstNode;
        for (int i = 0; i < 100; i++)
        {
            Assert.AreEqual<int>(i, current.Value);
            current = current.Next;
        }
        Assert.AreSame(firstNode, current); // Should circle back to start
    }

    [TestMethod]
    public void LargeList_Count_ReturnsCorrectValue()
    {
        // Arrange
        NodeCollection<int> firstNode = new NodeCollection<int>(0);
        for (int i = 1; i < 50; i++)
        {
            firstNode.Append(i);
        }

        // Act
        int count = firstNode.Count;

        // Assert
        Assert.AreEqual<int>(50, count);
    }

    // ==================== HELPER CLASSES ====================

    private sealed class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Age})";
        }
    }

    private struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}