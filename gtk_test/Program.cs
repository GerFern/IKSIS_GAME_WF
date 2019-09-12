using GameCore;
using Gdk;
using Gtk;
using gtk_test.Widgets;
using gtk_test.Windows;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Timers;
using Window = Gtk.Window;
//using Controller = gtk_test.GameClientController;
using EmptyTest.TStreamHandler;
using System.Net;
using System.Threading;
using Timer = System.Timers.Timer;

namespace gtk_test
{
    class Program
    {
        static System.Timers.Timer timer = new System.Timers.Timer(100);
        static Cairo.Color back = new Cairo.Color(0, 0, 0);
        static double r = 0.9, g = 0.9, b = 0.9;
        static rgbadd rgb = Program.rgbadd.minrgb;
        //static bool addr, addg, addb, minr, ming, minb;
        public static mw mv;
        static Widgets.ttt[] ttt;
        //static MainWidget MainWidget;
        //static GameWidget GameWidget;
        //static ConnectWidget ConnectWidget;
        //static RoomWidget RoomWidget;
        static Cairo.Operator op1;
        static Cairo.Operator op2;
        //static Process GameServer;
        //static Client Client;
        static GameCore.Interfaces.ClientManager ClientManager { get; set; }
        static EmptyTest.Proxy.Client<GameCore.Interfaces.IServer, GameCore.Interfaces.ClientManager> Client { get; set; }
        static void Connect(EndPoint endPoint)
        {
            Client = new EmptyTest.Proxy.Client<GameCore.Interfaces.IServer, GameCore.Interfaces.ClientManager>(ClientManager, endPoint);
        }
        static Game Game;
        [STAThread]
        static void Main(string[] args)
        {

            
            Console.WriteLine("Hello World!");
            Application.Init();
            //GameWidget = new GameWidget();
            //MainWidget = new MainWidget();
            //ConnectWidget = new ConnectWidget();
            //RoomWidget = new RoomWidget();
            //ClientManager = new GameCore.Interfaces.ClientManager(new Game());
            //EmptyTest.Proxy.Client<GameCore.Interfaces.IServer, GameCore.Interfaces.ClientManager> client =
            //    new EmptyTest.Proxy.Client<GameCore.Interfaces.IServer, GameCore.Interfaces.ClientManager>(ClientManager, endPoint);
            //Client = new Client(StaticDataHandlers.ServerDataHandler, StaticDataHandlers.ClientDataHandler);
            //Controller.Client = Client;
            mv = new mw();
            //mv.Drawn += GameWindow_Drawn;
            //ConnectWidget.Connecting += ConnectWidget_Connecting;

            //ConnectWidget.Connecting += new EventHandler<EventArgsValue<(System.Net.IPEndPoint EndPoint, string HostName)>>((o, e) =>
            //{
            //    Connect(e.Value.EndPoint);

            //    ClientManager.Server = Client.Server;
            //    mv.stack.VisibleChild = RoomWidget;
            //    //if(Controller.EnterLobby(e.Value.EndPoint))
            //    //{
            //    //    mv.stack.VisibleChild = RoomWidget;
            //    //    Client.Game = Game;
            //    //    Controller.Game = Game;
            //    //}
            //});

            //ConnectWidget.Cancel += new EventHandler((o, e) => mv.stack.VisibleChild = MainWidget);
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            var strs=asm.GetManifestResourceNames();

            //MainWidget.ButtonPlay += MainWindow_ButtonPlay;
            //MainWidget.ButtonConnect += MainWindow_ButtonConnect;
            //MainWidget.ButtonCreate += MainWindow_ButtonCreate;
            //MainWidget.ButtonSettings += MainWindow_ButtonSettings;
            //MainWidget.ButtonQuit += MainWindow_ButtonQuit;
            //Controller.Init();
            //Controller.AddPlayer += new Action<PlayerState>(a =>
            //{
            //         RoomWidget.AddPlayerState(a);
            //});

            //mv.stack.Add(MainWidget);
            //mv.stack.Add(ConnectWidget);
            //mv.stack.Add(RoomWidget);
            //mv.stack.Add(GameWidget);
          
            mv.Destroyed += new EventHandler((o, e) => Application.Quit());
            //GameWindow.ShowAll();
            //MainWindow.ShowAll();
            //mv.stack.Add(MainWindow.Child);
            //mv.stack.Add(GameWindow.Child);
            //ttt = new Widgets.ttt[4];
            //for (int i = 0; i < 4; i++)
            //{
            //    var t = ttt[i] = new Widgets.ttt();
            //    t.LabelText = i.ToString();
            //    t.BtnClicked += T_BtnClicked;

            //    mv.stack.Add(t);
            //    //mv.stack.GetProperty("position");
            //}


            //CssProvider cssProvider = new CssProvider();
            //var t =
            //System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("test.css");
            //System.IO.StreamReader streamReader = new System.IO.StreamReader(t);
            //string data =
            //streamReader.ReadToEnd();

            ////string data = System.IO.File.ReadAllText(Environment.CurrentDirectory + "\\Css\\test.css");
            //cssProvider.LoadFromData(data);
            //MainWidget.StyleContext.AddProvider(cssProvider, 800);
            mv.ShowAll();
            //MainWindow.Child.Visible = false;
            //timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            bool c = true;
            mv.Destroyed += new EventHandler((o, e) => c = false);
            //Application.Run();
            while(c)
            {
                Application.RunIteration();
            }
            return;

            //Gtk.Window myWin = new Gtk.Window("My first GTK# Application! ");
            //GameWidget gameWidget = new GameWidget();
            //Gtk.ScrolledWindow scrolledWindow = new ScrolledWindow();
            //Gtk.Viewport viewport = new Viewport();
            //Gtk.VBox vBox = new VBox(false, 5);
            //PrefabSelectorWidget[] widgets = new PrefabSelectorWidget[Game.Prefabs.Count];
            //var en = Game.Prefabs.GetEnumerator();
            //Cairo.Color back = new Cairo.Color(0.5, 0.5, 0.7, 1);
            //Cairo.Color fore = new Cairo.Color(0.5, 0.5, 0.5, 0.6);
            //for (int i = 0; i < widgets.Length; i++)
            //{
            //    en.MoveNext();
            //    var w = widgets[i] = new PrefabSelectorWidget();
            //    w.Prefab = en.Current.Value;
            //    w.PrefabID = en.Current.Key;
            //    w.BackgroundColor = back;
            //    w.ForeColor = fore;
            //    w.GameWidget = gameWidget;
            //}


            //foreach (var item in widgets)
            //{
            //    var s = item.Prefab.Size;
            //    item.SetSizeRequest(s.Width * 50 + 30, s.Height * 50 + 30);
            //    vBox.Add(item);
            //}
            //scrolledWindow.Add(viewport);
            //viewport.Add(vBox);
            //Grid grid = new Grid();

            ////var hbox = new HBox();
            ////hbox.PackStart(new VBox(), true, true, 0);
            ////hbox.PackStart(button, false, true, 0);
            ////grid.InsertColumn(0);
            ////grid.InsertRow(0);
            ////grid.InsertRow(1);


            //scrolledWindow.SetSizeRequest(200, 400);
            //scrolledWindow.Vexpand = true;

            //gameWidget.SetSizeRequest(400, 400);
            //gameWidget.Vexpand = gameWidget.Hexpand = true;
            //grid.Attach(scrolledWindow, 0, 0, 1, 1);
            //grid.Attach(gameWidget, 1, 0, 1, 1);

            //myWin.Add(grid);
            ////drawingArea.ScrollEvent += new ScrollEventHandler((o, e) =>
            //// {
            ////     if (e.Event.Direction == ScrollDirection.Down)
            ////         drawingArea.Scale -= 0.1;
            ////     else if (e.Event.Direction == ScrollDirection.Up)
            ////         drawingArea.Scale += 0.1;
            ////     drawingArea.QueueDraw();
            //// });
            ////gameWidget.AddEvents((int)EventMask.AllEventsMask);
            //gameWidget.ButtonReleaseEvent += new ButtonReleaseEventHandler((o, e) =>
            //{
            //});
            //gameWidget.WidgetEvent += new WidgetEventHandler((o, e) =>
            // {
            //     System.Console.WriteLine(e.Event.Type);
            //     //System.Diagnostics.Debug.WriteLine(e.Event.Type);
            //     //System.Diagnostics.Debug.WriteLine(e.Event);
            // });
            ////myWin.Add(gLArea);
            //myWin.Resize(200, 200);
            ////Label myLabel = new Label();
            ////myLabel.Text = "Hello World!!!!";

            ////Add the label to the form
            ////myWin.Add(myLabel);
            //myWin.KeyPressEvent += new KeyPressEventHandler((o, e) =>
            //{
            //    if (e.Event.Key == Gdk.Key.Control_L)
            //        gameWidget.Control = true;
            //});
            //myWin.KeyReleaseEvent += new KeyReleaseEventHandler((o, e) =>
            //{
            //    if (e.Event.Key == Gdk.Key.Control_L)
            //        gameWidget.Control = false;
            //});
            //gameWidget.Game = Game;
            //myWin.ShowAll();
            //myWin.Destroyed += new EventHandler((o, e) => Application.Quit());
            ////button.Clicked += new EventHandler((o, e) =>
            ////{
            ////    //drawingArea.

            ////    //SKSurface sk = CreateSKSurface(button);
            ////    //SKCanvas sKCanvas = sk.Canvas;
            ////    //Random r = new Random();
            ////    //SKPaint sKPaint = new SKPaint();
            ////    //gLArea.Render += new RenderHandler((oo, re) =>
            ////    //{
            ////    //    var alloc = gLArea.Allocation;
            ////    //    var imgInfo = new SKImageInfo(alloc.Width, alloc.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);

            ////    //    re.Context.Window.Beep();
            ////    //    sKCanvas.Clear(new SKColor((uint)r.Next()));
            ////    //    sKPaint.Color = new SKColor(255, 0, 128);
            ////    //    sKCanvas.DrawText("AAAA", 20, 20, sKPaint);
            ////    //    sKCanvas.DrawText("AAAA", 20, 120, sKPaint);
            ////    //    sKCanvas.DrawText("AAAA", 20, 220, sKPaint);
            ////    //    sKCanvas.DrawText("AAAA", 120, 320, sKPaint);
            ////    //    sKCanvas.Flush();
            ////    //});
            ////    //gLArea.StyleContext.Invalidate();
            ////    //drawingArea.Drawn += new DrawnHandler((oo, de) =>
            ////    //{
            ////    //    DrawingArea area = (DrawingArea)oo;
            ////    //    var cr = de.Cr;
            ////    //    cr.LineWidth = 9;
            ////    //    cr.SetSourceRGB(0.7, 0.2, 0.0);

            ////    //    int width, height;
            ////    //    width = area.Allocation.Width;
            ////    //    height = area.Allocation.Height;

            ////    //    cr.Translate(width / 2, height / 2);
            ////    //    cr.Arc(0, 0, (width < height ? width : height) / 2 - 10, 0, 2 * Math.PI);
            ////    //    cr.StrokePreserve();

            ////    //    cr.SetSourceRGB(0.3, 0.4, 0.6);
            ////    //    cr.Fill();
            ////    //});
            ////});
            //Application.Run();
        }

