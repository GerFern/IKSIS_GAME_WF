using EmptyTest.TStreamHandler;
using System;
using System.Net;
using System.Timers;

namespace EmptyTest
{
    public class Data
    {

    }

    class Program
    {
        static void Main(string[] args)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7979);
            //Server server = new Server(endPoint);
            //Client client = new Client();
            //client.Connect(endPoint);
            ServerManager serverManager = new ServerManager();
            Proxy.Server<ServerManager, IClient, Data> server = new Proxy.Server<ServerManager, IClient, Data>(serverManager);
            server.Launch(endPoint);

            
            ClientManager clientManager = new ClientManager();
            Proxy.Client<IServer, ClientManager> client = new Proxy.Client<IServer, ClientManager>(clientManager, endPoint);

            client.Server.Message("aaaaa");
            int r = client.Server.GetRandom;

            

            Timer timer = new Timer();
            timer.Interval = 5000;
            timer.Start();
            timer.AutoReset = false;
            timer.Elapsed += new ElapsedEventHandler((o, e) =>
            {
                timer.Stop();
                foreach (var item in server.Clients.Values)
                {
                    var cl = item.ClientInterface;
                    cl.Test();
                    System.Diagnostics.Debug.WriteLine(cl.GetRandom());
                    System.Diagnostics.Debug.WriteLine(cl.GetRandom());
                    System.Diagnostics.Debug.WriteLine(cl.GetRandom());
                    System.Diagnostics.Debug.WriteLine(cl.GetRandom());
                    System.Diagnostics.Debug.WriteLine(cl.GetRandom());
                    cl.Test();
                }
                timer.Start();
            });
            
            Console.WriteLine("Hello World!");
        }
    }

    public class ServerManager : IServer
    {
        static IClient Client => Proxy.ClientInfo<IClient, Data>.CurrentClientInterface;

        static Random random = new Random();
        public int GetRandom
        
        =>    random.Next();
        

        public void Message(string msg)
        {

            Console.WriteLine($"{msg}");
        }
    }

    public class ClientManager : IClient
    {
        static Random random = new Random();
       

        public int GetRandom()
        {
            return random.Next();
        }

        public void Test()
        {
            Console.WriteLine("AAAA");
            
            System.Diagnostics.Debug.WriteLine("AAAAA");
        }
    }

    public interface IServer
    {
        void Message(string msg);
        [Proxy.WaitResponse]
        int GetRandom { get; }
    }

    public interface IClient
    {
        void Test();
        [Proxy.WaitResponse]
        int GetRandom();
    }
}
