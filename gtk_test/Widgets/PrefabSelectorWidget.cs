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

        public Prefab Prefab
        {
            get => prefab;
            set
            {
                prefab = value;
            }
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
            if (Prefab != null)
            {
                Context context = args.Cr;
                context.Translate(20, 20);
                context.LineCap = LineCap.Round;
                context.LineWidth = 4;
                context.Rectangle(new Rectangle(0, 0, Prefab.Size.Width * 50, prefab.Size.Height * 50));
                context.Color = backgroundColor;
                context.Fill();
                context.Color = new Color(0, 0, 0, 1);
                GameWidgetOver.DrawGrid(context, 50, 50, Prefab.Size.Width, Prefab.Size.Height);
                context.LineWidth = 6;
                foreach (var item in Prefab.Points)
                {
                    GameWidgetOver.DrawCell(context, item.Point.X, item.Point.Y, 50, 50, 10, foreColor, GameCore.Border.None, false);
                }
            }
        }


    }
}
