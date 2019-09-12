using System;
using System.Collections.Generic;
using System.Text;
using EmptyTest.TExtensions;
using EmptyTest.TStreamHandler;

namespace EmptyTest.TNode
{
    public class NETDynamicMainContainer : NETDynamicStruct
    {
        DataHandler DataHandler { get; }
        Dictionary<int, DynamicNode> __Storage;
        int uid = 0;
        int GetNewUID()
        {
            return ++uid;
        }
        public NETDynamicMainContainer(StreamWRHandler handler, DataHandler dataHandler, bool isClient) : base(dataHandler.ContainerType, isClient)
        {
            //__IsClientStorage = isClientStorage;
            __Storage = new Dictionary<int, DynamicNode>();
            __Storage.Add(0, this);
            DataHandler = dataHandler ?? throw new ArgumentNullException(nameof(dataHandler));
        }

        internal void Register(DynamicNode node)
        {
            int uid = GetNewUID();
            node.__UID = uid;
            __Storage.Add(uid, node);
        }

        public DynamicNode CreateNode(DynamicNode parent, Type type)
        {
            DynamicNode node;
            //int UID = GetNewUID();
            TypeCodeExtande typeCode = type.GetTypeCodeExtande();
            switch (typeCode)
            {
                case TypeCodeExtande.Array:
                    node = (DynamicNode)Activator.CreateInstance(
                        typeof(NETDynamicArray<>).MakeGenericType(type.GetElementType()),
                        parent, type);
                        //= new NETDynamicArray(this, type, UID);
                    break;
                case TypeCodeExtande.List:
                    node = (DynamicNode)Activator.CreateInstance(
                        typeof(NETDynamicList<>).MakeGenericType(type.GenericTypeArguments),
                        parent, type);
                        //new NETDynamicList(this, type, UID);
                    break;
                case TypeCodeExtande.Dictionary:
                    node = (DynamicNode)Activator.CreateInstance(
                        typeof(NETDynamicDictionary<,>).MakeGenericType(type.GenericTypeArguments),
                        parent, type);
                    //new NETDynamicDictionary(this, type, UID);
                    break;
                case TypeCodeExtande.Structure:
                    node = new NETDynamicStruct(parent, type);
                    break;
                default:
                    node = new NETDynamic(parent, type);
                    break;
            }
            return node;
        }
    }
}
