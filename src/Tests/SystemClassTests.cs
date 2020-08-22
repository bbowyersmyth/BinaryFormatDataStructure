using Microsoft.VisualStudio.TestTools.UnitTesting;
using BinaryFormatDataStructure;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace BinaryFormatDataStructureTests
{
    [TestClass]
    public class SystemClassTests
    {
        [TestMethod]
        public void TestSystemClass()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            formatter.Serialize(ms, new Version("1.2.3"));
            ms.Position = 0;

            object result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(BinaryObject));

            BinaryObject objectResult = (BinaryObject)result;
            Assert.IsNull(objectResult.AssemblyName);
            Assert.AreEqual("System.Version", objectResult.TypeName);
            Assert.AreEqual(4, objectResult.Keys.Count());

            var enumer = objectResult.GetEnumerator();
            enumer.MoveNext();
            var current = enumer.Current;
            Assert.AreEqual("_Major", current.Key);
            Assert.IsInstanceOfType(current.Value, typeof(int));
            Assert.AreEqual(1, (int)current.Value);

            enumer.MoveNext();
            current = enumer.Current;
            Assert.AreEqual("_Minor", current.Key);
            Assert.IsInstanceOfType(current.Value, typeof(int));
            Assert.AreEqual(2, (int)current.Value);

            enumer.MoveNext();
            current = enumer.Current;
            Assert.AreEqual("_Build", current.Key);
            Assert.IsInstanceOfType(current.Value, typeof(int));
            Assert.AreEqual(3, (int)current.Value);

            enumer.MoveNext();
            current = enumer.Current;
            Assert.AreEqual("_Revision", current.Key);
            Assert.IsInstanceOfType(current.Value, typeof(int));
            Assert.AreEqual(-1, (int)current.Value);
        }

        [TestMethod]
        public void TestSystemClassTypesWhenNeeded()
        {
            Guid expected = new Guid("490efe20-fcf3-4b02-a73e-6877c0210718");
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.TypeFormat = System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesWhenNeeded;
            MemoryStream ms = new MemoryStream();

            formatter.Serialize(ms, expected);
            ms.Position = 0;

            object result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(BinaryObject));

            BinaryObject objectResult = (BinaryObject)result;
            Assert.IsNull(objectResult.AssemblyName);
            Assert.AreEqual("System.Guid", objectResult.TypeName);
            Assert.AreEqual(11, objectResult.Keys.Count());

            Assert.AreEqual(1225719328, (int)objectResult["_a"]);
            Assert.AreEqual(-781, (short)objectResult["_b"]);
            Assert.AreEqual(19202, (short)objectResult["_c"]);
            Assert.AreEqual(167, (byte)objectResult["_d"]);
            Assert.AreEqual(62, (byte)objectResult["_e"]);
            Assert.AreEqual(104, (byte)objectResult["_f"]);
            Assert.AreEqual(119, (byte)objectResult["_g"]);
            Assert.AreEqual(192, (byte)objectResult["_h"]);
            Assert.AreEqual(33, (byte)objectResult["_i"]);
            Assert.AreEqual(7, (byte)objectResult["_j"]);
            Assert.AreEqual(24, (byte)objectResult["_k"]);
        }

        [TestMethod]
        public void TestSystemEnumTypesWhenNeeded()
        {
            System.Runtime.Serialization.Formatters.FormatterTypeStyle expected = System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways;
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.TypeFormat = System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesWhenNeeded;
            MemoryStream ms = new MemoryStream();

            formatter.Serialize(ms, expected);
            ms.Position = 0;

            object result = NRBFReader.ReadStream(ms);

            Assert.IsInstanceOfType(result, typeof(BinaryObject));

            BinaryObject objectResult = (BinaryObject)result;
            Assert.IsNull(objectResult.AssemblyName);
            Assert.AreEqual("System.Runtime.Serialization.Formatters.FormatterTypeStyle", objectResult.TypeName);
            Assert.AreEqual(1, objectResult.Keys.Count());

            Assert.AreEqual(1, (int)objectResult["value__"]);
        }
    }
}
