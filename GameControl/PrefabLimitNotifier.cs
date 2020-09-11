using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace GameCore
{
    public class PrefabLimitNotifier 
    {
        public ReadOnlyDictionary<int, int> Prefabs { get; }
        private IDictionary<int, int> prefabs;
        
        internal void Change(int key, int value)
        {
            prefabs[key] = value;
            CountChanged?.Invoke(this, (key, value));
        }

        internal PrefabLimitNotifier(IDictionary<int, int> prefabs)
        {
            this.prefabs = prefabs;
            Prefabs = new ReadOnlyDictionary<int, int>(prefabs);
        }

        public event EventHandler<(int key, int count)> CountChanged;
    }
}
