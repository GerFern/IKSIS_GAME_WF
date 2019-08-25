using Gtk;
using System;
using System.Collections.Generic;
using System.Text;
using UI = Gtk.Builder.ObjectAttribute;

namespace gtk_test.Widgets
{
    class ttt : Widget
    {
        [UI]
        Button b1;
        [UI]
        Button b2;
        [UI]
        Button b3;
        [UI]
        Button b4;
        [UI]
        Label l;

        public event EventHandler<EventArgsInt> BtnClicked;

        public string LabelText { get => l.Text; set => l.Text = value; }

        public ttt() : this(new Builder("GameWindow.glade")) { }

        public ttt(Builder builder) : base(builder.GetObject("ttt").Handle)
        {
            builder.Autoconnect(this);
            b1.Clicked += new EventHandler((o, e) => BtnClicked?.Invoke(b1, new EventArgsInt(0)));
            b2.Clicked += new EventHandler((o, e) => BtnClicked?.Invoke(b2, new EventArgsInt(1)));
            b3.Clicked += new EventHandler((o, e) => BtnClicked?.Invoke(b3, new EventArgsInt(2)));
            b4.Clicked += new EventHandler((o, e) => BtnClicked?.Invoke(b4, new EventArgsInt(3)));
            //DeleteEvent += Window_DeleteEvent;
            //KeyPressEvent += GameWindow_KeyPressEvent;
            //KeyReleaseEvent += GameWindow_KeyReleaseEvent;
            ////_button1.Clicked += Button1_Clicked;
            //builder.Dispose();
            //gameWidget = new GameWidget(_gameWidget);
            //gameWidget.ScaleChanged += GameWidget_ScaleChanged;
            //_scale.ChangeValue += _scale_ChangeValue;
        }

        public class EventArgsInt:EventArgs
        {
            public EventArgsInt(int value)
            {
                Value = value;
            }

            public int Value { get; set; }

        }
    }
}