        //private static void ConnectWidget_Connecting(object sender, ConnectWidget.EventArgsConnecting e)
        //{
        //    Client = e.Client;
        //    ClientManager = Client.ClientManager;
        //    Game = ClientManager.Game;

        //    //RoomWidget = e.RoomWidget;
        //    //mv.stack.Add(e.RoomWidget);
        //    //RoomWidget.SetClientManager(ClientManager);
        //    mv.stack.VisibleChild = RoomWidget;
        //    ClientManager.EventGameStarted += ClientManager_EventGameStarted;
        //}

        //private static void ClientManager_EventGameStarted()
        //{
        //    Application.Invoke(new EventHandler((o, e) =>
        //    {
        //        GameWidget.Game = Game;
        //        GameWidget.SetClientManager(ClientManager);
        //        mv.stack.VisibleChild = GameWidget;
        //    }));
        //}

        //private static void MainWindow_ButtonQuit(object sender, EventArgs e)
        //{
        //    mv.Close();
        //}

        //private static void MainWindow_ButtonSettings(object sender, EventArgs e)
        //{
        //    //throw new NotImplementedException();
        //}

        //[System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        //static extern bool CloseHandle(IntPtr hHandle);

        //[return: MarshalAs(UnmanagedType.Bool)]
        //[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        //[DllImport("user32", CharSet = CharSet.Unicode, SetLastError = true)]
        //public static extern bool CloseWindowStation(IntPtr hWinsta);

