//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace GameServer.ClientServer
//{
//    public class SubAsset : MyDictEvent<short, SubAsset>, IAsset<short>
//    {
//        protected string name;
//        protected short idAsset;
//        protected SubAsset.SubAssetType subAssetType;
//        protected Type type;
//        public static string[] assetNames;
//        private SubAsset parent;

//        public SubAsset Parent { get => parent; set => parent = value; }
//        //public Dictionary<short, SubAsset> Childs { get; } = new Dictionary<short, SubAsset>();
//        public string Name
//        {
//            get
//            {
//                return this.name;
//            }
//        }

//        public short ID
//        {
//            get
//            {
//                return this.idAsset;
//            }
//        }

//        public SubAsset()
//        {
//            base.AddingNew += SubAsset_AddingNew;
//            base.EditExist += SubAsset_EditExist;
//            base.Removed += SubAsset_Removed;
//            base.Cleared += SubAsset_Cleared;
//            base.BeforeClear += SubAsset_BeforeClear;
//        }

//        private void SubAsset_BeforeClear(object sender, EventArgs e)
//        {
//            foreach (var item in this)
//            {
//                item.Value.parent = null;
//            }
//        }

//        private void SubAsset_Cleared(object sender, EventArgs e)
//        {
//            throw new NotImplementedException();
//        }

//        private void SubAsset_Removed(object sender, Remove<short, SubAsset> e)
//        {
//            e.Value.parent = null;
//        }

//        private void SubAsset_EditExist(object sender, Edit<short, SubAsset> e)
//        {
//            //throw new NotImplementedException();
//            e.OldValue.parent = null;
//            e.NewValue.parent = this;
//        }

//        private void SubAsset_AddingNew(object sender, AddNew<short, SubAsset> e)
//        {
//            e.Value.idAsset = e.Key;
//            e.Value.parent = this;
//        }

//        //public abstract void Clear();

//        public static string PeekSubAssetName(BinaryReader br)
//        {
//            int num = br.ReadInt32();
//            br.BaseStream.Position -= 4L;
//            return SubAsset.assetNames[(num >> 8) - 1];
//        }

//        public bool IsEmpty()
//        {
//            return string.IsNullOrEmpty(this.name);
//        }

//        public void Register(AssetContainer assetContainer)
//        {
//            if (this.IsEmpty())
//                return;
//            if (assetContainer.nameIDs.ContainsKey(this.name))
//                this.idAsset = assetContainer.nameIDs[this.name];
//            else
//                assetContainer.nameIDs.Add(this.name, assetContainer.nameIDs.Count + 1);
//        }

//        protected SubAsset.SubAssetType GetType(Type t)
//        {
//            if (t == typeof(bool))
//                return SubAsset.SubAssetType.boolType;
//            if (t == typeof(int))
//                return SubAsset.SubAssetType.intType;
//            if (t == typeof(float))
//                return SubAsset.SubAssetType.floatType;
//            if (t == typeof(string))
//                return this.name == "playerDisplayName" ? SubAsset.SubAssetType.stringUnicodeType : SubAsset.SubAssetType.stringType;
//            if (t == typeof(string[]))
//                return SubAsset.SubAssetType.stringNameValueType;
//            return t.BaseType == typeof(Enum) ? SubAsset.SubAssetType.intType : SubAsset.SubAssetType.unknownType;
//        }

//        public abstract void Save(BinaryWriter bw);

//        public abstract void Default();

//        public enum SubAssetType : byte
//        {
//            boolType = 0,
//            intType = 1,
//            floatType = 2,
//            stringType = 3,
//            stringUnicodeType = 4,
//            stringNameValueType = 5,
//            unknownType = 255, // 0xFF
//        }
//    }

//}
