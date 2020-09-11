using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GameCore.Interfaces
{
    public class ClientManager : IClient
    {
        public IServer Server { get; private set; }
        public PlayerState PlayerState { get; private set; }
        object locker = new object();
        EmptyTest.PrivateDictionary<int, PlayerState> players { get; set; }
        public Dictionary<int, PlayerState> Players
        {
            get
            {
                lock(locker)
                {
                    return new Dictionary<int, PlayerState>(players);
                }
            }
        }
        public GameCore.Game Game { get; }
        public ClientManager(GameCore.Game game)
        {
            Game = game ?? throw new ArgumentNullException(nameof(game));
        }
        Action<int, PlayerState> add;
        Action<int> remove;
        Action clear;
        public string UserName { get; private set; }
        public void SetServer(IServer server, string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("message", nameof(userName));
            }
            UserName = userName;
            Server = server;
            players = new EmptyTest.PrivateDictionary<int, PlayerState>(server.Players,out add, out remove, out clear);
            PlayerState = server.LogIn(userName);
        }

        #region InterfaceRealization
        public void ChangePlayer(int playerIndex)
        {
            Game.CurrentPlayer = playerIndex;
            EventChangePlayer?.Invoke(playerIndex);
        }
        public void NewPlayer(PlayerState playerState)
        {
            lock (locker)
            {
                add(playerState.ID, playerState);
            }
            EventNewPlayer?.Invoke(playerState);
        }
        public void Place(int playerID, int prefabID, Point location, GameCore.Prefab.Rotate rotate)
        {
            PlayerState playerState = Players[playerID];
            int playerIndex = playerState.Index;
            Game.SetPrefab(prefabID, rotate, location, out _, playerIndex, false, true);
            playerState.Count = Game.GetPlayerCount(playerIndex);
        }
        public void PlayerExit(int playerID)
        {
            lock(locker)
            {
                remove(playerID);
            }
            EventPlayerExit?.Invoke(playerID);
        }
        public void PlayerReadyChange(bool isReady, int playerID)
        {
            players[playerID].Ready = isReady;
            EventPlayerReadyChange?.Invoke(isReady, playerID);
        }
        public void Message(int playerID, string text)
        {
            EventMessage?.Invoke(playerID, text);
        }
        public void ReadyTimer(int time)
        {
            EventReadyTimer?.Invoke(time);
        }
        public void ServerMessage(string text)
        {
            EventServerMessage?.Invoke(text);
        }
        public void PlayerColorChange(Color color, int playerID)
        {
            players[playerID].Color = color;
            EventPlayerColorChange?.Invoke(color, playerID);
        }
        public void GameStart(Size gameSize, IDictionary<int, PrefabLimit> prefabs, IDictionary<int, (int index, Color color)> players)
        {
            Game.Prefabs.Set(prefabs);
            foreach (var item in this.Players)
            {
                var pl = players[item.Key];
                item.Value.Index = pl.index;
                item.Value.Color = pl.color;
            }
            var ps = players.ToDictionary(a => a.Value.index, a => new Player(a.Value.color));
            Game.Players.Set(ps);
            Game.StartMultiplayer(gameSize, players[this.PlayerState.ID].index);
            EventGameStarted?.Invoke();
        }
        public void GiveUpPlayer(int playerIndex)
        {
            Players.Where(a => a.Value.Index == playerIndex).First().Value.State = State.GiveUp;
            Game.GiveUpPlayer(playerIndex);
        }
        #endregion
        #region Events
        public event Action<int> EventChangePlayer;
        public event Action<bool, int> EventPlayerReadyChange;
        public event Action<Color, int> EventPlayerColorChange;
        public event Action EventGameStarted;
        public event Action<PlayerState> EventNewPlayer;
        public event Action<int, int, Point, GameCore.Prefab.Rotate> EventPlace;
        public event Action<int> EventPlayerExit;
        public event Action<bool, int> EventReadyChange;
        public event Action<int, string> EventMessage;
        public event Action<string> EventServerMessage;
        public event Action<int> EventReadyTimer;
        #endregion
    }
}
