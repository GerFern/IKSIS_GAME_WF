using EmptyTest.BinaryWriterExtensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EmptyTest.Proxy
{
    public static class WriterHelper
    {
        //BinaryWriterE writerE;
        static BinaryFormatter formatter = new BinaryFormatter();

        //Thread thread;
        //ConcurrentDictionary<int, ValueWaiter> list = new ConcurrentDictionary<int, ValueWaiter>();
        //object locker = new object();
        //BinaryReaderE reader;
        //public object GetValue(int id)
        //{
        //    var waiter = new ValueWaiter();
        //    list.TryAdd(id, waiter);
        //    var result = waiter.GetValue();
        //    list.Remove(id, out _);
        //    return result;
        //}

       

        //public Writer(BinaryWriterE writer)
        //{
        //    writerE = writer;
        //    //thread = new Thread(Listener) { Name = "ClientResultWaiter" };
        //    //thread.Start();
        //}

        public static void WriteResult(this BinaryWriter writer, int id, object result)
        {
            lock(writer)
            {
                writer.WriteEnum(MessageType.Result);
                formatter.Serialize(writer.BaseStream, new IDResult(id, result));
                //writer.Write(id);
                //formatter.Serialize(writer.BaseStream, result);
            }
        }

        public static void WriteAction(this BinaryWriter writer, MethodEventArgs methodEventArgs)
        {
            lock (writer)
            {
                writer.WriteEnum(MessageType.Action);
                formatter.Serialize(writer.BaseStream, methodEventArgs);
            }
        }

        internal static void WriteResult(Action<int, object> writeResult, int iD, object result)
        {
            throw new NotImplementedException();
        }

        //public event EventHandler<MethodEventArgs> MethodEvent;

    }

    [Serializable]
    public class IDResult
    {
        public int ID { get; set; }
        public object Result { get; set; }

        public IDResult(){}

        public IDResult(int iD, object result)
        {
            ID = iD;
            Result = result;
        }
    }
}
