using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ra
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            RA3MapGenerator.Form1 form = new RA3MapGenerator.Form1();
            form.Show();
            InitializeComponent();
            ra3form.Visible = true;

            RA3Map.MapGenerator
        }
    }
}
