using System.IO;

namespace BinaryFormatDataStructure
{
    internal class ClassWithMembersAndTypesRecord : ClassSerializationRecord
    {
        public int LibraryId { get; internal set; }

        internal void Read(BinaryReader reader)
        {
            ClassInfo = new ClassInfo();
            ClassInfo.Read(reader);
            MemberTypeInfo = new MemberTypeInfo();
            MemberTypeInfo.Read(ClassInfo.MemberCount, reader);
            LibraryId = reader.ReadInt32();
        }
    }
}