        //private static void MainWindow_ButtonCreate(object sender, EventArgs e)
        //{
        //    new Thread(() => GameServer.Program.Main(new string[] { "192.168.0.49", "7979" })) { Name = "GameServerThread"}.Start();
        //    //System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo("GameServer.exe");
        //    ////processStartInfo.RedirectStandardOutput = true;
        //    ////processStartInfo.Domain = "aaaaa";
        //    //GameServer = System.Diagnostics.Process.Start(processStartInfo);
        //    //CloseHandle(p.Handle);
        //    //p.WaitForExit();
        //}

        //private static void MainWindow_ButtonConnect(object sender, EventArgs e)
        //{
        //    mv.stack.VisibleChild = ConnectWidget;
        //}

        //private static void MainWindow_ButtonPlay(object sender, EventArgs e)
        //{
        //    InitOfflineGame();
        //    mv.stack.VisibleChild = GameWidget;
        //}

        //internal static void InitOfflineGame()
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

        enum rgbadd
        {
            addr,
            addg,
            addb,
            minr,
            ming,
            minb,
            addrgb,
            minrgb
        }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Timer t = (Timer)sender;
            t.Stop();
            const double min = 0.65;
            const double max = 0.95;
            const double d = 0.01;
            switch (rgb)
            {
                case rgbadd.addr:
                    if (r >= max)
                    {
                        rgb = rgbadd.minb;
                    }

                    break;
                case rgbadd.addg:
                    if (g >= max)
                    {
                        rgb = rgbadd.minr;
                    }

                    break;
                case rgbadd.addb:
                    if (b >= max)
                    {
                        rgb = rgbadd.ming;
                    }

                    break;
                case rgbadd.minr:
                    if (r <= min)
                    {
                        rgb = rgbadd.addb;
                    }

                    break;
                case rgbadd.ming:
                    if (g <= min)
                    {
                        rgb = rgbadd.addr;
                    }

                    break;
                case rgbadd.minb:
                    if (b <= min)
                    {
                        rgb = rgbadd.addg;
                    }

                    break;
                case rgbadd.addrgb:
                    if(b >= max)
                    {
                        rgb = rgbadd.minb;
                    }
                    break;
                case rgbadd.minrgb:
                    if (b <= min)
                    {
                        rgb = rgbadd.addb;
                    }
                    break;
                default:
                    break;
            }


