using EmptyTest.Proxy;
using GameCore.Interfaces;
using Gtk;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UI = Gtk.Builder.ObjectAttribute;

namespace gtk_test.Widgets
{
    public class ConnectWidget: Widget
    {
        [UI] private Entry _cvEnterName = null;
        [UI] private Entry _cvEnterIP = null;
        [UI] private Entry _cvEnterPort = null;
        [UI] private Button _cvAccept = null;
        [UI] private Button _cvCancel = null;
        public ConnectWidget() : this(new Builder("GameWindow.glade")) { }

        private ConnectWidget(Builder builder) : base(builder.GetObject(typeof(ConnectWidget).Name).Handle)
        {
            builder.Autoconnect(this);
            _cvAccept.Clicked += new EventHandler((o, e) =>
              {
                  try
                  {
                      string sHost = _cvEnterIP.Text, sPort = _cvEnterPort.Text, sName = _cvEnterName.Text;
                      if (int.TryParse(sPort, out int port))
                      {
                          IPAddress iPAddress;
                          IPHostEntry iPHostEntry = Dns.GetHostEntry(sHost);
                          if (!IPAddress.TryParse(sHost, out iPAddress))
                          {
                              iPAddress = iPHostEntry.AddressList[0];
                          }
                          IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, port);
                          ClientManager clientManager = new ClientManager(new GameCore.Game());
                          Client<IServer, ClientManager> client = new Client<IServer, ClientManager>(clientManager, iPEndPoint);
                          clientManager.SetServer(client.Server, sName);
                          Connecting?.Invoke(this, new EventArgsConnecting(iPEndPoint, client, sName));
                      }
                      else
                      {
                          throw new Exception($"Неверный порт: {sPort}");
                      }
                  }
                  catch (Exception ex)
                  {
                      ErrorValidating?.Invoke(this, new EventArgsValue<Exception>(ex));
                  }
              });
            _cvCancel.Clicked += new EventHandler((o, e) => Cancel?.Invoke(this, EventArgs.Empty));
        }

        public event EventHandler<EventArgsConnecting> Connecting;
        public event EventHandler<EventArgsValue<Exception>> ErrorValidating;
        public event EventHandler Cancel;

        public class EventArgsIP : EventArgs
        {
            public IPHostEntry IPHostEntry { get; }
            public EventArgsIP(IPHostEntry value)
            {
                IPHostEntry = value;
            }
        }

        public class EventArgsConnecting:EventArgs
        {
            public EventArgsConnecting(EndPoint endPoint,  Client<IServer, ClientManager> client, string userName)
            {
                if (string.IsNullOrWhiteSpace(userName))
                {
                    throw new ArgumentException("message", nameof(userName));
                }

                EndPoint = endPoint ?? throw new ArgumentNullException(nameof(endPoint));
                Client = client ?? throw new ArgumentNullException(nameof(client));
                UserName = userName;
            }

            public EndPoint EndPoint { get; }
            public Client<IServer, ClientManager> Client { get; }
            public string UserName { get; }
        }
    }
}
