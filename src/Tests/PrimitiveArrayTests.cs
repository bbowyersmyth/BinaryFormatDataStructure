using Microsoft.VisualStudio.TestTools.UnitTesting;
using BinaryFormatDataStructure;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BinaryFormatDataStructureTests
{
    [TestClass]
    public class PrimitiveArrayTests
    {
        [TestMethod]
        public void TestIntArray()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            formatter.Serialize(ms, new int[] { 1, 2, 3 });
            ms.Position = 0;

            object result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(int[]));

            int[] arrayResult = (int[])result;
            Assert.AreEqual(3, arrayResult.Length);
            Assert.AreEqual(1, arrayResult[0]);
            Assert.AreEqual(2, arrayResult[1]);
            Assert.AreEqual(3, arrayResult[2]);
        }

        [TestMethod]
        public void TestTimeSpanArray()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            formatter.Serialize(ms, new TimeSpan[] { new TimeSpan(1), new TimeSpan(2), new TimeSpan(3) });
            ms.Position = 0;

            object result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(TimeSpan[]));

            TimeSpan[] arrayResult = (TimeSpan[])result;
            Assert.AreEqual(3, arrayResult.Length);
            Assert.AreEqual(1, arrayResult[0].Ticks);
            Assert.AreEqual(2, arrayResult[1].Ticks);
            Assert.AreEqual(3, arrayResult[2].Ticks);
        }

        [TestMethod]
        public void TestDateTimeArray()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            formatter.Serialize(ms, new DateTime[] { new DateTime(1), new DateTime(2), new DateTime(3) });
            ms.Position = 0;

            object result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(DateTime[]));

            DateTime[] arrayResult = (DateTime[])result;
            Assert.AreEqual(3, arrayResult.Length);
            Assert.AreEqual(1, arrayResult[0].Ticks);
            Assert.AreEqual(2, arrayResult[1].Ticks);
            Assert.AreEqual(3, arrayResult[2].Ticks);
        }

        [TestMethod]
        public void TestStringArray()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            formatter.Serialize(ms, new string[] { "a", "b", "c", "a" });
            ms.Position = 0;

            object result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(string[]));

            string[] arrayResult = (string[])result;
            Assert.AreEqual(4, arrayResult.Length);
            Assert.AreEqual("a", arrayResult[0]);
            Assert.AreEqual("b", arrayResult[1]);
            Assert.AreEqual("c", arrayResult[2]);
            Assert.AreSame(arrayResult[0], arrayResult[3]);
        }

        [TestMethod]
        public void TestStringArrayWithNulls()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            // Single null
            formatter.Serialize(ms, new string[] { null, "a" });
            ms.Position = 0;

            object result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(string[]));

            string[] arrayResult = (string[])result;
            Assert.AreEqual(2, arrayResult.Length);
            Assert.IsNull(arrayResult[0]);
            Assert.AreEqual("a", arrayResult[1]);

            // Multi null
            ms = new MemoryStream();
            formatter.Serialize(ms, new string[] { null, null, "a" });
            ms.Position = 0;

            result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(string[]));

            arrayResult = (string[])result;
            Assert.AreEqual(3, arrayResult.Length);
            Assert.IsNull(arrayResult[0]);
            Assert.IsNull(arrayResult[1]);
            Assert.AreEqual("a", arrayResult[2]);

            // Multi null > 256
            ms = new MemoryStream();
            var largeArray = new string[260];
            largeArray[largeArray.Length - 1] = "abc";
            formatter.Serialize(ms, largeArray);
            ms.Position = 0;

            result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(string[]));

            arrayResult = (string[])result;
            Assert.AreEqual(largeArray.Length, arrayResult.Length);
            Assert.IsNull(arrayResult[0]);
            Assert.IsNull(arrayResult[arrayResult.Length - 2]);
            Assert.AreEqual("abc", arrayResult[arrayResult.Length - 1]);
        }

        [TestMethod]
        public void TestMultidimensionIntArray()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            formatter.Serialize(ms, new int[,] { { 1, 2, 3 }, { 4, 5, 6 } });
            ms.Position = 0;

            object result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(int[,]));

            int[,] arrayResult = (int[,])result;
            Assert.AreEqual(2, arrayResult.GetLength(0));
            Assert.AreEqual(3, arrayResult.GetLength(1));
            Assert.AreEqual(1, arrayResult[0, 0]);
            Assert.AreEqual(2, arrayResult[0, 1]);
            Assert.AreEqual(3, arrayResult[0, 2]);
            Assert.AreEqual(4, arrayResult[1, 0]);
            Assert.AreEqual(5, arrayResult[1, 1]);
            Assert.AreEqual(6, arrayResult[1, 2]);
        }

        [TestMethod]
        public void TestEmpytIntArray()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            formatter.Serialize(ms, new int[0]);
            ms.Position = 0;

            object result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(int[]));

            int[] arrayResult = (int[])result;
            Assert.AreEqual(0, arrayResult.Length);
        }

        [TestMethod]
        public void TestEmptyMultidimensionIntArray()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            formatter.Serialize(ms, new int[0,0]);
            ms.Position = 0;

            object result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(int[,]));

            int[,] arrayResult = (int[,])result;
            Assert.AreEqual(0, arrayResult.GetLength(0));
            Assert.AreEqual(0, arrayResult.GetLength(1));
        }
    }
}
