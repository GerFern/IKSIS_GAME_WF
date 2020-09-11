using EmptyTest.Proxy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

namespace EmptyTest.Server
{
    /// <summary>
    /// Класс для обмена сообщений с сервером
    /// </summary>
    /// <typeparam name="TServer"></typeparam>
    /// <typeparam name="TClient"></typeparam>
    public class ServerController<TServer, TClient> : DispatchProxy
    {
        Socket server;
        NetworkStream networkStream;
        BinaryReader reader;
        internal BinaryWriter writer;
        Thread threadRecieve;
        Reader reciever;
        protected virtual Func<MethodInfo, object[], object> func { get; }
        internal Reader Reciever { get => reciever; }

        public static TServer Create(Socket socket)
        {
            object t = Create<TServer, ServerController<TServer, TClient>>();
            var proxy = (ServerController<TServer, TClient>)t;
            proxy.server = socket;
            var ns = proxy.networkStream = new NetworkStream(socket);
            proxy.reader = new BinaryReader(ns);
            proxy.writer = new BinaryWriter(ns);
            proxy.reciever = new Reader(proxy.reader);
            proxy.threadRecieve = new Thread(proxy.InitThread) { Name = $"RecieverFromServer_{socket.RemoteEndPoint}" };
            proxy.threadRecieve.Start();
            return (TServer)t;
        }

        void InitThread()
        {
            try
            {
                Reciever.Listener();
            }
            catch (Exception ex)
            {
                ExceptionOnRecieve?.Invoke(this, ex);
            }
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            int id = this.id++;
            bool waitresponse;
            var property = GetPropertyInfo(targetMethod, out _);
            if (property != null) waitresponse = property.GetCustomAttribute<WaitResponseAttribute>() != null;
            else waitresponse = targetMethod.GetCustomAttribute<WaitResponseAttribute>() != null;
            MethodEventArgs method = new MethodEventArgs(
                id,
                waitresponse,
                targetMethod.Name,
                args);
            writer.WriteAction(method);
            if (waitresponse)
                return reciever.GetValue(id);
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
                        return typeof(TServer).GetProperty(methodInfo.Name.Substring(4));
                }
                else return null;
            }
            catch { }
            return null;
        }
        public event EventHandler<Exception> ExceptionOnRecieve;
    }
}
