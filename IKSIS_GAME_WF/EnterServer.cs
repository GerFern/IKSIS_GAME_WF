using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IKSIS_GAME_WF
{
    public partial class EnterServer : MyForm
    {
        public string PlayerName => textBox1.Text;
        public IPEndPoint IPEndPoint { get; private set; }
        public event EventHandler Input;
        public EnterServer()
        {
            InitializeComponent();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (!IPAddress.TryParse(textBox2.Text, out IPAddress address))
            {
                MessageBox.Show("Неверный IP адрес");
                return;
            }
            if (!Int32.TryParse(textBox3.Text, out int port))
            {
                MessageBox.Show("Неверный порт");
                return;
            }
            if (port < 0 || port > ushort.MaxValue)
            {
                MessageBox.Show("Неверный порт");
                return;
            }
            IPEndPoint = new IPEndPoint(address, port);
            Input?.Invoke(this, EventArgs.Empty);
        }
    }
}
