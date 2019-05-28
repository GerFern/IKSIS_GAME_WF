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
        public string UserName
        {
            get => textBox1.Text;
            set => textBox1.Text = value;
        }
        IPAddress _iPAddress;
        int _port;
        public IPAddress IPAddress
        {
            get => _iPAddress;
            set
            {
                _iPAddress = value;
                textBox2.Text = value.ToString();
            }
        }
        public int Port
        {
            get => _port;
            set
            {
                _port = value;
                textBox3.Text = value.ToString();
            }
        }
        public IPEndPoint IPEndPoint
        {
            get => new IPEndPoint(_iPAddress, _port);
            set
            {
                IPAddress = value.Address;
                Port = value.Port;
            }
        }
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
            _iPAddress = address;
            _port = port;
            OnExitForm(new EventArgsBtn());
            //ExitForm?.Invoke(this, new EventArgsBtn());
        }
    }
}
