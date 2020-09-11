using EmptyTest.Proxy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace EmptyTest.Client
{
    /// <summary>
    /// Класс для обмена сообщений с клиентом
    /// </summary>
    /// <typeparam name="TServer"></typeparam>
    /// <typeparam name="TClient"></typeparam>
public class ClientController<TServer, TClient, TClientData> : DispatchProxy
{
    public ClientInfo<TClient, TClientData> ClientInfo { get; private set; }
    protected int clientID;
    protected NetworkStream networkStream;
    protected BinaryReader reader;
    internal BinaryWriter writer;
    protected Thread threadRecieve;
    protected Socket socketClient;
    protected virtual Func<MethodInfo, object[], Task<object>> func { get; }
    internal Reader Reciever { get => reciever; }
    public static TClient Create(Socket socket, int uid)
    {
        object t = Create<TClient, ClientController<TServer, TClient, TClientData>>();
        var proxy = (ClientController<TServer, TClient, TClientData>)t;
        proxy.socketClient = socket;
        var ns = proxy.networkStream = new NetworkStream(socket);
        proxy.clientID = uid;
        proxy.ClientInfo = new ClientInfo<TClient, TClientData>((TClient)t, socket.RemoteEndPoint, uid);
        proxy.reader = new BinaryReader(ns);
        proxy.writer = new BinaryWriter(ns);
        proxy.reciever = new Reader(proxy.reader);
        proxy.threadRecieve = new Thread(proxy.InitThread) { Name = $"RecieverFromClient_{socket.RemoteEndPoint}" };
        proxy.threadRecieve.Start();
        return (TClient)t;
    }
    protected virtual void InitThread()
    {
        var client = ClientInfo<TClient, TClientData>.currentClientInterface = (TClient)(object)this;
        ClientInfo<TClient, TClientData>.currentClientInfo = new ClientInfo<TClient, TClientData>(client, socketClient.RemoteEndPoint, clientID);
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
    internal virtual object GetClientInfo() => ClientInfo;
    protected Reader reciever;
    int id = 0;
    public event EventHandler<Exception> ExceptionOnRecieve;
}
}
