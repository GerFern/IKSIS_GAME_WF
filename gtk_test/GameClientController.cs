using GameCore;
using GameServer.ClientObject;
using System;
using System.Net;
using System.Linq;

namespace gtk_test
{
    public static class GameClientController
    {
        private static Client client;
        public static int PublicID = -1;
        public static GameState GameState { get; private set; }
        public static PlayerStateCollection Players { get; } = new PlayerStateCollection();
        public static Game Game { get; set; }
        public static Client Client
        {
            get => client;
            set
            {
                if (client != null)
                {
                    client.ObjectSend -= Client_ObjectSend;
                    client.ObjectRecieve -= Client_ObjectRecieve;
                    client.CloseConnection -= Client_CloseConnection;
                }
                client = value;
                client.ObjectSend += Client_ObjectSend;
                client.ObjectRecieve += Client_ObjectRecieve;
                client.CloseConnection += Client_CloseConnection;
            }
        }

        public static void Init()
        {
            //Players.AddingNew += new System.ComponentModel.AddingNewEventHandler((o, e) => 
            //    AddPlayer?.Invoke((PlayerState)e.NewObject));
            //Players.ListChanged += new System.ComponentModel.ListChangedEventHandler((o,e)=>e)
        }

        public static bool EnterLobby(IPEndPoint endPoint)
        {
            if (client.Connect(endPoint))
            {
                var exp = client.SendObject(new LogIn() { guid = Guid.NewGuid(), name = $"Example_{new Random().Next(100, 999)}" });
                if (exp == null)
                {
                    GameState = GameState.LogInToLobby;
                }
                return true;
            }
            return false;
        }

        private static void Client_CloseConnection(object sender, EventArgsCloseConnection e)
        {

        }

        private static void Client_ObjectRecieve(object sender, EventArgsValue<JsonObject> e)
        {
            JsonObject jsonObject = e.Value;
            int player = default;
            if (jsonObject is GameServer.ClientObject.PlayerCMD pcmd)
            {
                player = pcmd.player;
                jsonObject = pcmd.cmd;
            }
            Type type = jsonObject.GetType();
            if (GameState == GameState.LogInToLobby)
            {
                if (jsonObject is ID id)
                {
                    PublicID = id.id;
                    if (id.gameStarted)
                        GameState = GameState.InGame;
                    else
                        GameState = GameState.InLobby;
                    client.SendObject(new GetPlayerList());
                }
            }
            else if (GameState == GameState.InLobby)
            {

                if (jsonObject is NewPlayer newPlayer)
                {
                    var playerState = new PlayerState(newPlayer.id, newPlayer.playerName);
                    Players.Add(playerState);
                    AddPlayer?.Invoke(playerState);

                    //NewPlayer?.Invoke(this, new EventArgsValue<PlayerState>(playerState));
                    //NewPlayer?.Invoke(playerState);
                    //NewPlayer += new Action<PlayerState>(p =>
                    //  {
                    //      Console.WriteLine($"addnew {p.Name}");
                    //  });
                }
                else if (jsonObject is Ready ready)
                {
                    if (player != 0)
                    {
                        var t = Players.Where(a => a.PublicID == player).FirstOrDefault();
                        if (t != null)
                            t.Ready = ready.isReady;
                    }
                }
                else if(jsonObject is GameStart gameStart)
                {
                    GameState = GameState.InGame;
                    var s = gameStart.size;
                    Game.Prefabs.Clear();
                    foreach (var (Count, Points) in gameStart.Prefabs)
                    {
                        Game.Prefabs.Add(Count, Points);
                    }
                    foreach (var item in Players)
                    {
                        item.Index = -1;
                    }
                    Game.Players.Clear();
                    foreach (var item in gameStart.Players)
                    {
                        var p = Players.Where(a => a.PublicID == item.Key).FirstOrDefault();
                        if(p!=null)
                        {
                            var v = item.Value;
                            p.Color = v.color;
                            p.Index = v.index;
                            Game.Players.Add(v.index, v.color);
                        }
                    }
                    Game.Start(s.Width, s.Height);
                }
            }




            if (jsonObject is PlayerList playerList)
            {
                Players.Clear();
                ResetPlayerList?.Invoke();
                foreach (var item in playerList.players)
                {
                    var v = item.Value;
                    var s = new PlayerState(item.Key, v.Name) { Index = v.Index, Color = v.Color, Ready = v.Ready };
                    Players.Add(s);
                    AddPlayer?.Invoke(s);
                }
            }
        }

        private static void Client_ObjectSend(object sender, EventArgsValue<JsonObject> e)
        {

        }

        //public static event EventHandler<EventArgsValue<PlayerState>> NewPlayer;
        public static event Action<PlayerState> AddPlayer;
        public static event Action ResetPlayerList;


    }

    //public delegate void EventHandlerStatic();
    //public delegate void EventHandlerStatic<TArgs>(TArgs args);
    public enum GameState
    {
        NotConnected,
        LogInToLobby,
        InLobby,
        LoadGame,
        InGame
    }
}
