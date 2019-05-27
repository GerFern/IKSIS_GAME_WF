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
        public bool CanMove { get; set; } = true;
        public MyForm()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            Debug.WriteLine(m);
            if(!CanMove)
            if (m.Msg == 0x3||m.Msg == 0x216||m.Msg == 0x231||m.Msg == 0xa0||m.Msg == 0x112) return;
            base.WndProc(ref m);
        }
    }
}
