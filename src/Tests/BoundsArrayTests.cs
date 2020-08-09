using Microsoft.VisualStudio.TestTools.UnitTesting;
using BinaryFormatDataStructure;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BinaryFormatDataStructureTests
{
    [TestClass]
    public class BoundsArrayTests
    {
        [TestMethod]
        public void TestLowerBoundArray()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            var array = Array.CreateInstance(typeof(int), new int[] { 2 }, new int[] { 3 });
            array.SetValue(1, 3);
            array.SetValue(2, 4);
            formatter.Serialize(ms, array);
            ms.Position = 0;

            object result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(Array));
            Assert.AreEqual("Int32[*]", result.GetType().Name);

            Array arrayResult = (Array)result;
            Assert.AreEqual(2, arrayResult.Length);
            Assert.AreEqual(3, arrayResult.GetLowerBound(0));
            Assert.AreEqual(1, (int)arrayResult.GetValue(3));
            Assert.AreEqual(2, (int)arrayResult.GetValue(4));
        }

        [TestMethod]
        public void Test2DimensionalArray()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            var array = new int[,] { { 10, 11, 12 }, { 20, 21, 22 } };
            formatter.Serialize(ms, array);
            ms.Position = 0;

            object result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(int[,]));
            Assert.AreEqual("Int32[,]", result.GetType().Name);

            int[,] arrayResult = (int[,])result;
            Assert.AreEqual(2, arrayResult.Rank);
            Assert.AreEqual(2, arrayResult.GetLength(0));
            Assert.AreEqual(3, arrayResult.GetLength(1));
            Assert.AreEqual(0, arrayResult.GetLowerBound(0));
            Assert.AreEqual(0, arrayResult.GetLowerBound(1));
            Assert.AreEqual(10, (int)arrayResult.GetValue(new int[] { 0, 0 }));
            Assert.AreEqual(11, (int)arrayResult.GetValue(new int[] { 0, 1 }));
            Assert.AreEqual(12, (int)arrayResult.GetValue(new int[] { 0, 2 }));
            Assert.AreEqual(20, (int)arrayResult.GetValue(new int[] { 1, 0 }));
            Assert.AreEqual(21, (int)arrayResult.GetValue(new int[] { 1, 1 }));
            Assert.AreEqual(22, (int)arrayResult.GetValue(new int[] { 1, 2 }));
        }

        [TestMethod]
        public void Test2DimensionalStringArray()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            var array = new string[,] { { "10", "11", "12" }, { "20", "21", "22" } };
            formatter.Serialize(ms, array);
            ms.Position = 0;

            object result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(string[,]));

            string[,] arrayResult = (string[,])result;
            Assert.AreEqual(2, arrayResult.Rank);
            Assert.AreEqual(2, arrayResult.GetLength(0));
            Assert.AreEqual(3, arrayResult.GetLength(1));
            Assert.AreEqual(0, arrayResult.GetLowerBound(0));
            Assert.AreEqual(0, arrayResult.GetLowerBound(1));
            Assert.AreEqual("10", (string)arrayResult.GetValue(new int[] { 0, 0 }));
            Assert.AreEqual("11", (string)arrayResult.GetValue(new int[] { 0, 1 }));
            Assert.AreEqual("12", (string)arrayResult.GetValue(new int[] { 0, 2 }));
            Assert.AreEqual("20", (string)arrayResult.GetValue(new int[] { 1, 0 }));
            Assert.AreEqual("21", (string)arrayResult.GetValue(new int[] { 1, 1 }));
            Assert.AreEqual("22", (string)arrayResult.GetValue(new int[] { 1, 2 }));
        }

        [TestMethod]
        public void TestMultiDimensionalArray()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            var array = new int[,,] { { { 10, 11, 12 }, { 20, 21, 22 } } };
            formatter.Serialize(ms, array);
            ms.Position = 0;

            object result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(int[,,]));
            Assert.AreEqual("Int32[,,]", result.GetType().Name);

            int[,,] arrayResult = (int[,,])result;
            Assert.AreEqual(3, arrayResult.Rank);
            Assert.AreEqual(1, arrayResult.GetLength(0));
            Assert.AreEqual(2, arrayResult.GetLength(1));
            Assert.AreEqual(3, arrayResult.GetLength(2));
            Assert.AreEqual(0, arrayResult.GetLowerBound(0));
            Assert.AreEqual(0, arrayResult.GetLowerBound(1));
            Assert.AreEqual(0, arrayResult.GetLowerBound(2));
            Assert.AreEqual(10, (int)arrayResult.GetValue(new int[] { 0, 0, 0 }));
            Assert.AreEqual(11, (int)arrayResult.GetValue(new int[] { 0, 0, 1 }));
            Assert.AreEqual(12, (int)arrayResult.GetValue(new int[] { 0, 0, 2 }));
            Assert.AreEqual(20, (int)arrayResult.GetValue(new int[] { 0, 1, 0 }));
            Assert.AreEqual(21, (int)arrayResult.GetValue(new int[] { 0, 1, 1 }));
            Assert.AreEqual(22, (int)arrayResult.GetValue(new int[] { 0, 1, 2 }));
        }

        [TestMethod]
        public void TestMultiDimensionalStringArray()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            var array = new string[,,] { { { "10", "11", "12" }, { "20", "21", "22" } } };
            formatter.Serialize(ms, array);
            ms.Position = 0;

            object result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(string[,,]));

            string[,,] arrayResult = (string[,,])result;
            Assert.AreEqual(3, arrayResult.Rank);
            Assert.AreEqual(1, arrayResult.GetLength(0));
            Assert.AreEqual(2, arrayResult.GetLength(1));
            Assert.AreEqual(3, arrayResult.GetLength(2));
            Assert.AreEqual(0, arrayResult.GetLowerBound(0));
            Assert.AreEqual(0, arrayResult.GetLowerBound(1));
            Assert.AreEqual(0, arrayResult.GetLowerBound(2));
            Assert.AreEqual("10", (string)arrayResult.GetValue(new int[] { 0, 0, 0 }));
            Assert.AreEqual("11", (string)arrayResult.GetValue(new int[] { 0, 0, 1 }));
            Assert.AreEqual("12", (string)arrayResult.GetValue(new int[] { 0, 0, 2 }));
            Assert.AreEqual("20", (string)arrayResult.GetValue(new int[] { 0, 1, 0 }));
            Assert.AreEqual("21", (string)arrayResult.GetValue(new int[] { 0, 1, 1 }));
            Assert.AreEqual("22", (string)arrayResult.GetValue(new int[] { 0, 1, 2 }));
        }

        [TestMethod]
        public void TestRaggedArray()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            var array = new int[][] { new int[] { 10, 11 }, new int[] { 20, 21, 22, 23 } };
            formatter.Serialize(ms, array);
            ms.Position = 0;

            object result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(int[][]));

            int[][] arrayResult = (int[][])result;
            Assert.AreEqual(1, arrayResult.Rank);
            Assert.AreEqual(2, arrayResult.Length);
            Assert.AreEqual(0, arrayResult.GetLowerBound(0));
            Assert.AreEqual(1, arrayResult[0].Rank);
            Assert.AreEqual(2, arrayResult[0].Length);
            Assert.AreEqual(0, arrayResult[0].GetLowerBound(0));
            Assert.AreEqual(1, arrayResult[1].Rank);
            Assert.AreEqual(4, arrayResult[1].Length);
            Assert.AreEqual(0, arrayResult[1].GetLowerBound(0));
            Assert.AreEqual(10, arrayResult[0][0]);
            Assert.AreEqual(11, arrayResult[0][1]);
            Assert.AreEqual(20, arrayResult[1][0]);
            Assert.AreEqual(21, arrayResult[1][1]);
            Assert.AreEqual(22, arrayResult[1][2]);
            Assert.AreEqual(23, arrayResult[1][3]);
        }

        [TestMethod]
        public void TestRaggedStringArray()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            var array = new string[][] { new string[] { "10", "11" }, new string[] { "20", "21", "22", "23" } };
            formatter.Serialize(ms, array);
            ms.Position = 0;

            object result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(string[][]));

            string[][] arrayResult = (string[][])result;
            Assert.AreEqual(1, arrayResult.Rank);
            Assert.AreEqual(2, arrayResult.Length);
            Assert.AreEqual(0, arrayResult.GetLowerBound(0));
            Assert.AreEqual(1, arrayResult[0].Rank);
            Assert.AreEqual(2, arrayResult[0].Length);
            Assert.AreEqual(0, arrayResult[0].GetLowerBound(0));
            Assert.AreEqual(1, arrayResult[1].Rank);
            Assert.AreEqual(4, arrayResult[1].Length);
            Assert.AreEqual(0, arrayResult[1].GetLowerBound(0));
            Assert.AreEqual("10", arrayResult[0][0]);
            Assert.AreEqual("11", arrayResult[0][1]);
            Assert.AreEqual("20", arrayResult[1][0]);
            Assert.AreEqual("21", arrayResult[1][1]);
            Assert.AreEqual("22", arrayResult[1][2]);
            Assert.AreEqual("23", arrayResult[1][3]);
        }
    }
}
