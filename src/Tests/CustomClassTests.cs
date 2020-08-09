using Microsoft.VisualStudio.TestTools.UnitTesting;
using BinaryFormatDataStructure;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

namespace BinaryFormatDataStructureTests
{
    [TestClass]
    public class CustomClassTests
    {
        [TestMethod]
        public void TestCustomClass()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            formatter.Serialize(ms, new TestModel());
            ms.Position = 0;

            object result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(BinaryObject));

            BinaryObject objectResult = (BinaryObject)result;
            Assert.AreEqual("Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", objectResult.AssemblyName);
            Assert.AreEqual("BinaryFormatDataStructureTests.CustomClassTests+TestModel", objectResult.TypeName);
            Assert.AreEqual(2, objectResult.Keys.Count());

            var enumer = objectResult.GetEnumerator();
            enumer.MoveNext();
            var current = enumer.Current;
            Assert.AreEqual("_one", current.Key);
            Assert.IsInstanceOfType(current.Value, typeof(int));
            Assert.AreEqual(99, (int)current.Value);

            enumer.MoveNext();
            current = enumer.Current;
            Assert.AreEqual("_two", current.Key);
            Assert.IsInstanceOfType(current.Value, typeof(bool));
            Assert.AreEqual(true, (bool)current.Value);
        }

        [TestMethod]
        public void TestReusedCustomClass()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            var model = new TestModel();
            formatter.Serialize(ms, new KeyValuePair<TestModel, TestModel>(model, model));
            ms.Position = 0;

            object result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(BinaryObject));

            BinaryObject objectResult = (BinaryObject)result;
            Assert.IsTrue(objectResult.ContainsKey("key"));
            Assert.IsTrue(objectResult.ContainsKey("value"));
            Assert.AreNotEqual(null, objectResult.ContainsKey("key"));
            Assert.AreSame(objectResult["key"], objectResult["value"]);
        }

        [Serializable]
        private class TestModel
        {
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable CS0414
            private int _one = 99;
            private bool _two = true;
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore IDE0044 // Add readonly modifier

            public decimal Three { get { return 0; } set { } }
        }
    }
}
