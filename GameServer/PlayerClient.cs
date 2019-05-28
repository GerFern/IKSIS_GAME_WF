using GameServer.ClientObject;
using static GameServer.ClientObject.JsonObject;
using System;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace GameServer
{
    public class PlayerClient
    {
        private Socket _handler;
        private Thread _thread;
        public Guid PrivateID;
        public int PublicID;
        public string Name;
        public GameCore.Player PlayerCell{ get; private set; }
        public bool IsReady { get; set; }
        public int Index { get; set; }
        public PlayerClient(Socket socket)
        {
            
            _handler = socket;
            _thread = new Thread(Listner);
            _thread.IsBackground = true;
            _thread.Start();
        }
        //public string UserName
        //{
        //    get { return _userName; }
        //}
        private void Listner()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    int bytesRec = _handler.Receive(buffer);
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRec);
                    HandleCommand(data);
                }
                catch { Program.Server.EndClient(this); return; }
            }
        }
        public void End()
        {
            try
            {
                _handler.Close();
                try
                {
                    _thread.Abort();
                }
                catch { } // г
            }
            catch (Exception exp) { Console.WriteLine("Error with end: {0}.", exp.Message); }
        }

        /// <summary>
        /// Обработка команд
        /// </summary>
        /// <param name="data"></param>
        private void HandleCommand(string data)
        {
            HandleJsonObject(FromJsonSendTCP(data));
            //var vs = data.Split(new char[] { '#' }, 3);
            //string command = vs[1].ToUpper();
            //string body = vs[2];
          

            Console.WriteLine(data);
        }

        private void HandleJsonObject(JsonObject jsonObject, int? player = null)
        {
            Type type = jsonObject.GetType();
            if(jsonObject is PlayerCMD playerCMD)
            {
                HandleJsonObject(playerCMD.cmd, playerCMD.player);
            }
            else if(jsonObject is CellColor cellColor)
            {
                PlayerCell = new GameCore.PlayerCell(cellColor.color1, cellColor.color2, Color.LightGray);
                Program.Server.SendAllClients(new PlayerCMD { player = Index, cmd = cellColor });
            }
            else if(jsonObject is Ready ready)
            {
                IsReady = ready.isReady;
                Program.Server.SendAllClients(new PlayerCMD { player = Index, cmd = ready });
            }
            else if(jsonObject is Step step)
            {
                GameCore.Game game = Program.Server.Game;
                Program.Server.Game.SetPrefab(game.Prefabs[step.prefabID], step.Point, out _);
                Program.Server.SendAllClients(new PlayerCMD { player = Index, cmd = step });
            }
            else if(jsonObject is LogIn logIn)
            {
                PrivateID = logIn.guid;
                Name = logIn.name;
                int id = Program.GetRandID();
                SendObject(new ID { id = id });
                Program.Server.SendAllClients(new NewPlayer { id = id, playerName = Name });
            }
        }

        public void UpdateChat()
        {
            //Send(ChatController.GetChat());
        }

        public void SendObject(JsonObject jsonObject)
        {
            Send(jsonObject.ToJsonSendTCP());
        }

        public void Send(string command)
        {
            try
            {
                int bytesSent = _handler.Send(Encoding.UTF8.GetBytes(command));
                if (bytesSent > 0) Console.WriteLine("Success");
            }
            catch (Exception exp) { Console.WriteLine("Error with send command: {0}.", exp.Message); Program.Server.EndClient(this); }
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
