using EmptyTest.TExtensions;
using EmptyTest.TNode;
using EmptyTest.TStreamHandler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EmptyTest
{
    public class BinaryWriterE : BinaryWriter, IReaderWriterAlliaces
    {
        public readonly object locked = new object();
        public BinaryReaderE Reader { get; set; }
        public bool IsRead => false;
        public bool IsWrite => true;
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

        public void WriteEnum(Enum @enum)
        {
            this.GetType().GetMethod("Write", new Type[] { @enum.GetType().GetEnumUnderlyingType() }).Invoke(this, new object[] { @enum });
        }

        public void Write7(int value)
        {
            this.Write7BitEncodedInt(value);
        }



        //public TypeCodeExtande WriteType(Type type)
        //{
        //    TypeCodeExtande typeCode;
        //    if (type.IsArray)
        //        typeCode = TypeCodeExtande.Array;
        //    //if(type.ise)
        //    else
        //        typeCode = (TypeCodeExtande)Type.GetTypeCode(type);
        //    Write((byte)typeCode);

        //    switch (typeCode)
        //    {
        //        case TypeCodeExtande.Array:
        //            Type typeElement = type.GetElementType();
        //            WriteType(typeElement);
        //            Write7(type.GetArrayRank());
        //            break;
        //        case TypeCodeExtande.Object:
        //            Write7(GetIdType(type));
        //            //var p = type.GetProperties();
        //            //Write7(p.Length);
        //            //foreach (var item in p)
        //            //{
        //            //    Write7(GetIdName(item));
        //            //}
        //            break;
        //        default:
                    
        //            break;
        //    }

        //    return typeCode;
        //}

        
        public void WriteObject(object value)
        {
            Type type = value.GetType();
            List<TypeCodeExtande> typeCodes = new List<TypeCodeExtande>();
            TypeCodeExtande code = WriteType(type, typeCodes);
            if (code == TypeCodeExtande.Array)
            {
                Array array = (Array)value;
                Write7(array.Length);
            }
        }

        public void WriteStruct(NETDynamic nETDynamic)
        {

        }

        public void WriteNode(NETDynamic nETDynamic)
        {
            
        }

        public TypeCodeExtande WriteType(Type type, List<TypeCodeExtande> typeCodes)
        {
            var t = type.GetTypeCodeExtande();
            typeCodes.Add(t);
            WriteEnum(t);
            if (t == TypeCodeExtande.Array)
            {
                int rank = type.GetArrayRank();
                Write7(type.GetArrayRank());
                WriteType(type.GetElementType(), typeCodes);
            }
            else if (t == TypeCodeExtande.List)
            {
                WriteType(type.GetGenericArguments()[0], typeCodes);
            }
            else if (t == TypeCodeExtande.Dictionary)
            {
                var types = type.GetGenericArguments();
                WriteType(types[0], typeCodes);
                WriteType(types[1], typeCodes);
            }
            else if (t == TypeCodeExtande.Structure)
            {
                //Write ID Str
            }
            return t;
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
            Type type = value.GetType();
            TypeCodeExtande typeCode = type.GetTypeCodeExtande();
            WriteEnum(typeCode);
            typeof(BinaryReaderE).GetMethod("Write", new Type[] { type }).Invoke(this, new object[] { value });
        }

        void IReaderWriterAlliaces.InvokeBoolean(ref bool value)
        {
            Write(value);
        }

        void IReaderWriterAlliaces.InvokeChar(ref char value)
        {
            Write(value);
        }

        void IReaderWriterAlliaces.InvokeSByte(ref sbyte value)
        {
            Write(value);
        }

        void IReaderWriterAlliaces.InvokeByte(ref byte value)
        {
            Write(value);
        }

        void IReaderWriterAlliaces.InvokeInt16(ref short value)
        {
            Write(value);
        }

        void IReaderWriterAlliaces.InvokeUInt16(ref ushort value)
        {
            Write(value);
        }

        void IReaderWriterAlliaces.InvokeInt32(ref int value)
        {
            Write(value);
        }

        void IReaderWriterAlliaces.InvokeUInt32(ref uint value)
        {
            Write(value);
        }

        void IReaderWriterAlliaces.InvokeInt64(ref long value)
        {
            Write(value);
        }

        void IReaderWriterAlliaces.InvokeUInt64(ref ulong value)
        {
            Write(value);
        }

        void IReaderWriterAlliaces.InvokeSingle(ref float value)
        {
            Write(value);
        }

        void IReaderWriterAlliaces.InvokeDouble(ref double value)
        {
            Write(value);
        }

        void IReaderWriterAlliaces.InvokeDecimal(ref decimal value)
        {
            Write(value);
        }

        void IReaderWriterAlliaces.InvokeDateTime(ref DateTime value)
        {
            Write(value.Ticks);
        }

        void IReaderWriterAlliaces.InvokeString(ref string value)
        {
            Write(value);
        }

        void IReaderWriterAlliaces.InvokeArray(ref Array value)
        {
            //Write7(value.Count);

        }

        void IReaderWriterAlliaces.InvokeInt7(ref int value)
        {
            Write7(value);
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
            WriteType(value, new List<TypeCodeExtande>());
        }

        void IReaderWriterAlliaces.InvokeSimpleIntArray(ref int[] childs)
        {
            throw new NotImplementedException();
        }

        void IReaderWriterAlliaces.InvokeIntArray(ref int[] value)
        {
            throw new NotImplementedException();
        }

        public void InvokeGuid(ref Guid guid)
        {
            Write(guid.ToByteArray());
        }

        public DictCounter<string> PropertyNames ;
        public DictCounter<Type> Types;
    }
}
