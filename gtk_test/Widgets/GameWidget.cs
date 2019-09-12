using System;
using System.Collections.Generic;
using GameCore;
using GameCore.Interfaces;
using Gtk;
using Game = GameCore.Game;
using UI = Gtk.Builder.ObjectAttribute;

namespace gtk_test.Widgets
{
    class GameWidget : Widget
    {
        [UI] private DrawingArea _gameWidget = null;
        [UI] private Box _vBox = null;
        [UI] private Scale _scale = null;

        [UI] private Box _players = null;
        [UI] private Button _btnGiveUp = null;

        public GameWidgetOver GameWidgetOver { get; private set; }
        public ClientManager ClientManager { get; private set; }

        public void SetClientManager(GameCore.Interfaces.ClientManager clientManager)
        {
            ClientManager = clientManager;
            GameWidgetOver.SetClientManager(clientManager);
            foreach (var item in clientManager.Players)
            {
                var gps = new GamePlayerState(item.Value);
                playerStates.Add(item.Key, gps);
                _players.Add(gps);
                gps.Visible = true;
            }
        }

        private void _btnGiveUp_ButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            var dialog = new Dialog("Закончить игру?", Program.mv, DialogFlags.Modal);
            dialog.AddButton("Да", ResponseType.Yes).ButtonReleaseEvent += new ButtonReleaseEventHandler((obj, e) =>
             {
                 if (ClientManager == null)
                 {
                     Game.GiveUpPlayer(Game.CurrentPlayer);
                 }
                 else
                 {
                     ClientManager.Server.GiveUp();
                 }
                 dialog.Destroy();
             });
            dialog.AddButton("Нет", ResponseType.No).ButtonReleaseEvent += new ButtonReleaseEventHandler((obj, e) =>
            {
                dialog.Destroy();
            });
            //dialog.ShowNow();
            var t = dialog.Run();
            System.Diagnostics.Debug.WriteLine(t);
        }

        Dictionary<int, GamePlayerState> playerStates = new Dictionary<int, GamePlayerState>();
        private int _counter;

        private GameCore.Game game;
        public Game Game
        {
            get => game;
            set
            {
                game = value;
                GameWidgetOver.Game = value;
                PrefabSelectorWidget[] widgets = new PrefabSelectorWidget[value.Prefabs.Count];
                var en = value.Prefabs.GetEnumerator();
                Cairo.Color back = new Cairo.Color(0.5, 0.5, 0.7, 1);
                Cairo.Color fore = new Cairo.Color(0.5, 0.5, 0.5, 0.6);
                foreach (var item in _vBox.Children)
                {
                    item.Destroy();
                }
                for (int i = 0; i < widgets.Length; i++)
                {
                    en.MoveNext();
                    var w = widgets[i] = new PrefabSelectorWidget();
                    w.Prefab = en.Current.Value.Prefab;
                    w.PrefabID = en.Current.Key;
                    w.BackgroundColor = back;
                    w.ForeColor = fore;
                    w.GameWidget = GameWidgetOver;
                }
                foreach (var item in widgets)
                {
                    var s = item.Prefab.Size;
                    item.SetSizeRequest(s.Width * 50 + 30, s.Height * 50 + 30);
                    _vBox.Add(item);
                }
                _vBox.ShowAll();
            }
        }

        public GameWidget() : this(new Builder("GameWindow.glade"))
        {
            _btnGiveUp.ButtonPressEvent += _btnGiveUp_ButtonPressEvent;

            DeleteEvent += Window_DeleteEvent;

            KeyPressEvent += GameWindow_KeyPressEvent;
            KeyReleaseEvent += GameWindow_KeyReleaseEvent;
            //_button1.Clicked += Button1_Clicked;
            GameWidgetOver = new GameWidgetOver(_gameWidget);
            GameWidgetOver.ScaleChanged += GameWidget_ScaleChanged;
            _scale.ChangeValue += _scale_ChangeValue;
            //CssProvider cssProvider = new CssProvider();
            //cssProvider.LoadFromResource("Css/test.css");
            //this.StyleContext.AddProvider(cssProvider, 800);
        }

        public GameWidget(Builder builder) : base(builder.GetObject(typeof(GameWidget).Name).Handle)
        {
            builder.Autoconnect(this);
            builder.Dispose();
        }

        private void _scale_ChangeValue(object o, ChangeValueArgs args)
        {
            GameWidgetOver.Scale = _scale.Value;
        }

        private void GameWidget_ScaleChanged(object sender, EventArgs e)
        {
            _scale.Value = GameWidgetOver.Scale;
        }

        private void GameWindow_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            var evnt = args.Event;
            if (evnt.Key == Gdk.Key.Control_L)
            {
                GameWidgetOver.Control = false;
            }
        }

        private void GameWindow_KeyPressEvent(object o, KeyPressEventArgs args)
        {
            var evnt = args.Event;
            if (evnt.Key == Gdk.Key.Control_L)
            {
                GameWidgetOver.Control = true;
            }
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        private void Button1_Clicked(object sender, EventArgs a)
        {
            _counter++;
            //_label1.Text = "Hello World! This button has been clicked " + _counter + " time(s).";
        }
    }
}