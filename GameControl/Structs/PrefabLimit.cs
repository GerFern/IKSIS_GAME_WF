using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GameCore.Interfaces
{
    [Serializable]
    public struct PrefabLimit
    {
        public Prefab prefab;
        public int count;

        public PrefabLimit(int count, Prefab prefab)
        {
            this.prefab = prefab;
            this.count = count;
        }

        public PrefabLimit(int count, params Point[] points)
        {
            this.prefab = new Prefab(points);
            this.count = count;
        }
    }
}
