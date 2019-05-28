using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IKSIS_GAME_WF
{
    static class Program
    {
        public static LogServer logServer;
        public static void WriteLog(string str)
        {
            if (logServer != null && !logServer.IsDisposed)
            {
                logServer.InvokeFix(() =>
                {
                    logServer.Visible = true;
                    logServer.WriteLine(str);
                });
            }
        }
        public static void InvokeFix(Action action) => mainForm.InvokeFix(action);

        public static Client _client;
        public static Client Client
        {
            get => _client;
            set
            {
                if(_client!=null)
                {
                    _client.NewPlayer -= client_NewPlayer;
                }
                _client = value;
                value.NewPlayer += client_NewPlayer;
            }
        }

        private static void client_NewPlayer(object sender, Client.EventArgsPlayer e)
        {
            mainForm.lobbyForm.NewPlayer(e.PlayerState);
        }

        public static void SetForm(MainForm.FormEnum formEnum)
        {
            mainForm.SetForm(formEnum);
        }
        public static void SetMsg(string str)
        {
            mainForm.SetMsg(str);
        }
        public static void SetBtn(List<MsgForm.FormButton> buttons)
        {
            mainForm.SetBtn(buttons);
        }

        public static List<Action<MsgForm.FormButton>> ButtonActions = new List<Action<MsgForm.FormButton>>();

        static MainForm mainForm;
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (Properties.Settings.Default.Guid == Guid.Empty)
            {
                Properties.Settings.Default.Guid = Guid.NewGuid();
                Properties.Settings.Default.Save();
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            logServer = new LogServer();
            logServer.Show();
            Application.Run(mainForm = new MainForm());
        }
    }
}
