using EmptyTest.Proxy;
using EmptyTest.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace EmptyTest.Client
{
    /// <summary>
    /// Клиент. Обеспечивает связь с сервером
    /// </summary>
    /// <typeparam name="TServer">Интерфейс сервера</typeparam>
    /// <typeparam name="TClient">Клиент, обработчик сообщений сервера. Тип должен быть производным интерфейса</typeparam>
public class Client<TServer, TClient>
{
    /// <summary>
    /// Server Remote
    /// </summary>
    public TServer Server { get; }
    internal Socket serverSocket;
    Thread thread;
    TClient clientManager;
    public TClient ClientManager
    {
        get => clientManager;
        private set
        {
            clientManager = value;
            ClientType = value.GetType();
        }
    }
    public Type ClientType { get; private set; }

    public Client(TClient client, EndPoint endPointServer)
    {
        ClientManager = client;
        serverSocket = new Socket(endPointServer.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.Connect(endPointServer);
        Server = ServerController<TServer, TClient>.Create(serverSocket);
        ((ServerController<TServer, TClient>)(object)Server).Reciever.MethodEvent += Reciever_MethodEvent;
    }

    private void Reciever_MethodEvent(object sender, MethodEventArgs e)
    {
        var method = ClientType.GetMethod(e.MethodName, e.Args.Select(a => a.GetType()).ToArray());
        var result = method.Invoke(clientManager, e.Args);
        if (e.ReturnRequest == true)
        {
            ((ServerController<TServer, TClient>)(object)Server)
                .writer.WriteResult(e.ID, result);
        }
    }
}
}
