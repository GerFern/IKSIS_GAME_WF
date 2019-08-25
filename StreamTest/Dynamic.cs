using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;
using System.Dynamic;
using System.Runtime.Remoting;
using System.Reflection;
using static NotifyTest.Proxy;
using System.Linq;
using System.Collections;

namespace NotifyTest
{
    //[LinkClass(typeof(Test))]
    public interface ITest
    {
        public void AAAA() { }
        int i { get; set; }
    }

    //[LinkInterface(typeof(ITest))]
    public class Test : ITest
    {
        public int i { get; set; }
    }

    //[LinkClass(typeof(T2))]
    public interface IT2
    {
        public ITest test1 { get; set; }
        public ITest test2 { get; set; }
        public IList<int> tets3 { get; set; }
    }

    //[LinkInterface(typeof(IT2))]
    public class T2 : IT2
    {
        public ITest test1 { get; set; }
        public ITest test2 { get; set; }
        public IList<int> tets3 { get; set; }
    }
    //public interface INode
    //{
    //    public int UID { get; }

    //    public TopNode TopNode { get; }
    //    public Dictionary<int, INode> Parents { get; }
    //    public Dictionary<int, Node> Childs { get; }

    //}

    public interface IT3<T>
    {
        public T Data { get; set; }
        public T Test() => Data;
        public Type GType() => typeof(T);
        public Type GTypeA<TA>() => typeof(TA);
    }

    public class T3<T> : IT3<T>
    {
        public T Data { get; set; }
    }

    public class Node
    {
        public int UID { get; }
        public virtual TopNode Top { get; private set; }
        public virtual List<int> Parents { get; } = new List<int>();
        public List<int> Childs { get; } = new List<int>();
        public Proxy Proxy { get; set; }


        

        public Node()
        {
            UID = NodeDict.Add(this).ID;
        }

        public Node(Proxy proxy) : this()
        {
            Proxy = proxy;
            proxy.Node = this;
        }

        public static Proxy CreateNode(Type TInterface, object real, out Node node)
        {
            object t = typeof(Proxy).GetMethod("Create").MakeGenericMethod(TInterface, typeof(Proxy)).Invoke(null, null);
            //object t = Proxy.Create<TInterface, Proxy>();
            Proxy proxy = (Proxy)t;
            proxy.SetTarget(real);
            node = new Node(proxy);
            return (Proxy)t;
        }

        public static Proxy CreateNode(Type TInterface, object real)
        {
            object t = typeof(Proxy).GetMethod("Create").MakeGenericMethod(TInterface, typeof(Proxy)).Invoke(null, null);
            //object t = Proxy.Create<TInterface, Proxy>();
            Proxy proxy = (Proxy)t;
            proxy.SetTarget(real);
            new Node(proxy);
            return (Proxy)t;
        }

        public static TInterface CreateNode<TInterface>(out Node node)
        {
            object t = Proxy.Create<TInterface, Proxy>();
            Proxy proxy = (Proxy)t;
            //proxy.SetTarget(Activator.CreateInstance(typeof(TInterface).GetLinkedType()));
            node = new Node(proxy);
            return (TInterface)t;
        }

        public static TInterface CreateNode<TInterface, TRealType>(out Node node) where TRealType : TInterface
        {
            object t = Proxy.Create<TInterface, Proxy>();
            Proxy proxy = (Proxy)t;
            proxy.SetTarget(Activator.CreateInstance(typeof(TRealType)));
            node = new Node(proxy);
            return (TInterface)t;
        }

        public static TInterface CreateNode<TInterface, TRealType>() where TRealType : TInterface
        {
            object t = Proxy.Create<TInterface, Proxy>();
            Proxy proxy = (Proxy)t;
            proxy.SetTarget(Activator.CreateInstance(typeof(TRealType)));
            new Node(proxy);
            return (TInterface)t;
        }

        public static TInterface CreateNode<TInterface>(TInterface realObject, out Node node)
        {
            object t = Proxy.Create<TInterface, Proxy>();
            Proxy proxy = (Proxy)t;
            proxy.SetTarget(realObject);
            node = new Node(proxy);
            return (TInterface)t;
        }

