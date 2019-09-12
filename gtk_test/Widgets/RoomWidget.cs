using GameCore.Interfaces;
using Gtk;
using System;
using System.Collections.Generic;
using System.Text;
using UI = Gtk.Builder.ObjectAttribute;

namespace gtk_test.Widgets
{
    public class RoomWidget : Widget
    {
        [UI] Box playersInfo;
        [UI] Entry messageEntry;
        [UI] Button messageButtonSend;
        [UI] TextView messages;
        TextBuffer textBuffer;

        public ClientManager ClientManager { get; private set; }
        public Dictionary<int, RoomPlayerState> PlayerStateWidgets { get; } = new Dictionary<int, RoomPlayerState>();

        public RoomPlayerState AddPlayerState(PlayerState playerState, bool readOnly)
        {
            RoomPlayerState r;
            if (PlayerStateWidgets.ContainsKey(playerState.ID))
            {
                r = PlayerStateWidgets[playerState.ID];
                //r.PlayerName = playerState.Name;
                //r.Color = playerState.Color.ToGdkColor();
                //r.Ready = playerState.Ready;
            }
            else
            {
                r = new RoomPlayerState(ClientManager, playerState, readOnly);
                //r.PlayerState = playerState;
                playersInfo.Add(r);
                PlayerStateWidgets.Add(playerState.ID, r);
                
            }
            return r;
            //r.PlayerState = playerState;
        }

        private void RemovePlayerState(RoomPlayerState roomPlayerState)
        {
            roomPlayerState.Destroy();
        }

        private void PlayerState_ReadyChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void PlayerState_ColorChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        public void ResetPlayers()
        {
            foreach (var item in PlayerStateWidgets)
            {
                item.Value.Destroy();
            }
            PlayerStateWidgets.Clear();
        }

        public void SetClientManager(ClientManager clientManager)
        {
            if (ClientManager != null) ReleaseClientManager();
            ClientManager = clientManager ?? throw new ArgumentNullException(nameof(clientManager));
            AddPlayerState(clientManager.PlayerState, false);
            foreach (var item in clientManager.Players.Values)
            {
                AddPlayerState(item, true);
            }
            ClientManager.EventNewPlayer += ClientManager_EventNewPlayer;
            ClientManager.EventReadyChange += ClientManager_EventReadyChange;
            ClientManager.EventPlayerColorChange += ClientManager_EventPlayerColorChange;
            ClientManager.EventPlayerExit += ClientManager_EventPlayerExit;
            ClientManager.EventMessage += ClientManager_EventMessage;
            ClientManager.EventServerMessage += ClientManager_EventServerMessage;
            ClientManager.EventReadyTimer += ClientManager_EventReadyTimer;
        }

        private void ClientManager_EventServerMessage(string obj)
        {
            Application.Invoke(new EventHandler((o, e) =>
            {
                var t = textBuffer.GetIterAtLine(textBuffer.LineCount);
                textBuffer.Insert(ref t, "|" + obj + Environment.NewLine);
            }));
        }

        private void ClientManager_EventReadyTimer(int obj)
        {
            ClientManager_EventServerMessage(obj.ToString());
        }

        private void ClientManager_EventMessage(int arg1, string arg2)
        {
            Application.Invoke(new EventHandler((o, e) =>
            {
                var t = textBuffer.GetIterAtLine(textBuffer.LineCount);
                textBuffer.Insert(ref t, $"{ClientManager.Players[arg1].Name}: {arg2}{Environment.NewLine}");
            }));
        }

        public void ReleaseClientManager()
        {
            ClientManager.EventNewPlayer -= ClientManager_EventNewPlayer;
            ClientManager.EventReadyChange -= ClientManager_EventReadyChange;
            ClientManager.EventPlayerColorChange -= ClientManager_EventPlayerColorChange;
            ClientManager.EventPlayerExit -= ClientManager_EventPlayerExit;
            ClientManager = null;
        }

        private void ClientManager_EventPlayerExit(int obj)
        {
            Application.Invoke(new EventHandler((o, e)=>
            {
                RemovePlayerState(PlayerStateWidgets[obj]);
            }));
        }

     

        private void ClientManager_EventPlayerColorChange(System.Drawing.Color arg1, int arg2)
        { 
            //throw new NotImplementedException();
        }

        private void ClientManager_EventReadyChange(bool arg1, int arg2)
        {
            //throw new NotImplementedException();
        }

        private void ClientManager_EventNewPlayer(PlayerState obj)
        {
            Application.Invoke(new EventHandler((o,e)=>
            {
                AddPlayerState(obj, true);
            }));

        }

        public RoomWidget() : this(new Builder("GameWindow.glade"))
        {
            textBuffer = messages.Buffer;
            //ClientManager = clientManager ?? throw new ArgumentNullException(nameof(clientManager));
        }

        public RoomWidget(Builder builder) : base(builder.GetObject(typeof(RoomWidget).Name).Handle)
        {
            builder.Autoconnect(this);
        }
    }
}
