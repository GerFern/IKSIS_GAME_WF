using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NotifyTest
{
    public interface INode : INotifyPropertyChanged
    {
        [PropertyChanged.DoNotNotify]
        public Node Node { get; set; }
    }


    public class Node
    {
        public int UID { get; private set; }
        private object value;
        private static Type INodeType = typeof(INode);
        private static MethodInfo node_get = Info.OfPropertyGet<INode>("Node");
        private static MethodInfo node_set = Info.OfPropertySet<INode>("Node");
        public virtual TopNode Top { get; private set; }
        public virtual List<int> Parents { get; } = new List<int>();
        public List<int> Childs { get; } = new List<int>();

        public object Value
        {
            get => value;
            set
            {
                Type type = value.GetType();
                if (this.value != null)
                {
                    Type oldtype = this.value.GetType();
                    if (oldtype.GetInterfaces().Contains(INodeType)) // Нужно удалить родителя
                    {
                        Node node = node_get.Invoke(value, null) as Node;
                        node.RemoveParent(this.UID);
                    }
                }
                if(type.GetInterfaces().Contains(INodeType))
                {
                    Node node = node_get.Invoke(value, null) as Node;
                    if(node==null)//Если узел не был присвоен
                    {
                        node_set.Invoke(value, new object[] { this });//Присвоить узел
                        foreach (var item in value.GetType().GetProperties().Where(a=>a.PropertyType.GetInterfaces().Contains(INodeType)))
                        {//Для всех свойств обхекта, которые могут иметь узел
                            var obj = item.GetValue(value, null);//Сам объект
                            if (obj != null)
                            {
                                Node subnode = obj.GetType().GetProperty("Node").GetValue(obj)as Node;//Ищем существующий узел
                                if (subnode == null)//Если его нет
                                {
                                    subnode = new Node();//Создаем новый
                                    subnode.Value = obj;//И присваиваем ему объект (Рекурсия)
                                }
                                subnode.AddParent(this.UID);//Добавляем родителя(this объект)
                            }
                        }
                    }
                    else//Если узел у объекта есть
                    {
                        if(node!=this)//Узел не является самим собой
                        {
                            foreach (var item in this.Parents)
                            {
                                node.AddParent(UID);//Добавляем родителя
                            }

                            NodeDict.RemoveKey(this.UID);
                            NodeDict[node.UID] = this;
                            node_set.Invoke(value, new object[] { this });
                            //this.UID = node.UID;
                            
                            //foreach (var item in node.Childs)
                            //{
                            //    NodeDict[item].Parents.Add(this.UID);
                            //}
                        }
                    }
                }
                this.value = value;

            }
        }



        public bool AddParent(int node)
        {
            var parent = NodeDict[node];
            if (!(this is TopNode))
            {
                parent.Childs.Add(UID);
                Parents.Add(node);
                return true;
            }
            return false;
        }
        public bool RemoveParent(int node)
        {
            var parent = NodeDict[node];
            if (this! is TopNode)
            {
                if (parent.Childs.Remove(UID))
                {
                    Parents.Remove(node);
                    if (Parents.Count == 0) NodeDict.RemoveKey(UID);
                    return true;
                }
            }
            return false;
        }
        public Node()
        {
            UID = NodeDict.Add(this).ID;
        }

        //public Node(Proxy proxy) : this()
        //{
        //    Proxy = proxy;
        //    proxy.Node = this;
        //}
        public static DictCounter<Node> NodeDict { get; } = new DictCounter<Node>();

    }


    public class TopNode : Node
    {
        public bool IsOwnerContainer { get; }
        public bool IsCommon { get; }
        public override List<int> Parents => null;
        public override TopNode Top => this;

        public TopNode() : base() { }
    }
}
