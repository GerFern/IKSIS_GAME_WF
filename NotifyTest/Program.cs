using System;
using System.Net;
using System.Collections.Generic;

namespace NotifyTest
{
    class Program
    {
        static void Main(string[] args)

        {
            Console.WriteLine("Hello World!");
        
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7979);
            Server server = new Server(iPEndPoint);
            Client client = new Client();
            server.ObjectGetted += Client_ObjectGetted;
            client.Connect(iPEndPoint);

            //server.AddType()
            client.Send("hi");
        }

        private static void Client_ObjectGetted(object sender, StreamWRHandler.GetObj e)
        {
            //throw new NotImplementedException();
        }

        private static void Class1_PropertyChanged1(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