        public static TInterface CreateNode<TInterface>()
        {
            object t = Proxy.Create<TInterface, Proxy>();
            Proxy proxy = (Proxy)t;
            //proxy.SetTarget(Activator.CreateInstance(typeof(TInterface).GetLinkedType()));
            new Node(proxy);
            return (TInterface)t;
        }

        public static TInterface CreateNode<TInterface>(TInterface realObject)
        {
            object t = Proxy.Create<TInterface, Proxy>();
            Proxy proxy = (Proxy)t;
            proxy.SetTarget(realObject);
            new Node(proxy);
            return (TInterface)t;
        }

        public static void SetProxyHandler(Proxy proxy, ProxyHandler proxyHandler)
        {
            proxy.ProxyHandler = proxyHandler;
        }
    }

    public class TopNode : Node
    {
        public ProxyHandler ProxyHandler { get; set; }
        public bool IsOwnerContainer { get; }
        public bool IsCommon { get; }
        public override List<int> Parents => null;
        public override TopNode Top => this;

        public TopNode() : base() { }
        public TopNode(Proxy proxy) : base(proxy) { }

        public static TInterface CreateTopNode<TInterface>(TInterface realObject ,out TopNode topNode)
        {
            object t = Proxy.Create<TInterface, Proxy>();
            Proxy proxy = (Proxy)t;
            proxy.SetTarget(realObject);
            topNode = new TopNode(proxy);
            return (TInterface)t;
        }

        public static TInterface CreateTopNode<TInterface>(out TopNode topNode)
        {
            object t = Proxy.Create<TInterface, Proxy>();
            Proxy proxy = (Proxy)t;
            //proxy.SetTarget(Activator.CreateInstance(typeof(TInterface).GetLinkedType()));
            topNode = new TopNode(proxy);
            return (TInterface)t;
        }

        public static TInterface CreateTopNode<TInterface, TObjectType>(out TopNode topNode) where TObjectType : TInterface
        {
            object t = Proxy.Create<TInterface, Proxy>();
            Proxy proxy = (Proxy)t;
            proxy.SetTarget(Activator.CreateInstance(typeof(TObjectType)));
            topNode = new TopNode(proxy);
            return (TInterface)t;
        }
    }

    public class Proxy : DispatchProxy
    {
        public Node Node { get; set; }
        protected object real;
        private Dictionary<int, Proxy> LocalObjectProperties = new Dictionary<int, Proxy>();
        public Type Type { get; private set; }
        public int TypeID { get; private set; }

        public (object Value, bool IsProxy ) GetProperty(string propertyName)
        {
            PropertyInfo propertyInfo = Type.GetProperty(propertyName);
            Type type = propertyInfo.PropertyType;
            if (Type.GetTypeCode(type) == TypeCode.Object)
            {
                int propertyId = PropertyNames.FindKey(propertyName);

                if (LocalObjectProperties.TryGetValue(propertyId, out Proxy value))
                {
                    return (value, true);
                }
                else
                {
                    //LocalObjectProperties.Add()
                    Node node = new Node();
                    node.Parents.Add(this.Node.UID);
                    this.Node.Childs.Add(this.Node.UID);
                    RecieveNode(node);
                    propertyInfo.SetValue(real, node.Proxy.real);
                    return (node.Proxy, true);
                    //propertyInfo.GetValue(real);
                    //throw new NotImplementedException();
                }
            }
            else return (propertyInfo.GetValue(real),false);
        }

        public void SetProperty(string propertyName, object value)
        {
            PropertyInfo propertyInfo = Type.GetProperty(propertyName);
            Type type = propertyInfo.PropertyType;
                int propertyId = PropertyNames.FindKey(propertyName);
            if (Type.GetTypeCode(type) == TypeCode.Object)
            {

                Proxy proxy;
                Type type1 = value.GetType();
                if (type1.IsArray)
                {
                    Type IList = type1.GetListInterface();
                    proxy = Node.CreateNode(IList, value);
                }
                else
                {
                    if (value is Proxy proxy1)
                    {
                        proxy = proxy1;
                    }
                    else
                    {
                        proxy = Node.CreateNode(type1.GetSingledInterface(), value);
                    }
                }
                if (LocalObjectProperties.ContainsKey(propertyId))
                {
                    LocalObjectProperties[propertyId] = proxy;
                }
                else LocalObjectProperties.Add(propertyId, proxy);

                SendNode(proxy.Node);
            }
            else
            {
                real.GetType().GetProperty(propertyName).SetValue(real, value);
                SendNode(this.Node, propertyId);
            }
        }

