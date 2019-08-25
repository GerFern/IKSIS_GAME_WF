using Gtk;
using System;
using System.Collections.Generic;
using System.Text;
using UI = Gtk.Builder.ObjectAttribute;

namespace gtk_test.Widgets
{
    class RoomPlayerState : Widget
    {
        [UI] Label _playerName;
        [UI] ColorButton _color;
        [UI] CheckButton _ready;
        private PlayerState playerState;

        public RoomPlayerState() : this(new Builder("GameWindow.glade")) { }

        private RoomPlayerState(Builder builder) : base(builder.GetObject(typeof(RoomPlayerState).Name).Handle)
        {
            builder.Autoconnect(this);
            _color.ColorSet += new EventHandler((o, e) =>
            {
                var c = _color.Color;
                if(PlayerState!=null)
                {
                    PlayerState.Color = c.ToSystemColor();
                }
                ColorChanged?.Invoke(c);
            });
            _ready.Toggled += new EventHandler((o, e) =>
            {
                var a = _ready.Active;
                if(PlayerState!=null)
                {
                    PlayerState.Ready = a;
                }
                ToggleChanged?.Invoke(a);
            });
        }

        public event Action<bool> ToggleChanged;
        public event Action<Gdk.Color> ColorChanged;

        public PlayerState PlayerState
        {
            get => playerState;
            set
            {
                if (playerState != null)
                {
                    playerState.ColorChanged -= PlayerState_ColorChanged;
                    playerState.ReadyChanged -= PlayerState_ReadyChanged;
                }
                playerState = value;
                PlayerName = playerState.Name;
                Color = playerState.Color.ToGdkColor();
                Ready = playerState.Ready;
                playerState.ColorChanged += PlayerState_ColorChanged;
                playerState.ReadyChanged += PlayerState_ReadyChanged;
            }
        }

        private void PlayerState_ReadyChanged(object sender, EventArgs e)
        {
            Ready = PlayerState.Ready;
        }

        private void PlayerState_ColorChanged(object sender, EventArgs e)
        {
            Color = PlayerState.Color.ToGdkColor();
        }

        public string PlayerName { get => _playerName.Text; set => _playerName.Text = value; }
        public Gdk.Color Color
        {
            get => _color.Color;
            set
            {
                var c = _color.Color;
                if (c.Pixel != value.Pixel)
                    _color.Color = value;
            }
        }
        public bool Ready
        {
            get => _ready.Active;
            set
            {
                if(_ready.Active!=value)
                _ready.Active = value;
            }
        }
    }
}
