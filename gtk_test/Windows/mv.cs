using Gtk;
using System;
using System.Collections.Generic;
using System.Text;
using UI = Gtk.Builder.ObjectAttribute;

namespace gtk_test.Windows
{
    
    class mw:Window
    {
        [UI]
        public Stack stack;
        [UI]
        public StackSwitcher stacksw;
        public mw() : this(new Builder("GameWindow.glade")) { }
        private mw(Builder builder) : base(builder.GetObject("mw").Handle)
        {
            builder.Autoconnect(this);
           
        }
    }
}
