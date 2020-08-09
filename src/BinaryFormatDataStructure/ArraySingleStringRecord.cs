using System.IO;

namespace BinaryFormatDataStructure
{
    internal class ArraySingleStringRecord
    {
        public ArrayInfo ArrayInfo { get; internal set; }

        internal void Read(BinaryReader reader)
        {
            ArrayInfo = new ArrayInfo();
            ArrayInfo.Read(reader);
        }
    }
}