        public virtual void SetTarget(object target)
        {
            this.real = target;
            Type = target.GetType();
            if(Type.IsGenericType)
            {
                TypeID = Types.FindKey(Type.GetGenericTypeDefinition());
            }
            else
            TypeID = Types.FindKey(Type);
        }


        public void SendNode(Node node)
        {

        }

        public void RecieveNode(Node node)
        {

        }

        public void SendNode(Node parent, int propertyID) { }
        public void RecieveNode(Node parent, int propertyID) { }

        public object SendMethod(Node parent, int methodID, object[] args)
        {
            return null;
        }

        public object RecieveMethod(Node parent, int methodID, object[] args)
        {
            return null;
        }

        public ProxyHandler ProxyHandler { get; set; }

        public bool AddParent(Node node)
        {
            if (!(this.Node is TopNode))
            {
                node.Childs.Add(Node.UID);
                Node.Parents.Add(node.UID);
                return true;
            }
            return false;
        }
        public bool RemoveParent(Node node)
        {
            if (this.Node! is TopNode)
            {
                if (node.Childs.Remove(Node.UID))
                {
                    Node.Parents.Remove(node.UID);
                    if (Node.Parents.Count == 0) NodeDict.RemoveKey(Node.UID);
                    return true;
                }
            }
            return false;
        }
        public bool AddParent(int node)
        {
            var parent = NodeDict[node];
            if (this.Node! is TopNode)
            {
                parent.Childs.Add(Node.UID);
                Node.Parents.Add(node);
                return true;
            }
            return false;
        }
        public bool RemoveParent(int node)
        {
            var parent = NodeDict[node];
            if (this.Node! is TopNode)
            {
                if (parent.Childs.Remove(Node.UID))
                {
                    Node.Parents.Remove(node);
                    if (Node.Parents.Count == 0) NodeDict.RemoveKey(Node.UID);
                    return true;
                }
            }
            return false;
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            object objret = null;
            int methodId;
            string methodName = targetMethod.Name;
            
            if(targetMethod.IsSpecialName)
            {
                if(targetMethod.Name.StartsWith("get_"))
                {
                    return GetProperty(methodName.Substring(4));
                }
                else if(methodName.StartsWith("set_"))
                {
                    SetProperty(methodName.Substring(4), args[0]);
                    //targetMethod.Invoke(real, args);
                    return null;
                }
            }


            if(targetMethod.GetCustomAttribute(typeof(ProxyOnServer))!=null)
            {
                
            }
            else if(targetMethod.DeclaringType.GetListInterface()!=null)
            {
                if(methodName == "Add"     ||
                   methodName == "Remove"  ||
                   methodName == "RemoveAt"||
                   methodName == "Clear"   ||
                   methodName == "set_Item"||
                   methodName == "get_Item")
                {

                }
            }
            try
            {

                if (targetMethod.IsGenericMethod)
                {
                    methodId = Methods.FindKey(targetMethod.GetGenericMethodDefinition());
                }
                else
                    methodId = Methods.FindKey(targetMethod);

            }
            catch
            {
                Console.WriteLine(targetMethod.Name + " Offline");
                return targetMethod.Invoke(real, args);
            }
            if (MethodsProp.TryGetValue(methodId, out int pid))
            {
                PropertyInfo propertyInfo = Properties[pid];
                Type type = propertyInfo.PropertyType;
                bool get = args.Length == 0;
                bool b = Interfaces.Contains(type);
                if (b)
                {
                    if (get)
                    {
                        if (LocalObjectProperties.TryGetValue(pid, out Proxy proxy))
                        {
                            objret = propertyInfo.GetValue(real);
                        }
                        else
                        {
                            //TODO запрос с сервера
                        }
                    }
                    else
                    {
                        var p = (Proxy)args[0];
                        if (LocalObjectProperties.ContainsKey(pid))
                        {
                            var oldval = LocalObjectProperties[pid];
                            oldval.RemoveParent(Node);
                            LocalObjectProperties[pid] = p;
                        }
                        else
                        {
                            LocalObjectProperties.Add(pid, p);
                        }
                        p.AddParent(Node);
                        propertyInfo.SetValue(real, args[0]);
                        //TODO отправка серверу
                    }
                }
                else
                {
                    if (get)
                    {
                        objret = propertyInfo.GetValue(real);
                    }
                    else
                    {
                        propertyInfo.SetValue(real, args[0]);
                    }
                }
                return objret;
            }

            Console.WriteLine(targetMethod.Name);
            return targetMethod.Invoke(real, args);
        }
        //protected override object Invoke(MethodInfo targetMethod, object[] args)
        //{
        //    object objret = null;
        //    int methodId;
        //    try
        //    {

