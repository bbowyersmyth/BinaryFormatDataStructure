using System.IO;

namespace BinaryFormatDataStructure
{
    internal class ObjectNullMultipleRecord
    {
        public int NullCount { get; internal set; }

        internal void Read(BinaryReader reader, bool is256)
        {
            if (is256)
            {
                NullCount = reader.ReadByte();
            }
            else
            {
                NullCount = reader.ReadInt32();
            }
        }
    }
}
