using System;
using System.IO;
using System.Runtime.Serialization;

namespace BinaryFormatDataStructure
{
    internal static class PrimitiveReader
    {
        public static TimeSpan ReadTimeSpan(BinaryReader reader)
        {
            return new TimeSpan(reader.ReadInt64());
        }

        public static DateTime ReadDateTime(BinaryReader reader)
        {
            const ulong TicksMask = 0x3FFFFFFFFFFFFFFF;
            long dateData = reader.ReadInt64();
            long ticks = dateData & (long)TicksMask;

            return new DateTime(ticks);
        }

        public static object Read(PrimitiveType type, BinaryReader reader)
        {
            switch (type)
            {
                case PrimitiveType.Boolean:
                    return reader.ReadBoolean();

                case PrimitiveType.Byte:
                    return reader.ReadByte();

                case PrimitiveType.Char:
                    return reader.ReadChar();

                case PrimitiveType.Double:
                    return reader.ReadDouble();

                case PrimitiveType.Int16:
                    return reader.ReadInt16();

                case PrimitiveType.Int32:
                    return reader.ReadInt32();

                case PrimitiveType.Int64:
                    return reader.ReadInt64();

                case PrimitiveType.SByte:
                    return reader.ReadSByte();

                case PrimitiveType.Single:
                    return reader.ReadSingle();

                case PrimitiveType.UInt16:
                    return reader.ReadUInt16();

                case PrimitiveType.UInt32:
                    return reader.ReadUInt32();

                case PrimitiveType.UInt64:
                    return reader.ReadUInt64();

                case PrimitiveType.Decimal:
                    return reader.ReadDecimal();

                case PrimitiveType.TimeSpan:
                    return ReadTimeSpan(reader);

                case PrimitiveType.DateTime:
                    return ReadDateTime(reader);

                default:
                    throw new SerializationException("Invalid primitive type: " + type.ToString());
            }
        }
    }
}
