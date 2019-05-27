using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using GameCore;

namespace GameServer
{

    class Program
    {
        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;
        public const int MF_ENABLED = 0x0000;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        public static extern int EnableMenuItem(IntPtr hMenu, int uIDEnableItem, int uEnable);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();





        public static Server Server { get; set; }
        static void Main(string[] args)
        {
            IntPtr intPtr = GetSystemMenu(GetConsoleWindow(), false);
            DeleteMenu(intPtr, SC_CLOSE, MF_BYCOMMAND);
            Console.CancelKeyPress += Console_CancelKeyPress;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            IPAddress iPAddress = null;
            int port = -1;
            try
            {
                iPAddress = IPAddress.Parse(args[0]);
                port = Int32.Parse(args[1]);
            }
            catch
            {
                try
                {
                    port = Int32.Parse(args[0]);
                }
                catch { }
            }
            if (port < 0) port = 7979;
            if (iPAddress == null)
            {
                string host = Dns.GetHostName();
                var entry = Dns.GetHostEntry(host);
                for (int i = 0; i < entry.AddressList.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {entry.AddressList[i]}");
                }
                int index;
                string str;
                do
                {
                    str = Console.ReadLine();
                }
                while (!(int.TryParse(str, out index) && index > 0 && index <= entry.AddressList.Length));

                iPAddress = entry.AddressList[index-1];
            }
            Console.WriteLine("{0}:{1}", iPAddress, port);
            IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, port);
            Server = new Server(iPEndPoint);
            //Server.Start();
            Thread thread = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        Console.ReadLine();
                    }
                }
                catch
                {

                }
                finally
                {
                    Console.WriteLine("Shutdown");
                    Thread.Sleep(1000);
                }
            })
            { Name = "ConsoleRead" };
            thread.Start();
            thread.Join();
            //GameServer gameServer = new GameServer(iPAddress, port);
            //gameServer.StartServer();
            //Console.ReadLine();
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            //throw new NotImplementedException();
        }
    }

    public class ClientList:List<PlayerClient>
    {

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