        //        if (targetMethod.IsGenericMethod)
        //        {
        //            methodId = Methods.FindKey(targetMethod.GetGenericMethodDefinition());
        //        }
        //        else
        //        methodId = Methods.FindKey(targetMethod);

        //    }
        //    catch
        //    {
        //        Console.WriteLine(targetMethod.Name + " Offline");
        //        return targetMethod.Invoke(real, args);
        //    }
        //    if (MethodsProp.TryGetValue(methodId, out int pid))
        //    {
        //        PropertyInfo propertyInfo = Properties[pid];
        //        Type type = propertyInfo.PropertyType;
        //        bool get = args.Length == 0;
        //        bool b = Interfaces.Contains(type);
        //        if (b)
        //        {
        //            if (get)
        //            {
        //                if(LocalObjectProperties.TryGetValue(pid, out Proxy proxy))
        //                {
        //                    objret = propertyInfo.GetValue(real);
        //                }
        //                else
        //                {
        //                    //TODO запрос с сервера
        //                }
        //            }
        //            else
        //            {
        //                var p = (Proxy)args[0];
        //                if(LocalObjectProperties.ContainsKey(pid))
        //                {
        //                    var oldval = LocalObjectProperties[pid];
        //                    oldval.RemoveParent(Node);
        //                    LocalObjectProperties[pid] = p;
        //                }
        //                else
        //                {
        //                    LocalObjectProperties.Add(pid, p);
        //                }
        //                p.AddParent(Node);
        //                propertyInfo.SetValue(real, args[0]);
        //                //TODO отправка серверу
        //            }
        //        }
        //        else
        //        {
        //            if (get)
        //            {
        //                objret = propertyInfo.GetValue(real);
        //            }
        //            else
        //            {
        //                propertyInfo.SetValue(real, args[0]);
        //            }
        //        }
        //        return objret;
        //    }

        //    Console.WriteLine(targetMethod.Name);
        //    return targetMethod.Invoke(real, args);
        //}


        //public static void AddType(Type type)
        //{
        //    Type[] itypes = type.GetInterfaces(); // Получить интерфейсы
        //    Type iType = type.GetLinkedInterface();
        //    if (iType==null || itypes.Length != 1 || itypes[0] != iType)
        //    {
        //        throw new Exception("AAAAA");
        //    }
        //    List<int> TypeMethodsId = new List<int>(); // Список ID всех методов 
        //    List<int> TypePropsId = new List<int>(); // Список ID всех свойств 
        //    var t = Types.Add(type); // ID типа
        //    if (t.WasContain) return; // Был добавлен ранее. Пропуск
        //    int typeId = t.ID; // ID типа
        //    int[] interfaceId = new int[itypes.Length]; // ID интерфейсов
        //    for (int i = 0; i < itypes.Length; i++) // Для всех интерфейсов из списка
        //    {
        //        int[] methodsID;
        //        int[] propertiesID;
        //        Type itype = itypes[i]; // Текущий интерфейс
        //        var it = Interfaces.Add(itype); // Сохранить интерфейс в список и получить ID
        //        int itypeId = it.ID; 
        //        if (it.WasContain)
        //        {
        //            methodsID = ITypeMethods[itypeId];
        //            propertiesID = ITypeProps[itypeId];

