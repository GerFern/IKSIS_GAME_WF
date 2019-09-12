using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EmptyTest.Proxy
{
    public class Reader
    {
        BinaryFormatter formatter = new BinaryFormatter();

        Thread thread;
        ConcurrentDictionary<int, ValueWaiter> dictWaiters = new ConcurrentDictionary<int, ValueWaiter>();
        
        object listLocker = new object();
        BinaryReaderE reader;
        public object GetValue(int id)
        {
            ValueWaiter waiter = CreateOrGetWaiter(id);
             //waiter = list.GetOrAdd(id, waiter);
            var result = waiter.GetValue();
            dictWaiters.Remove(id, out _);
            return result;
        }

        public void Listener()
        {
            while (true)
            {
                Read();
            }
        }

        public Reader(BinaryReaderE reader)
        {
            this.reader = reader;
            //thread = new Thread(Listener) { Name = "ClientResultWaiter" };
            //thread.Start();
        }

        ~Reader()
        {
            //thread.Abort();
        }

        public class ValueWaiter
        {
            readonly EventWaitHandle wait = new EventWaitHandle(false, EventResetMode.ManualReset);
            object value;

            ~ValueWaiter()
            {
                wait.Close();
            }

            public object GetValue()
            {
                wait.WaitOne();
                return value;
            }

            public void SetValue(object value)
            {
                this.value = value;
                wait.Set();
            }
        }
        public void Read()
        {
            lock(reader.locked)
            {
                var msg = reader.ReadEnum<MessageType>();
                switch (msg)
                {
                    case MessageType.Action:
                        object args = formatter.Deserialize(reader.BaseStream);
                        MethodEvent?.Invoke(this, (MethodEventArgs)args);
                        break;
                    case MessageType.Result:
                        //int id = reader.Read();
                        //object ret = formatter.Deserialize(reader.BaseStream);
                        var ret = formatter.Deserialize(reader.BaseStream);
                        var idret = (IDResult)ret;
                        ValueWaiter waiter;
                        //= new ValueWaiter();
                        //list.GetOrAdd(id, waiter).SetValue(ret);

                        waiter = CreateOrGetWaiter(idret.ID);
                        waiter.SetValue(idret.Result);

                        break;
                    default:
                        break;
                }
            }
        }

        public class StreamMessage
        {

        }

        private ValueWaiter CreateOrGetWaiter(int id)
        {
            ValueWaiter waiter;
            lock (listLocker)
            {
                if (dictWaiters.ContainsKey(id))
                    waiter = dictWaiters[id];
                else
                    waiter = new ValueWaiter();
                dictWaiters.TryAdd(id, waiter);
            }

            return waiter;
        }

        public event EventHandler<MethodEventArgs> MethodEvent;

    }
    [Serializable]
    public class MethodEventArgs : EventArgs
    {
        public MethodEventArgs(int id, bool returnRequest, string methodName, object[] args)
        {
            ID = id;
            ReturnRequest = returnRequest;
            MethodName = methodName ?? throw new ArgumentNullException(nameof(methodName));
            Args = args;
        }

        public MethodEventArgs()
        {
           
        }

        public int ID { get; set; }
        public bool ReturnRequest { get; set; }
        public string MethodName { get; set; }
        public object[] Args { get; set; }
    }


    public enum MessageType : byte
    {
        Action,
        Result
    }

}
