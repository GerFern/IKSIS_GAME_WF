using EmptyTest.TExtensions;
using EmptyTest.TStreamHandler;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace EmptyTest.TNode
{
    public abstract class DynamicNode: DynamicObject
    {
        public object SendMethod(System.Reflection.MethodInfo methodInfo, object[] args)
        {
            return null;
        }
        protected DynamicNode(DynamicNode parent, Type type)
        {
            __Type = type;
            Parent = parent;
            parent.MainContainer.Register(this);
        }

        protected DynamicNode()
        {
            __UID = 0;
        }

        protected DynamicNode(Type type)
        {
            MainContainer = (NETDynamicMainContainer)this;
            __Type = type;
        }

        public NETDynamicMainContainer MainContainer { get; protected set; }
        public DynamicNode Parent { get; internal set; }
        public virtual bool __IsClientStorage => MainContainer.__IsClientStorage;
        public virtual StreamWRHandler __StreamWRHandler => MainContainer.__StreamWRHandler;
        public abstract bool __Inited { get; protected set; }
        public abstract TypeCodeExtande __TypeCode { get; }
        public Type __Type { get; protected set; }
        public abstract bool __HasChild { get; }
        //public bool __IsStruct { get; }
        public abstract bool __IsCollection { get; }
        public int __UID { get; internal set; }
        //public int __StructID { get; }
        //internal void SendNode()
        //{
        //    __StreamWRHandler.SendObject(this);
        //}

        public abstract void GetRecourciveUID(List<int> list);
        public abstract void GetRecourciveSimpleNode(List<NETDynamic> list);
        internal abstract void SendNodeRecursive(BinaryWriterE writer = null, bool start = false);
        internal abstract void UpdateValue(object value);
        internal abstract void SetValue(object value);
        //internal void SendNode();

        //public static IDynamic CreateDynamicNode(StreamWRHandler handler, Type type, int UID, bool isClientStorage)
        //{
        //    TypeCodeExtande typeCode = type.GetTypeCodeExtande();
        //    switch (typeCode)
        //    {
        //        case TypeCodeExtande.Array:
        //            return new NETDynamicArray(handler, type, UID, isClientStorage);
        //        case TypeCodeExtande.List:
        //            return new NETDynamicList(handler, type, UID, isClientStorage);
        //        case TypeCodeExtande.Dictionary:
        //            return new NETDynamicDictionary(handler, type, UID, isClientStorage);
        //        case TypeCodeExtande.Structure:
        //            return new NETDynamicStruct(handler, type, UID, isClientStorage);
        //        default:
        //            return new NETDynamic(handler, type, UID, isClientStorage);
        //    }
        //}
    }
}
