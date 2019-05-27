using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing.Design;
using System.Net.Sockets;
using System.IO;
using GameCore;
using System.Threading;
using System.Net;
using GameServer.ClientObject;

namespace IKSIS_GAME_WF
{
    public partial class Form1 : Form
    {

        public static Game Game { get; set; }

        public Form1()
        {
            InitializeComponent();
            panel1.BackColor = colorDialog1.Color;
            //Color t = Color.AliceBlue;
            //dynamic d = t.GetType().GetCustomAttributes();
            //Type type1 = Type.GetType(d[3].EditorBaseTypeName);
            //Type type2 = Type.GetType(d[3].EditorTypeName);
            //dynamic c1 = Activator.CreateInstance(type1);
            //dynamic c2 = Activator.CreateInstance(type2);

            
        }
        PlayerClient PlayerClient { get; } = new PlayerClient();
        Socket Socket => PlayerClient.Socket;
        private void Button1_Click(object sender, EventArgs e)
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(textBox1.Text), int.Parse(textBox2.Text));
            Client Client;
            Client = new Client();
            Client.Connect(iPEndPoint);
            Program.Client = Client;
        }

        /// <summary>
        /// Connect
        /// </summary>
        private void Button2_Click(object sender, EventArgs e)
        {
            //IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(textBox1.Text), int.Parse(textBox2.Text));
            //Client = new Client();
            //Client.Connect(iPEndPoint);
            //Client = new Client();
            //Socket.Connect(textBox1.Text, Int32.Parse(textBox2.Text));
            //Thread thread = new Thread(new ThreadStart(() => { while (true) { PlayerClient.ReceivePacket(); } }));
            //thread.Start();
        }

        private void Panel1_Click(object sender, EventArgs e)
        {
            if(colorDialog1.ShowDialog()==DialogResult.OK)
            {
                panel1.BackColor = colorDialog1.Color;
            }
        }
        private void Panel2_Click(object sender, EventArgs e)
        {
            if (colorDialog2.ShowDialog() == DialogResult.OK)
            {
                panel2.BackColor = colorDialog2.Color;
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Program.Client.SendObject(new CellColor { color1 = panel1.BackColor, color2 = panel2.BackColor });
            //Client.SendColor(panel1.BackColor, panel2.BackColor);
            //PlayerClient.SendPacket(GameCode.SetCellColor, colorDialog1.Color);
            //PlayerClient.SendPacket(GameCode.SetBorderColor, colorDialog1.Color);
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            Program.Client.SendObject(new Ready { isReady = checkBox1.Checked });
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            Program.Client.SendObject(new PlayerCMD() { player = 3, cmd = new Ready() { isReady = true } });
        }
    }

    public class PlayerClient
    {
        GameControl control;
        public Socket Socket { get; } = new Socket(SocketType.Stream, ProtocolType.Tcp);
        MemoryStream ms;
        BinaryWriter bw;
        BinaryReader br;
        public PlayerClient()
        {
            ms = new MemoryStream(new byte[256], 0, 256, true, true);
            bw = new BinaryWriter(ms);
        }

        public void Send_MakeStep(Prefab prefab)
        {
            byte[][] vs = new byte[prefab.Points.Length][];
            for (int i = 0; i < vs.Length; i++)
            {
                byte[] t = vs[i] = new byte[2];
                Point p = prefab.Points[i].Point;
                t[0] = (byte)p.X;
                t[1] = (byte)p.Y;
            }
            SendPacket(GameCode.MakeStep, vs);
        }
        void SendPacket()
        {
            Socket.Send(ms.GetBuffer());
        }
        public void SendPacket(GameCode code, Prefab prefab)
        {
            bw.WriteObject(code, prefab);
            Socket.Send(ms.GetBuffer());
        }

        public void SendPacket(GameCode code, Color color)
        {
            bw.WriteObject(code, color);
            Socket.Send(ms.GetBuffer());
        }

        public void SendPacket(GameCode code, params byte[][] data)
        {
            bw.Write((byte)code);
            bw.Write((short)data.Sum(a=>a.Length));
            foreach (var item in data)
            {
                bw.Write(item);
            }
            Socket.Send(ms.GetBuffer());
        }

        public object ReceivePacket()
        {
            Socket.Receive(ms.GetBuffer());
            GameCode code = (GameCode)br.ReadByte();
            short length = br.ReadInt16();
            byte[] data = br.ReadBytes(length);
            //|1-code|2-length|length-data|
            object ret = null;
            
            switch (code)
            {
                case GameCode.ID:
                    break;
                default:
                    break;
            }
            return ret;
        }
    }

}