            switch (rgb)
            {
                case rgbadd.addr:
                    r += d;
                    break;
                case rgbadd.addg:
                    g += d;
                    break;
                case rgbadd.addb:
                    b += d;
                    break;
                case rgbadd.minr:
                    r -= d;
                    break;
                case rgbadd.ming:
                    g -= d;
                    break;
                case rgbadd.minb:
                    b -= d;
                    break;
                case rgbadd.addrgb:
                    r += d;
                    g += d;
                    b += d;
                    break;
                case rgbadd.minrgb:
                    r -= d;
                    g -= d;
                    b -= d;
                    break;
                default:
                    break;
            }

            //addr = addg = addb = minr = ming = minr = false;
            //if(r==1)
            //{
            //    if (g == 1) minr = true;
            //    else if (b == 1) minb = true;
            //    else if (b == 0) addg = true;
            //}
            //else if(g==1)
            //{
            //    if (r == 0) addb = true;
            //    if ()
            //}
            //if (r > max) r = max;
            //else if (r < min) r = min;
            //if (g > max) g = max;
            //else if (g < min) g = min;
            //if (b > max) b = max;
            //else if (b < min) b = min;
            back = new Cairo.Color(r, g, b, 0.999);
            //Application.Invoke(new EventHandler((o,_e)=>
            //mv.QueueDraw()));

            t.Start();
        }

        private static void GameWindow_Drawn(object o, DrawnArgs args)
        {
            Window w = (Window)o;
            var cr =  args.Cr;
            var t = cr.Operator;
            cr.Operator = op1;
            cr.Operator = Cairo.Operator.Source;
            var alloc = w.Allocation;
            cr.Rectangle(new Cairo.Rectangle(0, 0, alloc.Width, alloc.Height));
            //cr.Color = new Cairo.Color(0.2, 0.5, 0.7, 0.5);
            cr.Color = back;
            cr.Fill();
            cr.Translate(w.Child.Allocation.X, 0);
            cr.Operator = op2;
            cr.Operator = t;
            w.Child.Draw(cr);
        }

        private static void NewMethod(object o, DrawnArgs e)
        {
            
        }

        //private static void T_BtnClicked(object sender, Widgets.ttt.EventArgsInt e)
        //{
        //    //var st =(StackTransitionType)(new Random().Next() % 20);
        //    var st = StackTransitionType.SlideLeftRight;
        //    mv.stack.TransitionType = st;
        //   System.Console.WriteLine(st);
        //       mv.stack.VisibleChild = ttt[e.Value];
           
        //}


    }


}
