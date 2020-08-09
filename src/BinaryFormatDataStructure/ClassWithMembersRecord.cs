using System.IO;

namespace BinaryFormatDataStructure
{
    internal class ClassWithMembersRecord : ClassSerializationRecord
    {
        public int LibraryId { get; internal set; }

        internal void Read(BinaryReader reader)
        {
            ClassInfo = new ClassInfo();
            ClassInfo.Read(reader);
            LibraryId = reader.ReadInt32();
        }
    }
}