        //            TypeMethodsId.AddRange(methodsID);
        //            TypePropsId.AddRange(propertiesID);
        //        }
        //        interfaceId[i] = itypeId;
        //        MethodInfo[] methods = itype.GetMethods(); // Получить методы интерфейса
        //        methodsID = new int[methods.Length]; // ID методов интерфейса

        //        PropertyInfo[] properties = itype.GetProperties(); // Получить свойства интерфейса
        //        propertiesID = new int[properties.Length];   // ID свйоств интерфейса
        //        DoubleDict<int, string> tempDict = new DoubleDict<int, string>(); // Хранилище типов

        //        for (int j = 0; j < properties.Length; j++)
        //        {
        //            PropertyInfo p = properties[i];
        //            int pid = Properties.Add(properties[i]).ID;  // Сохранить свойство в список и получить ID
        //            propertiesID[i] = pid;
        //            TypePropsId.Add(pid);
        //            tempDict.Add(pid, p.Name);
        //        }
        //        for (int j = 0; j < methods.Length; j++) // Для каждого метода интерфейса
        //        {
        //            MethodInfo method = methods[j]; // Текущий метод
        //            int methodID = Methods.Add(method).ID; // Сохранить метод в список и получить ID
        //            TypeMethodsId.Add(methodID);
        //            methodsID[j] = methodID;
        //            if (method.IsSpecialName) // Проверка на свойство
        //            {
        //                if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_")) // Проверка на свойство
        //                {
        //                    PropertyInfo property = itype.GetProperty(method.Name.Substring(4)); // Поиск свойства
        //                    int propertyID = tempDict.FindT1(property.Name);
        //                    MethodsProp.Add(methodID, propertyID); // Связь между методом и свойством
        //                }
        //            }
        //        }
        //        ITypeProps.Add(itypeId, propertiesID);
        //        ITypeMethods.Add(itypeId, methodsID); // Связать интерфейс и методы
        //    }
        //    TypeProps.Add(typeId, TypePropsId.ToArray()); // Связать тип и свойства
        //    TypeMethods.Add(typeId, TypeMethodsId.ToArray()); // Связать тип и методы
        //    TypeInterfaces.Add(typeId, interfaceId); // Связать тип и интерфейсы
        //}

        //public static void AddType(Type type, Type itype=null)
        //{
        //    //Type[] itypes = type.GetInterfaces(); // Получить интерфейсы
        //    if (type.IsGenericType) type = type.GetGenericTypeDefinition();
        //    int typeId = -1; // ID типа
        //    if (type.IsInterface) itype = type;
        //    else
        //    {
        //        var t = Types.Add(type); // ID типа
        //        if (t.WasContain) return; // Был добавлен ранее. Пропуск
        //        if (itype == null)
        //            itype = type.GetSingledInterface();
        //        typeId = t.ID;
        //        if (itype.IsListInterface())
        //            ListTypes.Add(typeId);
        //    }
        //    if (itype == null/* || itypes.Length != 1 || itypes[0] != itype*/)
        //    {
        //        throw new Exception("AAAAA");
        //    }
        //    //int[] interfaceId = new int[itypes.Length]; // ID интерфейсов
        //    if (itype.IsGenericType) itype = itype.GetGenericTypeDefinition();                   
        //    var it = Interfaces.Add(itype);
        //    if (it.WasContain) return;
        //    int itypeId = it.ID;

        //    int[] methodsID;
        //    int[] propertiesID;
        //    MethodInfo[] methods = itype.GetMethods(); // Получить методы интерфейса
        //    methodsID = new int[methods.Length]; // ID методов интерфейса

        //    PropertyInfo[] properties = itype.GetProperties(); // Получить свойства интерфейса
        //    propertiesID = new int[properties.Length];   // ID свйоств интерфейса
        //    DoubleDict<int, string> tempDict = new DoubleDict<int, string>(); // Хранилище типов


