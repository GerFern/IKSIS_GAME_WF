using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

namespace EmptyTest.Proxy
{
    /// <summary>
    /// Класс для обмена сообщений с сервером
    /// </summary>
    /// <typeparam name="TServer"></typeparam>
    /// <typeparam name="TClient"></typeparam>
    public class ServerController<TServer, TClient> : DispatchProxy
    {
        //public List<Client<TClient>> clients = new List<Client<TClient>>();
        Socket server;

        NetworkStream networkStream;
        BinaryReaderE reader;
     internal   BinaryWriterE writer;
        Thread threadRecieve;
        Reader reciever;
        protected virtual Func<MethodInfo, object[], object> func { get; }
        internal Reader Reciever { get => reciever; }

        //protected TClient real;
        //protected TNode.DynamicNode owner;

        public static TServer Create(Socket socket)
        {

            object t = Create<TServer, ServerController<TServer, TClient>>();
            var proxy = ((ServerController<TServer, TClient>)t);
            proxy.server = socket;
            var ns = proxy.networkStream = new NetworkStream(socket);
            proxy.reader = new BinaryReaderE(ns);
            proxy.writer = new BinaryWriterE(ns);
            proxy.reciever = new Reader(proxy.reader);
            proxy.threadRecieve = new Thread(proxy.InitThread) { Name = $"RecieverFromServer_{socket.RemoteEndPoint}" };
            proxy.threadRecieve.Start();
            //proxy.owner = owner;
            return (TServer)t;
        }

        void InitThread()
        {
            //Server<TServer, TClient>.currentClient = (TClient)(object)this;
            try
            {
                Reciever.Listener();
            }
            catch (Exception ex)
            {
                ExceptionOnRecieve?.Invoke(this, ex);
            }
        }

        //public static T LaunchServer<T>(T serverController, EndPoint endPoint)
        //{
        //    if(serverController is ServerController<T> sc)
        //    {
        //        sc.LaunchServer(endPoint);
        //        return serverController;
        //    }
        //    else
        //    {
        //        object obj = Create<T>(serverController);
        //        ((ServerController<T>)obj).LaunchServer(endPoint);
        //        return (T)obj;
        //    }
        //}

        //void LaunchServer(EndPoint endPoint)
        //{
        //    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //    server.Bind(endPoint);
        //    server.Listen(100);
        //    thread = new Thread(Start) { Name = "ServerThreadAccept" };
        //    thread.Start();
        //}

        //void Start()
        //{
        //    while (true)
        //    {
        //        try
        //        {
        //            Socket client = server.Accept();
                    
        //            //clients.Add(client);
        //            //NewClient(user, ClientDataHandler);
        //        }
        //        catch (Exception exp) { Console.WriteLine("Error: {0}", exp.Message); }
        //    }
        //}

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            int id = this.id++;
            //this.id++;

            bool waitresponse;
            var property = GetPropertyInfo(targetMethod, out _);
            if (property!=null) waitresponse = property.GetCustomAttribute<WaitResponseAttribute>() != null;
            else waitresponse = targetMethod.GetCustomAttribute<WaitResponseAttribute>() != null;
            MethodEventArgs method = new MethodEventArgs(
                id,
                waitresponse,
                targetMethod.Name,
                args);
            WriterHelper.WriteAction(writer, method);
            if (waitresponse)
            {
                return reciever.GetValue(id);
            }
            else return null;
        }
        int id;

        PropertyInfo GetPropertyInfo(MethodInfo methodInfo, out bool isGet)
        {
            isGet = false;
            try
            {
                if (methodInfo.IsSpecialName)
                {
                    if (methodInfo.Name.StartsWith("get_"))
                    {
                        isGet = true;
                        return typeof(TServer).GetProperty(methodInfo.Name.Substring(4));
                    }
                    else if (methodInfo.Name.StartsWith("set_"))
                    {
                        return typeof(TServer).GetProperty(methodInfo.Name.Substring(4));
                    }
                }
                else return null;
            }
            catch {}
            return null;
        }

        public event EventHandler<Exception> ExceptionOnRecieve;
    }


    //public class ClientControllerCallback
}
