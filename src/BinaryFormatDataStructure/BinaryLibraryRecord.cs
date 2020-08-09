using System.IO;

namespace BinaryFormatDataStructure
{
    internal class BinaryLibraryRecord
    {
        public int LibraryId { get; internal set; }
        public string LibraryName { get; internal set; }

        internal void Read(BinaryReader reader)
        {
            LibraryId = reader.ReadInt32();
            LibraryName = reader.ReadString();
        }
    }
}
