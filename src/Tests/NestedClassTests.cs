using Microsoft.VisualStudio.TestTools.UnitTesting;
using BinaryFormatDataStructure;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace BinaryFormatDataStructureTests
{
    [TestClass]
    public class NestedClassTests
    {
        [TestMethod]
        public void TestNestedClass()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            formatter.Serialize(ms, new TestModel());
            ms.Position = 0;

            object result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(BinaryObject));

            BinaryObject objectResult = (BinaryObject)result;
            Assert.AreEqual("Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", objectResult.AssemblyName);
            Assert.AreEqual("BinaryFormatDataStructureTests.NestedClassTests+TestModel", objectResult.TypeName);
            Assert.AreEqual(3, objectResult.Keys.Count());

            var enumer = objectResult.GetEnumerator();
            enumer.MoveNext();
            var current = enumer.Current;
            Assert.AreEqual("_one", current.Key);
            Assert.IsInstanceOfType(current.Value, typeof(int));
            Assert.AreEqual(99, (int)current.Value);

            enumer.MoveNext();
            current = enumer.Current;
            Assert.AreEqual("_inner1", current.Key);
            Assert.IsInstanceOfType(current.Value, typeof(BinaryObject));

            // begin inner 1
            var inner1 = (BinaryObject)current.Value;
            Assert.AreEqual("Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", inner1.AssemblyName);
            Assert.AreEqual("BinaryFormatDataStructureTests.NestedClassTests+TestModelInner1", inner1.TypeName);
            var enumerInner1 = inner1.GetEnumerator();
            enumerInner1.MoveNext();
            var currentInner1 = enumerInner1.Current;
            Assert.AreEqual("_inner2", currentInner1.Key);
            Assert.IsInstanceOfType(currentInner1.Value, typeof(BinaryObject));

            // begin inner 2
            var inner2 = (BinaryObject)currentInner1.Value;
            Assert.AreEqual("Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", inner2.AssemblyName);
            Assert.AreEqual("BinaryFormatDataStructureTests.NestedClassTests+TestModelInner2", inner2.TypeName);
            var enumerInner2 = inner2.GetEnumerator();
            enumerInner2.MoveNext();
            var currentInner2 = enumerInner2.Current;
            Assert.AreEqual("_one", currentInner2.Key);
            Assert.IsInstanceOfType(currentInner2.Value, typeof(int));
            Assert.AreEqual(88, (int)currentInner2.Value);
            // end inner 2

            // end inner 1

            enumer.MoveNext();
            current = enumer.Current;
            Assert.AreEqual("_two", current.Key);
            Assert.IsInstanceOfType(current.Value, typeof(bool));
            Assert.AreEqual(true, (bool)current.Value);
        }

        [TestMethod]
        public void TestCycle()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            formatter.Serialize(ms, new CycleParent());
            ms.Position = 0;

            object result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(BinaryObject));

            BinaryObject parentObject = (BinaryObject)result;
            Assert.AreEqual("Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", parentObject.AssemblyName);
            Assert.AreEqual("BinaryFormatDataStructureTests.NestedClassTests+CycleParent", parentObject.TypeName);
            Assert.AreEqual(1, parentObject.Keys.Count());

            var child = parentObject["_child"];
            Assert.IsInstanceOfType(child, typeof(BinaryObject));
            
            var childObject = (BinaryObject)child;
            Assert.AreEqual("Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", childObject.AssemblyName);
            Assert.AreEqual("BinaryFormatDataStructureTests.NestedClassTests+CycleChild", childObject.TypeName);
            Assert.AreEqual(1, childObject.Keys.Count());

            var childsParentReference = childObject["_parent"];
            Assert.AreSame(parentObject, childsParentReference);
        }

        [Serializable]
        private class TestModel
        {
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable CS0414
#pragma warning disable IDE0052 // Remove unread private members
            private int _one = 99;
            private TestModelInner1 _inner1 = new TestModelInner1();
            private bool _two = true;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore IDE0044 // Add readonly modifier
#pragma warning restore IDE0051 // Remove unused private members
        }

        [Serializable]
        private class TestModelInner1
        {
            private TestModelInner2 _inner2 = new TestModelInner2();
        }

        [Serializable]
        private class TestModelInner2
        {
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0044 // Add readonly modifier
            private int _one = 88;
#pragma warning restore IDE0044 // Add readonly modifier
#pragma warning restore IDE0051 // Remove unused private members
        }

        [Serializable]
        private class CycleParent
        {
            private CycleChild _child;

            public CycleParent()
            {
                _child = new CycleChild(this);
            }
        }

        [Serializable]
        private class CycleChild
        {
            private CycleParent _parent;

            public CycleChild(CycleParent parent)
            {
                _parent = parent;
            }
        }
    }
}
