using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace NotifyTest
{
    public class Client : StreamWRHandler
    {
     
        public event EventHandler<EventArgsInvoke> InvokeEvent;
        public event EventHandler<EventArgsCloseConnection> CloseConnection;
        private Socket _serverSocket;
        private Thread _clientThread;
        NetworkStream NetworkStream;
        public TopNode ServerContainer;
        public TopNode ClientContainer;


        public Socket Socket => _serverSocket;

        public void Invoke(Action action)
        {
            InvokeEvent?.Invoke(this, new EventArgsInvoke(action));
        }

        public void Invoke(Object sender, Action action)
        {
            InvokeEvent?.Invoke(sender, new EventArgsInvoke(action));
        }

      
       

        public Client() : base()
        {
            //Players = new PlayerStateCollection(this);
            //ServerContainer = new TopNode();
            //ClientContainer = new TopNode();
        }

        public bool Connect(IPEndPoint iPEndPoint)
        {
            if (_serverSocket!=null && _serverSocket.Connected) return false;
            bool br = false;
            try
            {
                _serverSocket = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _serverSocket.Connect(iPEndPoint);
                if(_serverSocket.Connected)
                {
                    NetworkStream = new NetworkStream(_serverSocket);
                    Reader = new BinaryReaderE(NetworkStream);
                    Writer = new BinaryWriterE(NetworkStream);
                    Reader.PropertyNames = Writer.PropertyNames = PropertyNames;
                    Reader.Types = Writer.Types = Types;
                    _clientThread = new Thread(Listner)
                    {
                        IsBackground = true,
                        Name = "ClientListener"
                    };
                    _clientThread.Start();
                }
                return true;
            }
            catch(Exception ex) { br = true; }
            finally
            {
                if (br)
                    _serverSocket?.Dispose();
            }
            return false;
        }

        public void Disconnect()
        {
            _serverSocket.Close(5);
        }
   
        //public Exception SendObject(JsonObject jsonObject)
        //{
        //    return Send(jsonObject.ToJsonSendTCP());
        //}

        //public Exception Send(string data)
        //{
        //    try
        //    {
        //        byte[] buffer = Encoding.UTF8.GetBytes(data + '^');
        //        int bytesSent = _serverSocket.Send(buffer);
        //    }
        //    catch (Exception ex) { return ex; }
        //    return null;
        //}


        private void Listner()
        {
            ServerContainer = new TopNode();
            ClientContainer = new TopNode();
            while (_serverSocket.Connected)
            {
                try
                {
                    Recieve();
                    //byte[] buffer = new byte[8196];
                    //int bytesRec = _serverSocket.Receive(buffer);
                    //string data = Encoding.UTF8.GetString(buffer, 0, bytesRec);
                    //HandleData(data);
                    //if (data.Contains("#updatechat"))
                    //{
                    //    //UpdateChat(data);
                    //    continue;
                    //}
                }
                catch (Exception ex)
                {
                    CloseConnection?.Invoke(this, new EventArgsCloseConnection(ex, true));
                    //if (_serverSocket.Connected)
                    //{
                    //    //MessageBox.Show(ex.Message, "Ошибка");
                    //    {
                    //        Program.InvokeFix(() =>
                    //        {
                    //            Program.ButtonActions.Add(new Action<MsgForm.FormButton>((a) =>
                    //            {
                    //                Program.SetForm(MainForm.FormEnum.EnterServer);
                    //                Program.ButtonActions.Clear();
                    //            }));
                    //            Program.SetMsg("Потеряно соединение");
                    //            Program.SetBtn(new List<MsgForm.FormButton> { MsgForm.FormButton.Cont });
                    //            Program.SetForm(MainForm.FormEnum.MsgForm);
                    //            _serverSocket.Disconnect(false);
                    //        });
                    //    }
                    //}
                    //else MessageBox.Show(ex.Message, "Потеряно соединение");
                }
            }
        }

       

        public override void Send(object value)
        {
            base.Send(value);
            //SendObject(value);
        }


        int GetIdName(string property)
        {
            return PropertyNames.FindKey(property);
        }

        int GetIdType(Type type)
        {
            if (type.IsGenericType) type = type.GetGenericTypeDefinition();
            return Types.FindKey(type);
        }



        //private void HandleObject(JsonObject jsonObject)
        //{
        //    ObjectRecieve?.Invoke(this, new EventArgsValue<JsonObject>(jsonObject));
        //}


        //public class EventArgsPlayer : EventArgs
        //{
        //    public PlayerState PlayerState { get; }
        //    public EventArgsPlayer(PlayerState playerState) => PlayerState = playerState;
        //}

        //public event EventHandler<EventArgsPlayer> NewPlayer;


        //public event EventHandler<EventArgsValue<JsonObject>> ObjectSend;
        //public event EventHandler<EventArgsValue<JsonObject>> ObjectRecieve;
    }
    public class EventArgsInvoke : EventArgs
    {
        public Action Action { get; }
        public EventArgsInvoke(Action action)
        {
            Action = action;
        }
    }
    public class EventArgsCloseConnection : EventArgs
    {
        public EventArgsCloseConnection(Exception exception, bool error)
        {
            Exception = exception;
            Error = error;
        }

        public Exception Exception { get; }
        public bool Error { get; }
    }
}
