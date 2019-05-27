using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IKSIS_GAME_WF
{
    static class Program
    {
        public static void InvokeFix(Action action) => mainForm.InvokeFix(action);
        public static Client Client { get; set; }
        public static void SetForm(MainForm.FormEnum formEnum)
        {
            mainForm.SetForm(formEnum);
        }
        public static void SetMsg(string str)
        {
            mainForm.SetMsg(str);
        }
        public static void SetBtn(List<MsgForm.MsgFormButton> buttons)
        {
            mainForm.SetBtn(buttons);
        }

        public static List<Action<MsgForm.MsgFormButton>> ButtonActions = new List<Action<MsgForm.MsgFormButton>>();

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

            Application.Run(mainForm = new MainForm());
        }
    }
}
