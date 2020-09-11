using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace gtk_test.Widgets
{
    class MainWidget : Widget
    {
        //[UI]
        //private Label _label1 = null;
        //[UI]
        //private Button _button1 = null;

        private int _counter;
        [UI] public Box _mvBox;
        /// <summary> Play Offline </summary>
        [UI] public Button _mvButton1;
        /// <summary> Connect Server </summary>
        [UI] public Button _mvButton2;
        /// <summary> Create Server </summary>
        [UI] public Button _mvButton3;
        /// <summary> Settings </summary>
        [UI] public Button _mvButton4;
        /// <summary> Quit </summary>
        [UI] public Button _mvButton5;

        //public event EventHandler ButtonPlay;
        public event EventHandler ButtonConnect;
        public event EventHandler ButtonCreate;
        public event EventHandler ButtonSettings;
        public event EventHandler ButtonQuit;

        public MainWidget() : this(new Builder("GameWindow.glade")) { }

        private MainWidget(Builder builder) : base(builder.GetObject(typeof(MainWidget).Name).Handle)
        {
            builder.Autoconnect(this);
            //_mvButton1.Clicked += new EventHandler((o, e) => ButtonPlay?.Invoke(this, e));
            _mvButton2.Clicked += new EventHandler((o, e) => ButtonConnect?.Invoke(this, e));
            _mvButton3.Clicked += new EventHandler((o, e) => ButtonCreate?.Invoke(this, e));
            _mvButton4.Clicked += new EventHandler((o, e) => ButtonSettings?.Invoke(this, e));
            _mvButton5.Clicked += new EventHandler((o, e) => ButtonQuit?.Invoke(this, e));
        }


        //private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        //{
        //    Application.Quit();
        //}

        //private void Button1_Clicked(object sender, EventArgs a)
        //{
        //    _counter++;
        //    _label1.Text = "Hello World! This button has been clicked " + _counter + " time(s).";
        //}
    }
}