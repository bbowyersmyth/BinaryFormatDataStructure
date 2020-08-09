using System.IO;

namespace BinaryFormatDataStructure
{
    internal class SystemClassWithMembersRecord : ClassSerializationRecord
    {
        internal void Read(BinaryReader reader)
        {
            ClassInfo = new ClassInfo();
            ClassInfo.Read(reader);
        }
    }
}
