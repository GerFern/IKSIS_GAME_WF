using EmptyTest.TStreamHandler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace EmptyTest
{


    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TypeAlliacesAttribute : Attribute
    {
        public TypeCodeExtande Type { get; }
        public TypeAlliacesAttribute(TypeCodeExtande type) => Type = type;
    }


    public interface IReaderWriterAlliaces
    {
        bool IsRead { get; }
        bool IsWrite { get; }

        void InvokeGuid(ref Guid guid);
        //public void InvokeNode(ref NETDynamic node, ref int[] childs)
        //{
        //    InvokeEnum(ref node.__TypeCode);
        //    switch (node.TypeCode)
        //    {
        //        case TypeCodeExtande.Empty:
        //            break;
        //        case TypeCodeExtande.Object:
        //            break;
        //        case TypeCodeExtande.DBNull:
        //            break;
        //        case TypeCodeExtande.Boolean:
        //            break;
        //        case TypeCodeExtande.Char:
        //            break;
        //        case TypeCodeExtande.SByte:
        //            break;
        //        case TypeCodeExtande.Byte:
        //            break;
        //        case TypeCodeExtande.Int16:
        //            break;
        //        case TypeCodeExtande.UInt16:
        //            break;
        //        case TypeCodeExtande.Int32:
        //            break;
        //        case TypeCodeExtande.UInt32:
        //            break;
        //        case TypeCodeExtande.Int64:
        //            break;
        //        case TypeCodeExtande.UInt64:
        //            break;
        //        case TypeCodeExtande.Single:
        //            break;
        //        case TypeCodeExtande.Double:
        //            break;
        //        case TypeCodeExtande.Decimal:
        //            break;
        //        case TypeCodeExtande.DateTime:
        //            break;
        //        case TypeCodeExtande.String:
        //            break;
        //        case TypeCodeExtande.Array:
        //            break;
        //        case TypeCodeExtande.List:
        //            break;
        //        case TypeCodeExtande.Dictionary:
        //            break;
        //        case TypeCodeExtande.Structure:
        //            InvokeSimpleIntArray(ref childs); // Передача детей
        //            ////foreach (var item in childs)        -------------// Должно реализовываться в InvokeChilds
        //            ////{                                                //
        //            ////    InvokeInt7(ref item); // Передача UID узлов  //
        //            ////}                                   -------------// Должно реализовываться в InvokeChilds
        //            break;
        //        case TypeCodeExtande.Error:
        //            break;
        //        default:
        //            break;
        //    }
        //}

        void InvokeSimpleIntArray(ref int[] childs);

        [TypeAlliaces(TypeCodeExtande.Int32)]
        void InvokeInt7(ref int value);
        void InvokeSimpleObject(ref Object value);
        //public void InvokeDBNull        (ref DBNull   value);
        [TypeAlliaces(TypeCodeExtande.Boolean)]
        void InvokeBoolean(ref Boolean value);
        [TypeAlliaces(TypeCodeExtande.Char)]
        void InvokeChar(ref Char value);
        [TypeAlliaces(TypeCodeExtande.SByte)]
        void InvokeSByte(ref SByte value);
        [TypeAlliaces(TypeCodeExtande.Byte)]
        void InvokeByte(ref Byte value);
        [TypeAlliaces(TypeCodeExtande.Int16)]
        void InvokeInt16(ref Int16 value);
        [TypeAlliaces(TypeCodeExtande.UInt16)]
        void InvokeUInt16(ref UInt16 value);
        void InvokeInt32(ref Int32 value);
        [TypeAlliaces(TypeCodeExtande.UInt32)]
        void InvokeUInt32(ref UInt32 value);
        [TypeAlliaces(TypeCodeExtande.Int64)]
        void InvokeInt64(ref Int64 value);
        [TypeAlliaces(TypeCodeExtande.UInt64)]
        void InvokeUInt64(ref UInt64 value);
        [TypeAlliaces(TypeCodeExtande.Single)]
        void InvokeSingle(ref Single value);
        [TypeAlliaces(TypeCodeExtande.Double)]
        void InvokeDouble(ref Double value);
        [TypeAlliaces(TypeCodeExtande.Decimal)]
        void InvokeDecimal(ref Decimal value);
        [TypeAlliaces(TypeCodeExtande.DateTime)]
        void InvokeDateTime(ref DateTime value);
        [TypeAlliaces(TypeCodeExtande.String)]
        void InvokeString(ref String value);
        [TypeAlliaces(TypeCodeExtande.Array)]
        void InvokeArray(ref Array value);
        [TypeAlliaces(TypeCodeExtande.List)]

        void InvokeIntArray(ref int[] value);
        void InvokeList(ref ICollection value);
        [TypeAlliaces(TypeCodeExtande.Dictionary)]
        void InvokeDictionate(ref IDictionary value);
        //[TypeAlliaces(TypeCodeExtande.UntypedArray)]
        //public void InvokeUntypedArray(ref ICollection value);
        //[TypeAlliaces(TypeCodeExtande.UntypedList)]
        //public void InvokeUntypedList(ref ICollection value);
        //[TypeAlliaces(TypeCodeExtande.UntypedDictionary)]
        //public void InvokeUntypedDictionate(ref IDictionary value);
        void InvokeType(ref Type value);

        //public static void InitAlliaces()
        //{
        //    Methods = new Dictionary<TypeCodeExtande, string>();
        //    foreach (var item in typeof(IReaderWriterAlliaces).GetMethods())
        //    {
        //        TypeAlliacesAttribute attribute = item.GetCustomAttribute<TypeAlliacesAttribute>();
        //        if (attribute != null) Methods.Add(attribute.Type, item.Name);
        //    }
        //}
        //public static Dictionary<TypeCodeExtande, string> Methods { get; private set; }
    }
}
