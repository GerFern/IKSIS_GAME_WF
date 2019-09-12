using EmptyTest.TExtensions;
using EmptyTest.TStreamHandler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace EmptyTest.TNode
{
    public class NETDynamicArray<T> : IDynamicCollection<T> where T:struct
    {
    
        public int Rank;
        public int[] lowBounds;
        public int[] length;
        

     

        //object __This;
    

        public ArrayNode<T> __Childs;

        public override bool __Inited { get; protected set; }
        public override TypeCodeExtande __TypeCode { get; }
        public override bool __HasChild { get; }
        public override bool __IsCollection { get; }

        public NETDynamicArray(NETDynamic parent, Type type) : base(parent, type)
        {
            //__IsClientStorage = isClientStorage;
        }

        //public NETDynamicArray(StreamWRHandler handler, Type type, object value)
        //{
        //    Array arr = (Array)value;
        //    __StreamWRHandler = handler;
        //    __Type = type;

        //}

    

        public class ArrayNode<T2>
        {
            NETDynamicMainContainer mainContainer;
            //public StreamWRHandler __StreamWRHandler { get; }
            Array Nodes;
            public int Rank => Nodes.Rank;
            protected int[] LowerBounds { get; }
            protected int[] Lengths { get; }
            public DynamicNode this[int[] indices]
            {
                get => (DynamicNode)Nodes.GetValue(indices);
            }
            //public IDynamic GetNode(params int[] index)
            //{
            //    return __StreamWRHandler.ServerNodeStorage[this[index]];
            //}
            public ArrayNode(Array reals, NETDynamicMainContainer mainContainer, DynamicNode parent)
            {
                Type elementType = reals.GetType().GetElementType();
                if (elementType != typeof(T2)) throw new Exception("AAAA");
                int rank = reals.Rank;
                LowerBounds = new int[rank];
                Lengths = new int[rank];
                for (int i = 0; i < rank; i++)
                {
                    Lengths[i] = reals.GetLength(i);
                    LowerBounds[i] = reals.GetLowerBound(i);
                }
                Nodes = Array.CreateInstance(typeof(int), Lengths, LowerBounds);
                Nodes.SetForEach(() => mainContainer.CreateNode(parent, elementType));
                this.mainContainer = mainContainer;
                //__StreamWRHandler = handler;
                LowerBounds = new int[rank];
                Lengths = new int[rank];
                for (int i = 0; i < rank; i++)
                {
                    LowerBounds[i] = reals.GetLowerBound(i);
                    Lengths[i] = reals.GetLength(i);
                }
            }

        //    IEnumerator<int> IEnumerable<int>.GetEnumerator()
        //    {
        //        return ((IEnumerable<int>)Nodes).GetEnumerator();
        //    }

        //    IEnumerator IEnumerable.GetEnumerator()
        //    {
        //        return Nodes.GetEnumerator();
        //    }

        //    IEnumerator<IDynamic> IEnumerable<IDynamic>.GetEnumerator()
        //    {
        //        foreach (int item in Nodes)
        //        {
        //            yield return __StreamWRHandler.ServerNodeStorage[item];
        //        }
        //        yield break;
        //    }
        }

    

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = null;
            try
            {
                result = __Childs[indexes.Cast<int>().ToArray()];
                return true;
            }
            catch { return false; }
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            try
            {
                __Childs[indexes.Cast<int>().ToArray()].UpdateValue(value);
                return true;
            }
            catch { return false; }
        }

        //public IEnumerator GetEnumerator()
        //{
        //    return ((IEnumerable)__Childs).GetEnumerator();
        //}


        //IEnumerator<int> IEnumerable<int>.GetEnumerator()
        //{
        //    return ((IEnumerable<int>)__Childs).GetEnumerator();
        //}

        public override int Add(T item)
        {
            throw new NotImplementedException();
        }

        public override void Remove(int index)
        {
            throw new NotImplementedException();
        }

        public override int Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public override void Init(Array values)
        {
            throw new NotImplementedException();
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override IEnumerator<DynamicNode> GetEnumerator()
        {
            return ((IEnumerable<DynamicNode>)__Childs).GetEnumerator();
        }

        public override void GetRecourciveUID(List<int> list)
        {
            throw new NotImplementedException();
        }

        public override void GetRecourciveSimpleNode(List<NETDynamic> list)
        {
            throw new NotImplementedException();
        }

        internal override void SendNodeRecursive(BinaryWriterE writer = null, bool start = false)
        {
            throw new NotImplementedException();
        }

        internal override void UpdateValue(object value)
        {
            throw new NotImplementedException();
        }

        internal override void SetValue(object value)
        {
            throw new NotImplementedException();
        }
    }

    


}
