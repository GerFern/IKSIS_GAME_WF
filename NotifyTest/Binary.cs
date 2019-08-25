using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace NotifyTest
{
    public class BinaryWriterE:BinaryWriter
    {
        public BinaryWriterE(Stream output) : base(output)
        {
        }

        public BinaryWriterE(Stream output, Encoding encoding) : base(output, encoding)
        {
        }

        public BinaryWriterE(Stream output, Encoding encoding, bool leaveOpen) : base(output, encoding, leaveOpen)
        {
        }

        protected BinaryWriterE()
        {
        }

        public void Write7(int value)
        {
            this.Write7BitEncodedInt(value);
        }

        public TypeCodeExtande WriteType(Type type)
        {
            TypeCodeExtande typeCode;
            if (type.IsArray)
                typeCode = TypeCodeExtande.Array;
            //if(type.ise)
            else
                typeCode = (TypeCodeExtande)Type.GetTypeCode(type);
            Write((byte)typeCode);

            switch (typeCode)
            {
                case TypeCodeExtande.Array:
                    Type typeElement = type.GetElementType();
                    WriteType(typeElement);
                    Write7(type.GetArrayRank());
                    break;
                case TypeCodeExtande.Object:
                    Write7(GetIdType(type));
                    //var p = type.GetProperties();
                    //Write7(p.Length);
                    //foreach (var item in p)
                    //{
                    //    Write7(GetIdName(item));
                    //}
                    break;
                default:
                    
                    break;
            }

            return typeCode;
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

        public DictCounter<string> PropertyNames ;
        public DictCounter<Type> Types;
    }

    public class BinaryReaderE : BinaryReader
    {
        public BinaryReaderE(Stream input) : base(input)
        {
        }

        public BinaryReaderE(Stream input, Encoding encoding) : base(input, encoding)
        {
        }

        public BinaryReaderE(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
        {
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

        public DictCounter<string> PropertyNames;
        public DictCounter<Type> Types;
    }
}
