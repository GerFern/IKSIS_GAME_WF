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

        public void SetBtn(List<MsgForm.MsgFormButton> buttons)
        {
            msgForm.Buttons = buttons;
        }

        EnterServer enterServer = new EnterServer();
        Lobby lobbyForm = new Lobby();
        MsgForm msgForm = new MsgForm();
        IPEndPoint IPEndPoint;
        MyForm visibleForm;
        Thread tryConnect;
        Graphics g;
        Bitmap bitmap;
        public MainForm()
        {
            InitializeComponent();
            msgForm.ButtonClick += new EventHandler<MsgForm.EventArgsBtn>((o, e) =>
            {
                foreach (var item in new List<Action<MsgForm.MsgFormButton>>(Program.ButtonActions))
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
            enterServer.Input += new EventHandler((o, eh) =>
            {
                IPEndPoint = enterServer.IPEndPoint;
                msgForm.Message = "Ожидание...";
                msgForm.Buttons = new List<MsgForm.MsgFormButton> { MsgForm.MsgFormButton.Cancel };
                msgForm.ButtonClick += LoadForm_ConectionCancel;
                SetForm(msgForm, DockStyle.None, AnchorStyles.None);
                //Thread.Sleep(150);
                tryConnect = new Thread(TryConnect) { Name = "TryConnect" };
                tryConnect.Start();
                //SetForm(enterServer, DockStyle.None, AnchorStyles.None);
            });
            SetForm(enterServer, DockStyle.None, AnchorStyles.None);

            //lobbyForm = new Lobby();
            //SetForm(lobbyForm, DockStyle.None, AnchorStyles.None);
        }

       

        private void TryConnect()
        {
            Client Client = new Client();
            if (Client.Connect(IPEndPoint))
            {
                Program.Client = Client;
                Client.SendObject(new GameServer.ClientObject.PlayerGUID { guid = Properties.Settings.Default.Guid });
                Client.SendObject(new GameServer.ClientObject.Name() { playerName = enterServer.PlayerName });
                this.InvokeFix(() => SetForm(lobbyForm, DockStyle.None, AnchorStyles.None));
            }
            else
            {
                this.InvokeFix(() =>
                {
                    msgForm.ButtonClick -= LoadForm_ConectionCancel;
                    msgForm.Message = "Сервер недоступен!";
                    msgForm.Buttons = new List<MsgForm.MsgFormButton> { MsgForm.MsgFormButton.Cont };
                    msgForm.ButtonClick += LoadForm_ErroroContinue;
                });
            }
        }
        private void LoadForm_ConectionCancel(object sender, MsgForm.EventArgsBtn e)
        {
            tryConnect.Abort();
            LoadForm_ErroroContinue(sender, e);
            //loadForm.ButtonClick -= LoadForm_ConectionCancel;
            //loadForm.ButtonClick -= LoadForm_ErroroContinue;
            //SetForm(enterServer, DockStyle.None, AnchorStyles.None);
        }

        private void LoadForm_ErroroContinue(object sender, MsgForm.EventArgsBtn e)
        {
            msgForm.ButtonClick -= LoadForm_ConectionCancel;
            msgForm.ButtonClick -= LoadForm_ErroroContinue;
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
        }

    }
}
