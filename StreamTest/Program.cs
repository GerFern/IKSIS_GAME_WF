using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace NotifyTest
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            byte[] vs = new byte[99999];
            FileStream s1 = new FileStream("example.bin", FileMode.Create, FileAccess.Write, FileShare.Read);
            FileStream s2 = new FileStream("example.bin", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7979);
            Server server = new Server(iPEndPoint);
            Client client = new Client();
            client.ObjectGetted += Client_ObjectGetted;
            client.Connect(iPEndPoint);
            //var s1 = File.Create("example.bin");
            //BinaryReaderE br = new BinaryReaderE(s2);
            ////var s2 = File.OpenRead("example.bin");
            //BinaryWriterE bw = new BinaryWriterE(s1);
            //Class1 c = new Class1();
            server.AddType(typeof(TestObject));
            //c.Stream = s;

            //client.Send("Hello world");
            //bw.Flush();

            //var str = c.RecieveObject(out _);
            //var test = new TestObject();
            //test.Id = 85384;
            //test.Message = "aaaaaaaaaa";
            //c.SendObject(test);
            //bw.Flush();
            //var test2 = c.RecieveObject(out _);
            //var vs1 = new string[] { "aaa", "bcd", "" };
            //c.SendObject(vs1);
            //bw.Flush();
            //var aaa = c.RecieveObject(out _);
            Proxy.InitIList();
            Proxy.AddType(typeof(Test));
            Proxy.AddType(typeof(T2));
            //Proxy.AddIListType(typeof(List<int>), typeof(IList<int>));
            Proxy.AddType(typeof(T3<>));


            Test test = new Test();
            T2 test2 = new T2();

            //var list = /*Proxy.Create<IList<int>, ProxyList<int>>();*/
            //Node.CreateNode<IList<int>, List<int>>();
            ////((ProxyList<int>)list).SetTargetArr(new List<int>());
            //var a = list.GetEnumerator();
            //TopNode top;
            //IT2 t2 = TopNode.CreateTopNode<IT2, T2>(out top);
            //list.Add(1);
            //dynamic buf = list;
            //t2.tets3 = buf;
            //t2.tets3[1] = 1;
            //var arr = t2.tets3;
            //t2.test1 = Node.CreateNode<ITest>();


            var t1 = TopNode.CreateTopNode<IT2>(out TopNode topNode);
            topNode.Proxy.SetTarget(test2);
            var t3 = Node.CreateNode<IT3<int>, T3<int>>();
            ProxyHandler proxyHandler = new ProxyHandler(true, new StreamWRHandler(server.Writer, server.Reader), topNode);

            t3.Data = 3;
            var aaa1 = t3.Test();
            var aaa2 = t3.GType();
            var aaa3 = t3.GTypeA<string>();
        }

        private static void Client_ObjectGetted(object sender, StreamWRHandler.GetObj e)
        {

        }
    }
}
