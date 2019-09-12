using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace EmptyTest.Proxy
{
    /// <summary>
    /// Класс для обмена сообщений с клиентом
    /// </summary>
    /// <typeparam name="TServer"></typeparam>
    /// <typeparam name="TClient"></typeparam>
    public class ClientController<TServer, TClient, TClientData> : DispatchProxy
    {
        public ClientInfo<TClient, TClientData> ClientInfo { get; private set; }
        protected int clientID;
        protected NetworkStream networkStream;
        protected BinaryReaderE reader;
        internal BinaryWriterE writer;
        protected Thread threadRecieve;
        protected Socket socketClient;
        //private readonly Server<TServer, TClient> server;
        //Reader reciever;
        protected virtual Func<MethodInfo, object[], Task<object>> func { get; }
        internal Reader Reciever { get => reciever; }

        //protected TClient real;


        //protected TNode.DynamicNode owner;



        public static TClient Create(Socket socket, int uid)
        {

            object t = Create<TClient, ClientController<TServer, TClient, TClientData>>();
            var proxy = ((ClientController<TServer, TClient, TClientData>)t);
            proxy.socketClient = socket;
            var ns = proxy.networkStream = new NetworkStream(socket);
            proxy.clientID = uid;
            proxy.ClientInfo = new ClientInfo<TClient, TClientData>((TClient)t, socket.RemoteEndPoint, uid);
            proxy.reader = new BinaryReaderE(ns);
            proxy.writer = new BinaryWriterE(ns);
            proxy.reciever = new Reader(proxy.reader);
            proxy.threadRecieve = new Thread(proxy.InitThread) { Name = $"RecieverFromClient_{socket.RemoteEndPoint}" };
            proxy.threadRecieve.Start();
            //proxy.owner = owner;
            return (TClient)t;
        }

        protected virtual void InitThread()
        {
            var client = ClientInfo<TClient, TClientData>.currentClientInterface = (TClient)(object)this;
            ClientInfo<TClient, TClientData>.currentClientInfo = new Proxy.ClientInfo<TClient, TClientData>(client, this.socketClient.RemoteEndPoint, this.clientID);
            try
            {
                Reciever.Listener();
            }
            catch(Exception ex)
            {
                ExceptionOnRecieve?.Invoke(this, ex);
            }
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            int id = this.id++;
            //this.id++;

            bool waitresponse;
            var property = GetPropertyInfo(targetMethod, out _);
            if (property != null) waitresponse = property.GetCustomAttribute<WaitResponseAttribute>() != null;
            else waitresponse = targetMethod.GetCustomAttribute<WaitResponseAttribute>() != null;
            MethodEventArgs method = new MethodEventArgs(
                id,
                waitresponse,
                targetMethod.Name,
                args);
            WriterHelper.WriteAction(writer, method);
            if (waitresponse)
            {
                return reciever.GetValue(id);
            }
            else return null;
        }

        PropertyInfo GetPropertyInfo(MethodInfo methodInfo, out bool isGet)
        {
            isGet = false;
            try
            {
                if (methodInfo.IsSpecialName)
                {
                    if (methodInfo.Name.StartsWith("get_"))
                    {
                        isGet = true;
                        return typeof(TServer).GetProperty(methodInfo.Name.Substring(4));
                    }
                    else if (methodInfo.Name.StartsWith("set_"))
                    {
                        return typeof(TServer).GetProperty(methodInfo.Name.Substring(4));
                    }
                }
                else return null;
            }
            catch { }
            return null;
        }

        internal virtual object GetClientInfo()
        {
            return ClientInfo;
        }

        protected Reader reciever;

        int id = 0;

        public event EventHandler<Exception> ExceptionOnRecieve;

        //public ClientController(Server<TServer, TClient> server, Socket client)
        //{
        //    this.server = server;
        //    this.socketClient = client;
        //}
    }

    //public class ClientController<TServer, TClient, TClientData> : ClientController<TServer, TClient>
    //{
    //    public new ClientInfo<TClient, TClientData> ClientInfo { get; private set; }

    //    internal override object GetClientInfo()
    //    {
    //        return ClientInfo;
    //    }

    //    public static TClient Create(Socket socket, int uid)
    //    {

    //        object t = Create<TClient, ClientController<TServer, TClient, TClientData>>();
    //        var proxy = ((ClientController<TServer, TClient, TClientData>)t);
    //        proxy.socketClient = socket;
    //        var ns = proxy.networkStream = new NetworkStream(socket);
    //        proxy.clientID = uid;
    //        proxy.reader = new BinaryReaderE(ns);
    //        proxy.writer = new BinaryWriterE(ns);
    //        proxy.reciever = new Reader(proxy.reader);
    //        proxy.threadRecieve = new Thread(proxy.InitThread) { Name = $"RecieverFromClient_{socket.RemoteEndPoint}" };
    //        proxy.threadRecieve.Start();
    //        //proxy.owner = owner;
    //        return (TClient)t;
    //    }

    //    protected override void InitThread()
    //    {
    //        Server<TServer, TClient, TClientData>.currentClientInfoTyped = ClientInfo;
    //        ClientInfo = new ClientInfo<TClient, TClientData>((TClient)(object)this, socketClient.RemoteEndPoint, clientID);
    //        base.InitThread();
    //    }
    //}

    //public class QueueAction
    //{
    //    Thread thread;
    //    ConcurrentDictionary<int, ValueWaiter> list = new ConcurrentDictionary<int, ValueWaiter>();
    //    object locker = new object();
    //    BinaryReaderE reader;
    //    public object GetValue(int id)
    //    {
    //        var waiter = new ValueWaiter();
    //        list.TryAdd(id, waiter);
    //        var result = waiter.GetValue();
    //        list.Remove(id, out _);
    //        return result;
    //    }
    //    void Read()
    //    {
    //        lock (locker)
    //        {
    //            int id = reader.Read7();
    //            list[id].SetValue(2);
    //        }
    //    }

    //    void Listener()
    //    {
    //        while(true)
    //        {
    //            Read();
    //        }
    //    }

    //    public QueueAction(BinaryReaderE reader)
    //    {
    //        thread = new Thread(Listener) { Name = "ClientResultWaiter" };
    //        thread.Start();
    //    }

    //    ~QueueAction()
    //    {
    //        thread.Abort();
    //    }

    //    public class ValueWaiter
    //    {
    //        readonly EventWaitHandle wait = new EventWaitHandle(false, EventResetMode.ManualReset);
    //        object value;


    //        public object GetValue()
    //        {
    //            wait.WaitOne();
    //            return value;
    //        }

    //        ~ValueWaiter()
    //        {
    //            wait.Close();
    //        }

    //        public void SetValue(object value)
    //        {
    //            this.value = value;
    //            wait.Set();
    //        }
    //    }
    //}


    //public class ClientControllerCallback
}
