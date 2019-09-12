using EmptyTest.TStreamHandler;
using System;
using System.Collections.Generic;

namespace EmptyTest.TNode
{
    internal class NETDynamicDictionary<TKey, TValue> : DynamicNode
    {
        public NETDynamicDictionary(NETDynamic parent, Type type)
        {
        }

        public override bool __IsCollection => true;

        public override bool __Inited { get; protected set; }
        public override TypeCodeExtande __TypeCode { get; }
        public override bool __HasChild { get; }

        public override void GetRecourciveSimpleNode(List<NETDynamic> list)
        {
            throw new NotImplementedException();
        }

        public override void GetRecourciveUID(List<int> list)
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
            __Inited = true;
            throw new NotImplementedException();
        }


    }
}