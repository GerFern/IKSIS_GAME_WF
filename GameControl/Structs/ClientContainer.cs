using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GameCore.Interfaces
{
    public interface ClientContainer
    {
        Guid guid { get; set; }
        string name { get; set; }
    }

    public interface PlayerStateLocal
    {
        Color color { get; set; }
        bool ready { get; set; }
    }
}
