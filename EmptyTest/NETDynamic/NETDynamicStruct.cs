using EmptyTest.TStreamHandler;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

namespace EmptyTest.TNode
{
    public class NETDynamicStruct : DynamicNode
    {
        
        protected int __structID;
        public int __StructID => __structID;
        public override bool __HasChild => true;

        StructNodeChild __Child;
        IEnumerable<DynamicNode> __ChildNodeGet
        {
            get
            {
                return this.__Child.Values.Select(a => a.Node);
                //Dictionary<int, DynamicNode> storage;
                //if (__IsClientStorage) storage = __StreamWRHandler.ClientNodeStorage;
                //else storage = __StreamWRHandler.ServerNodeStorage;
                //foreach (var (_, UIDNode) in __Child.Values)
                //    yield return storage[UIDNode];
                //yield break;
            }
        }

        public override bool __Inited { get; protected set; }
        public override TypeCodeExtande __TypeCode { get; }
        public override bool __IsCollection { get; }


        //protected NETDynamicStruct(StreamWRHandler handler, Type type, bool isClientStorage)
        //{
        //    __IsClientStorage = isClientStorage;
        //    __StreamWRHandler = handler;
        //    __Type = type;
        //    __UID = 0;
        //}
        protected NETDynamicStruct(Type type, bool isClient) : base(type)
        {
            __Child = new StructNodeChild(this, isClient);
        }
        public NETDynamicStruct(DynamicNode parent, Type type) : base(parent, type)
        {
            //__IsClientStorage = isClientStorage;
            //__StreamWRHandler = handler;
            var mainContainer = parent.MainContainer;
            //var propertyNames = mainContainer.__StreamWRHandler.PropertyNames;
            __Child = new StructNodeChild(this, mainContainer.__IsClientStorage);
            //foreach (var item in type.GetProperties())
            //{
            //    __Child.Add(item.Name, (propertyNames.FindKey(item.Name), true, mainContainer.CreateNode(parent, item.PropertyType);
            //    //__Child.Add(handler.PropertyNames.FindKey(item.Name), (true, handler.CreateNode(item.PropertyType, isClientStorage).__UID));
            //}
            //foreach (var item in type.GetFields())
            //{
            //    __Child.Add(item.Name, (propertyNames.FindKey(item.Name), false, mainContainer.CreateNode(parent, item.FieldType);
            //    //__Child.Add(handler.PropertyNames.FindKey(item.Name), (false, handler.CreateNode(item.FieldType, isClientStorage).__UID));
            //}
            __structID = mainContainer.__StreamWRHandler.Types.FindKey(type);
        }

        /// <summary>
        /// Key - propertyName, Value - UIDNode
        /// </summary>
        public class StructNodeChild : Dictionary<string, (int PropertyID, bool IsProperty, DynamicNode Node)>
        {
            Type Type => Owner.__Type;
            DynamicNode Owner;
            //DictCounter<string> PropertyNames;
            //Dictionary<int, DynamicNode> NodeStorage;
            public StructNodeChild(DynamicNode owner, bool isClient)
            {
                Owner = owner;
                var mainContainer = owner.MainContainer;
                var propertyNames = mainContainer.__StreamWRHandler.PropertyNames;
                var type = owner.__Type;
                foreach (var item in type.GetProperties())
                {
                    this.Add(item.Name, (propertyNames.FindKey(item.Name), true, mainContainer.CreateNode(owner, item.PropertyType)));
                }
                foreach (var item in type.GetFields())
                {
                    this.Add(item.Name, (propertyNames.FindKey(item.Name), false, mainContainer.CreateNode(owner, item.FieldType)));
                }
                //foreach (var item in type.GetProperties())
                //{
                //    this.Add(item.Name, (true, mainContainer.CreateNode(owner, item.PropertyType)));
                //}
                //foreach (var item in type.GetFields())
                //{
                //    this.Add(item.Name, (false, mainContainer.CreateNode(owner, item.FieldType)));
                //}

                //PropertyNames = owner.PropertyNames;
                //if (isClient)
                //    NodeStorage = streamWRHandler.ClientNodeStorage;
                //else NodeStorage = streamWRHandler.ServerNodeStorage;
            }

