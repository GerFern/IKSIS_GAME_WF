using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NotifyTest
{
    public class MyArray : IArray
    {
        public object this[int index]
        {
            get => Array.GetValue(index);
            set => Array.SetValue(value, index);
        }
        Array Array;

        public bool IsFixedSize => true;
        public bool IsReadOnly { get; }
        public int Count { get; }
        public bool IsSynchronized { get; }
        public object SyncRoot { get; }

        public void SetArray(Array array) => Array = array;
        public Array GetArray(Array array) => Array ;

        public int Add(object value)
        {
            return 0;
        }

        public void Clear()
        {
            return;
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public interface IArray : IList
    {
        //Array
        //IList<>
        //public object this[int index]
        //{ get;set; }
        //public void SetArray(Array array);
        //public Array GetArray(Array array);

    }
}
