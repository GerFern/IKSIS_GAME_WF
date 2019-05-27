using GameCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace IKSIS_GAME_WF
{
    public partial class Lobby : MyForm
    {
        public Lobby()
        {
            InitializeComponent();
            listView2.LargeImageList = new ImageList() { ImageSize = new Size(32,32)};
            listView2.LargeImageList.Images.Add(new PlayerCell(System.Drawing.Color.Red, System.Drawing.Color.IndianRed, System.Drawing.Color.White).GetImage(new Size(32, 32), 8));
            listView2.Items.Add("Red", 0);
            listView2.LargeImageList.Images.Add(new PlayerCell(System.Drawing.Color.Blue, System.Drawing.Color.Cyan, System.Drawing.Color.White).GetImage(new Size(32, 32), 8));
            listView2.Items.Add("Blue", 1);

            //listView2.Items.I
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Program.Client.Disconnect();
            Program.SetForm(MainForm.FormEnum.EnterServer);
        }
    }




}

