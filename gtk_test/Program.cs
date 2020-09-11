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
        static Cairo.Operator op1;
        static Cairo.Operator op2;
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
            mv = new mw();
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            var strs=asm.GetManifestResourceNames();
            mv.Destroyed += new EventHandler((o, e) => Application.Quit());
            mv.ShowAll();
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            bool c = true;
            mv.Destroyed += new EventHandler((o, e) => c = false);
            Application.Run();
            return;
        }

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

            back = new Cairo.Color(r, g, b, 0.999);
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
            cr.SetSourceColor(back);
            cr.Fill();
            cr.Translate(w.Child.Allocation.X, 0);
            cr.Operator = op2;
            cr.Operator = t;
            w.Child.Draw(cr);
        }

        private static void NewMethod(object o, DrawnArgs e)
        {
            
        }
    }
}
