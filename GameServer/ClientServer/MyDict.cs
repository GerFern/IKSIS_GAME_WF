using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Linq;

namespace GameServer.ClientServer
{
    public class MyDictEvent<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>
    {
        readonly Dictionary<TKey, TValue> bdict = new Dictionary<TKey, TValue>();

        public TValue this[TKey key]
        {
            get => ((IDictionary<TKey, TValue>)bdict)[key];
            set
            {
                var oldValue = bdict[key];
                ((IDictionary<TKey, TValue>)bdict)[key] = value;
                EditExist?.Invoke(this, new Edit<TKey, TValue>(key, oldValue, value));
            }
        }

        public int Count => ((ICollection<KeyValuePair<TKey, TValue>>)bdict).Count;

        public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)bdict).IsReadOnly;

        public ICollection<TKey> Keys => ((IDictionary<TKey, TValue>)bdict).Keys;

        public ICollection<TValue> Values => ((IDictionary<TKey, TValue>)bdict).Values;


        public void Add(KeyValuePair<TKey, TValue> item)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)bdict).Add(item);
            AddingNew?.Invoke(this, new AddNew<TKey, TValue>(item.Key, item.Value));
        }

        public void Add(TKey key, TValue value)
        {
            ((IDictionary<TKey, TValue>)bdict).Add(key, value);
            AddingNew?.Invoke(this, new AddNew<TKey, TValue>(key, value));
        }

        public void Clear()
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)bdict).Clear();
            Cleared?.Invoke(this, EventArgs.Empty);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)bdict).Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            return ((IDictionary<TKey, TValue>)bdict).ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)bdict).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)bdict).GetEnumerator();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            bool b = ((ICollection<KeyValuePair<TKey, TValue>>)bdict).Remove(item);
            Removed?.Invoke(this, new Remove<TKey, TValue>(item.Key, item.Value));
            return b;
        }

        public bool Remove(TKey key)
        {
            TValue value = bdict[key];
            bool b = ((IDictionary<TKey, TValue>)bdict).Remove(key);
            Removed?.Invoke(this, new Remove<TKey, TValue>(key, value));
            return b;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return ((IDictionary<TKey, TValue>)bdict).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)bdict).GetEnumerator();
        }

        public event EventHandler<AddNew<TKey, TValue>> AddingNew;
        public event EventHandler<Remove<TKey, TValue>> Removed;
        public event EventHandler<Edit<TKey, TValue>> EditExist;
        public event EventHandler Cleared;
        public event EventHandler BeforeClear;
    }

    public class AddNew<TKey, TValue> : EventArgs
    {
        public AddNew(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public TKey Key { get; }
        public TValue Value { get; }
    }

    public class Edit<TKey, TValue> : EventArgs
    {
        public Edit(TKey key, TValue oldValue, TValue newValue)
        {
            Key = key;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public TKey Key { get; }
        public TValue OldValue { get; }
        public TValue NewValue { get; }
    }

    public class Remove<TKey, TValue>
    {
        public Remove(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public TKey Key { get; }
        public TValue Value { get; }
    }
}
