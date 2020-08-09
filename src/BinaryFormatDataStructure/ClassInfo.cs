using System.IO;

namespace BinaryFormatDataStructure
{
    internal class ClassInfo
    {
        public int ObjectId { get; internal set; }
        public string Name { get; internal set; }
        public int MemberCount { get; internal set; }
        public string[] MemberNames { get; internal set; }

        internal void Read(BinaryReader reader)
        {
            ObjectId = reader.ReadInt32();
            Name = reader.ReadString();
            MemberCount = reader.ReadInt32();
            MemberNames = new string[MemberCount];

            for (int i = 0; i < MemberNames.Length; i++)
            {
                MemberNames[i] = reader.ReadString();
            }
        }
    }
}
