namespace BinaryFormatDataStructure
{
    internal abstract class ClassSerializationRecord
    {
        public ClassInfo ClassInfo { get; internal set; }
        public MemberTypeInfo MemberTypeInfo { get; internal set; }

        public BinaryObject Value { get; set; }
    }
}
