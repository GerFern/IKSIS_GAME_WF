using System.Collections.Generic;
using System.Drawing;

namespace GameCore
{
    public class Player
    {
        //public PlayerCellBase PlayerCell { get; set; }
        public Color PlayerCell { get; set; }
        /// <summary>
        /// Key - prefabID, Value - CountAllow (-1 is infinity)
        /// </summary>
        public Dictionary<int, int> PrefabCountAllow { get; } = new Dictionary<int, int>();

        //public Player(PlayerCellBase playerCell)
        //{
        //    PlayerCell = playerCell;
        //}
        public Player(Color playerCell)
        {
            PlayerCell = playerCell;
        }

    }
}