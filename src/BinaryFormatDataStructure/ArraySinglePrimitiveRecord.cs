using System.IO;

namespace BinaryFormatDataStructure
{
    internal class ArraySinglePrimitiveRecord
    {
        public ArrayInfo ArrayInfo { get; internal set; }
        public PrimitiveType PrimitiveType { get; internal set; }

        internal void Read(BinaryReader reader)
        {
            ArrayInfo = new ArrayInfo();
            ArrayInfo.Read(reader);
            PrimitiveType = (PrimitiveType)reader.ReadByte();
        }
    }
}
