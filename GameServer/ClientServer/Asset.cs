//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;

//namespace GameServer.ClientServer
//{
//    public abstract class Asset : IAsset
//    {
//        protected int id;
//        //protected short header;

//        public string Name
//        {
//            get
//            {
//                return this.GetType().Name;
//            }
//        }

//        public int ID
//        {
//            get
//            {
//                return this.id;
//            }
//            set
//            {
//                this.id = value;
//            }
//        }

//        protected Asset()
//        {
//            this.id = -1;
//            Asset.assetContainer.Register((IAsset)this);
//        }

//        protected Asset(BinaryReader br)
//        {
//            this.id = br.ReadInt32();
//            this.header = br.ReadInt16();
//            Asset.assetContainer.Register((IAsset)this);
//        }

//        public virtual void Save(BinaryWriter bw)
//        {
//            bw.Write(this.id);
//            bw.Write(this.header);
//            this.SaveData(bw);
//        }

//        protected virtual void RegisterSelf(AssetContainer assetContainer)
//        {
//            if (assetContainer.nameIDs.ContainsKey(this.GetType().Name))
//                this.id = assetContainer.nameIDs[this.GetType().Name];
//            else
//                assetContainer.nameIDs.Add(this.GetType().Name, assetContainer.nameIDs.Count + 1);
//        }

//        public abstract void Register(AssetContainer assetContainer);

//        protected abstract void SaveData(BinaryWriter bw);

//        public abstract void Default();

//        public virtual int CalculateDataSize()
//        {
//            return 0;
//        }
//    }




//    public interface IAsset<TID>
//    {
//        string Name { get; }

//        TID ID { get; }

//        void Default();

//        void Save(BinaryWriter bw);
//    }

//}
