using EmptyTest.TStreamHandler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EmptyTest
{
    public class BinaryReaderE : BinaryReader, IReaderWriterAlliaces
    {
        public bool IsRead => true;
        public bool IsWrite => false;
        public readonly object locked = new object();
        public BinaryWriterE Writer { get; set; }

        public BinaryReaderE(Stream input) : base(input)
        {
        }

        public BinaryReaderE(Stream input, Encoding encoding) : base(input, encoding)
        {
        }

        public BinaryReaderE(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
        {
        }

        public TEnum ReadEnum<TEnum>() where TEnum : Enum
        {
            Type baseType = typeof(TEnum).GetEnumUnderlyingType();
            return (TEnum)this.GetType().GetMethod("Read" + baseType.Name).Invoke(this, null);
        }

        public int Read7()
        {
            return this.Read7BitEncodedInt();
        }


        public Type ReadType(out TypeCodeExtande typeCode)
        {
            typeCode = (TypeCodeExtande)ReadByte();
            Type type;

            switch (typeCode)
            {
                case TypeCodeExtande.Array:
                    Type typeElement = ReadType(out _);
                    type = typeElement.MakeArrayType(Read7());
                    break;
                case TypeCodeExtande.Object:
                    type = Types[Read7()];
                    //Write7(GetIdType(type));
                    //var p = type.GetProperties();
                    //Write7(p.Length);
                    //foreach (var item in p)
                    //{
                    //    Write7(GetIdName(item));
                    //}
                    break;
                default:
                    type = Type.GetType("System." + typeCode);
                    break;
            }

            return type;
        }

        int GetIdName(string property)
        {
            return PropertyNames.FindKey(property);
        }

        int GetIdType(Type type)
        {
            if (type.IsGenericType) type = type.GetGenericTypeDefinition();
            return Types.FindKey(type);
        }

        void IReaderWriterAlliaces.InvokeSimpleObject(ref object value)
        {
            TypeCodeExtande typeCode = ReadEnum<TypeCodeExtande>();
            value = typeof(BinaryReaderE).GetMethod("Read" + typeCode).Invoke(this, null);
        }

        void IReaderWriterAlliaces.InvokeBoolean(ref bool value)
        {
            value = ReadBoolean();
        }

        void IReaderWriterAlliaces.InvokeChar(ref char value)
        {
            value = ReadChar();
        }

        void IReaderWriterAlliaces.InvokeSByte(ref sbyte value)
        {
            value = ReadSByte();
        }

        void IReaderWriterAlliaces.InvokeByte(ref byte value)
        {
            value = ReadByte();
        }

        void IReaderWriterAlliaces.InvokeInt16(ref short value)
        {
            value = ReadInt16();
        }

        void IReaderWriterAlliaces.InvokeUInt16(ref ushort value)
        {
            value = ReadUInt16();
        }

        void IReaderWriterAlliaces.InvokeInt32(ref int value)
        {
            value = ReadInt32();
        }

        void IReaderWriterAlliaces.InvokeUInt32(ref uint value)
        {
            value = ReadUInt32();
        }

        void IReaderWriterAlliaces.InvokeInt64(ref long value)
        {
            value = ReadInt64();
        }

        void IReaderWriterAlliaces.InvokeUInt64(ref ulong value)
        {
            value = ReadUInt64();
        }

        void IReaderWriterAlliaces.InvokeSingle(ref float value)
        {
            value = ReadSingle();
        }

        void IReaderWriterAlliaces.InvokeDouble(ref double value)
        {
            value = ReadDouble();
        }

        void IReaderWriterAlliaces.InvokeDecimal(ref decimal value)
        {
            value = ReadDecimal();
        }

        void IReaderWriterAlliaces.InvokeDateTime(ref DateTime value)
        {
            value = new DateTime(ReadInt64());
        }

        void IReaderWriterAlliaces.InvokeString(ref string value)
        {
            value = ReadString();
        }

        void IReaderWriterAlliaces.InvokeInt7(ref int value)
        {
            value = Read7();
        }

        void IReaderWriterAlliaces.InvokeSimpleIntArray(ref int[] childs)
        {
            throw new NotImplementedException();
        }

        void IReaderWriterAlliaces.InvokeArray(ref Array value)
        {
            throw new NotImplementedException();
        }

        void IReaderWriterAlliaces.InvokeIntArray(ref int[] value)
        {
            throw new NotImplementedException();
        }

        void IReaderWriterAlliaces.InvokeList(ref ICollection value)
        {
            throw new NotImplementedException();
        }

        void IReaderWriterAlliaces.InvokeDictionate(ref IDictionary value)
        {
            throw new NotImplementedException();
        }

        void IReaderWriterAlliaces.InvokeType(ref Type value)
        {
            throw new NotImplementedException();
        }

        public void InvokeGuid(ref Guid guid)
        {
            guid = new Guid(ReadBytes(16));
        }

        public DictCounter<string> PropertyNames;
        public DictCounter<Type> Types;
    }
}