        //    for (int j = 0; j < properties.Length; j++)
        //    {
        //        PropertyInfo p = properties[j];
        //        int pid = Properties.Add(properties[j]).ID;  // Сохранить свойство в список и получить ID
        //        propertiesID[j] = pid;
        //        tempDict.Add(pid, p.Name);
        //    }
        //    for (int j = 0; j < methods.Length; j++) // Для каждого метода интерфейса
        //    {
        //        MethodInfo method = methods[j]; // Текущий метод
        //        int methodID = Methods.Add(method).ID; // Сохранить метод в список и получить ID
        //        methodsID[j] = methodID;
        //        if (method.IsSpecialName) // Проверка на свойство
        //        {
        //            if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_")) // Проверка на свойство
        //            {
        //                PropertyInfo property = itype.GetProperty(method.Name.Substring(4)); // Поиск свойства
        //                int propertyID = tempDict.FindT1(property.Name);
        //                MethodsProp.Add(methodID, propertyID); // Связь между методом и свойством
        //            }
        //        }
        //    }
        //    //ITypeProps.Add(itypeId, propertiesID);
        //    //ITypeMethods.Add(itypeId, methodsID); // Связать интерфейс и методы
        //    ITypeProps.Add(itypeId, propertiesID); // Связать тип и свойства
        //    ITypeMethods.Add(itypeId, methodsID); // Связать тип и методы
        //    //TypeInterfaces.Add(typeId, interfaceId); // Связать тип и интерфейсы
        //}

        public static void AddType(Type type)
        {
            Types.Add(type);
            AddIType(type.GetSingledInterface());
        }

        public static void AddIType(Type type)
        {
            //type.GetMethod(,)
            if (type.IsGenericType && type.IsGenericTypeDefinition) throw new Exception("Make Generic Type Defenition!");
            var tw = Interfaces.Add(type);
            if (tw.WasContain) return;
            foreach (var item in type.GetProperties().Where(a=>a.GetCustomAttribute<ProxyOffline>()==null))
            {
                PropertyNames.Add(item.Name);
            }

            foreach (var item in type.GetMethods().Where(a=>a.GetCustomAttribute<ProxyOnServer>()!=null))
            {
                MethodNames.Add(item.Name);
            }
            
            //var methodArr = type.GetMethods().Select(a => a.Name + a.n)
        }

        public static void InitIList()
        {
            MethodNames.Add("Add");
            MethodNames.Add("Remove");
            MethodNames.Add("RemoveAt");
            MethodNames.Add("Clear");
            MethodNames.Add("set_Item");
            MethodNames.Add("get_Item");
        }

        public static void AddIListType(Type type, Type itype)
        {
            int typeId = -1; // ID типа
            if (type.IsInterface) throw new Exception("aaa");
            else
            {
                var t = Types.Add(type); // ID типа
                if (t.WasContain) return; // Был добавлен ранее. Пропуск
                if (itype == null)
                    itype = type.GetSingledInterface();
                typeId = t.ID;
                if (itype.IsListInterface())
                    ListTypes.Add(typeId);
            }
            if (itype == null/* || itypes.Length != 1 || itypes[0] != itype*/)
            {
                throw new Exception("AAAAA");
            }
            //int[] interfaceId = new int[itypes.Length]; // ID интерфейсов
            var it = Interfaces.Add(itype);
            if (it.WasContain) return;
            int itypeId = it.ID;

            int[] methodsID;
            int[] propertiesID;
            MethodInfo[] methods = itype.GetMethods(); // Получить методы интерфейса
            methodsID = new int[methods.Length]; // ID методов интерфейса

            PropertyInfo[] properties = itype.GetProperties(); // Получить свойства интерфейса
            propertiesID = new int[properties.Length];   // ID свйоств интерфейса
            DoubleDict<int, string> tempDict = new DoubleDict<int, string>(); // Хранилище типов

            if (itype.IsListInterface())
            {
                Type subIType = itype.GetInterface("ICollection`1");
                MethodInfo methodSet = itype.GetMethod("set_Item");
                MethodInfo methodGet = itype.GetMethod("get_Item");
                MethodInfo methodAdd = subIType.GetMethod("Add");
                MethodInfo methodRemove = subIType.GetMethod("Remove");
                MethodInfo methodRemoveAt = itype.GetMethod("RemoveAt");
                MethodInfo methodClear = subIType.GetMethod("Clear");

                if (typeId != -1)
                    ListITypes.Add(itypeId);

                ITypeMethods.Add(itypeId, new int[]
                {
                    Methods.Add(methodSet).ID,
                    Methods.Add(methodGet).ID,
                    Methods.Add(methodAdd).ID,
                    Methods.Add(methodRemove).ID,
                    Methods.Add(methodRemoveAt).ID,
                    Methods.Add(methodClear).ID,
                });
                return;
            }

            for (int j = 0; j < properties.Length; j++)
            {
                PropertyInfo p = properties[j];
                int pid = Properties.Add(properties[j]).ID;  // Сохранить свойство в список и получить ID
                propertiesID[j] = pid;
                tempDict.Add(pid, p.Name);
            }
            for (int j = 0; j < methods.Length; j++) // Для каждого метода интерфейса
            {
                MethodInfo method = methods[j]; // Текущий метод
                int methodID = Methods.Add(method).ID; // Сохранить метод в список и получить ID
                methodsID[j] = methodID;
                if (method.IsSpecialName) // Проверка на свойство
                {
                    if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_")) // Проверка на свойство
                    {
                        PropertyInfo property = itype.GetProperty(method.Name.Substring(4)); // Поиск свойства
                        int propertyID = tempDict.FindT1(property.Name);
                        MethodsProp.Add(methodID, propertyID); // Связь между методом и свойством
                    }
                }
            }
            //ITypeProps.Add(itypeId, propertiesID);
            //ITypeMethods.Add(itypeId, methodsID); // Связать интерфейс и методы
            ITypeProps.Add(itypeId, propertiesID); // Связать тип и свойства
            ITypeMethods.Add(itypeId, methodsID); // Связать тип и методы
        }

