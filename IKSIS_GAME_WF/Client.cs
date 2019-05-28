using GameCore;
using GameServer.ClientObject;
using static GameServer.ClientObject.JsonObject;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;

namespace IKSIS_GAME_WF
{
    class Client
    {
        private Socket _serverSocket;
        private Thread _clientThread;
        public int PublicID = -1;

        public class PlayerStateCollection : BindingList<PlayerState>
        {
            Client Parent { get; }
            public new void Add(PlayerState playerState)
            {
                Program.InvokeFix(() =>
                {
                    base.Add(playerState);
                    Parent.NewPlayer?.Invoke(this, new EventArgsPlayer(playerState));
                });
            }
            public PlayerStateCollection(Client parent) => Parent = parent;
        }
        public Game Game { get; set; }
        public PlayerStateCollection Players { get; } 

        public Client()
        {
            Players = new PlayerStateCollection(this);
        }

        public bool Connect(IPEndPoint iPEndPoint)
        {
            bool br = false;
            try
            {
                _serverSocket = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _serverSocket.Connect(iPEndPoint);
                if(_serverSocket.Connected)
                {
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
            SendObject(new Exit());
            _serverSocket.Close(5);
        }
        public void SendColor(Color color, Color color2)
        {
            Send($"#Color#{color.R}:{color.G}:{color.B};{color2.R}:{color2.G}:{color2.B}");
        }

        public void SendObject(JsonObject jsonObject)
        {
            Send(jsonObject.ToJsonSendTCP());
        }

        public void Send(string data)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                int bytesSent = _serverSocket.Send(buffer);
            }
            catch { MessageBox.Show("Связь с сервером прервалась..."); }
        }


        private void Listner()
        {
            while (_serverSocket.Connected)
            {
                try
                {
                    byte[] buffer = new byte[8196];
                    int bytesRec = _serverSocket.Receive(buffer);
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRec);
                    HandleData(data);
                    //if (data.Contains("#updatechat"))
                    //{
                    //    //UpdateChat(data);
                    //    continue;
                    //}
                }
                catch (Exception ex)
                {
                    if (_serverSocket.Connected)
                    {
                        //MessageBox.Show(ex.Message, "Ошибка");
                        {
                            Program.InvokeFix(() =>
                            {
                                Program.ButtonActions.Add(new Action<MsgForm.FormButton>((a) =>
                                {
                                    Program.SetForm(MainForm.FormEnum.EnterServer);
                                    Program.ButtonActions.Clear();
                                }));
                                Program.SetMsg("Потеряно соединение");
                                Program.SetBtn(new List<MsgForm.FormButton> { MsgForm.FormButton.Cont });
                                Program.SetForm(MainForm.FormEnum.MsgForm);
                                _serverSocket.Disconnect(false);
                            });
                        }
                    }
                    else MessageBox.Show(ex.Message, "Потеряно соединение");
                }
            }
        }

        private void HandleObject(JsonObject jsonObject)
        {
            if(jsonObject is ID id)
            {
                if (PublicID == -1) PublicID = id.id;
            }
            else if(jsonObject is NewPlayer newPlayer)
            {
                PlayerState playerState = new PlayerState();
                playerState.publicID = newPlayer.id;
                playerState.name = newPlayer.playerName;
                Players.Add(playerState);
            }
        }

        private void HandleData(string data)
        {
            //if (!(logServer == null || logServer.IsDisposed))
            //{
            //    logServer.Visible = true;
            //    logServer.WriteLine(data);
            //}
            Program.WriteLog(data);
            HandleObject(FromJsonSendTCP(data));
            //var vs = data.Split(new char[] { '#' }, 3);
            //string command = vs[1].ToUpper();
            //string body = vs[2];
            //Type type = JsonObject.GetType(command);
            //if (type == CellColor.TypeDef)
            //{
            //    CellColor cellColor = FromJson<CellColor>(body);
            //    //Player = new GameCore.PlayerCell(cellColor.color1, cellColor.color2, Color.LightGray);
            //}
            //else if (type == Ready.TypeDef)
            //{
            //    Ready ready = FromJson<Ready>(body);
            //    //IsReady = ready.isReady;
            //}
            //else if (type == Step.TypeDef)
            //{
            //    Step turn = FromJson<Step>(body);
            //    Prefab prefab = Game.Prefabs[turn.prefabID].GetRotatedPrefab(turn.Rotate);
            //    //prefab.Ro
            //    Game.SetPrefab(prefab, turn.Point, out _, turn.player, false);
            //}
        }


        public class EventArgsPlayer : EventArgs
        {
            public PlayerState PlayerState { get; }
            public EventArgsPlayer(PlayerState playerState) => PlayerState = playerState;
        }

        public event EventHandler<EventArgsPlayer> NewPlayer;
    }
}
