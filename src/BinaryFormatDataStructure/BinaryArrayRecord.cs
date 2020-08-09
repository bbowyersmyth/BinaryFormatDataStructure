using System.IO;

namespace BinaryFormatDataStructure
{
    internal class BinaryArrayRecord
    {
        public int ObjectId { get; internal set; }
        public BinaryArrayType BinaryArrayType { get; internal set; }
        public int Rank { get; internal set; }
        public int[] Lengths { get; internal set; }
        public int[] LowerBounds { get; internal set; }
        public BinaryType BinaryType { get; internal set; }
        public PrimitiveType PrimitiveType { get; internal set; }
        public string SystemClassName { get; internal set; }
        public ClassTypeInfo ClassTypeInfo { get; internal set; }

        internal void Read(BinaryReader reader)
        {
            ObjectId = reader.ReadInt32();
            BinaryArrayType = (BinaryArrayType)reader.ReadByte();
            Rank = reader.ReadInt32();
            Lengths = new int[Rank];
            for (int i = 0; i < Rank; i++)
            {
                Lengths[i] = reader.ReadInt32();
            }
            if (BinaryArrayType == BinaryArrayType.SingleOffset || BinaryArrayType == BinaryArrayType.JaggedOffset || BinaryArrayType == BinaryArrayType.RectangularOffset)
            {
                LowerBounds = new int[Rank];
                for (int i = 0; i < Rank; i++)
                {
                    LowerBounds[i] = reader.ReadInt32();
                }
            }
            BinaryType = (BinaryType)reader.ReadByte();
            if (BinaryType == BinaryType.Primitive || BinaryType == BinaryType.PrimitiveArray)
            {
                PrimitiveType = (PrimitiveType)reader.ReadByte();
            }
            else if (BinaryType == BinaryType.SystemClass)
            {
                SystemClassName = reader.ReadString();
            }
            else if (BinaryType == BinaryType.Class)
            {
                ClassTypeInfo = new ClassTypeInfo();
                ClassTypeInfo.Read(reader);
            }
        }
    }
}
