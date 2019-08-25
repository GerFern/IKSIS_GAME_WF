using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace GameServer.ClientServer
{
    public static class Class1
    {
        public static StreamWriter Stream { get;}
        public static void SendObject(object obj)
        {
            Type typestream = Stream.GetType();
            Type typeobj = obj.GetType();
            TypeCode typeCode = Type.GetTypeCode(typeobj);
            switch (typeCode)
            {
                case TypeCode.Object:
                    //TODO aaaa
                    break;
                case TypeCode.String:
                    Stream.Write((byte)typeCode);
                    string str = (string)obj;
                    Stream.Write(str.Length);
                    Stream.Write(str);
                    break;
                default:
                    Stream.Write((byte)typeCode);
                    var m = typestream.GetMethod("Write", new Type[] { typeobj });
                    m.Invoke(Stream, new object[] { obj });
                    break;
            }
        }
    }


    public enum TypeCmd
    {
        Action,
        Object,
        Insert,
        ObjectID
    }
}
