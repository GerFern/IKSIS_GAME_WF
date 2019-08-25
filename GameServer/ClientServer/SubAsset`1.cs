//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace GameServer.ClientServer
//{
//    public class SubAsset<T> : SubAsset
//    {
//        public T data;

//        public SubAsset(string name, T data, Dictionary<string, SubAsset> owner)
//        {
//            this.data = data;
//            this.name = name;
//            this.idAsset = 0;
//            this.type = typeof(T);
//            if (this.type == typeof(bool))
//                this.subAssetType = SubAsset.SubAssetType.boolType;
//            else if (this.type == typeof(int))
//                this.subAssetType = SubAsset.SubAssetType.intType;
//            else if (this.type == typeof(float))
//                this.subAssetType = SubAsset.SubAssetType.floatType;
//            else if (this.type == typeof(string))
//            {
//                if (name == "playerDisplayName")
//                    this.subAssetType = SubAsset.SubAssetType.stringUnicodeType;
//                else
//                    this.subAssetType = SubAsset.SubAssetType.stringType;
//            }
//            else if (this.type == typeof(string[]))
//            {
//                this.subAssetType = SubAsset.SubAssetType.stringNameValueType;
//            }
//            else
//            {
//                if (this.type.BaseType != typeof(Enum))
//                    throw new Exception("unknown sub-asset type: " + this.type.Name);
//                this.subAssetType = SubAsset.SubAssetType.intType;
//            }
//            SubAsset.assetContainer.Register((IAsset)this);
//            owner.Add(name, (SubAsset)this);
//        }

//        public SubAsset(BinaryReader br, string name, Dictionary<string, SubAsset> owner)
//        {
//            this.type = typeof(T);
//            this.name = name;
//            this.data = default(T);
//            this.subAssetType = SubAsset.SubAssetType.unknownType;
//            this.idAsset = -1;
//            if (br == null)
//                return;
//            this.idAsset = br.ReadInt32();
//            this.subAssetType = (SubAsset.SubAssetType)(this.idAsset & (int)byte.MaxValue);
//            this.idAsset >>= 8;
//            switch (this.subAssetType)
//            {
//                case SubAsset.SubAssetType.boolType:
//                    this.data = (T)Convert.ChangeType((object)br.ReadBoolean(), this.type);
//                    break;
//                case SubAsset.SubAssetType.intType:
//                    this.data = (T)(object)br.ReadInt32();
//                    break;
//                case SubAsset.SubAssetType.floatType:
//                    this.data = (T)Convert.ChangeType((object)br.ReadSingle(), this.type);
//                    break;
//                case SubAsset.SubAssetType.stringType:
//                    this.data = (T)Convert.ChangeType((object)IOUtility.ReadString(br), this.type);
//                    break;
//                case SubAsset.SubAssetType.stringUnicodeType:
//                    this.data = (T)Convert.ChangeType((object)IOUtility.ReadStringUnicode(br), this.type);
//                    break;
//                case SubAsset.SubAssetType.stringNameValueType:
//                    this.data = (T)Convert.ChangeType((object)IOUtility.ReadString(br), this.type);
//                    break;
//                default:
//                    throw new Exception("unknown sub-asset type: " + (object)this.subAssetType);
//            }
//            if (this.subAssetType != this.GetType(this.type) && (this.type != typeof(string) || this.subAssetType != SubAsset.SubAssetType.stringType && this.subAssetType != SubAsset.SubAssetType.stringUnicodeType && this.subAssetType != SubAsset.SubAssetType.stringNameValueType))
//                TextboxConsole.WriteLine("!\t Sub-asset: {2} got wrong type, should be {0}, instead of {1}", (object)this.subAssetType, (object)this.GetType(this.type), (object)name);
//            if (name != SubAsset.assetNames[this.idAsset - 1])
//                TextboxConsole.WriteLine("!\t Sub-asset got wrong name, should be {0}, instead of {1}", (object)SubAsset.assetNames[this.idAsset - 1], (object)name);
//            SubAsset.assetContainer.Register((IAsset)this);
//            owner.Add(name, (SubAsset)this);
//        }

//        public override void Save(BinaryWriter bw)
//        {
//            if (this.IsEmpty() | this.idAsset < 1)
//                return;
//            bw.Write((int)(this.subAssetType + (byte)(this.idAsset << 8)));
//            switch (this.subAssetType)
//            {
//                case SubAsset.SubAssetType.boolType:
//                    bw.Write((bool)Convert.ChangeType((object)this.data, typeof(bool)));
//                    break;
//                case SubAsset.SubAssetType.intType:
//                    bw.Write((int)Convert.ChangeType((object)this.data, typeof(int)));
//                    break;
//                case SubAsset.SubAssetType.floatType:
//                    bw.Write((float)Convert.ChangeType((object)this.data, typeof(float)));
//                    break;
//                case SubAsset.SubAssetType.stringType:
//                    IOUtility.WriteString(bw, (string)Convert.ChangeType((object)this.data, typeof(string)));
//                    break;
//                case SubAsset.SubAssetType.stringUnicodeType:
//                    IOUtility.WriteStringUnicode(bw, (string)Convert.ChangeType((object)this.data, typeof(string)));
//                    break;
//                case SubAsset.SubAssetType.stringNameValueType:
//                    IOUtility.WriteString(bw, (string)(object)this.data);
//                    break;
//                default:
//                    throw new Exception("unknown sub-asset type: " + (object)this.subAssetType);
//            }
//        }

//        public override string ToString()
//        {
//            if (this.name == null)
//                return "";
//            return this.data.ToString();
//        }

//        public override void Default()
//        {
//            this.name = (string)null;
//            this.data = default(T);
//            this.idAsset = -1;
//            this.subAssetType = SubAsset.SubAssetType.unknownType;
//        }

//        public static implicit operator T(SubAsset<T> val)
//        {
//            return val.data;
//        }

//        public override void Clear()
//        {
//            this.name = (string)null;
//            this.idAsset = -1;
//            this.data = default(T);
//            this.subAssetType = SubAsset.SubAssetType.unknownType;
//        }
//    }

//}
