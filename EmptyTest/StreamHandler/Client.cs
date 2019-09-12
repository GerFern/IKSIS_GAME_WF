using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using EmptyTest.TCommand;
using System.Collections.ObjectModel;

namespace EmptyTest.TStreamHandler
{
    public class Client : StreamWRHandler
    {
     
        public event EventHandler<EventArgsInvoke> InvokeEvent;
        public event EventHandler<EventArgsCloseConnection> CloseConnection;
        private Socket _server;
        private Thread _clientThread;
        NetworkStream NetworkStream;
        public DataHandler ServerDataHandler { get; }
        public DataHandler ClientDataHandler => DataHandler;

        public dynamic ServerPublicContainer;
        public dynamic ClientPrivateContainer;


        public void Invoke(Action action)
        {
            InvokeEvent?.Invoke(this, new EventArgsInvoke(action));
        }

        public void Invoke(Object sender, Action action)
        {
            InvokeEvent?.Invoke(sender, new EventArgsInvoke(action));
        }




        public Client(DataHandler serverDataHandler, DataHandler clientDataHandler) : base(clientDataHandler)
        {
            //Players = new PlayerStateCollection(this);
            //ServerContainer = new TopNode();
            //ClientContainer = new TopNode();
        }

        public bool Connect(IPEndPoint iPEndPoint)
        {
            if (_server!=null && _server.Connected) return false;
            bool br = false;
            try
            {
                _server = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _server.Connect(iPEndPoint);
                if (_server.Connected)
                {
                    NetworkStream = new NetworkStream(_server);
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
                    _server?.Dispose();
            }
            return false;
        }

        public void Disconnect()
        {
            _server.Close(5);
        }
   
    
  


        private void Listner()
        {
            //ServerContainer = new TopNode();
            //ClientContainer = new TopNode();
            while (_server.Connected)
            {
                try
                {
                    CommandInvoke(Recieve(), true, ActionCommand);
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



        //public override void Send(object value)
        //{
        //    base.Send(value);
        //    //SendObject(value);
        //}

        private static readonly ReadOnlyDictionary<Type, Action<Client, Command>> actions =
            new ReadOnlyDictionary<Type, Action<Client, Command>>(new Dictionary<Type, Action<Client, Command>>()
            {
                {typeof(CommandLogIn), (client, cmd)=>{ } },
                {typeof(CommandWelcomeClient), (client, cmd)=>{ } },
                {typeof(CommandGetTypeListRequest), (client, cmd)=>{ } },
                {typeof(CommandSendTypeList), (client, cmd)=>{ } },
                {typeof(CommandGetObjectRequest), (client, cmd)=>{ } },
                {typeof(CommandRecieveObject), (client, cmd)=>{ } },
                {typeof(CommandAction), (client, cmd)=>{ } },
                {typeof(CommandActionResult), (client, cmd)=>{ } },
                {typeof(CommandDisconnect), (client, cmd)=>{ } },
            });

        public override void ActionCommand(Command command)
        {
            actions[command.GetType()].Invoke(this, command);
            //switch (command.CmdType)
            //{
            //    case Command.CommandType.SendTypeList:
            //        if (this.Types == null || this.Types.Count == 0)
            //        {
            //            this.Types = new DictCounter<Type>();
            //            foreach (var item in command.dictTypes)
            //            {
            //                Types.data.Add(item.Key, item.Value);
            //            }
            //        }
            //        break;
            //    case Command.CommandType.GetObjectRequest:
            //        break;
            //    case Command.CommandType.RecieveObject:
            //        break;
            //    default:
            //        break;
            //}
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
