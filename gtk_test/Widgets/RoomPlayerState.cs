using GameCore.Interfaces;
using Gtk;
using System;
using UI = Gtk.Builder.ObjectAttribute;

namespace gtk_test.Widgets
{
    public class RoomPlayerState : Widget
    {
        [UI] Label _playerName;
        [UI] ColorButton _color;
        [UI] CheckButton _ready;
        PlayerState PlayerState { get; }

        ClientManager ClientManager { get; }
        public bool ReadOnlyState { get; }
        bool toogleState = false;

        public RoomPlayerState(ClientManager clientManager, PlayerState playerState, bool readOnlyState) : this(new Builder("GameWindow.glade"))
        {
            ClientManager = clientManager ?? throw new ArgumentNullException(nameof(clientManager));
            PlayerState = playerState ?? throw new ArgumentNullException(nameof(playerState));
            playerState.PropertyChanged += PlayerState_PropertyChanged;
            this.Destroyed += new EventHandler((o, e) => { playerState.PropertyChanged -= PlayerState_PropertyChanged; });
            ReadOnlyState = readOnlyState;
            _color.Color = playerState.Color.ToGdkColor();
            _ready.Active = playerState.Ready;
            _playerName.Text = playerState.Name;
            if (!readOnlyState)
            {
                _color.Sensitive = true;
                _ready.Sensitive = true;
                _color.ColorSet += new EventHandler((o, e) =>
                {
                    if (!toogleState)
                    {
                        var c = _color.Color;
                        if (PlayerState != null)
                        {
                            var sysColor = c.ToSystemColor();
                            PlayerState.Color = sysColor;
                            clientManager.Server.Color = sysColor;
                        }
                        ColorChanged?.Invoke(c);
                    }
                });
                _ready.Toggled += new EventHandler((o, e) =>
                {
                    if (!toogleState)
                    {
                        var a = _ready.Active;
                        if (PlayerState != null)
                        {
                            PlayerState.Ready = a;
                            clientManager.Server.IsReady = a;
                        }
                        ToggleChanged?.Invoke(a);
                    }
                });
            }
            else
            {
                _ready.Toggled += new EventHandler((o, e) =>
                 {
                     if (_ready.Active != PlayerState.Ready)
                         _ready.Active = PlayerState.Ready;
                 });
                _color.ColorSet += new EventHandler((o, e) =>
                 {
                     if(!toogleState)
                     {
                         var c1 = _color.Color;
                         var c2 = PlayerState.Color.ToGdkColor();

                         if (c1.Blue != c2.Blue || c1.Red != c2.Red || c1.Green != c2.Green)
                             _color.Color = c2;
                     }
                 });
            }
        }

        private void PlayerState_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            PlayerState playerState = (PlayerState)sender;
            toogleState = true;
            Application.Invoke(new EventHandler((o, eh) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(playerState.Color):
                        _color.Color = playerState.Color.ToGdkColor();
                        //playerState.Color
                        break;
                    case nameof(playerState.Ready):
                        _ready.Active = playerState.Ready;
                        break;
                    default:
                        break;
                }
            }));
            toogleState = false;
        }

        private RoomPlayerState(Builder builder) : base(builder.GetObject(typeof(RoomPlayerState).Name).Handle)
        {
            builder.Autoconnect(this);
        }

        public event Action<bool> ToggleChanged;
        public event Action<Gdk.Color> ColorChanged;

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
