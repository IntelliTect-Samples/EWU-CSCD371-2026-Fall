using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GenericsHomework;

namespace GenericsHomework.Tests;

    [TestClass]
    public sealed class NodeTests
    {
        [TestMethod]
        public void Constructor_ShouldInitializeValueandSelfReference()
        {
            var node = new Node<int>(5);
            Assert.AreEqual<int>(5, node.Value);
            Assert.AreEqual<Node<int>>(node,node.Next);
        }

        [TestMethod]
        public void ToString_ValueGiven_ReturnsValueAsString()
        {
            var node = new Node<string>("Hello");
            Assert.AreEqual<string>("Hello", node.ToString());
        }

        [TestMethod]
        public void Append_ValidValue_NodeAppendedAfterCurrent()
        {
            var node1 = new Node<int>(1);
            node1.Append(2);
            var node2 = node1.Next;


            Assert.AreEqual<int>(2, node2.Value);
            Assert.AreEqual<Node<int>>(node1, node2.Next);

        }

        [TestMethod]
        public void Append_DuplicateValue_ThrowsInvalidOperationException()
        {
            var node = new Node<int>(1);
            node.Append(2);

            Assert.ThrowsException<InvalidOperationException>(() => node.Append(2));
        }

        [TestMethod]
        public void Exists_FindsValuesCorrectly()
        {
            var node = new Node<int>(10);
            node.Append(20);
            node.Append(30);

            Assert.IsTrue(node.Exists(10));
            Assert.IsTrue(node.Exists(20));
            Assert.IsTrue(node.Exists(30));
            Assert.IsFalse(node.Exists(99));
        }

        [TestMethod]
        public void Clear_MultipleNodes_RemovesAllExceptCurrent()
        {
            var node = new Node<int>(1);
            node.Append(2);
            node.Append(3);
            node.Append(4);

            //capture a removed node reference (node.Next is the most recently appended)
            var removed = node.Next;

            node.Clear();

            //original node should be isolated
            Assert.AreEqual<Node<int>>(node, node.Next);
            Assert.IsFalse(node.Exists(2));
            Assert.IsFalse(node.Exists(3));
            Assert.IsFalse(node.Exists(4));

            //removed nodes should form their own circular list (preserve relative order).
            //After appending 2,3,4 the insertion order around the removed list is:
            //removed (4) -> 3 -> 2 -> removed (4)
            Assert.AreEqual<int>(3, removed.Next.Value);
            Assert.AreEqual<int>(2, removed.Next.Next.Value);
            //loop closes back to the captured removed node
            Assert.AreEqual<Node<int>>(removed, removed.Next.Next.Next);
        }

        [TestMethod]
        public void Clear_AfterClear_CanAppendNewValues()
        {
            var node = new Node<int>(1);
            node.Append(2);
            node.Append(3);

            node.Clear();
            node.Append(5);
            node.Append(6);

            Assert.AreEqual<int>(6, node.Next.Value);
            Assert.AreEqual<int>(5, node.Next.Next.Value);
            Assert.AreEqual<Node<int>>(node, node.Next.Next.Next);
        }

    }

