using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            multiLayersZoomableControl1.Layers.Add(("l1", new GameCore.Controls.MultiLayersZoomableControl.BitmapStruct()));
            multiLayersZoomableControl1.Layers.Add(("l2", new GameCore.Controls.MultiLayersZoomableControl.BitmapStruct()));
            multiLayersZoomableControl1.Layers.Add(("l3", new GameCore.Controls.MultiLayersZoomableControl.BitmapStruct()));
            multiLayersZoomableControl1.Layers.SortLayers();
            int count = 0;
            foreach (var (_, layer) in multiLayersZoomableControl1.Layers)
            {
                layer.Bitmap = new Bitmap(400, 400);
                layer.Index = count;
                count++;
            }
            multiLayersZoomableControl1.Layers.SortLayers();
            Graphics[] g = new Graphics[3];
            g[0] = Graphics.FromImage(multiLayersZoomableControl1.Layers.SortedLayers[0].Bitmap);
            g[1] = Graphics.FromImage(multiLayersZoomableControl1.Layers.SortedLayers[1].Bitmap);
            g[2] = Graphics.FromImage(multiLayersZoomableControl1.Layers.SortedLayers[2].Bitmap);

            g[0].DrawRectangle(Pens.Black, new Rectangle(0, 0, 400, 400));
            g[1].FillRectangle(Brushes.Red, new Rectangle(100, 100, 200, 200));
            //g[1].FillRectangle(Brushes.Red, new Rectangle(100, 100, 200, 200));
            g[2].FillRectangle(Brushes.White, new Rectangle(0, 100, 250, 150));
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            multiLayersZoomableControl1.Layers[0].layer.Index = (int)numericUpDown1.Value;
            if (multiLayersZoomableControl1.Layers.SortLayers())
                multiLayersZoomableControl1.Refresh();
        }

        private void NumericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            multiLayersZoomableControl1.Layers[1].layer.Index = (int)numericUpDown2.Value;
            if (multiLayersZoomableControl1.Layers.SortLayers())
                multiLayersZoomableControl1.Refresh();
        }

        private void NumericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            multiLayersZoomableControl1.Layers[2].layer.Index = (int)numericUpDown3.Value;
            if (multiLayersZoomableControl1.Layers.SortLayers())
                multiLayersZoomableControl1.Refresh();
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            multiLayersZoomableControl1.InterpolationMode = (System.Drawing.Drawing2D.InterpolationMode)comboBox.SelectedValue;
        }
    }
}
