using PropertyChanged;
using ReactiveUI;
//using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using static Reactive.StatClass;

namespace Reactive
{


    [AttributeUsage(AttributeTargets.All)]
    public class MyAttribute : Attribute
    {
        public int Counter { get; set; } = 0;
    }

    [My]
    [AddINotifyPropertyChangedInterface]
    public class Test<A, B> : INotifyPropertyChanged
    {
        public T GetT<T>()
        {
            return (T)Activator.CreateInstance(typeof(T));
        }
        public T GetT<T, T2>()
        {
            return (T)Activator.CreateInstance(typeof(T));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public interface INetMethod
    {
        //public object Send(MethodInfo methodInfo, object[] args);
         
        public object SendMethodRequest(string methodName, object[] args);
        public object SendMethodRequest(string methodName, Type[] TDefs, object[] args);
    }

    public interface INodeBinder
    {
        [DoNotNotify]
        public Node Node { get; internal set; }
    }


    public class Node
    {
        public int UID { get; }
        public virtual TopNode Top { get; private set; }
        public virtual List<int> Parents { get; } = new List<int>();
        public List<int> Childs { get; } = new List<int>();
        public INodeBinder Proxy { get; set; }




        public Node()
        {
            UID = NodeDict.Add(this).ID;
        }

        public Node(INodeBinder proxy) : this()
        {
            Proxy = proxy;
            proxy.Node = this;
        }
    }

    public class TopNode : Node
    {
        public bool IsOwnerContainer { get; }
        public bool IsCommon { get; }
        public override List<int> Parents => null;
        public override TopNode Top => this;

        public TopNode() : base() { }
        public TopNode(INodeBinder proxy) : base(proxy) { }
    }



    static class StatClass
    {
        public static DictCounter<Node> NodeDict { get; } = new DictCounter<Node>();

    }
}
