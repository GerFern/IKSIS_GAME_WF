using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NotifyTest
{
    class Test : INode
    {
        public Node Node { get; set; }

        public Test2 T2 = new Test2();
        public int TInt;

        public event PropertyChangedEventHandler PropertyChanged;
    }

    class Test2 : INode
    {
        private int aA;

        public int AA { get => aA; set => aA = value; }
        public Node Node { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
