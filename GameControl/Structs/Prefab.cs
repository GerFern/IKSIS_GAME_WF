using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GameCore.Interfaces
{
    [Serializable]
    public struct Prefab
    {
        public System.Drawing.Point[] points;
        public Prefab(params Point[] points)
        {
            this.points = points;
        }
    }
}
