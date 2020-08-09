using System.IO;

namespace BinaryFormatDataStructure
{
    internal class MemberReferenceRecord
    {
        public int IdRef { get; internal set; }

        internal void Read(BinaryReader reader)
        {
            IdRef = reader.ReadInt32();
        }
    }
}
