using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NotifyTest
{
    public class ClientList : List<PlayerClient>
    {

    }
    public class Server : StreamWRHandler
    {
        NetworkStream NetworkStream;
        private Thread _serverThread;
        public bool IsLoby { get; set; }

        public Socket Socket { get; }
        public ClientList Clients { get; } = new ClientList();

        public Server(IPEndPoint iP): base()
        {
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                              ProtocolType.Tcp);
            //Привязка сокета к локальной точке подключения
            Socket.Bind(iP);
            Socket.Listen(100);
            //NetworkStream = new NetworkStream(Socket);
            //Reader = new BinaryReaderE(NetworkStream);
            //Writer = new BinaryWriterE(NetworkStream);
            _serverThread = new Thread(Start) { Name = "ServerThread" };
            _serverThread.Start();
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
                    NewClient(user);
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


        public void NewClient(Socket handle)
        {
            try
            {
                PlayerClient newClient = new PlayerClient(handle);
                newClient.ObjectGetted += NewClient_ObjectGetted;
                //SendAllClients(new NewPlayer() { message = "присоединился к игре" });
                Clients.Add(newClient);
                Console.WriteLine("New client connected: {0}", handle.RemoteEndPoint);
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



        //public void SendAllClients(JsonObject jsonObject)
        //{
        //    foreach (var item in Clients)
        //    {
        //        item.SendObject(jsonObject);
        //    }
        //}

        public void SendAllClients(object value)
        {
            foreach (PlayerClient item in Clients)
            {
                item.Send(value);
            }
        }
    }

    //public class Clients:Dictionary<EndPoint, ClientPlayer>
    //{
    //    public void Add(Socket socket)
    //    {
    //        this.Add(socket.LocalEndPoint, new ClientPlayer(socket));
    //    }
    //    public ClientPlayer this[Socket socket]
    //    {
    //        get => this[socket.LocalEndPoint];
    //    }
    //}
    //public class ClientPlayer
    //{
    //    Socket Client { get; }
    //    Player playerCell;
    //    int playerID;

    //    public ClientPlayer(Socket socket)
    //    {
    //        Client = socket;
    //    }
    //}
    //class GameServer
    //{
    //    Dictionary<EndPoint,ClientPlayer> clientPlayers;
    //    TcpListener server;
    //    List<TcpClient> clients = new List<TcpClient>();
    //    List<Player> players;
    //    Game.PrefabCollection prefabs;
    //    public Game Game { get; private set; }
    //    public void AddPlayer(Player player)
    //    {
    //        players.Add(player);
    //    }
    //    public void AddPrefab(Prefab prefab)
    //    {
    //        prefabs.Add(prefab);
    //    }
    //    public void StartGame(int columns, int rows)
    //    {
    //        Game = new Game(players.ToArray(), prefabs);
    //        Game.Init(columns, rows);
    //    }
    //    public void StartServer()
    //    {
    //        server.Start();
    //        Console.WriteLine("Запущен");
    //        //while(true)
    //        //{
    //        //TcpClient tcpClient = server.AcceptTcpClient();
    //        server.BeginAcceptTcpClient(AcceptCallBack, null);
    //        //}
    //    }

    //    void AcceptCallBack(IAsyncResult ar)
    //    {
    //        Socket client = server.EndAcceptSocket(ar);
    //        Thread thread = new Thread(HandleClient);
    //        clientPlayers.Add(client.LocalEndPoint, new ClientPlayer(client));
    //        thread.Start(client);
    //        Console.WriteLine("Новый клиент {0}", client);
    //        server.BeginAcceptTcpClient(AcceptCallBack, null);
    //    }
    //    void HandleClient(object obj)
    //    {
    //        Socket client = (Socket)obj;
    //        MemoryStream ms = new MemoryStream(new byte[256], 0, 256, true, true);
    //        BinaryReader br = new BinaryReader(ms);
    //        BinaryWriter bw = new BinaryWriter(ms);

    //        while(true)
    //        {
    //            client.Receive(ms.GetBuffer());
    //        }
    //    }
    //    public GameServer(IPAddress iPAddress, int port)
    //    {
    //        server = new TcpListener(iPAddress, port);
    //    }
    //}
}
