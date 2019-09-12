using System;
using System.Collections;
using System.Collections.Generic;

namespace EmptyTest.TStreamHandler
{
    public class ReadOnlyDoubleDict<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>, IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IReadOnlyDoubleDict<TKey, TValue> 
    {
        Dictionary<TKey, TValue> dict1;
        Dictionary<TValue, TKey> dict2;

        public TKey FindKey(TValue value) => dict2[value];
        public TValue FindValue(TKey key) => dict1[key];

        public TValue this[TKey key] => ((IReadOnlyDictionary<TKey, TValue>)dict1)[key];

        TValue IDictionary<TKey, TValue>.this[TKey key] { get => dict1[key]; set => throw new NotSupportedException(); }

        public IEnumerable<TKey> Keys => ((IReadOnlyDictionary<TKey, TValue>)dict1).Keys;

        public IEnumerable<TValue> Values => ((IReadOnlyDictionary<TKey, TValue>)dict1).Values;

        public int Count => ((IReadOnlyDictionary<TKey, TValue>)dict1).Count;

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
            return
            ((ICollection<KeyValuePair<TKey, TValue>>)dict1).Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            return ((IReadOnlyDictionary<TKey, TValue>)dict1).ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)dict1).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return ((IReadOnlyDictionary<TKey, TValue>)dict1).GetEnumerator();
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
            return ((IReadOnlyDictionary<TKey, TValue>)dict1).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IReadOnlyDictionary<TKey, TValue>)dict1).GetEnumerator();
        }

        public ReadOnlyDoubleDict(IDictionary<TKey, TValue> dictionary)
        {
            foreach (var item in dictionary)
            {
                dict1.Add(item.Key, item.Value);
                dict2.Add(item.Value, item.Key);
            }
        }
    }
}
