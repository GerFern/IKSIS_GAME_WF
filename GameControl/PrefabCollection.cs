#define test

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;

namespace GameCore
{

    //public class tttt : IEnumerable<KeyValuePair<int, Prefab>>, ICollection<Prefab>, IEnumerable, ICollection, IReadOnlyDictionary<int, Prefab>, IReadOnlyCollection<KeyValuePair<int, Prefab>>, ISerializable, IDeserializationCallback
    //{
    //    public Dictionary<int, Prefab> values = new Dictionary<int, Prefab>();

    //    public Prefab this[int key] => ((IReadOnlyDictionary<int, Prefab>)values)[key];

    //    public IEnumerable<int> Keys => ((IReadOnlyDictionary<int, Prefab>)values).Keys;

    //    public IEnumerable<Prefab> Values => ((IReadOnlyDictionary<int, Prefab>)values).Values;

    //    public int Count => ((ICollection)values).Count;

    //    public object SyncRoot => ((ICollection)values).SyncRoot;

    //    public bool IsSynchronized => ((ICollection)values).IsSynchronized;

    //    public bool IsReadOnly { get => false; }

    //    public void Add(params Point[] points)
    //    {
    //        //int key = values.Keys.Count > 0 ? values.Keys.Max() + 1 : 0;
    //        Add(new Prefab(points));
    //    }

    //    public void Add(Prefab item)
    //    {
    //        int key = values.Keys.Count > 0 ? values.Keys.Max() + 1 : 0;
    //        values.Add(key, item);
    //    }

    //    public void Clear()
    //    {
    //        values.Clear();
    //    }

    //    public bool Contains(Prefab item)
    //    {
    //        return values.ContainsValue(item);
    //    }

    //    public bool ContainsKey(int key)
    //    {
    //        return values.ContainsKey(key);
    //    }

    //    public void CopyTo(Array array, int index)
    //    {
    //        ((ICollection)values).CopyTo(array, index);
    //    }

    //    public void CopyTo(Prefab[] array, int arrayIndex)
    //    {
    //        ((ICollection)values).CopyTo(array, arrayIndex);
    //    }

    //    public IEnumerator GetEnumerator()
    //    {
    //        return ((IEnumerable)values).GetEnumerator();
    //    }

    //    public void GetObjectData(SerializationInfo info, StreamingContext context)
    //    {
    //        ((ISerializable)values).GetObjectData(info, context);
    //    }

    //    public void OnDeserialization(object sender)
    //    {
    //        ((IDeserializationCallback)values).OnDeserialization(sender);
    //    }

    //    public bool Remove(Prefab item)
    //    {
    //        foreach (var localitem in values)
    //        {
    //            if(localitem.Value.Equals(item))
    //            {
    //                values.Remove(localitem.Key);
    //                return true;
    //            }
    //        }
    //        return false;
    //    }

    //    public bool TryGetValue(int key, out Prefab value)
    //    {
    //        return ((IReadOnlyDictionary<int, Prefab>)values).TryGetValue(key, out value);
    //    }

    //    IEnumerator<KeyValuePair<int, Prefab>> IEnumerable<KeyValuePair<int, Prefab>>.GetEnumerator()
    //    {
    //        return ((IReadOnlyDictionary<int, Prefab>)values).GetEnumerator();
    //    }

    //    IEnumerator<Prefab> IEnumerable<Prefab>.GetEnumerator()
    //    {
    //        return values.Values.GetEnumerator();
    //    }
    //}


    public class PrefabCollection : Dictionary<int, (int Count, Prefab Prefab)>, ICollection<Prefab>, ICollection<(int count, Prefab prefab)>
    {
        public bool IsReadOnly { get => false; }

        public void Add(params Point[] points)
        {
            //int key = values.Keys.Count > 0 ? values.Keys.Max() + 1 : 0;
            Add(new Prefab(points));
        }

        public void Add(int count, params Point[] points)
        {
            //int key = values.Keys.Count > 0 ? values.Keys.Max() + 1 : 0;
            Add(count, new Prefab(points));
        }

        public void Add(Prefab item)
        {
            int key = Keys.Count > 0 ? Keys.Max() + 1 : 0;
            Add(key, (1, item));
        }

        public void Add(int count, Prefab item)
        {
            int key = Keys.Count > 0 ? Keys.Max() + 1 : 0;
            Add(key, (count, item));
        }

        public void Add((int count, Prefab prefab) item)
        {
            int key = Keys.Count > 0 ? Keys.Max() + 1 : 0;
            Add(key, item);
        }

        public bool Contains(Prefab item)
        {
            return ((ICollection<Prefab>)this).Contains(item);
        }

        public bool Contains((int count, Prefab prefab) item)
        {
            return this.ContainsValue(item);
        }

        public void CopyTo(Prefab[] array, int arrayIndex)
        {
            //this.Values.CopyTo(array.ToArray<Prefab>(), arrayIndex);
            ((ICollection<Prefab>)this).CopyTo(array, arrayIndex);
        }

        public void CopyTo((int count, Prefab prefab)[] array, int arrayIndex)
        {
            this.Values.CopyTo(array, arrayIndex);
        }

        public bool Remove(Prefab item)
        {
            foreach (var localitem in this)
            {
                if (localitem.Value.Equals(item))
                {
                    this.Remove(localitem.Key);
                    return true;
                }
            }
            return false;
        }

        public bool Remove((int count, Prefab prefab) item)
        {
            throw new NotImplementedException();
        }

        //internal Dictionary<int, int> ToDictionary(Func<object, object> p1, Func<object, int> p2)
        //{
        //    throw new NotImplementedException();
        //}

        IEnumerator<Prefab> IEnumerable<Prefab>.GetEnumerator()
        {
           return  Values.Select(a=>a.Prefab).GetEnumerator();
        }

        IEnumerator<(int count, Prefab prefab)> IEnumerable<(int count, Prefab prefab)>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }



}