            //public StreamWRHandler StreamWRHandler { get; }
            //public int GetUIDNode(int propertyNameID)
            //{
            //    return this[propertyNameID].UIDNode;
            //}

            //public DynamicNode GetNode(int propertyNameID)
            //{
            //    return NodeStorage[this[propertyNameID].UIDNode];
            //}

            public (int propertyID, bool IsProperty, DynamicNode Node) GetNodeInfo(string propertyNameID)
            {
                return this[propertyNameID];
                //var v = this[propertyNameID];
                //return (v.IsProperty, NodeStorage[v.UIDNode]);
            }

            //public DynamicNode GetNode(int propertyNameID, Dictionary<int, DynamicNode> dynamics)
            //{
            //    return dynamics[this[propertyNameID].UIDNode];
            //}

            public DynamicNode GetNode(string propertyName)
            {
                return this[propertyName].Node;
                //return NodeStorage[this[PropertyNames.FindKey(propertyName)].UIDNode];
                //return GetNode(StreamWRHandler.PropertyNames.FindKey(propertyName), NodeStorage);
            }

            public void ForEachGetNode(Action<DynamicNode> action)
            {
                foreach (var item in this.Keys)
                {
                    // Для каждого узла возвращается значение в соответсвии с функцией
                    // которая получает ID имени узла
                    action.Invoke(GetNode(item));
                }
            }

            //public void ForEachGetNode(Action<int, DynamicNode> action)
            //{
            //    foreach (var item in this.Keys)
            //    {
            //        // Для каждого узла возвращается значение в соответсвии с функцией
            //        // которая получает ID имени узла
            //        action.Invoke(item, GetNode(item));
            //    }
            //}

            public void ForEachGetNode(Action<string, DynamicNode> action)
            {
                foreach (var item in this)
                {
                    // Для каждого узла возвращается значение в соответсвии с функцией,
                    // которая получает имя узла
                    action.Invoke(item.Key, item.Value.Node);
                }
            }

            // T1 - MemberName, T2 - IsProperty, T3 - Node
            public void ForEachGetNode(Action<string, bool, DynamicNode> action)
            {
                foreach (var item in this)
                {
                    // Для каждого узла возвращается значение в соответсвии с функцией,
                    // которая получает имя узла
                    //var v = GetNodeInfo(item);
                    var v = item.Value;
                    action.Invoke(item.Key, v.IsProperty, v.Node);
                }
            }

            //public void ForEachSet(Func<int, object> func)
            //{
            //    foreach (var item in this.Keys)
            //    {
            //        // Для каждого узла обновляется значение в соответсвии с функцией
            //        // которая получает ID имени узла
            //        GetNode(item).UpdateValue(func.Invoke(item));
            //    }
            //}

            //public void ForEachSet(Func<string, object> func)
            //{
            //    foreach (var item in this.Keys)
            //    {
            //        // Для каждого узла обновляется значение в соответсвии с функцией,
            //        // которая получает имя узла
            //        GetNode(item).UpdateValue(func.Invoke(PropertyNames[item]));
            //    }
            //}

            //public void ForEachSet(Func<string, bool , object> func)
            //{
            //    foreach (var item in this.Keys)
            //    {
            //        // Для каждого узла обновляется значение в соответсвии с функцией,
            //        // которая получает имя узла
            //        var v = GetNodeInfo(item);
            //        v.Node.UpdateValue(func.Invoke(PropertyNames[item], v.IsProperty));
            //    }
            //}

            //public void ForEachSet(Func<int, string, bool, object> func)
            //{
            //    foreach (var item in this.Keys)
            //    {
            //        // Для каждого узла обновляется значение в соответсвии с функцией,
            //        // которая получает имя узла
            //        var v = GetNodeInfo(item);
            //        v.Node.UpdateValue(func.Invoke(item, PropertyNames[item], v.IsProperty));
            //    }
            //}
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return __Child.Keys;
            //string[] names = new string[__Child.Count];
            //int index = 0;
            //var idnames = __StreamWRHandler.PropertyNames;
            //foreach (var item in __Child)
            //{
            //    yield return idnames[item.Key];
            //    //names[index] = idnames[item.Key];
            //    //index++;
            //}
            ////return names;
            //yield break;
        }

