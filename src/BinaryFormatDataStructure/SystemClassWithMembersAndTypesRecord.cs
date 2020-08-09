using System.IO;

namespace BinaryFormatDataStructure
{
    internal class SystemClassWithMembersAndTypesRecord : ClassSerializationRecord
    {
        internal void Read(BinaryReader reader)
        {
            ClassInfo = new ClassInfo();
            ClassInfo.Read(reader);
            MemberTypeInfo = new MemberTypeInfo();
            MemberTypeInfo.Read(ClassInfo.MemberCount, reader);
        }
    }
}
