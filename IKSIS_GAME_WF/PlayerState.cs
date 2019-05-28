using GameCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKSIS_GAME_WF
{
    public class PlayerState
    {
        //[System.ComponentModel.TypeConverter(typeof(Bitmap))]
        [System.ComponentModel.Browsable(false)]
        public Player playerCell { get; set; }
        [DisplayName("Цвет")]
        [System.ComponentModel.Browsable(true)]
        public Bitmap Image => playerCell == null ? new Bitmap(10,10) : playerCell.SimpleImage;

        [System.ComponentModel.Browsable(false)]
        public int publicID { get; set; }
        [DisplayName("Готов")]
        public bool ready { get; set; }
        [DisplayName("Имя")]
        public string name { get; set; }
    }
}
