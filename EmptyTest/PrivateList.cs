using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EmptyTest
{
    public class PrivateList<TValue> : IReadOnlyList<TValue>, IList<TValue>, IList
    {
        public PrivateList(out Action<TValue> addPrivateMethod, 
            out Action<TValue> removePrivateMethod, out Action clearPrivateMethod)
        {
            addPrivateMethod = new Action<TValue>((value)=> list.Add(value));
            removePrivateMethod = new Action<TValue>((value)=> list.Remove(value));
            clearPrivateMethod = new Action(() => list.Clear());
            list = new List<TValue>();
        }

        public PrivateList(ICollection<TValue> list, out Action<TValue> addPrivateMethod,
            out Action<TValue> removePrivateMethod, out Action clearPrivateMethod)
            : this(out addPrivateMethod, out removePrivateMethod, out clearPrivateMethod)
        {
            foreach (var item in list)
                this.list.Add(item);
        }

        readonly List<TValue> list;
        public TValue this[int index] { get => ((IList<TValue>)list)[index]; set => throw new NotSupportedException(); }

        public int Count => ((IList<TValue>)list).Count;

        public bool IsReadOnly => true;

        public bool IsFixedSize => ((IList)list).IsFixedSize;

        public bool IsSynchronized => ((IList)list).IsSynchronized;

        public object SyncRoot => ((IList)list).SyncRoot;

        object IList.this[int index] { get => ((IList)list)[index]; set => throw new NotSupportedException(); }

        public void Add(TValue item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(TValue item)
        {
            return ((IList<TValue>)list).Contains(item);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            ((IList<TValue>)list).CopyTo(array, arrayIndex);
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return ((IList<TValue>)list).GetEnumerator();
        }

        public int IndexOf(TValue item)
        {
            return ((IList<TValue>)list).IndexOf(item);
        }

        public void Insert(int index, TValue item)
        {
            throw new NotSupportedException();
        }

        public bool Remove(TValue item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<TValue>)list).GetEnumerator();
        }

        public int Add(object value)
        {
            throw new NotSupportedException();
        }

        public bool Contains(object value)
        {
            return ((IList)list).Contains(value);
        }

        public int IndexOf(object value)
        {
            return ((IList)list).IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        public void Remove(object value)
        {
            throw new NotSupportedException();
        }

        public void CopyTo(Array array, int index)
        {
            ((IList)list).CopyTo(array, index);
        }
    }


    public class PrivateDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>, IDictionary<TKey,TValue>
    {
        readonly Dictionary<TKey, TValue> dictionary;

        public PrivateDictionary(out Action<TKey, TValue> addPrivateMethod,
             out Action<TKey> removePrivateMethod, out Action clearPrivateMethod)
        {
            addPrivateMethod = new Action<TKey, TValue>((key, value) => dictionary[key] = value);
            removePrivateMethod = new Action<TKey>((key) => dictionary.Remove(key));
            clearPrivateMethod = new Action(() => dictionary.Clear());
            dictionary = new Dictionary<TKey, TValue>();
        }

        public PrivateDictionary(ICollection<KeyValuePair<TKey, TValue>> collection, out Action<TKey, TValue> addPrivateMethod,
             out Action<TKey> removePrivateMethod, out Action clearPrivateMethod)
            : this(out addPrivateMethod, out removePrivateMethod, out clearPrivateMethod)
        {
            foreach (var item in collection)
                this.dictionary.Add(item.Key, item.Value);
        }

        public TValue this[TKey key] => dictionary[key];

        TValue IDictionary<TKey, TValue>.this[TKey key] { get => dictionary[key]; set => throw new NotSupportedException(); }

        public IEnumerable<TKey> Keys => ((IReadOnlyDictionary<TKey, TValue>)dictionary).Keys;

        public IEnumerable<TValue> Values => ((IReadOnlyDictionary<TKey, TValue>)dictionary).Values;

        public int Count => dictionary.Count;

        public bool IsReadOnly { get; }
        ICollection<TKey> IDictionary<TKey, TValue>.Keys { get; }
        ICollection<TValue> IDictionary<TKey, TValue>.Values { get; }

        public void Add(TKey key, TValue value)
        {
            throw new NotSupportedException();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return ((IReadOnlyDictionary<TKey, TValue>)dictionary).GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            throw new NotSupportedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IReadOnlyDictionary<TKey, TValue>)dictionary).GetEnumerator();
        }
    }
}
