using EmptyTest.TCommand;
using EmptyTest.TExtensions;
using EmptyTest.TNode;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace EmptyTest.TStreamHandler
{
    public class ClientList : List<PlayerClient>
    {

    }
    public class Server : StreamWRHandlerMany
    {
        //TopNode PublicDataContainer;


        internal NETDynamicMainContainer ServerContainer;
        NetworkStream NetworkStream;
        public bool IsLoby { get; set; }

        public DataHandler ServerDataHandler => DataHandler;
        public DataHandler ClientDataHandler { get; }

        private Thread _serverThread;
        public Socket Socket { get; }
        public ClientList Clients { get; } = new ClientList();

        public Server(IPEndPoint iP, DataHandler serverDataHandler, DataHandler clientDataHandler): base(serverDataHandler)
        {
            //PublicDataContainer = new TopNode();
            //PublicDataContainer.Value = new Test();
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                              ProtocolType.Tcp);
            //Привязка сокета к локальной точке подключения
            Socket.Bind(iP);
            Socket.Listen(100);
            //NetworkStream = new NetworkStream(Socket);
            //Reader = new BinaryReaderE(NetworkStream);
            //Writer = new BinaryWriterE(NetworkStream);
            this.AddType(typeof(TStructures.ServerContainer));
            ServerContainer = new NETDynamicMainContainer(this, serverDataHandler, false);
            //ServerContainer = (NETDynamicStruct)this.CreateNode(typeof(TStructures.ServerContainer), false);
            ((DynamicNode)ServerContainer).SetValue(Activator.CreateInstance(typeof(TStructures.ServerContainer)).SetAllNullStringEmpty());
            _serverThread = new Thread(Start) { Name = "ServerThread" };
            _serverThread.Start();
            ClientDataHandler = clientDataHandler ?? throw new ArgumentNullException(nameof(clientDataHandler));
            //object socket = null;
            //server.BeginAccept(new AsyncCallback(AcceptCallback), socket);
        }

        internal void Start()
        {
            while (true)
            {
                try
                {
                    Socket user = Socket.Accept();
                    NewClient(user, ClientDataHandler);
                }
                catch (Exception exp) { Console.WriteLine("Error: {0}", exp.Message); }
            }
        }

        //private void AcceptCallback(IAsyncResult ar)
        //{
        //    try
        //    {
        //        Socket handler = server.EndAccept(ar);
        //        Socket client = handler;
        //        int bufferSize = handler.ReceiveBufferSize;
        //        buffer = new byte[512];
        //        Console.WriteLine("Принимаем данные от клиента...");
        //        client.BeginReceive(buffer, 0, bufferSize, 0,
        //                            new AsyncCallback(ReceiveCallback), client);
        //        //Начало операции принятия попытки следующего входящего подключения
        //        server.BeginAccept(new AsyncCallback(AcceptCallback), server);
        //        //    server.Close();
        //    }
        //    catch (SocketException e)
        //    {
        //        //   MessageBox.Show(e.Message);
        //        Console.WriteLine("Ошибка связи с криеном: " + e.Message);
        //    }
        //}


        public void NewClient(Socket socket, DataHandler dataHandler)
        {
            try
            {
                PlayerClient newClient = new PlayerClient(this, socket, dataHandler);
                newClient.ObjectGetted += NewClient_ObjectGetted;
                //SendAllClients(new NewPlayer() { message = "присоединился к игре" });
                Clients.Add(newClient);
                //newClient.SendTypes();
                //((IDynamic)newClient.ClientContainer).UpdateValue(Activator.CreateInstance(typeof(Structures.Substruct1)).SetAllNullStringEmpty());

                Console.WriteLine("New client connected: {0}", socket.RemoteEndPoint);
            }
            catch (Exception exp) { Console.WriteLine("Error with addNewClient: {0}.", exp.Message); }
        }

        private void NewClient_ObjectGetted(object sender, GetObj e)
        {
        }

        public void EndClient(PlayerClient client)
        {
            try
            {
                client.End();
                Clients.Remove(client);
                Console.WriteLine("User {0} has been disconnected.", client);
            }
            catch (Exception exp) { Console.WriteLine("Error with endClient: {0}.", exp.Message); }
        }

        private static readonly ReadOnlyDictionary<Type, Action<Server, Command>> actions =
           new ReadOnlyDictionary<Type, Action<Server, Command>>(new Dictionary<Type, Action<Server, Command>>()
           {
                {typeof(CommandLogIn), (server, cmd)=>{ } },
                {typeof(CommandWelcomeClient), (server, cmd)=>{ } },
                {typeof(CommandGetTypeListRequest), (server, cmd)=>{ } },
                {typeof(CommandSendTypeList), (server, cmd)=>{ } },
                {typeof(CommandGetObjectRequest), (server, cmd)=>{ } },
                {typeof(CommandRecieveObject), (server, cmd)=>{ } },
                {typeof(CommandAction), (server, cmd)=>{ } },
                {typeof(CommandActionResult), (server, cmd)=>{ } },
                {typeof(CommandDisconnect), (server, cmd)=>{ } },
           });
        public override void ActionCommand(Command command)
        {
            actions[command.GetType()].Invoke(this, command);
        }



        //public void SendAllClients(JsonObject jsonObject)
        //{
        //    foreach (var item in Clients)
        //    {
        //        item.SendObject(jsonObject);
        //    }
        //}

        //public void SendAllClients(object value)
        //{
        //    foreach (PlayerClient item in Clients)
        //    {
        //        item.Send(value);
        //    }
        //}
    }
}
