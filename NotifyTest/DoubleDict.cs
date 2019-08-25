using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NotifyTest
{
    public class DoubleDict<T1, T2> 
    {
        Dictionary<T1, T2> dict1 = new Dictionary<T1, T2>();
        Dictionary<T2, T1> dict2 = new Dictionary<T2, T1>();

        public bool ContainsT1(T1 item)
        {
            return dict1.ContainsKey(item);
        }
        public bool ContainsT2(T2 item)
        {
            return dict2.ContainsKey(item);
        }

        public void Clear()
        {
            dict1.Clear();
            dict2.Clear();
        }

        public void CopyToT1(T1[] array, int t)
        {
            dict1.Keys.CopyTo(array, t);
        }

        public void CopyToT2(T2[] array, int t)
        {
            dict2.Keys.CopyTo(array, t);
        }

        public int Count => dict1.Count;

        public void Add(T1 t1, T2 t2)
        {
            dict1.Add(t1, t2);
            try
            {
                dict2.Add(t2, t1);
            }
            catch(Exception ex)
            {
                dict1.Remove(t1);
                throw ex;
            }
        }

        public bool RemoveT1(T1 t1)
        {
            dict2.Remove(dict1[t1]);
            return dict1.Remove(t1);
        }

        public bool RemoveT2(T2 t2)
        {
            dict1.Remove(dict2[t2]);
            return dict2.Remove(t2);
        }

        public T1 FindT1(T2 t2)
        {
            return dict2[t2];
        }

        public T2 FindT2(T1 t1)
        {
            return dict1[t1];
        }

        public IEnumerator<T1> GetEnumeratorT1()
        {
            return dict2.Values.GetEnumerator();
        }

        public IEnumerator<T2> GetEnumeratorT2()
        {
            return dict1.Values.GetEnumerator();
        }
    }

    public class DictCounter<TValue> : IEnumerable<TValue>, ICollection<TValue>
    {
        DoubleDict<int, TValue> data = new DoubleDict<int, TValue>();
        int counter;
        public TValue this[int index]
        {
            get => data.FindT2(index);
            set
            {
                RemoveKey(index);
                data.Add(index, value);
            }
        }
        public int FindKey(TValue value)
        {
            return data.FindT1(value);
        }

        public int Count => data.Count;
        public bool IsReadOnly => false;

        public (int ID, bool WasContain) Add(TValue value)
        {
            int t = counter + 1;
            bool b = false;
            try
            {
                data.Add(t, value);
                counter = t;
            }
            catch
            {
                b = true;
                t = data.FindT1(value);
            }
            return (t, b);
        }

        public void Clear()
        {
            data.Clear();
        }

        public bool Contains(TValue item)
        {
            return data.ContainsT2(item);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            data.CopyToT2(array, arrayIndex);
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return data.GetEnumeratorT2();
        }

        public bool Remove(TValue item)
        {
            return data.RemoveT2(item);
        }

        public bool RemoveKey(int key)
        {
            return data.RemoveT1(key);
        }


        void ICollection<TValue>.Add(TValue item)
        {
            Add(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
