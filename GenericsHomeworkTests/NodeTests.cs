using GenericsHomework;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace GenericsHomeworkTests;

public class NodeTests
{
    public record class TestClass(string Name, int Id);

    [Fact]
    public void NameSingleNodeLinkedListNextIsSetToSelf()
    {
        //Arrange
        int input = 0;
        Node<int> node = new(input);

        //Act
        Node<int> next = node.Next;

        //Assert
        Assert.Equal<Node<int>>(node, next);
    }
    [Fact]
    public void NodeInputIsTestClassNodeIsAbleToStoreData()
    {
        //Arrange
        TestClass testClass = new("GaryTheSentientTestClass", 1234);

        //Act
        Node<TestClass> node = new(testClass);

        //Assert
        Assert.Equal<TestClass>(testClass, node.Data);
    }
    [Fact]
    public void AppendInputIntFirstNodeNextIsSecondNode ()
    {
        //Arrange
        int nodeTwoData = 1;
        Node<int> node = new(0);

        //Act
        node.Append(nodeTwoData);

        //Assert
        Assert.Equal<int>(1, node.Next.Data);
    }
    [Fact]
    public void AppendInputTestClassFirstNodeNextIsSecondNode()
    {
        //Arrange
        TestClass secondNodeData = new("AmI?WhatIsThis...Life?", 1222);
        Node<TestClass> node = new(new TestClass("IamADifferent(nonSentient)TestClass", 1233));

        //Act
        node.Append(secondNodeData);

        //Assert
        Assert.Equal<TestClass>(secondNodeData, node.Next.Data);
    }
    [Fact]
    public void AppendInputIntLastNodeCirclesToFront()
    {
        //Arrange
        int nodeOneData = 0;
        Node<int> node = new(nodeOneData);

        //Act
        node.Append(1);

        //Assert
        Assert.Equal<int>(0, node.Next.Next.Data);
    }
    [Fact]
    public void AppendInputTestClassLastNodeCirclesToFront()
    {
        //Arrange
        TestClass firstNodeData = new("AlrightGuysThisIsGettingWeird", 1222);
        Node<TestClass> node = new(firstNodeData);

        //Act
        node.Append(new TestClass("IamADifferent(nonSentient)TestClass", 1233));

        //Assert
        Assert.Equal<TestClass>(firstNodeData, node.Next.Next.Data);
    }
    [Fact]
    public void AppendAppendingMultipleEachNodesNextIsCorrect()
    {
        //Arrange
        //These Are not directly Used but they are here to demonstrate
        //The data added in the for loop
        int nodeOneData = 0;
        int oneNextData = 1;
        int twoNextData = 2;
        int threeNextData = 3;
        int fourNextData = nodeOneData;
        Node<int> node = new(nodeOneData);

        //Act
        Node<int> cur = node;
        for (int i = 1; i < 4; i++)
        {
            cur.Append(i);
            cur = cur.Next;
        }
        Node<int> node2 = node.Next;
        Node<int> node3 = node2.Next;
        Node<int> node4 = node3.Next;

        //Assert
        Assert.Equal<int>(oneNextData, node.Next.Data);
        Assert.Equal<int>(twoNextData, node2.Next.Data);
        Assert.Equal<int>(threeNextData, node3.Next.Data);
        Assert.Equal<int>(fourNextData, node4.Next.Data);
    }
    [Fact]
    public void ExistsWithObjectInLLReturnsTrue()
    {
        //Arrange
        TestClass testClassToFind = new("ItIsSuchAStrangeThingToBeAnythingAtAll", 1);
        Node<TestClass> head = new(new TestClass("IamADifferentClass", 2));
        head.Append(new TestClass("AnotherDifferentClass", 3));
        head.Next.Append(testClassToFind);
        head.Next.Next.Append(new TestClass("YetAnotherDifferentClass", 4));

        //Act
        bool isInLinkedL = head.Exists(testClassToFind);

        //Assert
        Assert.True(isInLinkedL);
    }
    [Fact]
    public void ExistsWithoutObjectInLLReturnsFalse()
    {
        //Arrange
        TestClass testClassToFind = new("IDontKnowWhatIsHappeningAnymore...MaybeATestClassShouldNotHaveSentience", 1);
        Node<TestClass> head = new(new TestClass("IamADifferentClass", 2));
        head.Append(new TestClass("AnotherDifferentClass", 3));
        head.Next.Append(new TestClass("Ronald", 4));
        head.Next.Next.Append(new TestClass("YetAnotherDifferentClass", 5));
        //Act
        bool isInLinkedL = head.Exists(testClassToFind);

        //Assert
        Assert.False(isInLinkedL);
    }
    [Fact]
    public void ArrangeExistsDuplicateAppendedThrowsException()
    {
        //Arrange
        int nodeData = 0;
        Node<int> head = new(nodeData);

        //Act
        head.Append(1);
        head.Next.Append(2);

        //Assert
        Assert.Throws<InvalidOperationException>(() => head.Next.Next.Append(nodeData));
    }
    [Fact]
    public void ClearClearIsSetsNextToSelfOtherNodesCanAccessClearedNode()
    {
        //Arrange
        Node<int> head = new(0);
        head.Append(1);
        head.Next.Append(2);

        Node<int> second = head.Next;
        Node<int> third = second.Next;

        //Act
        head.Clear();

        //Assert
        Assert.Same(head, head.Next); //Head now points to itself

        Assert.Same(third, second.Next);
        Assert.NotSame(second, third.Next); //This Means the cleared pieces dont make their own loop
        Assert.Same(head, third.Next); //This Shows that third.Next is still accessing head (which is a problem)

        //An itterative clear is necessary to prevent head from getting accessed by nodes it
        //no longer connected to
    }
    [Fact]
    public void IterativeClearClearSetsAllNodesNextToSelfNoNodesCanAccessClearedNode()
    {
        //Arrange
        Node<int> head = new(0);
        head.Append(1);
        head.Next.Append(2);

        Node<int> second = head.Next;
        Node<int> third = second.Next;

        //Act
        head.IterativeClear();

        //Assert
        Assert.Same(head, head.Next);
        Assert.Same(second, second.Next);
        Assert.Same(third, third.Next);
    }
    [Fact]
    public void ToStringTwoStringOnTestClassConvertsToStringCorrectly()
    {
        //Arrange
        TestClass testClass = new TestClass("Gumbus", 1);
        string testClassToString = testClass.ToString();
        Node<TestClass> head = new(testClass);

        //Act
        String toStringReturned = head.ToString();

        //Assert
        Assert.Equal(toStringReturned, testClassToString);
    }
}
