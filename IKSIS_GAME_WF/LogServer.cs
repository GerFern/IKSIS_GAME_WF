using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IKSIS_GAME_WF
{
    public partial class LogServer : Form
    {
        public LogServer()
        {
            InitializeComponent();
        }

        public void WriteLine(string str)
        {
            textBox1.AppendText(str + Environment.NewLine);
        }
    }
}
