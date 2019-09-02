using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Interfaces
{
    [Serializable]
    public struct Game
    {
        public int[,] grid;
        //public System.Drawing.Color[] colors;
        public PrefabLimit[] prefabs;
        public int currentPlayer;
    }
}
