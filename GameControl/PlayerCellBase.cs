#define test

using System.Drawing;

namespace GameCore
{
    public abstract class PlayerCellBase
    {
        public virtual Color CellColor { get; set; }
        public virtual Color BorderColor { get; set; }
        //public virtual Color InnerCellColor { get; set; }
        public virtual Color SelectedCellColor { get; set; }
      
    }
}

