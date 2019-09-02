using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace GameCore.Interfaces
{
    [Serializable]
    public class PlayerState : INotifyPropertyChanged
    {
        public Color Color { get; set; }
        public bool Ready { get; set; }
        public PrefabLimit[] Prefabs { get; set; }
        public string Name { get; set; }
        public int Index { get; set; }
        public int ID { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
