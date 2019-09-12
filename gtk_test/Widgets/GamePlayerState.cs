using GameCore;
using GameCore.Interfaces;
using Gtk;
using System;
using System.Collections.Generic;
using System.Text;
using UI = Gtk.Builder.ObjectAttribute;

namespace gtk_test.Widgets
{
    public class GamePlayerState : Widget
    {
        [UI] Label _ginfo;
        [UI] ColorButton _gcolor;
        //[UI] Label _gcount;
        //[UI] Label _gstate;

        public PlayerState PlayerState { get; }

        public GamePlayerState(PlayerState playerState) : this(new Builder("GameWindow.glade"))
        {
            PlayerState = playerState;
            playerState.PropertyChanged += PlayerState_PropertyChanged;
            _gcolor.Color = playerState.Color.ToGdkColor();
            UpdatePlayerState(playerState);
            //_gplayerName.Text = playerState.Name;
            //_gcolor.Color = playerState.Color.ToGdkColor();
            //_gcount.Text = playerState.Count.ToString();
            //_gstate.Text = playerState.State.DescriptionAttr();
        }

        private void UpdatePlayerState(PlayerState playerState)
        {
            _ginfo.Text = $"{playerState.Name} - {playerState.State.DescriptionAttr()} ({playerState.Count})";
        }

        private void PlayerState_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            PlayerState playerState = (PlayerState)sender;
            Application.Invoke(new EventHandler((o, ee) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(playerState.State):
                    case nameof(playerState.Count):
                        UpdatePlayerState(playerState);
                        break;
                    default:
                        break;
                }
            }));
        }

        private GamePlayerState(Builder builder) : base(builder.GetObject(typeof(GamePlayerState).Name).Handle)
        {
            builder.Autoconnect(this);
        }

    }
}
