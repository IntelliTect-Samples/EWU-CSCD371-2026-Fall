using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GenericsHomework;

namespace GenericsHomework.Tests;

    [TestClass]
    public sealed class NodeTests
    {
        [TestMethod]
        public void Constructor_ValidInitialization_SelfReferences()
        {
            Node<int> node = new Node<int>(5);
            Assert.AreEqual<int>(5, node.Value);
            Assert.AreEqual<Node<int>>(node,node.Next);
        }

        [TestMethod]
        public void ToString_ValueGiven_ReturnsValueAsString()
        {
            Node<string> node = new Node<string>("Hello");
            Assert.AreEqual<string>("Hello", node.ToString());
        }

        [TestMethod]
        public void Append_ValidValue_NodeAppendedAfterCurrent()
        {
            Node<int> node1 = new Node<int>(1);
            node1.Append(2);
            Node<int> node2 = node1.Next;


            Assert.AreEqual<int>(2, node2.Value);
            Assert.AreEqual<Node<int>>(node1, node2.Next);

        }

        [TestMethod]
        public void Append_DuplicateValue_ThrowsInvalidOperationException()
        {
            Node<int> node = new Node<int>(1);
            node.Append(2);

            Assert.ThrowsException<InvalidOperationException>(() => node.Append(2));
        }

        [TestMethod]
        public void Exists_ExistingValue_FindsValue()
        {
            Node<int> node = new Node<int>(10);
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
            Node<int> node = new Node<int>(1);
            node.Append(2);
            node.Append(3);
            node.Append(4);

            Node<int> removed = node.Next;

            node.Clear();

            Assert.AreEqual<Node<int>>(node, node.Next);
            Assert.IsFalse(node.Exists(2));
            Assert.IsFalse(node.Exists(3));
            Assert.IsFalse(node.Exists(4));

            Assert.AreEqual<int>(3, removed.Next.Value);
            Assert.AreEqual<int>(2, removed.Next.Next.Value);
            Assert.AreEqual<Node<int>>(removed, removed.Next.Next.Next);
        }

        [TestMethod]
        public void Clear_AfterClear_CanAppendNewValues()
        {
            Node<int> node = new Node<int>(1);
            node.Append(2);
            node.Append(3);

            node.Clear();
            node.Append(5);
            node.Append(6);

            Assert.AreEqual<int>(6, node.Next.Value);
            Assert.AreEqual<int>(5, node.Next.Next.Value);
            Assert.AreEqual<Node<int>>(node, node.Next.Next.Next);
        }

            [TestMethod]
        public void Exists_NonExistingValue_ReturnsFalse()
        {
            Node<int> node = new Node<int>(1);
            node.Append(2);
            node.Append(3);
            Assert.IsFalse(node.Exists(42));
        }
    }

