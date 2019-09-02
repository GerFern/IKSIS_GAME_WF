using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GameCore.Interfaces
{
    public interface ClientContainer
    {
        public Guid guid { get; set; }
        public string name { get; set; }
    }

    public interface PlayerStateLocal
    {
        public Color color { get; set; }
        public bool ready { get; set; }
    }
}
