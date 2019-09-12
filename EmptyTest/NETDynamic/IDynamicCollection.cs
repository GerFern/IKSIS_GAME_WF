using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace EmptyTest.TNode
{
    public abstract class IDynamicCollection<T> : DynamicNode, IEnumerable<DynamicNode> where T:struct
    {
        public IDynamicCollection(NETDynamic parent, Type type) : base(parent, type) { }
        public abstract int Add(T item);
        public abstract void Remove(int index);
        public abstract int Insert(int index, T item);
        public abstract void Init(Array values);
        public abstract void Clear();
        public abstract IEnumerator<DynamicNode> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
