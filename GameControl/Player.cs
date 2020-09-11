using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        //public Dictionary<int, int> PrefabCountAllow { get; } = new Dictionary<int, int>();

        public PrefabLimitNotifier PrefabLimitNotifier { get; internal set; }

        public IReadOnlyDictionary<int, int> PrefabCountAllow => PrefabLimitNotifier.Prefabs;

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