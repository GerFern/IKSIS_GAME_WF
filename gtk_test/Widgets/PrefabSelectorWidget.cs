using Cairo;
using GameCore;
using Gdk;
using Gtk;
using System;
using System.Collections.Generic;
using System.Text;
using Color = Cairo.Color;
using Rectangle = Cairo.Rectangle;

namespace gtk_test.Widgets
{
    class PrefabSelectorWidget : Button
    {
        private Prefab prefab;
        private Color foreColor;
        private Color backgroundColor;
        public GameWidgetOver GameWidget { get; set; }
        public int Count { get; set; }
        public Prefab Prefab

        {
            get => prefab;
            set => prefab = value;
        }

        public Color ForeColor { get => foreColor; set => foreColor = value; }
        public Color BackgroundColor { get => backgroundColor; set => backgroundColor = value; }
        public int PrefabID { get; set; }

        public PrefabSelectorWidget() : base()
        {
            //this.AddEvents((int)EventMask.ButtonPressMask);
            Drawn += PrefabSelectorWidget_Drawn;
            ButtonPressEvent += PrefabSelectorWidget_ButtonPressEvent;
            ButtonReleaseEvent += PrefabSelectorWidget_ButtonReleaseEvent;
        }

        private void PrefabSelectorWidget_ButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
        {
            if (GameWidget != null)
            {
                GameWidget.SelectedPrefabID = PrefabID;
            }
        }

        private void PrefabSelectorWidget_ButtonPressEvent(object o, ButtonPressEventArgs args)
        {
        }

        private void PrefabSelectorWidget_Drawn(object o, DrawnArgs args)
        {
            if (prefab != null)
            {
                const int DSize = 30;
                Context context = args.Cr;
                context.Translate(20, 20);
                context.LineCap = LineCap.Round;
                context.LineWidth = 4;
                context.Rectangle(new Rectangle(0, 0, prefab.Size.Width * DSize, prefab.Size.Height * DSize));
                context.SetSourceColor(backgroundColor);
                context.Fill();
                context.SetSourceColor(new Color(0, 0, 0, 1));
                GameWidgetOver.DrawGrid(context, DSize, DSize, prefab.Size.Width, prefab.Size.Height);
                context.LineWidth = 6;
                foreach (var item in prefab.Points)
                {
                    GameWidgetOver.DrawCell(context, item.Point.X, item.Point.Y, DSize, DSize, 6, foreColor, GameCore.Border.None, false);
                }

                string text = Count.ToString();
                context.SelectFontFace("Sans", FontSlant.Normal, FontWeight.Normal);
                FontFace ffSans = context.GetContextFontFace();
                Matrix fm = new Matrix(/*font size*/ 30.0, 0.0, 0.0, /*font size*/ 30.0,
                                                /*translationX*/ 0.0, /*translationY*/ 0.0);
                Matrix tm = new Matrix(1, 0.0, 0.0, 1.0, 0.0, 0.0);
                FontOptions fo = new FontOptions();
                ScaledFont sfSans = new ScaledFont(ffSans, fm, tm, fo);
                context.SetScaledFont(sfSans);

                var te = context.TextExtents(text);
                var size = this.Allocation.Size;
                double x = size.Width - te.Width - 40;
                double y = size.Height - te.Height - 10;
                context.SetSourceColor(new Color(0.4, 0.8, 0.6, 1));
                context.MoveTo(x, y);
                context.ShowText(text);

            }
        }


    }
}
