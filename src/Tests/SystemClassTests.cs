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
    }
}
