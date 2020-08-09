using System.IO;
using System.Runtime.Serialization;

namespace BinaryFormatDataStructure
{
    internal class SerializationHeaderRecord
    {
        public int RootId { get; internal set; }
        public int HeaderId { get; internal set; }
        public int MajorVersion { get; internal set; }
        public int MinorVersion { get; internal set; }

        internal void ReadAndValidate(BinaryReader reader)
        {
            RootId = reader.ReadInt32();
            HeaderId = reader.ReadInt32();
            MajorVersion = reader.ReadInt32();
            MinorVersion = reader.ReadInt32();

            if (MajorVersion != 1 || MinorVersion != 0)
            {
                throw new SerializationException("Invalid NRBF stream");
            }
        }
    }
}
