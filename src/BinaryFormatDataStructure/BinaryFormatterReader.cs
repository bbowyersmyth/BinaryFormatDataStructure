using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BinaryFormatDataStructure
{
    public class NRBFReader
    {
        private BinaryReader _reader;
        private bool _endOfStream = false;
        private Dictionary<int, object> _objectTracker = new Dictionary<int, object>();
        private Dictionary<int, string> _libraries = new Dictionary<int, string>();
        private List<DeferredItem> _deferredItems = new List<DeferredItem>();

        private NRBFReader(Stream inputStream)
        {
            _reader = new BinaryReader(inputStream, Encoding.UTF8);
        }

        public static object ReadStream(Stream inputStream)
        {
            NRBFReader instance = new NRBFReader(inputStream);
            return instance.Parse();
        }

        private object Parse()
        {
            // Confirm this is a NRBF stream
            RecordType recordType = (RecordType)_reader.ReadByte();

            if (recordType != RecordType.SerializedStreamHeader)
            {
                throw new SerializationException("Invalid NRBF stream");
            }

            var header = new SerializationHeaderRecord();
            header.ReadAndValidate(_reader);

            // Parse the rest of the stream
            while (!_endOfStream)
            {
                Read();
            }

            CompleteDeferredItems();

            return DereferenceTrackedObject(header.RootId);
        }

        private object Read()
        {
            return Read(out _);
        }

        private object Read(out RecordType recordType)
        {
            object currentObject = null;
            recordType = (RecordType)_reader.ReadByte();

            switch (recordType)
            {
                case RecordType.ClassWithId:
                    {
                        var result = new ClassWithIdRecord();
                        result.Read(_reader);

                        ClassSerializationRecord classRef = (ClassSerializationRecord)_objectTracker[result.MetadataId];
                        BinaryObject objectRef = classRef.Value;

                        var owner = new BinaryObject()
                        {
                            TypeName = objectRef.TypeName,
                            AssemblyName = objectRef.AssemblyName
                        };
                        if (result.ObjectId != 0)
                        {
                            _objectTracker[result.ObjectId] = owner;
                        }
                        currentObject = owner;
                        if (classRef.MemberTypeInfo == null)
                        {
                            ReadUntypedMembers(owner, owner.TypeName, classRef.ClassInfo.MemberNames);
                        }
                        else
                        {
                            ReadMembers(owner, classRef.ClassInfo.MemberNames, classRef.MemberTypeInfo);
                        }
                    }
                    break;

                case RecordType.SystemClassWithMembers:
                    {
                        var result = new SystemClassWithMembersRecord();
                        result.Read(_reader);
                        result.Value = new BinaryObject()
                        {
                            TypeName = result.ClassInfo.Name
                        };
                        if (result.ClassInfo.ObjectId != 0)
                        {
                            _objectTracker[result.ClassInfo.ObjectId] = result;
                        }
                        currentObject = result.Value;
                        ReadUntypedMembers(result.Value, result.ClassInfo.Name, result.ClassInfo.MemberNames);
                    }
                    break;

                case RecordType.ClassWithMembers:
                    {
                        var result = new ClassWithMembersRecord();
                        result.Read(_reader);
                        result.Value = new BinaryObject()
                        {
                            TypeName = result.ClassInfo.Name,
                            AssemblyName = _libraries[result.LibraryId]
                        };
                        if (result.ClassInfo.ObjectId != 0)
                        {
                            _objectTracker[result.ClassInfo.ObjectId] = result;
                        }
                        currentObject = result.Value;
                        ReadUntypedMembers(result.Value, result.ClassInfo.Name, result.ClassInfo.MemberNames);
                    }
                    break;

                case RecordType.SystemClassWithMembersAndTypes:
                    {
                        var result = new SystemClassWithMembersAndTypesRecord();
                        result.Read(_reader);
                        result.Value = new BinaryObject()
                        {
                            TypeName = result.ClassInfo.Name
                        };
                        if (result.ClassInfo.ObjectId != 0)
                        {
                            _objectTracker[result.ClassInfo.ObjectId] = result;
                        }
                        currentObject = result.Value;
                        ReadMembers(result.Value, result.ClassInfo.MemberNames, result.MemberTypeInfo);
                    }
                    break;

                case RecordType.ClassWithMembersAndTypes:
                    {
                        var result = new ClassWithMembersAndTypesRecord();
                        result.Read(_reader);
                        result.Value = new BinaryObject()
                        {
                            TypeName = result.ClassInfo.Name,
                            AssemblyName = _libraries[result.LibraryId]
                        };
                        if (result.ClassInfo.ObjectId != 0)
                        {
                            _objectTracker[result.ClassInfo.ObjectId] = result;
                        }
                        currentObject = result.Value;
                        ReadMembers(result.Value, result.ClassInfo.MemberNames, result.MemberTypeInfo);
                    }
                    break;

                case RecordType.BinaryObjectString:
                    {
                        var result = new BinaryObjectStringRecord();
                        result.Read(_reader);
                        currentObject = result.Value;
                        if (result.ObjectId != 0)
                        {
                            _objectTracker[result.ObjectId] = currentObject;
                        }
                    }
                    break;

                case RecordType.BinaryArray:
                    {
                        var result = new BinaryArrayRecord();
                        result.Read(_reader);
                        currentObject = ReadArray(result);
                        if (result.ObjectId != 0)
                        {
                            _objectTracker[result.ObjectId] = currentObject;
                        }
                    }
                    break;

                case RecordType.MemberPrimitiveTyped:
                    {
                        var result = new MemberPrimitiveTypedRecord();
                        result.Read(_reader);
                        currentObject = result.Value;
                    }
                    break;

                case RecordType.MemberReference:
                    {
                        var result = new MemberReferenceRecord();
                        result.Read(_reader);

                        if (_objectTracker.TryGetValue(result.IdRef, out object objectRef))
                        {
                            if (objectRef is ClassSerializationRecord classRef)
                            {
                                currentObject = classRef.Value;
                            }
                            else
                            {
                                currentObject = objectRef;
                            }
                        }
                        else
                        {
                            currentObject = new DeferredReference()
                            {
                                Id = result.IdRef
                            };
                        }
                    }
                    break;

                case RecordType.ObjectNull:
                    return null;

                case RecordType.MessageEnd:
                    _endOfStream = true;
                    break;

                case RecordType.BinaryLibrary:
                    {
                        var result = new BinaryLibraryRecord();
                        result.Read(_reader);

                        _libraries[result.LibraryId] = result.LibraryName;
                    }
                    break;

                case RecordType.ObjectNullMultiple256:
                    {
                        var result = new ObjectNullMultipleRecord();
                        result.Read(_reader, is256: true);
                        currentObject = result;
                    }
                    break;

                case RecordType.ObjectNullMultiple:
                    {
                        var result = new ObjectNullMultipleRecord();
                        result.Read(_reader, is256: false);
                        currentObject = result;
                    }
                    break;

                case RecordType.ArraySinglePrimitive:
                    {
                        var result = new ArraySinglePrimitiveRecord();
                        result.Read(_reader);
                        currentObject = ReadArray(result);
                        if (result.ArrayInfo.ObjectId != 0)
                        {
                            _objectTracker[result.ArrayInfo.ObjectId] = currentObject;
                        }
                    }
                    break;

                case RecordType.ArraySingleObject:
                    {
                        var result = new ArraySingleObjectRecord();
                        result.Read(_reader);
                        currentObject = ReadArray(result);
                        if (result.ArrayInfo.ObjectId != 0)
                        {
                            _objectTracker[result.ArrayInfo.ObjectId] = currentObject;
                        }
                    }
                    break;

                case RecordType.ArraySingleString:
                    {
                        var result = new ArraySingleStringRecord();
                        result.Read(_reader);
                        currentObject = ReadArray(result);
                        if (result.ArrayInfo.ObjectId != 0)
                        {
                            _objectTracker[result.ArrayInfo.ObjectId] = currentObject;
                        }
                    }
                    break;

                case RecordType.MethodCall:
                case RecordType.MethodReturn:
                case RecordType.SerializedStreamHeader: // Second header record
                default:
                    throw new NotSupportedException("RecordType: " + recordType.ToString());
            }

            return currentObject;
        }

        private void ReadMembers(BinaryObject owner, string[] memberNames, MemberTypeInfo memberTypeInfo)
        {
            for (int i = 0; i < memberNames.Length; i++)
            {
                if (memberTypeInfo.BinaryType[i] == BinaryType.Primitive)
                {
                    owner.AddMember(memberNames[i], PrimitiveReader.Read((PrimitiveType)memberTypeInfo.AdditionalInfos[i], _reader));
                }
                else
                {
                    object memberClass = Read();

                    if (memberClass is DeferredReference deferredRef)
                    {
                        _deferredItems.Add(
                            new DeferredItem(owner, memberNames[i], deferredRef.Id)
                        );

                        owner.AddMember(memberNames[i], null);
                    }
                    else
                    {
                        owner.AddMember(memberNames[i], memberClass);
                    }
                }
            }
        }

        private void ReadUntypedMembers(BinaryObject owner, string className, string[] memberNames)
        {
            if (className == "System.Guid" && memberNames.Length == 11)
            {
                owner.AddMember("_a", _reader.ReadInt32());
                owner.AddMember("_b", _reader.ReadInt16());
                owner.AddMember("_c", _reader.ReadInt16());
                owner.AddMember("_d", _reader.ReadByte());
                owner.AddMember("_e", _reader.ReadByte());
                owner.AddMember("_f", _reader.ReadByte());
                owner.AddMember("_g", _reader.ReadByte());
                owner.AddMember("_h", _reader.ReadByte());
                owner.AddMember("_i", _reader.ReadByte());
                owner.AddMember("_j", _reader.ReadByte());
                owner.AddMember("_k", _reader.ReadByte());
                return;
            }
            else if (memberNames.Length == 1)
            {
                if (memberNames[0] == "value__")
                {
                    // Likely an enum but we don't know the size. Take a chance at the default int
                    owner.AddMember(memberNames[0], _reader.ReadInt32());
                    return;
                }
            }

            throw new SerializationException("Unsupported untyped member: " + className);
        }

        private object ReadArray(ArraySinglePrimitiveRecord record)
        {
            switch (record.PrimitiveType)
            {
                case PrimitiveType.Boolean:
                    {
                        bool[] result = new bool[record.ArrayInfo.Length];

                        for (int i = 0; i < result.Length; i++)
                        {
                            result[i] = _reader.ReadBoolean();
                        }

                        return result;
                    }

                case PrimitiveType.Byte:
                    {
                        byte[] result = new byte[record.ArrayInfo.Length];

                        for (int i = 0; i < result.Length; i++)
                        {
                            result[i] = _reader.ReadByte();
                        }

                        return result;
                    }

                case PrimitiveType.Char:
                    {
                        char[] result = new char[record.ArrayInfo.Length];

                        for (int i = 0; i < result.Length; i++)
                        {
                            result[i] = _reader.ReadChar();
                        }

                        return result;
                    }

                case PrimitiveType.Double:
                    {
                        double[] result = new double[record.ArrayInfo.Length];

                        for (int i = 0; i < result.Length; i++)
                        {
                            result[i] = _reader.ReadDouble();
                        }

                        return result;
                    }

                case PrimitiveType.Int16:
                    {
                        short[] result = new short[record.ArrayInfo.Length];

                        for (int i = 0; i < result.Length; i++)
                        {
                            result[i] = _reader.ReadInt16();
                        }

                        return result;
                    }

                case PrimitiveType.Int32:
                    {
                        int[] result = new int[record.ArrayInfo.Length];

                        for (int i = 0; i < result.Length; i++)
                        {
                            result[i] = _reader.ReadInt32();
                        }

                        return result;
                    }

                case PrimitiveType.Int64:
                    {
                        long[] result = new long[record.ArrayInfo.Length];

                        for (int i = 0; i < result.Length; i++)
                        {
                            result[i] = _reader.ReadInt64();
                        }

                        return result;
                    }

                case PrimitiveType.SByte:
                    {
                        sbyte[] result = new sbyte[record.ArrayInfo.Length];

                        for (int i = 0; i < result.Length; i++)
                        {
                            result[i] = _reader.ReadSByte();
                        }

                        return result;
                    }

                case PrimitiveType.Single:
                    {
                        float[] result = new float[record.ArrayInfo.Length];

                        for (int i = 0; i < result.Length; i++)
                        {
                            result[i] = _reader.ReadSingle();
                        }

                        return result;
                    }

                case PrimitiveType.UInt16:
                    {
                        ushort[] result = new ushort[record.ArrayInfo.Length];

                        for (int i = 0; i < result.Length; i++)
                        {
                            result[i] = _reader.ReadUInt16();
                        }

                        return result;
                    }

                case PrimitiveType.UInt32:
                    {
                        uint[] result = new uint[record.ArrayInfo.Length];

                        for (int i = 0; i < result.Length; i++)
                        {
                            result[i] = _reader.ReadUInt32();
                        }

                        return result;
                    }

                case PrimitiveType.UInt64:
                    {
                        ulong[] result = new ulong[record.ArrayInfo.Length];

                        for (int i = 0; i < result.Length; i++)
                        {
                            result[i] = _reader.ReadUInt64();
                        }

                        return result;
                    }

                case PrimitiveType.Decimal:
                    {
                        decimal[] result = new decimal[record.ArrayInfo.Length];

                        for (int i = 0; i < result.Length; i++)
                        {
                            result[i] = decimal.Parse(_reader.ReadString(), CultureInfo.InvariantCulture);
                        }

                        return result;
                    }

                case PrimitiveType.TimeSpan:
                    {
                        TimeSpan[] result = new TimeSpan[record.ArrayInfo.Length];

                        for (int i = 0; i < result.Length; i++)
                        {
                            result[i] = PrimitiveReader.ReadTimeSpan(_reader);
                        }

                        return result;
                    }

                case PrimitiveType.DateTime:
                    {
                        DateTime[] result = new DateTime[record.ArrayInfo.Length];

                        for (int i = 0; i < result.Length; i++)
                        {
                            result[i] = PrimitiveReader.ReadDateTime(_reader);
                        }

                        return result;
                    }

                default:
                    throw new SerializationException("Invalid primitive type: " + record.PrimitiveType.ToString());
            }
        }

        private string[] ReadArray(ArraySingleStringRecord record)
        {
            string[] result = new string[record.ArrayInfo.Length];

            for (int i = 0; i < result.Length; i++)
            {
                object value = Read();

                if (value is string stringValue)
                {
                    result[i] = stringValue;
                }
                else if (value is ObjectNullMultipleRecord nullMultiple)
                {
                    i += nullMultiple.NullCount - 1;
                }
            }

            return result;
        }

        private object[] ReadArray(ArraySingleObjectRecord record)
        {
            object[] result = new object[record.ArrayInfo.Length];

            for (int i = 0; i < result.Length; i++)
            {
                object value = Read(out var recordType);
                if (recordType == RecordType.BinaryLibrary)
                {
                    // According to [MS-NRBF] section 2.7, ArraySingleObject element member references can be preceded
                    // by exactly zero or one BinaryLibrary records, so if we just read a BinaryLibrary record, we should
                    // skip it and read the following record.
                    value = Read();
                }

                if (value is ObjectNullMultipleRecord nullMultiple)
                {
                    i += nullMultiple.NullCount - 1;
                }
                else if (value is DeferredReference deferredRef)
                {
                    int setIndex = i;
                    _deferredItems.Add(
                        new DeferredItem((resolvedValue) => result.SetValue(resolvedValue, setIndex), deferredRef.Id)
                    );
                }
                else
                {
                    result[i] = value;
                }
            }

            return result;
        }

        private object ReadArray(BinaryArrayRecord record)
        {
            Type arrayType;

            switch (record.PrimitiveType)
            {
                case PrimitiveType.Boolean:
                    if (record.BinaryArrayType == BinaryArrayType.Jagged)
                    {
                        arrayType = typeof(bool[]);
                    }
                    else
                    {
                        arrayType = typeof(bool);
                    }
                    break;

                case PrimitiveType.Byte:
                    if (record.BinaryArrayType == BinaryArrayType.Jagged)
                    {
                        arrayType = typeof(byte[]);
                    }
                    else
                    {
                        arrayType = typeof(byte);
                    }
                    break;

                case PrimitiveType.Char:
                    if (record.BinaryArrayType == BinaryArrayType.Jagged)
                    {
                        arrayType = typeof(char[]);
                    }
                    else
                    {
                        arrayType = typeof(char);
                    }
                    break;

                case PrimitiveType.Double:
                    if (record.BinaryArrayType == BinaryArrayType.Jagged)
                    {
                        arrayType = typeof(double[]);
                    }
                    else
                    {
                        arrayType = typeof(double);
                    }
                    break;

                case PrimitiveType.Int16:
                    if (record.BinaryArrayType == BinaryArrayType.Jagged)
                    {
                        arrayType = typeof(short[]);
                    }
                    else
                    {
                        arrayType = typeof(short);
                    }
                    break;

                case PrimitiveType.Int32:
                    if (record.BinaryArrayType == BinaryArrayType.Jagged)
                    {
                        arrayType = typeof(int[]);
                    }
                    else
                    {
                        arrayType = typeof(int);
                    }
                    break;

                case PrimitiveType.Int64:
                    if (record.BinaryArrayType == BinaryArrayType.Jagged)
                    {
                        arrayType = typeof(long[]);
                    }
                    else
                    {
                        arrayType = typeof(long);
                    }
                    break;

                case PrimitiveType.SByte:
                    if (record.BinaryArrayType == BinaryArrayType.Jagged)
                    {
                        arrayType = typeof(sbyte[]);
                    }
                    else
                    {
                        arrayType = typeof(sbyte);
                    }
                    break;

                case PrimitiveType.Single:
                    if (record.BinaryArrayType == BinaryArrayType.Jagged)
                    {
                        arrayType = typeof(float[]);
                    }
                    else
                    {
                        arrayType = typeof(float);
                    }
                    break;

                case PrimitiveType.UInt16:
                    if (record.BinaryArrayType == BinaryArrayType.Jagged)
                    {
                        arrayType = typeof(ushort[]);
                    }
                    else
                    {
                        arrayType = typeof(ushort);
                    }
                    break;

                case PrimitiveType.UInt32:
                    if (record.BinaryArrayType == BinaryArrayType.Jagged)
                    {
                        arrayType = typeof(uint[]);
                    }
                    else
                    {
                        arrayType = typeof(uint);
                    }
                    break;

                case PrimitiveType.UInt64:
                    if (record.BinaryArrayType == BinaryArrayType.Jagged)
                    {
                        arrayType = typeof(ulong[]);
                    }
                    else
                    {
                        arrayType = typeof(ulong);
                    }
                    break;

                case PrimitiveType.Decimal:
                    if (record.BinaryArrayType == BinaryArrayType.Jagged)
                    {
                        arrayType = typeof(decimal[]);
                    }
                    else
                    {
                        arrayType = typeof(decimal);
                    }
                    break;

                case PrimitiveType.TimeSpan:
                    if (record.BinaryArrayType == BinaryArrayType.Jagged)
                    {
                        arrayType = typeof(TimeSpan[]);
                    }
                    else
                    {
                        arrayType = typeof(TimeSpan);
                    }
                    break;

                case PrimitiveType.DateTime:
                    if (record.BinaryArrayType == BinaryArrayType.Jagged)
                    {
                        arrayType = typeof(DateTime[]);
                    }
                    else
                    {
                        arrayType = typeof(DateTime);
                    }
                    break;

                case PrimitiveType.None:
                    if (record.BinaryType == BinaryType.StringArray)
                    {
                        arrayType = typeof(string[]);
                    }
                    else if (record.BinaryType == BinaryType.String)
                    {
                        if (record.BinaryArrayType == BinaryArrayType.Jagged)
                        {
                            arrayType = typeof(string[]);
                        }
                        else
                        {
                            arrayType = typeof(string);
                        }
                    }
                    else if (record.BinaryArrayType == BinaryArrayType.Jagged)
                    {
                        arrayType = typeof(object[]);
                    }
                    else
                    {
                        arrayType = typeof(object);
                    }
                    break;

                default:
                    throw new SerializationException("Invalid primitive type: " + record.PrimitiveType.ToString());
            }

            Array result;

            if (record.LowerBounds == null)
            {
                result = Array.CreateInstance(arrayType, record.Lengths);
            }
            else
            {
                result = Array.CreateInstance(arrayType, record.Lengths, record.LowerBounds);
            }


            int[] firstIndex(Array array)
            {
                int[] indices = Enumerable.Range(0, array.Rank)
                    .Select(_i => array.GetLowerBound(_i))
                    .ToArray();

                for (int i = 0; i < indices.Length; i++)
                {
                    if (indices[i] > array.GetUpperBound(i))
                        return null;
                }

                return indices;
            }

            int[] nextIndex(Array array, int[] indices)
            {
                for (int i = indices.Length - 1; i >= 0; --i)
                {
                    indices[i]++;
                    if (indices[i] <= array.GetUpperBound(i))
                        return indices;
                    indices[i] = array.GetLowerBound(i);
                }
                return null;
            }

            if (record.PrimitiveType == PrimitiveType.None || record.BinaryArrayType == BinaryArrayType.Jagged)
            {
                if (record.BinaryType != BinaryType.Primitive)
                {
                    int continueCount = 0;

                    for (var indices = firstIndex(result); indices != null; indices = nextIndex(result, indices))
                    {
                        if (continueCount > 0)
                        {
                            continueCount--;
                            continue;
                        }

                        object value = Read(out var recordType);
                        if (recordType == RecordType.BinaryLibrary)
                        {
                            // According to [MS-NRBF] section 2.7, BinaryArray element member references can be preceded
                            // by exactly zero or one BinaryLibrary records, so if we just read a BinaryLibrary record, we should
                            // skip it and read the following record.
                            value = Read();
                        }

                        if (value is ObjectNullMultipleRecord nullMultiple)
                        {
                            continueCount = nullMultiple.NullCount - 1;
                        }
                        else if (value is DeferredReference deferredRef)
                        {
                            int[] setIndex = (int[])indices.Clone();
                            _deferredItems.Add(
                                new DeferredItem((resolvedValue) => result.SetValue(resolvedValue, setIndex), deferredRef.Id)
                            );
                        }
                        else
                        {
                            result.SetValue(value, indices);
                        }
                    }
                }
                else
                {
                    // TODO
                    throw new SerializationException("Unsupported array structure");
                }
            }
            else
            {
                for (var indices = firstIndex(result); indices != null; indices = nextIndex(result, indices))
                {
                    result.SetValue(PrimitiveReader.Read(record.PrimitiveType, _reader), indices);
                }
            }

            return result;
        }

        private void CompleteDeferredItems()
        {
            foreach (var deferredItem in _deferredItems)
            {
                var referencedItem = DereferenceTrackedObject(deferredItem.Id);

                if (deferredItem.DeferredAction != null)
                {
                    deferredItem.DeferredAction.Invoke(referencedItem);
                }
                else
                {
                    deferredItem.Owner.AddMember(deferredItem.Member, referencedItem);
                }
            }
        }

        private object DereferenceTrackedObject(int id)
        {
            var referencedItem = _objectTracker[id];

            if (referencedItem is ClassSerializationRecord classRef)
            {
                referencedItem = classRef.Value;
            }

            return referencedItem;
        }

        private class DeferredItem
        {
            public readonly BinaryObject Owner;
            public readonly string Member;
            public readonly int Id;
            public readonly Action<object> DeferredAction;

            public DeferredItem(BinaryObject owner, string member, int id)
            {
                Owner = owner;
                Member = member;
                Id = id;
            }

            public DeferredItem(Action<object> deferredAction, int id)
            {
                DeferredAction = deferredAction;
                Id = id;
            }
        }
    }
}
