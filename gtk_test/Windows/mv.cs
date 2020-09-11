using Gtk;
using gtk_test.Widgets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UI = Gtk.Builder.ObjectAttribute;

namespace gtk_test.Windows
{
    
    class mw:Window
    {
        [UI] ToggleButton _btn3D;
        [UI] Scale _hScale;

        [UI] Stack stack;
        [UI] StackSwitcher stacksw;

        public MainWidget MainWidget { get; }
        public GameWidget GameWidget { get; }
        public ConnectWidget ConnectWidget { get; }
        public RoomWidget RoomWidget { get; }
        public GameCore.Game Game { get; private set; }
        public GameCore.Interfaces.ClientManager ClientManager { get; private set; }
        public EmptyTest.Proxy.Client<GameCore.Interfaces.IServer, GameCore.Interfaces.ClientManager> Client { get; private set; }

        public mw() : this(new Builder("GameWindow.glade"))
        {

            MainWidget = new MainWidget();
            GameWidget = new GameWidget();
            ConnectWidget = new ConnectWidget();
            RoomWidget = new RoomWidget();
            stack.Add(MainWidget);
            stack.Add(ConnectWidget);
            stack.Add(RoomWidget);
            stack.Add(GameWidget);


            //MainWidget.ButtonPlay += MainWindow_ButtonPlay;
            MainWidget.ButtonConnect += MainWindow_ButtonConnect;
            MainWidget.ButtonCreate += MainWindow_ButtonCreate;
            //MainWidget.ButtonSettings += MainWindow_ButtonSettings;
            MainWidget.ButtonQuit += MainWindow_ButtonQuit;


            ConnectWidget.Connecting += ConnectWidget_Connecting;
            ConnectWidget.Cancel += new EventHandler((o, e) => stack.VisibleChild = MainWidget);

        }
        private mw(Builder builder) : base(builder.GetObject("mw").Handle)
        {
            builder.Autoconnect(this);
        }




        private void ConnectWidget_Connecting(object sender, ConnectWidget.EventArgsConnecting e)
        {
            Client = e.Client;
            ClientManager = Client.ClientManager;
            Game = ClientManager.Game;

        
            RoomWidget.SetClientManager(ClientManager);
            stack.VisibleChild = RoomWidget;
            ClientManager.EventGameStarted += ClientManager_EventGameStarted;
        }

        private void ClientManager_EventGameStarted()
        {
            Application.Invoke(new EventHandler((o, e) =>
            {
                GameWidget.Game = Game;
                GameWidget.SetClientManager(ClientManager);
                stack.VisibleChild = GameWidget;

                _btn3D.Visible = true;
                _hScale.Visible = true;

                _btn3D.WidgetEvent += _btn3D_WidgetEvent;
                _hScale.WidgetEvent += _hScale_WidgetEvent;
            }));
        }

        private void _hScale_WidgetEvent(object o, WidgetEventArgs args)
        {
            GameWidget.GameWidgetOver.HeigthCell = (int)_hScale.Adjustment.Value;

        }

        private void _btn3D_WidgetEvent(object o, WidgetEventArgs args)
        {
            GameWidget.GameWidgetOver.Is3D = _btn3D.Active;
        }

        private void MainWindow_ButtonQuit(object sender, EventArgs e)
        {
            Close();
        }

        private void MainWindow_ButtonSettings(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void MainWindow_ButtonCreate(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    GameServer.Program.Main(new string[] { });
                }
                catch(Exception ex)
                {

                }
            }) { Name = "GameServerThread" }.Start();
            //System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo("GameServer.exe");
            ////processStartInfo.RedirectStandardOutput = true;
            ////processStartInfo.Domain = "aaaaa";
            //GameServer = System.Diagnostics.Process.Start(processStartInfo);
            //CloseHandle(p.Handle);
            //p.WaitForExit();
        }

        private void MainWindow_ButtonConnect(object sender, EventArgs e)
        {
            stack.VisibleChild = ConnectWidget;
        }

        //private void MainWindow_ButtonPlay(object sender, EventArgs e)
        //{
        //    InitOfflineGame();
        //    stack.VisibleChild = GameWidget;
        //}

        //private void InitOfflineGame()
        //{
        //    Game = new GameCore.Game();
        //    Game.Prefabs.Set(GameCore.Interfaces.ServerManager.standartPrefabLimit);
        //    //Game.Prefabs.Add(new System.Drawing.Point(0, 0));
        //    //Game.Prefabs.Add(new System.Drawing.Point(0, 0),
        //    //                 new System.Drawing.Point(0, 1));
        //    //Game.Prefabs.Add(new System.Drawing.Point(0, 0),
        //    //                 new System.Drawing.Point(0, 1),
        //    //                 new System.Drawing.Point(1, 0));
        //    //Game.Prefabs.Add(new System.Drawing.Point(0, 0),
        //    //                 new System.Drawing.Point(0, 1),
        //    //                 new System.Drawing.Point(1, 0),
        //    //                 new System.Drawing.Point(1, 1));
        //    //Game.Prefabs.Add(new System.Drawing.Point(0, 0),
        //    //                 new System.Drawing.Point(0, 1),
        //    //                 new System.Drawing.Point(0, 2),
        //    //                 new System.Drawing.Point(1, 0));

        //    System.Drawing.Color blue = System.Drawing.Color.FromArgb(64, 128, 255);
        //    System.Drawing.Color red = System.Drawing.Color.FromArgb(192, 32, 32);
        //    //System.Drawing.Color green = System.Drawing.Color.FromArgb(32, 192, 32);
        //    Game.Players.Add(1, blue);
        //    Game.Players.Add(2, red);
        //    //Game.Players.Add(3, green);
        //    Game.CurrentPlayer = 1;
        //    Game.Start(15, 15);
        //    GameWidget.Game = Game;
        //}
    }
}
