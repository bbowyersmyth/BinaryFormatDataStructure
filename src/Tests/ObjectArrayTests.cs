using Microsoft.VisualStudio.TestTools.UnitTesting;
using BinaryFormatDataStructure;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BinaryFormatDataStructureTests
{
    [TestClass]
    public class ObjectArrayTests
    {
        [TestMethod]
        public void TestObjectArray()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            formatter.Serialize(ms, new object[] { "a", "b", "c", "a" });
            ms.Position = 0;

            object result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(object[]));

            object[] arrayResult = (object[])result;
            Assert.AreEqual(4, arrayResult.Length);
            Assert.AreEqual("a", arrayResult[0]);
            Assert.AreEqual("b", arrayResult[1]);
            Assert.AreEqual("c", arrayResult[2]);
            Assert.AreSame(arrayResult[0], arrayResult[3]);
        }

        [TestMethod]
        public void TestObjectArrayWithNulls()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            // Single null
            formatter.Serialize(ms, new object[] { null, "a" });
            ms.Position = 0;

            object result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(object[]));

            object[] arrayResult = (object[])result;
            Assert.AreEqual(2, arrayResult.Length);
            Assert.IsNull(arrayResult[0]);
            Assert.AreEqual("a", arrayResult[1]);

            // Only nulls
            ms = new MemoryStream();
            formatter.Serialize(ms, new object[] { null, null });
            ms.Position = 0;

            result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(object[]));

            var objectArrayResult = (object[])result;
            Assert.AreEqual(2, objectArrayResult.Length);
            Assert.IsNull(objectArrayResult[0]);
            Assert.IsNull(objectArrayResult[1]);
        }

        [TestMethod]
        public void TestMultidimensionObjectArray()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            formatter.Serialize(ms, new object[,] { { 1, 2, 3 }, { 4, 5, 6 } });
            ms.Position = 0;

            object result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(object[,]));

            object[,] arrayResult = (object[,])result;
            Assert.AreEqual(2, arrayResult.GetLength(0));
            Assert.AreEqual(3, arrayResult.GetLength(1));
            Assert.AreEqual(1, arrayResult[0, 0]);
            Assert.AreEqual(2, arrayResult[0, 1]);
            Assert.AreEqual(3, arrayResult[0, 2]);
            Assert.AreEqual(4, arrayResult[1, 0]);
            Assert.AreEqual(5, arrayResult[1, 1]);
            Assert.AreEqual(6, arrayResult[1, 2]);
        }
    }
}
