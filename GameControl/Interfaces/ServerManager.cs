using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using EmptyTest.Proxy;
using GameCore.Structs;

namespace GameCore.Interfaces
{
    public class ServerManager : IServer
    {
        public static readonly ReadOnlyDictionary<int, PrefabLimit> standartPrefabLimit = new ReadOnlyDictionary<int, PrefabLimit>(
            new Dictionary<int, PrefabLimit>()
            {
                { 0, new PrefabLimit(1, new Point(0,0)) },
                { 1, new PrefabLimit(1, new Point(0,0), new Point(0,1)) },
                { 2, new PrefabLimit(1, new Point(0,0), new Point(1,1)) },
                { 3, new PrefabLimit(2, new Point(0,0), new Point(0,1), new Point(1,1)) },
                { 4, new PrefabLimit(2, new Point(0,0), new Point(0,1), new Point(0,2)) },
                { 5, new PrefabLimit(2, new Point(0,0), new Point(0,1), new Point(0,2), new Point(1,1)) },
                { 6, new PrefabLimit(1, new Point(0,0), new Point(0,1), new Point(0,2), new Point(0,3)) },
                { 7, new PrefabLimit(3, new Point(0,0), new Point(0,1), new Point(1,0), new Point(1,1)) },
                { 8, new PrefabLimit(1, new Point(1,0), new Point(0,1), new Point(1,2), new Point(2,1)) },
                { 9, new PrefabLimit(1, new Point(0,0), new Point(0,1), new Point(0,2), new Point(0,3), new Point(1,0)) },
                { 10, new PrefabLimit(1, new Point(0,0), new Point(0,1), new Point(0,2), new Point(1,0), new Point(1,1)) },
                { 11, new PrefabLimit(1, new Point(0,0), new Point(0,1), new Point(0,2), new Point(1,0), new Point(1,2)) },
                { 12, new PrefabLimit(1, new Point(1,0), new Point(1,1), new Point(1,2), new Point(0,1), new Point(2,1)) },
                { 13, new PrefabLimit(3, new Point(0,0), new Point(0,1), new Point(0,2), new Point(1,0), new Point(1,1), new Point(1,2)) },
                { 14, new PrefabLimit(1, new Point(1,0), new Point(0,1), new Point(1,2), new Point(2,0), new Point(1,1), new Point(2,2)) },
                { 15, new PrefabLimit(2, new Point(0,0), new Point(0,1), new Point(0,2), new Point(1,1), new Point(1,2)) },
                { 16, new PrefabLimit(5, new Point(0,0), new Point(0,1), new Point(0,2), new Point(1,2), new Point(2,2),
                                         new Point(2,1), new Point(2,0), new Point(1,0)) },
            });
        Thread startGameTimer;
        Server<ServerManager, IClient, ClientData> Server { get; set; }
        IReadOnlyDictionary<int, ClientInfo<IClient, ClientData>> ClientInfoCollection => Server.Clients;
        IClient[] Clients => ClientInfoCollection.Select(a => a.Value.ClientInterface).ToArray();
        Dictionary<int, PrefabLimit> prefabLimitDict = new Dictionary<int, PrefabLimit>();
        static readonly Random random = new Random();
        bool startingTimer = false;
        bool breakTimer = false;
        GameCore.Game gameController;
        public ServerManager(GameCore.Game game)
        {
            gameController = game ?? throw new ArgumentNullException(nameof(game));
            game.Started += new EventHandler((o, e) =>
            {
                var shuffled = players.Shuffle();
                for (int i = 0; i < shuffled.Length; i++)
                {
                    shuffled[i].Value.Index = i;
                }
                ForEachClient(a => a.GameStart(new Size(game.Columns, game.Rows), prefabLimitDict, players.ToDictionary(b => b.Key, c => (c.Value.Index, color: c.Value.Color))));
            });
            game.PrefabPlaced += new EventHandler<GameCore.Game.EventArgsPrefab>((o, e) =>
            {
                ForEachClient(a => a.Place(e.PlayerID, e.PrefabID, e.Location, e.Rotate));
            });
            game.PlayerChanged += new EventHandler((o, e) =>
            {
                ForEachClient(a => a.ChangePlayer(game.CurrentPlayer));
            });
        }
        public void SetServer(Server<ServerManager, IClient, ClientData> server)
        {
            if (Server != null)
                Server.ClientDisconnect -= Server_ClientDisconnect;
            Server = server ?? throw new ArgumentNullException(nameof(server));
            Server.ClientDisconnect += Server_ClientDisconnect;
        }
        private void Server_ClientDisconnect(object sender, ClientExceptionArgs<IClient, ClientData> e)
        {
            ForEachClient(a => a.PlayerExit(e.ClientInfo.UID));
        }
        void CheckAllReady()
        {
            if (players.Count >= 2 && players.All(a => a.Value.Ready))
            {
                startGameTimer = new Thread(() =>
                {
                    breakTimer = false;
                    Thread.Sleep(1000);
                    for (int i = 5; i > 0; i--)
                    {
                        ForEachClient(a => a.ReadyTimer(i));
                        Thread.Sleep(990);
                        if (breakTimer) break;
                    }
                    if (breakTimer) return;
                    startGameTimer = null;
                    startingTimer = false;
                    Thread.Sleep(150);
                    new Thread(StartGame) { Name = "StartGame" }.Start();
                })
                { Name = "StartGameTimer"};
                startGameTimer.Start();
            }
            else if (startGameTimer != null)
            {
                breakTimer = true;
                startGameTimer = null;
            }
        }
        int GetPlayerID()
        {
            int id;
            do
            {
                id = random.Next();
            } while (!players.ContainsKey(id));
            return id;
        }
        static IClient Client => ClientInfo<IClient, Structs.ClientData>.CurrentClientInterface;
        static ClientInfo<IClient, ClientData> ClientInfo => ClientInfo<IClient, Structs.ClientData>.CurrentClientInfo;
        static Structs.ClientData ClientData => ClientInfo.Data;
        static PlayerState Player => ClientInfo.Data.playerState;
        bool isGameRunning;
        private Size gameSizeStart;
        readonly Dictionary<int, PlayerState> players = new Dictionary<int, PlayerState>();
        void ForEachClient(Action<IClient> action)
        {
            foreach (var item in ClientInfoCollection.Values)
            {
                new Thread(() =>
                {
                    try
                    {
                        action.Invoke(item.ClientInterface);
                    }
                    catch (Exception) { }
                }){ Name = $"ActionPlayer_{item.Data.name}({item.UID}_" }.Start();
            }
        }
        void StartGame()
        {
            int index = 1;
            foreach (var item in players.Shuffle())
                item.Value.Index = index++;
            int size = 5 + (players.Count * random.Next(4, 6));
            gameSizeStart = new Size(size + random.Next(-5, 5), size + random.Next(-5, 5));
            gameController = new GameCore.Game();
            gameController.Prefabs.Set(standartPrefabLimit);
            gameController.Players.Set(players.ToDictionary(a => a.Value.Index, a => new Player(a.Value.Color)));
            gameController.Start(gameSizeStart.Width, gameSizeStart.Height);
            ForEachClient(a => a.ReadyTimer(0));
            Thread.Sleep(200);
            ForEachClient(a => a.GameStart(gameSizeStart, standartPrefabLimit, players.ToDictionary(b => b.Key, c => (c.Value.Index, c.Value.Color))));
            Console.WriteLine("GameStarted");
            isGameRunning = true;
        }
        #region InterfaceRealization
        public bool IsReady
        {
            get
            {
                if (isGameRunning) return true;
                else return Player.Ready;
            }
            set
            {
                if (isGameRunning) return;
                else
                {
                    var player = Player;
                    player.Ready = value;
                    ForEachClient(client => client.PlayerReadyChange(value, player.ID));
                    Console.WriteLine($"IsReady {player.Name} {value}");
                    CheckAllReady();
                }
            }
        }
        public Game Game { get; }
        public bool IsGameRunning { get => isGameRunning; }
        public Dictionary<int, PlayerState> Players { get => players; }
        public Color Color
        {
            get => Player.Color;
            set
            {
                Player.Color = value;
                int id = Player.ID;
                ForEachClient(a =>
                a.PlayerColorChange(value, id));
            }
        }
        public PlayerState LogIn(string Name)
        {
            var clientInfo = ClientInfo;
            var data = clientInfo.Data;
            if (!data.logIn)
            {
                data.playerState = new PlayerState
                {
                    ID = clientInfo.UID,
                    Name = Name,
                    Ready = false,
                    Color = Color.FromArgb(random.Next(255), random.Next(255), random.Next(255))
                };
                data.logIn = true;
                players.Add(clientInfo.UID, data.playerState);
                ForEachClient(a => a.NewPlayer(data.playerState));
                Console.WriteLine($"NewPlayer {Name}");
            }
            return data.playerState;
        }
        public bool Place(int prefabID, Point location, GameCore.Prefab.Rotate rotate)
        {
            var player = Player;
            bool valid = gameController.SetPrefab(prefabID, rotate, location, out _, player: player.Index, validate: true, decrement: true);
            if (valid)
            {
                ForEachClient(a =>
                {
                    a.Place(player.ID, prefabID, location, rotate);
                    a.ChangePlayer(gameController.CurrentPlayer);
                });
                return true;
            }
            else return false;
        }
        public void Message(string text)
        {
            ForEachClient(a => a.Message(Player.ID, text));
        }
        public void GiveUp()
        {
            if(isGameRunning)
            {
                var index = Player.Index;
                var currentPlayer = gameController.CurrentPlayer;
                gameController.GiveUpPlayer(index);
                if (currentPlayer == gameController.CurrentPlayer)
                    ForEachClient(a => a.GiveUpPlayer(index));
                else
                    ForEachClient(a =>
                    {
                        a.GiveUpPlayer(index);
                        a.ChangePlayer(gameController.CurrentPlayer);
                    });
            }
        }
        #endregion
    }
}
