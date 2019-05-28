using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IKSIS_GAME_WF
{
    public partial class MyForm : Form
    {
        public enum FormButton
        {
            None,
            Cont,
            Cancel,
        }

        //public bool AutoHide { get; set; } = true;
        public bool CanMove { get; set; } = true;
        public MyForm()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            Debug.WriteLine(m);
            if (!CanMove)
                if (m.Msg == 0x3 || m.Msg == 0x216 || m.Msg == 0x231 || m.Msg == 0xa0 || m.Msg == 0x112) return;
            base.WndProc(ref m);
        }

        protected internal void OnEnterForm(EventArgs e)
        {
            EnterForm?.Invoke(this, e);
        }

        protected internal void OnExitForm(EventArgsBtn e)
        {
            //if (AutoHide) Visible = false;
            ExitForm?.Invoke(this, e);
        }

        public class EventArgsBtn : EventArgs
        {
            public FormButton Button { get; }

            public EventArgsBtn() => Button = FormButton.None;
            public EventArgsBtn(FormButton button) => Button = button;
        }

        public event EventHandler EnterForm;
        public event EventHandler<EventArgsBtn> ExitForm;
    }
}
