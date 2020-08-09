using System.IO;

namespace BinaryFormatDataStructure
{
    internal class ClassWithIdRecord
    {
        public int ObjectId { get; internal set; }
        public int MetadataId { get; internal set; }

        internal void Read(BinaryReader reader)
        {
            ObjectId = reader.ReadInt32();
            MetadataId = reader.ReadInt32();
        }
    }
}
