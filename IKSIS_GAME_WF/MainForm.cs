using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static IKSIS_GAME_WF.MyForm;

namespace IKSIS_GAME_WF
{
    public partial class MainForm : Form
    {
        public enum FormEnum
        {
            EnterServer,
            Lobby,
            MsgForm,
            GameForm
        }

        public void SetMsg(string str)
        {
            msgForm.Message = str;
        }

        public void SetBtn(List<MsgForm.FormButton> buttons)
        {
            msgForm.Buttons = buttons;
        }

        EnterServer enterServer = new EnterServer();
        public Lobby lobbyForm = new Lobby();
        MsgForm msgForm = new MsgForm();
        IPEndPoint IPEndPoint;
        MyForm visibleForm;
        Thread tryConnect;
        Graphics g;
        Bitmap bitmap;
        public MainForm()
        {
            InitializeComponent();
            msgForm.ExitForm += new EventHandler<EventArgsBtn>((o, e) =>
            {
                foreach (var item in new List<Action<FormButton>>(Program.ButtonActions))
                {
                    item.Invoke(e.Button);
                }
            });
            bitmap = new Bitmap(1000, 1000);
            g = Graphics.FromImage(bitmap);
            using (LinearGradientBrush brush = new LinearGradientBrush(Point.Empty, new PointF(0, 1000), Color.FromArgb(10,10,10), BackColor))
            {
                g.FillRectangle(brush, new Rectangle(Point.Empty, new Size(1000,1000)));
            }
            BackgroundImage = bitmap;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //using (LinearGradientBrush brush = new LinearGradientBrush(Point.Empty, new PointF(0, Height), ForeColor, BackColor))
            //{
            //    e.Graphics.FillRectangle(brush, ClientRectangle);
            //}
            base.OnPaint(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            //Refresh();
            base.OnSizeChanged(e);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //GameServer.ClientObject.ArrCMD arr = new GameServer.ClientObject.ArrCMD();
            //arr.vs = new List<GameServer.ClientObject.JsonObject>();
            //arr.vs.Add(new GameServer.ClientObject.Ready { isReady = false });
            //arr.vs.Add(new GameServer.ClientObject.Name { playerName = "qwerty" });
            //string t = arr.ToJsonSendTCP();
            //GameServer.ClientObject.ArrCMD arr1 = (GameServer.ClientObject.ArrCMD)GameServer.ClientObject.JsonObject.FromJsonSendTCP(t);
            msgForm.ButtonClick += new EventHandler<EventArgsBtn>((o, be) =>
            {
                foreach (var item in Program.ButtonActions.ToList())
                {
                    item.Invoke(be.Button);
                }
            });
            msgForm.ExitForm += new EventHandler<EventArgsBtn>((o, be) => Program.ButtonActions.Clear());
            enterServer.ExitForm += new EventHandler<EventArgsBtn>((o, eh) =>
            {
                ///Попытка войти в сервер
                IPEndPoint = enterServer.IPEndPoint;
                msgForm.Message = "Ожидание...";
                msgForm.Buttons = new List<MsgForm.FormButton> { MsgForm.FormButton.Cancel };
                Program.ButtonActions.Add((be) =>
                {
                    tryConnect.Abort();
                    SetForm(enterServer, DockStyle.None, AnchorStyles.None);
                });
                SetForm(msgForm, DockStyle.None, AnchorStyles.None);
                //Thread.Sleep(150);
                tryConnect = new Thread(TryConnect) { Name = "TryConnect" };
                tryConnect.Start();
                //SetForm(enterServer, DockStyle.None, AnchorStyles.None);
            });
            lobbyForm.ExitForm += new EventHandler<EventArgsBtn>((o, eh) =>
            {
                if (eh.Button == FormButton.Cont) ;
                else
                {
                    Program.Client?.Disconnect();
                    Program.Client = null;
                    SetForm(FormEnum.EnterServer);
                }
            });
            enterServer.UserName = Properties.Settings.Default.UserName;
            if (IPAddress.TryParse(Properties.Settings.Default.IPAdress, out IPAddress address))
                enterServer.IPAddress = address;
            int port = Properties.Settings.Default.Port;
            if(port>0&&port<short.MaxValue)
                enterServer.Port = port;
            SetForm(enterServer, DockStyle.None, AnchorStyles.None);

            //lobbyForm = new Lobby();
            //SetForm(lobbyForm, DockStyle.None, AnchorStyles.None);
        }

       

        /// <summary>
        /// Попытка соединиться с сервером
        /// </summary>
        private void TryConnect()
        {
            Client Client = new Client();
            if (Client.Connect(IPEndPoint))
            {
                var set = (Properties.Settings)System.Configuration.SettingsBase.Synchronized(Properties.Settings.Default);
                set.IPAdress = IPEndPoint.Address.ToString();
                set.Port = (short)IPEndPoint.Port;
                set.UserName = enterServer.UserName;
                set.Save();
                Program.Client = Client;
                Client.SendObject(new GameServer.ClientObject.LogIn()
                {
                    name = enterServer.UserName,
                    guid = Properties.Settings.Default.Guid
                });
                //Client.SendObject(new GameServer.ClientObject.PlayerGUID { guid = Properties.Settings.Default.Guid });
                //Client.SendObject(new GameServer.ClientObject.Name() { playerName = enterServer.UserName });
                this.InvokeFix(() =>
                {
                    lobbyForm.playerStateCollectionBindingSource.DataSource = Program.Client.Players;
                    SetForm(lobbyForm, DockStyle.None, AnchorStyles.None);
                });
            }
            else
            {
                this.InvokeFix(() =>
                {
                    //msgForm.ButtonClick -= LoadForm_ConectionCancel;
                    msgForm.Message = "Сервер недоступен!";
                    msgForm.Buttons = new List<MsgForm.FormButton> { MsgForm.FormButton.Cont };
                    //msgForm.ButtonClick += LoadForm_ErroroContinue;
                });
            }
        }
        private void LoadForm_ConectionCancel(object sender, MsgForm.EventArgsBtn e)
        {
            tryConnect.Abort();
            //LoadForm_ErroroContinue(sender, e);
            //loadForm.ButtonClick -= LoadForm_ConectionCancel;
            //loadForm.ButtonClick -= LoadForm_ErroroContinue;
            //SetForm(enterServer, DockStyle.None, AnchorStyles.None);
        }

        private void LoadForm_ErroroContinue(object sender, MsgForm.EventArgsBtn e)
        {
            //msgForm.ButtonClick -= LoadForm_ConectionCancel;
            //msgForm.ButtonClick -= LoadForm_ErroroContinue;
            SetForm(enterServer, DockStyle.None, AnchorStyles.None);
        }

        public void SetForm(FormEnum formEnum)
        {
            switch (formEnum)
            {
                case FormEnum.EnterServer:
                    SetForm(enterServer);
                    break;
                case FormEnum.Lobby:
                    SetForm(lobbyForm);
                    break;
                case FormEnum.MsgForm:
                    SetForm(msgForm);
                    break;
                case FormEnum.GameForm:
                    //SetForm();
                    break;
            }
        }

        public void SetForm(MyForm form, DockStyle dockStyle = DockStyle.None, AnchorStyles anchorStyles = AnchorStyles.None)
        {
            if (visibleForm != null)
                visibleForm.Visible = false;
            visibleForm = form;
            form.TopLevel = false;
            form.StartPosition = FormStartPosition.CenterParent;
            form.Dock = dockStyle;
            form.Anchor = anchorStyles;
            form.Parent = this;
            form.Visible = true;
            Rectangle mainRect = this.DisplayRectangle;
            Rectangle formRect = form.Bounds;
            formRect.X = (mainRect.Width / 2 - formRect.Width / 2);
            formRect.Y = (mainRect.Height / 2 - formRect.Height / 2);
            form.Location = formRect.Location;
            //form.Load += new EventHandler((o, e) => form.CanMove = false);
            form.CanMove = false;
            form.OnEnterForm(EventArgs.Empty);
        }

    }
}