        public override DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return base.GetMetaObject(parameter);
        }

        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
            return base.TryBinaryOperation(binder, arg, out result);
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            result = null;
            if (binder.Type != typeof(NETDynamicStruct))
            {
                Type type = __Type;

                object ret = Activator.CreateInstance(binder.Type);
                __Child.ForEachGetNode((string name, bool isProperty, DynamicNode node) =>
                {
                    if (isProperty) type.GetProperty("name").SetValue(ret, node);
                    else type.GetField("name").SetValue(ret, node);
                });
                if (binder.Type != __Type) result = Convert.ChangeType(ret, binder.Type);
                else result = ret;
                return true;
                //var str = __StreamWRHandler.__Structures;
                //var names = __StreamWRHandler.__MemberNames;
                //if (__StreamWRHandler.__Structures.FindKey(binder.Type) == __StructID)
                //{
                //    result = Activator.CreateInstance(binder.Type);
                //    foreach (var item in binder.Type.GetFields())
                //    {
                //        item.SetValue(result, Convert.ChangeType(__StreamWRHandler.NETS[__Child[names.FindKey(item.Name)]], item.FieldType));
                //    }
                //    foreach (var item in binder.Type.GetProperties())
                //    {
                //        item.SetValue(result, Convert.ChangeType(__StreamWRHandler.NETS[__Child[names.FindKey(item.Name)]], item.PropertyType));
                //    }
                //}
                //else return false;
            }
            else result = this;
            return true;
        }

        public override bool TryCreateInstance(CreateInstanceBinder binder, object[] args, out object result)
        {
            return base.TryCreateInstance(binder, args, out result);
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            return base.TryGetIndex(binder, indexes, out result);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = __Child.GetNode(binder.Name);
            //result = __StreamWRHandler.NETS[__Child[__StreamWRHandler.PropertyNames.FindKey(binder.Name)].UIDNode];
            return true;
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            return base.TryInvoke(binder, args, out result);
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            return base.TryInvokeMember(binder, args, out result);
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            return base.TrySetIndex(binder, indexes, value);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            try
            {
                __Child.GetNode(binder.Name).UpdateValue(value);
                //__StreamWRHandler.
                //    __NodeDict[__Child[__StreamWRHandler.PropertyNames.FindKey(binder.Name)]]
                //    .UpdateValue(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            return base.TryUnaryOperation(binder, out result);
        }

        internal override void UpdateValue(object value)
        {
            if (__Type == value.GetType())
            {
                Type type = __Type;
                __Child.ForEachGetNode((name, isProperty, node) =>
                {
                    object newValue;
                    if (isProperty) newValue = type.GetProperty(name).GetValue(value);
                    else newValue = type.GetField(name).GetValue(value);
                    node.SetValue(newValue);
                });
                //__StreamWRHandler.SendObject();
                ((DynamicNode)this).SendNodeRecursive(start: true);
            }
        }

        internal override void SendNodeRecursive(BinaryWriterE writer, bool start)
        {
            if (start)
            {
                // TODO метка начала рекурсивной передачи 
                __Child.ForEachGetNode(a => a.SendNodeRecursive(writer));
                // Конец метки
            }
            else
                __Child.ForEachGetNode(a => a.SendNodeRecursive(writer));
        }

        internal override void SetValue(object value)
        {
            if (__Type == value.GetType())
            {
                Type type = __Type;
                __Child.ForEachGetNode((name, isProperty, node) =>
                {
                    object newValue;
                    if (isProperty) newValue = type.GetProperty(name).GetValue(value);
                    else newValue = type.GetField(name).GetValue(value);
                    node.SetValue(newValue);
                });
            }
        }


        //public void GetRecourciveSimpleNode(List<IDynamic> list)
        //{
        //    __Child.ForEachGetNode(a => a.GetRecourciveSimpleNode(list));
        //}

        public override void GetRecourciveSimpleNode(List<NETDynamic> list)
        {
            __Child.ForEachGetNode(a => a.GetRecourciveSimpleNode(list));
        }

        public override void GetRecourciveUID(List<int> list)
        {
            __Child.ForEachGetNode(a => a.GetRecourciveUID(list));
        }
    }
}
