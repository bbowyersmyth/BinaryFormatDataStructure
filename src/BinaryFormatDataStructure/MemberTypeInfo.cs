using System.IO;

namespace BinaryFormatDataStructure
{
    internal class MemberTypeInfo
    {
        public BinaryType[] BinaryType { get; internal set; }
        public object[] AdditionalInfos { get; internal set; }

        internal void Read(int count, BinaryReader reader)
        {
            BinaryType = new BinaryType[count];

            for (int i = 0; i < count; i++)
            {
                BinaryType[i] = (BinaryType)reader.ReadByte();
            }

            AdditionalInfos = new object[count];

            for (int i = 0; i < count; i++)
            {
                if (BinaryType[i] == BinaryFormatDataStructure.BinaryType.Primitive || BinaryType[i] == BinaryFormatDataStructure.BinaryType.PrimitiveArray)
                {
                    AdditionalInfos[i] = (PrimitiveType)reader.ReadByte();
                }
                else if (BinaryType[i] == BinaryFormatDataStructure.BinaryType.SystemClass)
                {
                    AdditionalInfos[i] = reader.ReadString(); // System class name
                }
                else if (BinaryType[i] == BinaryFormatDataStructure.BinaryType.Class)
                {
                    var typeInfo = new ClassTypeInfo();
                    typeInfo.Read(reader);
                    AdditionalInfos[i] = typeInfo;
                }
            }
        }
    }
}
