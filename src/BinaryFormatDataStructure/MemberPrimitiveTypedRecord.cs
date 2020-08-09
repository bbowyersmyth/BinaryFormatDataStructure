using System.IO;

namespace BinaryFormatDataStructure
{
    internal class MemberPrimitiveTypedRecord
    {
        public PrimitiveType PrimitiveType { get; internal set; }
        public object Value { get; internal set; }

        internal void Read(BinaryReader reader)
        {
            PrimitiveType = (PrimitiveType)reader.ReadByte();
            Value = PrimitiveReader.Read(PrimitiveType, reader);
        }
    }
}
