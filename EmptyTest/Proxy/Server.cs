using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace EmptyTest.Proxy
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TServer">Сервер, обработчик сообщений клиентов. Тип должен быть производным интерфейса</typeparam>
    /// <typeparam name="TClient">Интерфейс клиента</typeparam>
    public class Server<TServer, TClient, TClientData>
    {
        static Random random = new Random();
        //public PrivateDictionary<int, ClientInfo<TClient>> Clients { get; }
        public PrivateDictionary<int, ClientInfo<TClient, TClientData>> Clients { get; }
        internal Socket serverSocket;
        Thread thread;
        //protected TServer real;
        TServer serverManager;
        Type serverType;
        public TServer ServerManager { get => serverManager;
            private set
            {
                serverManager = value;
                serverType = value.GetType();
            }
        }
        //TServer serverProxy;
        //protected TNode.DynamicNode owner;

        Action<int, ClientInfo<TClient, TClientData>> addClients;
        Action<int> removeClients;
        Action clearClients;

        public Server(TServer server) 
        {
            ServerManager = server;
            
            Clients = new PrivateDictionary<int, ClientInfo<TClient, TClientData>>(out addClients, out removeClients, out clearClients);
        }

        public void Launch(EndPoint endPoint)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(endPoint);
            serverSocket.Listen(100);
            thread = new Thread(Start) { Name = "ServerThreadAccept" };
            thread.Start();
        }

        //[ThreadStatic] internal static TClient currentClient;
        //[ThreadStatic] internal static ClientInfo<TClient, TClientData> currentClientInfo;
        //public static TClient CurrentClient => currentClient;
        //public static ClientInfo<TClient, TClientData> CurrentClientInfo => currentClientInfo;

        //public object RecieveMethod(ClientController<TServer, TClient> client)
        //{

        //}
        //public static T LaunchServer<T>(T serverController, EndPoint endPoint)
        //{
        //    if (serverController is ServerController<T> sc)
        //    {
        //        sc.LaunchServer(endPoint);
        //        return serverController;
        //    }
        //    else
        //    {
        //        object obj = Create<T>(serverController);
        //        ((ServerController<T>)obj).LaunchServer(endPoint);
        //        return (T)obj;
        //    }
        //}

        void LaunchServer(EndPoint endPoint)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(endPoint);
            serverSocket.Listen(100);
            thread = new Thread(Start) { Name = "ServerThreadAccept" };
            thread.Start();
        }

        void Start()
        {
            while (true)
            {
                try
                {
                    Socket client = serverSocket.Accept();
                    int id = GetUID();
                    TClient newClient = CreateClientController(client, id);
                    var proxy = ((ClientController<TServer, TClient, TClientData>)(object)newClient);
                    proxy.Reciever.MethodEvent += Reciever_MethodEvent;
                    proxy.ExceptionOnRecieve += Proxy_ExceptionOnRecieve;
                    addClients.Invoke(id, proxy.ClientInfo);
                    //new Thread(() => NewClient?.Invoke(this, new NewClientEventArgs<TClient>(proxy.ClientInfo)))
                    new Thread(NewClientInvoker)
                    { Name = $"NewClient_{client.RemoteEndPoint}" }.Start(proxy.GetClientInfo());
                    //clients.Add(client);
                    //NewClient(user, ClientDataHandler);
                }
                catch (Exception exp) { Console.WriteLine("Error: {0}", exp.Message); }
            }
        }

        private void Proxy_ExceptionOnRecieve(object sender, Exception e)
        {
            ClientController<TServer, TClient, TClientData> clientController =
                (ClientController<TServer, TClient, TClientData>)sender;
            removeClients(clientController.ClientInfo.UID);
            ClientDisconnect?.Invoke(this,
                new ClientExceptionArgs<TClient, TClientData>(e, ((ClientController<TServer, TClient, TClientData>)sender).ClientInfo));
        }

        int GetUID()
        {
            int id;
            do { id = random.Next(); }
            while (id == 0 || Clients.ContainsKey(id));
            return id;
        }

        public virtual void NewClientInvoker(object argClientInfo)
        {
            if (argClientInfo is ClientInfo<TClient, TClientData> clientInfo)
            {
                NewClient?.Invoke(this, new NewClientEventArgs<TClient, TClientData>(clientInfo));
            }
        }

        protected virtual TClient CreateClientController(Socket socket, int uid)
        {
            return ClientController<TServer, TClient, TClientData>.Create(socket, uid);
        }

        private void Reciever_MethodEvent(object sender, MethodEventArgs e)
        {
            var method = serverType.GetMethod(e.MethodName, e.Args.Select(a => a.GetType()).ToArray());
            var result = method.Invoke(serverManager, e.Args);
            if(e.ReturnRequest==true)
            {
                WriterHelper.WriteResult(((ClientController<TServer, TClient, TClientData>)(object)ClientInfo<TClient, TClientData>.currentClientInterface).writer, e.ID, result);
            }
        }
        public event EventHandler<NewClientEventArgs<TClient, TClientData>> NewClient;
        public event EventHandler<ClientExceptionArgs<TClient, TClientData>> ClientDisconnect;

        //public event EventHandler<NewClientEventArgs<TClient>> NewClient;

        //protected override object Invoke(MethodInfo targetMethod, object[] args)
        //{
        //    return func.Invoke(targetMethod, args);
        //    //throw new NotImplementedException();
        //}
    }

    //public class Server<TServer, TClient, TClientData> 
    //{
    //    public static int u;


    //    public PrivateDictionary<int, ClientInfo<TClient, TClientData>> Clients { get; }


    //    public override void NewClientInvoker(object argClientInfo)
    //    {
    //        if (argClientInfo is ClientInfo<TClient, TClientData> clientInfo)
    //        {
    //            NewClient?.Invoke(this, new NewClientEventArgs<TClient, TClientData>(clientInfo));
    //        }
    //        base.NewClientInvoker(argClientInfo);
    //    }

    //    public new event EventHandler<NewClientEventArgs<TClient, TClientData>> NewClient;
    //}


    //public class NewClientEventArgs<TClient>
    //{
    //    public NewClientEventArgs(ClientInfo<TClient> client)
    //    {
    //        //Client = client.Client;
    //        ClientInfo = client ?? throw new ArgumentNullException(nameof(client));
    //        //EndPoint = endPoint ?? throw new ArgumentNullException(nameof(endPoint));
    //    }

    //    public ClientInfo<TClient> ClientInfo{get;}
    //    public TClient Client => ClientInfo.Client;
    //    public EndPoint EndPoint => ClientInfo.RemoteEndPoint;
    //}

    public class ClientExceptionArgs<TClient, TClientData>
    {
        public ClientExceptionArgs(Exception exception, ClientInfo<TClient, TClientData> clientInfo)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            ClientInfo = clientInfo ?? throw new ArgumentNullException(nameof(clientInfo));
        }

        public Exception Exception { get; }
        public ClientInfo<TClient, TClientData> ClientInfo { get; }
    }

    public class NewClientEventArgs<TClient, TClientData>
    {
        public NewClientEventArgs(ClientInfo<TClient, TClientData> client)
        {
            //Client = client.Client;
            ClientInfo = client ?? throw new ArgumentNullException(nameof(client));
        }
        //public ClientInfo<TClient> ClientInfo { get; }
        public TClient Client => ClientInfo.ClientInterface;
        public EndPoint EndPoint => ClientInfo.RemoteEndPoint;
        public ClientInfo<TClient, TClientData> ClientInfo { get; }
    }
}
