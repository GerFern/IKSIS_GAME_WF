using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace EmptyTest.Proxy
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TServer">Интерфейс сервера</typeparam>
    /// <typeparam name="TClient">Клиент, обработчик сообщений сервера. Тип должен быть производным интерфейса</typeparam>
    public class Client<TServer, TClient>
    {
        /// <summary>
        /// Server Remote
        /// </summary>
        public TServer Server { get; }
        //public PrivateList<TClient> Clients { get; }
        internal Socket serverSocket;
        Thread thread;
        //protected TServer real;
        TClient clientManager;
        public TClient ClientManager
        {
            get => clientManager;
            private set
            {
                clientManager = value;
                ClientType = value.GetType();
            }
        }
        public Type ClientType { get; private set; }
        //TServer serverProxy;
        //protected TNode.DynamicNode owner;

        Action<TClient> addClients;
        Action<TClient> removeClients;
        Action clearClients;

        public Client(TClient client, EndPoint endPointServer)
        {
            ClientManager = client;
            serverSocket = new Socket(endPointServer.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Connect(endPointServer);
            Server = ServerController<TServer, TClient>.Create(serverSocket);
            ((ServerController<TServer, TClient>)(object)Server).Reciever.MethodEvent += Reciever_MethodEvent;
            //Clients = new PrivateList<TClient>(out addClients, out removeClients, out clearClients);
        }

        private void Reciever_MethodEvent(object sender, MethodEventArgs e)
        {
            var method = ClientType.GetMethod(e.MethodName, e.Args.Select(a => a.GetType()).ToArray());
            var result = method.Invoke(clientManager, e.Args);
            if (e.ReturnRequest == true)
            {
                
                WriterHelper.WriteResult(((ServerController<TServer, TClient>)(object)Server).writer, e.ID, result);
            }
        }

        //public void Launch(EndPoint endPoint)
        //{
        //    serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //    serverSocket.Bind(endPoint);
        //    serverSocket.Listen(100);
        //    thread = new Thread(Start) { Name = "ServerThreadAccept" };
        //    thread.Start();
        //}

        //[ThreadStatic]
        //internal static TClient currentClient;
        //public static TClient CurrentClient => currentClient;

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

        //void LaunchServer(EndPoint endPoint)
        //{
        //    serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //    serverSocket.Bind(endPoint);
        //    serverSocket.Listen(100);
        //    thread = new Thread(Start) { Name = "ServerThreadAccept" };
        //    thread.Start();
        //}

        //void Start()
        //{
        //    while (true)
        //    {
        //        try
        //        {
        //            Socket client = serverSocket.Accept();
        //            TClient newClient = ClientController<TServer, TClient>.Create<TClient, TClient>(client);
        //            addClients.Invoke(newClient);
        //            new Thread(() => NewClient?.Invoke(this, new NewClientEventArgs<TClient>(newClient, client.RemoteEndPoint)))
        //            { Name = $"NewClient_{client.RemoteEndPoint}" }.Start();
        //            //clients.Add(client);
        //            //NewClient(user, ClientDataHandler);
        //        }
        //        catch (Exception exp) { Console.WriteLine("Error: {0}", exp.Message); }
        //    }
        //}
    }
}