        //public static void AddIListType(Type type, Type itype)
        //{
        //    int typeId = -1; // ID типа
        //    if (type.IsInterface) throw new Exception("aaa");
        //    else
        //    {
        //        var t = Types.Add(type); // ID типа
        //        if (t.WasContain) return; // Был добавлен ранее. Пропуск
        //        if (itype == null)
        //            itype = type.GetSingledInterface();
        //        typeId = t.ID;
        //        if (itype.IsListInterface())
        //            ListTypes.Add(typeId);
        //    }
        //    if (itype == null/* || itypes.Length != 1 || itypes[0] != itype*/)
        //    {
        //        throw new Exception("AAAAA");
        //    }
        //    //int[] interfaceId = new int[itypes.Length]; // ID интерфейсов
        //    var it = Interfaces.Add(itype);
        //    if (it.WasContain) return;
        //    int itypeId = it.ID;

        //    int[] methodsID;
        //    int[] propertiesID;
        //    MethodInfo[] methods = itype.GetMethods(); // Получить методы интерфейса
        //    methodsID = new int[methods.Length]; // ID методов интерфейса

        //    PropertyInfo[] properties = itype.GetProperties(); // Получить свойства интерфейса
        //    propertiesID = new int[properties.Length];   // ID свйоств интерфейса
        //    DoubleDict<int, string> tempDict = new DoubleDict<int, string>(); // Хранилище типов

        //    if (itype.IsListInterface())
        //    {
        //        Type subIType = itype.GetInterface("ICollection`1");
        //        MethodInfo methodSet = itype.GetMethod("set_Item");
        //        MethodInfo methodGet = itype.GetMethod("get_Item");
        //        MethodInfo methodAdd = subIType.GetMethod("Add");
        //        MethodInfo methodRemove = subIType.GetMethod("Remove");
        //        MethodInfo methodRemoveAt = itype.GetMethod("RemoveAt");
        //        MethodInfo methodClear = subIType.GetMethod("Clear");

        //        if (typeId != -1)
        //            ListITypes.Add(itypeId);

        //        ITypeMethods.Add(itypeId, new int[]
        //        {
        //            Methods.Add(methodSet).ID,
        //            Methods.Add(methodGet).ID,
        //            Methods.Add(methodAdd).ID,
        //            Methods.Add(methodRemove).ID,
        //            Methods.Add(methodRemoveAt).ID,
        //            Methods.Add(methodClear).ID,
        //        });
        //        return;
        //    }

