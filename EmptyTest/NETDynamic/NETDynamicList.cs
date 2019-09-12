using EmptyTest.TStreamHandler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EmptyTest.TNode
{
    internal class NETDynamicList<T> : IDynamicCollection<T> where T:struct
    {
        private StreamWRHandler handler;
        private Type type;
        private int uID;


        //List<IDynamic> __childs;
        //List<object> __objects;
        //public int[] __childsUID
        //{
        //    get => __childs.Select(a => a.__UID).ToArray();
        //}
        List<DynamicNode> __ChildNodes;


         

        public NETDynamicList(NETDynamic parent, Type type):base(parent,type)
        {
            //__IsClientStorage = isClientStorage;
        }

        public override bool __Inited { get; protected set; }
        public override TypeCodeExtande __TypeCode { get; }
        public override bool __HasChild { get; }
        public override bool __IsCollection { get; }

        public override int Add(T item)
        {
            throw new NotImplementedException();
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override IEnumerator<DynamicNode> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override void GetRecourciveSimpleNode(List<NETDynamic> list)
        {
            throw new NotImplementedException();
        }

        public override void GetRecourciveUID(List<int> list)
        {
            throw new NotImplementedException();
        }

        public override void Init(Array values)
        {
            throw new NotImplementedException();
        }

        public override int Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public override void Remove(int index)
        {
            throw new NotImplementedException();
        }

        internal override void SendNodeRecursive(BinaryWriterE writer = null, bool start = false)
        {
            throw new NotImplementedException();
        }

        internal override void SetValue(object value)
        {
            throw new NotImplementedException();
        }

        internal override void UpdateValue(object value)
        {
            throw new NotImplementedException();
        }
    }
}