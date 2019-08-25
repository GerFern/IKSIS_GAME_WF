//using System;
//using System.Collections.Generic;
//using System.Net.Sockets;
//using System.Reflection;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.Text;

//namespace GameServer.ClientServer
//{
//    public class tSend
//    {
//        //public Client
//    }

//    public interface IClientServerObject
//    {
//        public static event Action<IClientServerObject> SendAllEvent;
//        public static event Action<IClientServerObject, PlayerClient> SendEvent;
//    }

//    public class Package
//    {
//        public PacketType IsAction;
//        public int UID { get; }
//        public string Hierarchy;
//        public int PackageSize;
//        public ISerializable o;
//        BinaryFormatter binaryFormatter = new BinaryFormatter();
//        System.IO.MemoryStream Stream = new System.IO.MemoryStream(1024);
//        public Package()
//        {
//        }
//        public Package(INode node)
//        {
//            //Socket socket = new Socket();
//            //socket.Send(Stream.)
//            binaryFormatter.Serialize(Stream, node);
//        }
//    }

//    public enum PacketType:byte
//    {
//        Undefined,
//        Object,
//        Action
//    }

//    public interface INode
//    {
//        public static Socket Server;
//        public int UID { get; }
//        public string PropertyName;
//        public Dictionary<string, INode> Childs { get; }
//        public void RecieveProperties(params NObject[] objects)
//        {
//            Type type = this.GetType();
//            foreach (var item in objects)
//            {
//                type.GetProperty(item.Property).SetValue(this, item.Value);
//            }
//        }

//        public void SendProperties(params string[] vs)
//        {
//            Type type = this.GetType();
//            NObject[] nObjects = new NObject[vs.Length];
//            for (int i = 0; i < vs.Length; i++)
//            {
//                var item = vs[i];
//                PropertyInfo propertyInfo = type.GetProperty(item);
//                nObjects[i] = new NObject(item, propertyInfo.GetValue(this));
//            }
//            SendProperties(nObjects);
//        }
//        public void SendProperties(params NObject[] vs)
//        {
//            SendPropsEvent?.Invoke(Server,this, vs);
//        }

//        public void SendProperties(Socket socket, params string[] vs)
//        {
//            Type type = this.GetType();
//            NObject[] nObjects = new NObject[vs.Length];
//            for (int i = 0; i < vs.Length; i++)
//            {
//                var item = vs[i];
//                PropertyInfo propertyInfo = type.GetProperty(item);
//                nObjects[i] = new NObject(item, propertyInfo.GetValue(this));
//            }
//            SendProperties(socket, nObjects);
//        }
//        public void SendProperties(Socket socket, params NObject[] vs)
//        {
//            SendPropsEvent?.Invoke(socket,this, vs);
//        }

//        public void SendProperties(IEnumerable<Socket> sockets, params string[] vs)
//        {
//            Type type = this.GetType();
//            NObject[] nObjects = new NObject[vs.Length];
//            for (int i = 0; i < vs.Length; i++)
//            {
//                var item = vs[i];
//                PropertyInfo propertyInfo = type.GetProperty(item);
//                nObjects[i] = new NObject(item, propertyInfo.GetValue(this));
//            }
//            SendProperties(sockets, nObjects);
//        }
//        public void SendProperties(IEnumerable<Socket> sockets, params NObject[] vs)
//        {
//            foreach (var item in sockets)
//            {
//                SendPropsEvent?.Invoke(Server, this, vs);
//            }
//        }
//        public INode Parent { get; }
//        public static event Action<Socket, INode, NObject[]> SendPropsEvent;
//        public static event Action<Socket, INode> SendEvent;
//        public static event Action<Socket, Package> SendPackageEvent;
//        //public static event Action<NObject[]> SendPropsEvent;
//        //public static event Action<INode> SendEvent;
//    }

//    public interface IRoot : INode
//    {
//        public void Recieve(Package package);
//    }

//    public struct NObject
//    {
//        public readonly string Property;
//        public readonly object Value;

//        public NObject(string property, object value)
//        {
//            Property = property;
//            Value = value;
//        }
//    }


//    public class RootObject : IRoot
//    {
//        public int UID => 0;
//        public Dictionary<string, INode> Childs { get; } = new Dictionary<string, INode>();
//        public INode Parent => null;
//        public RootObject ()
//        {

//        }

//        public void Recieve(Package package)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