        //    for (int j = 0; j < properties.Length; j++)
        //    {
        //        PropertyInfo p = properties[j];
        //        int pid = Properties.Add(properties[j]).ID;  // Сохранить свойство в список и получить ID
        //        propertiesID[j] = pid;
        //        tempDict.Add(pid, p.Name);
        //    }
        //    for (int j = 0; j < methods.Length; j++) // Для каждого метода интерфейса
        //    {
        //        MethodInfo method = methods[j]; // Текущий метод
        //        int methodID = Methods.Add(method).ID; // Сохранить метод в список и получить ID
        //        methodsID[j] = methodID;
        //        if (method.IsSpecialName) // Проверка на свойство
        //        {
        //            if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_")) // Проверка на свойство
        //            {
        //                PropertyInfo property = itype.GetProperty(method.Name.Substring(4)); // Поиск свойства
        //                int propertyID = tempDict.FindT1(property.Name);
        //                MethodsProp.Add(methodID, propertyID); // Связь между методом и свойством
        //            }
        //        }
        //    }
        //    //ITypeProps.Add(itypeId, propertiesID);
        //    //ITypeMethods.Add(itypeId, methodsID); // Связать интерфейс и методы
        //    ITypeProps.Add(itypeId, propertiesID); // Связать тип и свойства
        //    ITypeMethods.Add(itypeId, methodsID); // Связать тип и методы
        //}


        public static DictCounter<string> MethodNames { get; } = new DictCounter<string>();
        public static DictCounter<string> PropertyNames { get; } = new DictCounter<string>();
        public static DictCounter<Type> Types { get; } = new DictCounter<Type>();
        public static DictCounter<Type> Interfaces { get; } = new DictCounter<Type>();
        public static DictCounter<PropertyInfo> Properties { get; } = new DictCounter<PropertyInfo>();
        public static DictCounter<MethodInfo> Methods { get; } = new DictCounter<MethodInfo>();
        public static DictCounter<Node> NodeDict { get; } = new DictCounter<Node>();

        //public static Dictionary<int, int[]> ITypeMethods { get; } = new Dictionary<int, int[]>();
        public static HashSet<int> ListTypes { get; } = new HashSet<int>();
        public static HashSet<int> ListITypes { get; } = new HashSet<int>();
        public static Dictionary<int, int[]> ITypeMethods { get; } = new Dictionary<int, int[]>();
        public static Dictionary<int, int> MethodsProp { get; } = new Dictionary<int, int>();
        //public static Dictionary<int, int[]> TypeInterfaces { get; } = new Dictionary<int, int[]>();
        //public static Dictionary<int, int[]> TypeProps { get; } = new Dictionary<int, int[]>();
        public static Dictionary<int, int[]> ITypeProps { get; } = new Dictionary<int, int[]>();
        //public static int NodeCounter { get; internal set; }
        //public static int TypeCounter { get; internal set; }
        //public static int PropertyCounter { get; internal set; }
        //public static int MethodCounter { get; internal set; }


    }

    public class ProxyList<T> : Proxy
    {
        public override void SetTarget(object target)
        {
            if (target.GetType().GetInterfaces().Contains(typeof(IList<T>)))
            {
                base.SetTarget(target);
            }
            else throw new Exception("Only IList!");
        }

        //IList<T> real;

        public void SetTargetArr(object target)
        {
            if (target.GetType().GetInterfaces().Contains(typeof(IList<T>)))
            {
                real = target;
            }
            else throw new Exception("Only IList!");
        }

        

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if(targetMethod.IsSpecialName)
            {
                object obj;
                if(targetMethod.Name == "set_Item")
                {
                    obj = targetMethod.Invoke(real, args);
                    //TODO send
                    return obj;
                }
                else  if(targetMethod.Name == "get_Item")
                {
                    obj = targetMethod.Invoke(real, args);
                    //TODO nothing?
                    return obj;
                }
            }
            return base.Invoke(targetMethod, args);
        }
    }

    public class ProxyHandler
    {
        public ProxyHandler(bool isServer, StreamWRHandler streamHandler, TopNode topNode)
        {
            IsServer = isServer;
            StreamHandler = streamHandler;
            TopNode = topNode;
        }

        public bool IsServer { get; }
        public StreamWRHandler StreamHandler { get; }

        public TopNode TopNode { get; set; }

    }
}
