//using GameCore;
//using GameServer.ClientObject;
//using static GameServer.ClientObject.JsonObject;
//using System;
//using System.Drawing;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading;
//using System.Linq;

//namespace gtk_test
//{
//    public class Client
//    {
     
//        public event EventHandler<EventArgsInvoke> InvokeEvent;
//        public event EventHandler<EventArgsCloseConnection> CloseConnection;
//        private Socket _serverSocket;
//        private Thread _clientThread;

//        public void Invoke(Action action)
//        {
//            InvokeEvent?.Invoke(this, new EventArgsInvoke(action));
//        }

//        public void Invoke(Object sender, Action action)
//        {
//            InvokeEvent?.Invoke(sender, new EventArgsInvoke(action));
//        }

      
//        public Game Game { get; set; }
       

//        public Client()
//        {
//            //Players = new PlayerStateCollection(this);
//        }

//        public bool Connect(IPEndPoint iPEndPoint)
//        {
//            if (_serverSocket!=null && _serverSocket.Connected) return false;
//            bool br = false;
//            try
//            {
//                _serverSocket = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
//                _serverSocket.Connect(iPEndPoint);
//                if(_serverSocket.Connected)
//                {
//                    _clientThread = new Thread(Listner)
//                    {
//                        IsBackground = true,
//                        Name = "ClientListener"
//                    };
//                    _clientThread.Start();
//                }
//                return true;
//            }
//            catch(Exception ex) { br = true; }
//            finally
//            {
//                if (br)
//                    _serverSocket?.Dispose();
//            }
//            return false;
//        }

//        public void Disconnect()
//        {
//            SendObject(new Exit());
//            _serverSocket.Close(5);
//        }
//        public void SendColor(Color color, Color color2)
//        {
//            Send($"#Color#{color.R}:{color.G}:{color.B};{color2.R}:{color2.G}:{color2.B}");
//        }

//        public Exception SendObject(JsonObject jsonObject)
//        {
//            return Send(jsonObject.ToJsonSendTCP());
//        }

//        public Exception Send(string data)
//        {
//            try
//            {
//                byte[] buffer = Encoding.UTF8.GetBytes(data + '^');
//                int bytesSent = _serverSocket.Send(buffer);
//            }
//            catch (Exception ex) { return ex; }
//            return null;
//        }


//        private void Listner()
//        {
//            while (_serverSocket.Connected)
//            {
//                try
//                {
//                    byte[] buffer = new byte[8196];
//                    int bytesRec = _serverSocket.Receive(buffer);
//                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRec);
//                    HandleData(data);
//                    //if (data.Contains("#updatechat"))
//                    //{
//                    //    //UpdateChat(data);
//                    //    continue;
//                    //}
//                }
//                catch (Exception ex)
//                {
//                    CloseConnection?.Invoke(this, new EventArgsCloseConnection(ex, true));
//                    //if (_serverSocket.Connected)
//                    //{
//                    //    //MessageBox.Show(ex.Message, "Ошибка");
//                    //    {
//                    //        Program.InvokeFix(() =>
//                    //        {
//                    //            Program.ButtonActions.Add(new Action<MsgForm.FormButton>((a) =>
//                    //            {
//                    //                Program.SetForm(MainForm.FormEnum.EnterServer);
//                    //                Program.ButtonActions.Clear();
//                    //            }));
//                    //            Program.SetMsg("Потеряно соединение");
//                    //            Program.SetBtn(new List<MsgForm.FormButton> { MsgForm.FormButton.Cont });
//                    //            Program.SetForm(MainForm.FormEnum.MsgForm);
//                    //            _serverSocket.Disconnect(false);
//                    //        });
//                    //    }
//                    //}
//                    //else MessageBox.Show(ex.Message, "Потеряно соединение");
//                }
//            }
//        }

//        private void HandleObject(JsonObject jsonObject)
//        {
//            ObjectRecieve?.Invoke(this, new EventArgsValue<JsonObject>(jsonObject));
//        }

//        private void HandleData(string data)
//        {
//            //if (!(logServer == null || logServer.IsDisposed))
//            //{
//            //    logServer.Visible = true;
//            //    logServer.WriteLine(data);
//            //}
//            //Program.WriteLog(data);
//            string[] vs = data.Split('^');
//            foreach (var item in vs)
//            {
//                if (item != "")
//                    HandleObject(FromJsonSendTCP(item));
//            }
//            //var vs = data.Split(new char[] { '#' }, 3);
//            //string command = vs[1].ToUpper();
//            //string body = vs[2];
//            //Type type = JsonObject.GetType(command);
//            //if (type == CellColor.TypeDef)
//            //{
//            //    CellColor cellColor = FromJson<CellColor>(body);
//            //    //Player = new GameCore.PlayerCell(cellColor.color1, cellColor.color2, Color.LightGray);
//            //}
//            //else if (type == Ready.TypeDef)
//            //{
//            //    Ready ready = FromJson<Ready>(body);
//            //    //IsReady = ready.isReady;
//            //}
//            //else if (type == Step.TypeDef)
//            //{
//            //    Step turn = FromJson<Step>(body);
//            //    Prefab prefab = Game.Prefabs[turn.prefabID].GetRotatedPrefab(turn.Rotate);
//            //    //prefab.Ro
//            //    Game.SetPrefab(prefab, turn.Point, out _, turn.player, false);
//            //}
//        }
//        public class EventArgsPlayer : EventArgs
//        {
//            public PlayerState PlayerState { get; }
//            public EventArgsPlayer(PlayerState playerState) => PlayerState = playerState;
//        }

//        public event EventHandler<EventArgsPlayer> NewPlayer;


//        public event EventHandler<EventArgsValue<JsonObject>> ObjectSend;
//        public event EventHandler<EventArgsValue<JsonObject>> ObjectRecieve;
//    }
//    public class EventArgsInvoke : EventArgs
//    {
//        public Action Action { get; }
//        public EventArgsInvoke(Action action)
//        {
//            Action = action;
//        }
//    }
//    public class EventArgsCloseConnection : EventArgs
//    {
//        public EventArgsCloseConnection(Exception exception, bool error)
//        {
//            Exception = exception;
//            Error = error;
//        }

//        public Exception Exception { get; }
//        public bool Error { get; }
//    }
//}
