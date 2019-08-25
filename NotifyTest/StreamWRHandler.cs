using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace NotifyTest
{
    public class StreamWRHandler
    {
        public BinaryWriterE Writer
        {
            get => writer;
            set
            {
                writer = value;
                value.PropertyNames = PropertyNames;
                value.Types = Types;
            }
        }
        public BinaryReaderE Reader
        {
            get => reader;
            set
            {
                reader = value;
                value.PropertyNames = PropertyNames;
                value.Types = Types;
            }
        }


        //public TData Data { get; }

        public StreamWRHandler()
        {
            //Data = new TData();
        }

        public StreamWRHandler(BinaryWriterE writer, BinaryReaderE reader)
        {
            Writer = writer;
            Reader = reader;
        }

        public void SendTopNode(TopNode topNode)
        {
            Writer.Write7(topNode.UID);
        }

        public void SendNode(Node node)
        {
            Writer.Write7(node.UID);
            Type type = node.Value.GetType();
            TypeCodeExtande typeCode = Writer.WriteType(type);
            var p = type.GetProperties();
            Writer.Write7(p.Length);
            foreach (var item in p)
            {
                Writer.Write7(PropertyNames.FindKey(item.Name));
                var obj = item.GetValue(node.Value);
                Type tobj;
                if (obj==null)
                {

                }
                else
                {

                 tobj =   obj.GetType();
                if(tobj.GetInterfaces().Contains(typeof(INode)))
                    {
                        //if()
                    }
                }
            }
            //if(typeCode == TypeCodeExtande.Object)
            //{
                
            //}
        }


        public virtual void Send(object value)
        {
            SendObject(value);
        }

        public void SendObject(object obj)
        {
            SendObject(0, obj);
        }

        public void SendObject(int idname, object obj)
        {
            Writer.Write7(idname);
            Type typestream = Writer.GetType();
            Type type = obj.GetType();
            TypeCodeExtande typeCode =
            Writer.WriteType(type);
            //if (type.IsArray)
            //    typeCode = TypeCodeExtande.Array;
            //else
            //    typeCode = (TypeCodeExtande)Type.GetTypeCode(type);
            //Writer.Write((byte)typeCode);
            switch (typeCode)
            {
                case TypeCodeExtande.Array:
                    Type typeElement = type.GetElementType();
                    System.Array array = (System.Array)obj;
                    int rank = type.GetArrayRank();
                    int[] length = new int[rank];
                    int[] indices = new int[rank];
                    Writer.Write7(rank);
                    for (int i = 0; i < rank; i++)
                    {
                        var t = length[i] = array.GetLength(i);
                        Writer.Write7(t);
                    }
                    bool b1 = true;
                    bool b2 = true;
                    int curRank = 0;
                    while (b1)
                    {
                        SendObject(array.GetValue(indices));
                        while (true)
                        {
                            var limit = ++indices[curRank];
                            if (limit == length[curRank])
                            {
                                indices[curRank] = 0;
                                curRank++;
                                if (curRank == rank)
                                {
                                    b1 = false;
                                    break;
                                }
                            }

                            else break;
                        }
                        curRank = 0;
                    }
                    break;
                case TypeCodeExtande.Object:
                    var p = type.GetProperties();
                    int idType = GetIdType(type);
                    int childCount = p.Length;
                    //Writer.Write(idType);
                    Writer.Write7(childCount);
                    foreach (var item in p)
                    {
                        SendObject(GetId(item), item.GetValue(obj));
                        //item.GetValue(obj);
                    }
                    break;
                //case TypeCode.String:
                //    Writer.Write((byte)typeCode);
                //    string str = (string)obj;
                //    //Writer.Write(str.Length);
                //    Writer.Write(str);
                //    break;
                default:
                    //Writer.Write((byte)typeCode);
                    var m = typestream.GetMethod("Write", new Type[] { type });
                    m.Invoke(Writer, new object[] { obj });
                    break;
            }
        }

        public object RecieveObject(out int id)
        {
            Type typeStream = Reader.GetType();
            id = Reader.Read7();
            TypeCodeExtande typeCode; //= (TypeCodeExtande)Reader.ReadByte();
            Type type = Reader.ReadType(out typeCode);
            switch (typeCode)
            {
                case TypeCodeExtande.Array:

                    int rank = Reader.Read7();
                    int[] length = new int[rank];
                    int[] indices = new int[rank];
                    //Writer.Write7(rank);
                    for (int i = 0; i < rank; i++)
                    {
                        length[i] = Reader.Read7();
                    }
                    System.Array array = System.Array.CreateInstance(type.GetElementType(), length);
                    bool b1 = true;
                    bool b2 = true;
                    int curRank = 0;
                    while (b1)
                    {
                        array.SetValue(RecieveObject(out _), indices);
                        while (true)
                        {
                            var limit = ++indices[curRank];
                            if (limit == length[curRank])
                            {
                                indices[curRank] = 0;
                                curRank++;
                                if (curRank == rank)
                                {
                                    b1 = false;
                                    break;
                                }
                            }

                            else break;
                        }
                        curRank = 0;
                    }
                    return array;
                case TypeCodeExtande.Object:
                    //ushort idtype = Reader.ReadUInt16();
                    //Type type = Types[idtype];
                    var obj1 = Activator.CreateInstance(type);
                    int childCount = Reader.Read7();
                    for (int i = 0; i < childCount; i++)
                    {
                        var obj2 = RecieveObject(out int id2);
                        type.GetProperty(PropertyNames[id2]).SetValue(obj1, obj2);
                    }
                    return obj1;
                    //TODO aaaa
                    break;
                //case TypeCode.String:
                //    Writer.Write((byte)typeCode);
                //    string str = (string)obj;
                //    //Writer.Write(str.Length);
                //    Writer.Write(str);
                //    break;
                default:
                    //Writer.Write((byte)typeCode);
                    //Type type = Type.GetType("System." + typeCode);
                    var m = typeStream.GetMethod("Read" + typeCode);
                    return m.Invoke(Reader, new object[0]);
                    break;
            }
            return null;
        }

        protected void Recieve()
        {
            var obj = RecieveObject(out _);
            InvokeGetObj(obj);
        }

        int GetId(PropertyInfo property)
        {
            return 
            PropertyNames.FindKey(property.Name);
            //foreach (var item in Properties)
            //{
            //    if (item.Value == property) return item.Key;
            //}
            //throw new Exception("aaaa");
        }

        int GetIdType(Type type)
        {
            if (type.IsGenericType) type = type.GetGenericTypeDefinition();
            return Types.FindKey(type);
            //foreach (var item in Types)
            //{
            //    if (item.Value == type)
            //        return item.Key;
            //}
            //throw new Exception("aaaa");
        }

        //public Dictionary<int, PropertyInfo> Properties = new Dictionary<int, PropertyInfo>();
        public DictCounter<string> PropertyNames { get; } = new DictCounter<string>();
        public DictCounter<Type> Types { get; } = new DictCounter<Type>();


        private BinaryWriterE writer;
        private BinaryReaderE reader;

        public void AddType(Type type)
        {
            if (type.IsGenericType) type = type.GetGenericTypeDefinition();
            if (Types.Add(type).WasContain) return;
            foreach (var item in type.GetProperties())
            {
                PropertyNames.Add(item.Name);
            }
        }


        public event EventHandler<GetObj> ObjectGetted;

        protected void InvokeGetObj(object value)
        {
            ObjectGetted?.Invoke(this, new GetObj(new int[0], value));
        }

        public class GetObj : EventArgs
        {
            public GetObj(int[] vs, object value)
            {
                Vs = vs;
                Value = value;
            }

            public int[] Vs { get; }
            public object Value { get; }
        }
    }


    public enum TypeCmd
    {
        Action,
        Object,
        Insert,
        ObjectID
    }

    public enum TypeCodeExtande : byte
    {
        Empty = 0,
        Object = 1,
        DBNull = 2,
        Boolean = 3,
        Char = 4,
        SByte = 5,
        Byte = 6,
        Int16 = 7,
        UInt16 = 8,
        Int32 = 9,
        UInt32 = 10,
        Int64 = 11,
        UInt64 = 12,
        Single = 13,
        Double = 14,
        Decimal = 15,
        DateTime = 16,
        String = 18,
        Array = 32,
        SpecialObject = 33
    }
}
