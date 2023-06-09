using Microsoft.VisualStudio.TestTools.UnitTesting;
using BinaryFormatDataStructure;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Linq;

namespace BinaryFormatDataStructureTests
{
    [TestClass]
    public class CustomClassArrayTests
    {
        [TestMethod]
        public void TestObjectArray()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            formatter.Serialize(ms, new Dictionary<string, object>() { { "abc", new TestModel() }, { "xyz", new TestModel() } });
            ms.Position = 0;

            object result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(BinaryObject));
            var dictionaryResult = (BinaryObject)result;
            Assert.AreEqual(4, dictionaryResult.Keys.Count());
            var keys = (object[])dictionaryResult["KeyValuePairs"];
            Assert.AreEqual(2, keys.Length);
            Assert.IsInstanceOfType(keys[0], typeof(BinaryObject));
            Assert.AreEqual("abc", (string)((BinaryObject)keys[0])["key"]);
            Assert.IsInstanceOfType(keys[1], typeof(BinaryObject));
            Assert.AreEqual("xyz", (string)((BinaryObject)keys[1])["key"]);
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

        private enum TestEnum
        {
            One = 1,
            Two = 2
        }
    }
}
