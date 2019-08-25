using Gtk;
using System;
using System.Collections.Generic;
using System.Text;
using UI = Gtk.Builder.ObjectAttribute;

namespace gtk_test.Widgets
{
    class RoomWidget : Widget
    {
        [UI] Box playersInfo;

        public Dictionary<int, RoomPlayerState> PlayerStateWidgets { get; } = new Dictionary<int, RoomPlayerState>();

        public void AddPlayerState(PlayerState playerState)
        {
            RoomPlayerState r;
            if (PlayerStateWidgets.ContainsKey(playerState.PublicID))
            {
                r = PlayerStateWidgets[playerState.PublicID];
                //r.PlayerName = playerState.Name;
                //r.Color = playerState.Color.ToGdkColor();
                //r.Ready = playerState.Ready;
            }
            else
            {
                r = new RoomPlayerState();
                //r.PlayerState = playerState;
                playersInfo.Add(r);
                PlayerStateWidgets.Add(playerState.PublicID, r);
                
            }
            r.PlayerState = playerState;
        }

        private void PlayerState_ReadyChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void PlayerState_ColorChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void ResetPlayers()
        {
            foreach (var item in PlayerStateWidgets)
            {
                item.Value.Destroy();
            }
            PlayerStateWidgets.Clear();
        }
        public RoomWidget() : this(new Builder("GameWindow.glade")) { }

        public RoomWidget(Builder builder) : base(builder.GetObject(typeof(RoomWidget).Name).Handle)
        {
            builder.Autoconnect(this);
        }
    }
}
