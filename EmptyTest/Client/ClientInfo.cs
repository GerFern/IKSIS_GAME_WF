using System;
using System.Net;

namespace EmptyTest.Client
{
    public class ClientInfo<IClient, TClientData>
    {
        [ThreadStatic] internal static IClient currentClientInterface;
        [ThreadStatic] internal static ClientInfo<IClient, TClientData> currentClientInfo;
        public static IClient CurrentClientInterface => currentClientInterface;
        public static ClientInfo<IClient, TClientData> CurrentClientInfo => currentClientInfo;


        public ClientInfo(IClient client, EndPoint remoteEndPoint, int uID)
        {
            ClientInterface = client;
            RemoteEndPoint = remoteEndPoint ?? throw new ArgumentNullException(nameof(remoteEndPoint));
            UID = uID;
            Data = (TClientData)Activator.CreateInstance(typeof(TClientData));
        }

        public IClient ClientInterface { get; }
        public EndPoint RemoteEndPoint { get; }
        public int UID { get; }
        public TClientData Data { get; }

        public void Disconnect()
        {
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Disconnected;
    }


    //public class ClientInfo<IClient, TClientData>
    //{
    //    //TClientData data;

    //    //public ClientInfo(IClient client, EndPoint remoteEndPoint, int uID)
    //    //    : base(client, remoteEndPoint, uID) { }

    //    //public override object Data
    //    //{
    //    //    get => data;
    //    //    set => data = (TClientData)value;
    //    //}
    //    //public TClientData TypedData
    //    //{
    //    //    get => data;
    //    //    set => data = value;
    //    //}

    //}
}