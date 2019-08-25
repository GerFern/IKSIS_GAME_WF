using GameCore;
using GameServer.ClientObject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gtk_test
{
    public class PlayerState : ISyncClientServerObjects
    {
        private Color color;
        private int index;
        private bool ready;

        public PlayerState(int publicID, string name)
        {
            PublicID = publicID;
            Name = name;
        }

        //[System.ComponentModel.TypeConverter(typeof(Bitmap))]
        [System.ComponentModel.Browsable(false)]
        public Color Color { get => color; set { color = value; ColorChanged?.Invoke(this, EventArgs.Empty); } }
        [DisplayName("Цвет")]
        //[System.ComponentModel.Browsable(true)]
        //public Bitmap Image => playerCell == null ? new Bitmap(10,10) : playerCell.SimpleImage;

        [System.ComponentModel.Browsable(false)]
        public int PublicID { get; }
        [DisplayName("Готов")]
        public bool Ready
        {
            get => ready;
            set
            {
                var oldvalue = ready;
                ready = value;
                if (oldvalue != value)
                    ReadyChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        [DisplayName("Имя")]
        public string Name { get; }
        public int Index
        {
            get => index;
            set
            {
                var oldvalue = index;
                index = value;
                if (oldvalue != value)
                    IndexChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool IsSync { get; set; }

        public event EventHandler IndexChanged;
        //public event EventHandler NameChanged;
        public event EventHandler ReadyChanged;
        public event EventHandler ColorChanged;

        public JsonObject GetJsonObject()
        {
            throw new NotImplementedException();
        }

        public void SendUpdate()
        {
            throw new NotImplementedException();
        }

        public void RecieveUpdate()
        {
            throw new NotImplementedException();
        }

        public void UseJsonObject(JsonObject jsonObject)
        {
            throw new NotImplementedException();
        }
    }
}
