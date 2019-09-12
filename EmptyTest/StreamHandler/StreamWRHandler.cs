using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using EmptyTest.TCommand;
using EmptyTest.TExtensions;
using EmptyTest.TNode;

namespace EmptyTest.TStreamHandler
{

    public class TaskLogger
    {
        public Random random = new Random();
        Dictionary<int, Command> commands = new Dictionary<int, Command>();
        public int AddCommand(Command command)
        {
            int id;
            do
            {
                id = random.Next(20000);
            }
            while (commands.ContainsKey(id));
            commands.Add(id, command);
            return id;

        }
    }

    //public static class CommandExtensions
    //{
    //    public static Command SetUIDNode(this Command command, int uid)
    //    {
    //        command.UIDNode = uid;
    //        return command;
    //    }

    //    public static Command SetNodeChildNames(this Command command, int[] nodeChildNames)
    //    {
    //        command.nodeChildNames = nodeChildNames;
    //        return command;
    //    }
    //}


    public abstract class StreamWRHandlerMany : StreamWRHandler
    {
        TaskLogger taskLogger = new TaskLogger();
        List<BinaryWriterE> _writers = new List<BinaryWriterE>();
        List<BinaryReaderE> _readers = new List<BinaryReaderE>();

        public StreamWRHandlerMany(DataHandler dataHandler) : base(dataHandler)
        {
        }

        public IEnumerator<BinaryWriterE> Writers => _writers.GetEnumerator();
        public IEnumerator<BinaryReaderE> Readers => _readers.GetEnumerator();

        public override BinaryWriterE Writer { get => null; set { } }
        public override BinaryReaderE Reader { get => null; set { } }

    }

    public class QueueActionInvoker : IDisposable
    {
        BlockingCollection<(Command command, Action<Command> action)> commands = new BlockingCollection<(Command, Action<Command>)>();
        Thread thread;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public void Add(Command command, Action<Command> action)
        {
            commands.Add((command, action));
        }
        bool run = true;
        public QueueActionInvoker()
        {
            thread = new Thread(() =>
            {
                while (run)
                {
                    var (command, action) = commands.Take(cancellationTokenSource.Token);
                    action?.Invoke(command);
                }
            })
            { Name = "QueueActionInvoker" };
            thread.Start();
        }

        public void Close(bool abort)
        {
            Dispose();
            if (abort) thread.Abort();
        }
        public void Dispose()
        {
            cancellationTokenSource.Cancel();
            run = false;
        }

    }

    public abstract class StreamWRHandler
    {
        public DataHandler DataHandler { get; }
        public abstract void ActionCommand(Command command);
        QueueActionInvoker actionInvoker = new QueueActionInvoker();
        public DictCounter<Type> __Structures { get; } = new DictCounter<Type>();
        //public DictCounter<IDynamic> __NodeDict { get; } = new DictCounter<IDynamic>();
        public DictCounter<string> __MemberNames { get; } = new DictCounter<string>();
        public StreamAcces StreamAcces { get; protected set; }
        public bool IsServer { get; protected set; }

        public virtual BinaryWriterE Writer
        {
            get => writer;
            set
            {
                writer = value;
                value.PropertyNames = PropertyNames;
                value.Types = Types;
            }
        }
        public virtual BinaryReaderE Reader
        {
            get => reader;
            set
            {
                reader = value;
                value.PropertyNames = PropertyNames;
                value.Types = Types;
            }
        }


        //public TData Data { get; }

        public StreamWRHandler(DataHandler dataHandler)
        {
            DataHandler = dataHandler ?? throw new ArgumentNullException(nameof(dataHandler));
        }

        //public StreamWRHandler(BinaryWriterE writer, BinaryReaderE reader)
        //{
        //    Writer = writer;
        //    Reader = reader;
        //}

   
        //public virtual void SendNode(DynamicNode node)
        //{
        //    new CommandRecieveObject() { Node = node }.Write(writer);
        //    //new Command(Command.CommandType.RecieveObject) { Node = node }.Write(writer);
        //}

        //public static Command CommandSendGetObject(int UID)
        //{
        //    new CommandGetObjectRequest()
        //    return new Command(Command.CommandType.GetObjectRequest)
        //        .SetUIDNode(UID);
        //}

        //public static Command CommandSendGetObject(int UID, params int[] chieldNameIDs)
        //{
        //    return new Command(Command.CommandType.GetObjectRequest)
        //        .SetUIDNode(UID)
        //        .SetNodeChildNames(chieldNameIDs);
        //}

        //public virtual void SendGetObject(int UID)
        //{
        //    CommandSendGetObject(UID).Write(writer);
        //}
        //public virtual void SendGetObject(int UID, params int[] chieldNameIDs)
        //{
        //    CommandSendGetObject(UID, chieldNameIDs).Write(writer);
        //}
        //public virtual void SendGetObject(int UID, params string[] chieldNames)
        //{

        //}

        //public virtual void GetObject(DynamicNode node)
        //{
        //    CommandSendGetObject(node.__UID).Write(writer);
        //}

        public virtual void SendObject(NETDynamic value)
        {
            writer.Write7(value.__UID);
            writer.WriteObject(value.__This);
        }
        public virtual void SendObject(DynamicNode value, int UIDParent) { }

        public event Action<Command> GetCommand;

        public void RecieveCommand(Command command)
        {
            switch (command.CmdType)
            {
                case Command.CommandType.SendTypeList:
                    break;
                case Command.CommandType.GetObjectRequest:
                    break;
                case Command.CommandType.RecieveObject:
                    foreach (var item in command.NodeValues)
                    {
                        //ServerNodeStorage[item.Key].SetValue(item.Value);
                    }
                    break;
                default:
                    break;
            }
        }
        //public virtual void Send(object value)
        //{
        //    SendObject(value);
        //}

        //public void SendObject(object obj)
        //{
        //    SendObject(0, obj);
        //}

        //public void SendObject(int idname, object obj)
        //{
        //    Writer.Write7(idname);
        //    Type typestream = Writer.GetType();
        //    Type type = obj.GetType();
        //    TypeCodeExtande typeCode =
        //    Writer.WriteType(type);
        //    //if (type.IsArray)
        //    //    typeCode = TypeCodeExtande.Array;
        //    //else
        //    //    typeCode = (TypeCodeExtande)Type.GetTypeCode(type);
        //    //Writer.Write((byte)typeCode);
        //    switch (typeCode)
        //    {
        //        case TypeCodeExtande.Array:
        //            Type typeElement = type.GetElementType();
        //            System.Array array = (System.Array)obj;
        //            int rank = type.GetArrayRank();
        //            int[] length = new int[rank];
        //            int[] indices = new int[rank];
        //            Writer.Write7(rank);
        //            for (int i = 0; i < rank; i++)
        //            {
        //                var t = length[i] = array.GetLength(i);
        //                Writer.Write7(t);
        //            }
        //            bool b1 = true;
        //            bool b2 = true;
        //            int curRank = 0;
        //            while (b1)
        //            {
        //                SendObject(array.GetValue(indices));
        //                while (true)
        //                {
        //                    var limit = ++indices[curRank];
        //                    if (limit == length[curRank])
        //                    {
        //                        indices[curRank] = 0;
        //                        curRank++;
        //                        if (curRank == rank)
        //                        {
        //                            b1 = false;
        //                            break;
        //                        }
        //                    }

        //                    else break;
        //                }
        //                curRank = 0;
        //            }
        //            break;
        //        case TypeCodeExtande.Object:
        //            var p = type.GetProperties();
        //            int idType = GetIdType(type);
        //            int childCount = p.Length;
        //            //Writer.Write(idType);
        //            Writer.Write7(childCount);
        //            foreach (var item in p)
        //            {
        //                SendObject(GetId(item), item.GetValue(obj));
        //                //item.GetValue(obj);
        //            }
        //            break;
        //        //case TypeCode.String:
        //        //    Writer.Write((byte)typeCode);
        //        //    string str = (string)obj;
        //        //    //Writer.Write(str.Length);
        //        //    Writer.Write(str);
        //        //    break;
        //        default:
        //            //Writer.Write((byte)typeCode);
        //            var m = typestream.GetMethod("Write", new Type[] { type });
        //            m.Invoke(Writer, new object[] { obj });
        //            break;
        //    }
        //}

        //public object RecieveObject(out int idTask)
        //{
        //    Type typeStream = Reader.GetType();
        //    idTask = Reader.Read7();
        //    TypeCodeExtande typeCode; //= (TypeCodeExtande)Reader.ReadByte();
        //    Type type = Reader.ReadType(out typeCode);
        //    switch (typeCode)
        //    {
        //        case TypeCodeExtande.Array:

        //            int rank = Reader.Read7();
        //            int[] length = new int[rank];
        //            int[] indices = new int[rank];
        //            //Writer.Write7(rank);
        //            for (int i = 0; i < rank; i++)
        //            {
        //                length[i] = Reader.Read7();
        //            }
        //            System.Array array = System.Array.CreateInstance(type.GetElementType(), length);
        //            bool b1 = true;
        //            bool b2 = true;
        //            int curRank = 0;
        //            while (b1)
        //            {
        //                array.SetValue(RecieveObject(out _), indices);
        //                while (true)
        //                {
        //                    var limit = ++indices[curRank];
        //                    if (limit == length[curRank])
        //                    {
        //                        indices[curRank] = 0;
        //                        curRank++;
        //                        if (curRank == rank)
        //                        {
        //                            b1 = false;
        //                            break;
        //                        }
        //                    }

        //                    else break;
        //                }
        //                curRank = 0;
        //            }
        //            return array;
        //        case TypeCodeExtande.Object:
        //            //ushort idtype = Reader.ReadUInt16();
        //            //Type type = Types[idtype];
        //            var obj1 = Activator.CreateInstance(type);
        //            int childCount = Reader.Read7();
        //            for (int i = 0; i < childCount; i++)
        //            {
        //                var obj2 = RecieveObject(out int id2);
        //                type.GetProperty(PropertyNames[id2]).SetValue(obj1, obj2);
        //            }
        //            return obj1;
        //            //TODO aaaa
        //            break;
        //        //case TypeCode.String:
        //        //    Writer.Write((byte)typeCode);
        //        //    string str = (string)obj;
        //        //    //Writer.Write(str.Length);
        //        //    Writer.Write(str);
        //        //    break;
        //        default:
        //            //Writer.Write((byte)typeCode);
        //            //Type type = Type.GetType("System." + typeCode);
        //            var m = typeStream.GetMethod("Read" + typeCode);
        //            return m.Invoke(Reader, new object[0]);
        //            break;
        //    }
        //    return null;
        //}

        protected Command Recieve(BinaryReaderE reader = null)
        {
            if (reader == null) reader = this.reader;
            Command c = Command.ReadCommand(reader);
            new Thread((object value) =>
            {
                Command cmd = (Command)value;
                Command.OnCommandRecieved(cmd);
            })
            { Name = $"CommandInvoke {IsServer}" }.Start(c);
            return c;
        }

        protected void CommandInvoke(Command command, bool inQueue, Action<Command> action)
        {
            if (inQueue)
            {
                actionInvoker.Add(command, action);
            }
            else
            {
                Thread thread = new Thread(() => action.Invoke(command)) { Name = $"CommandInvoke {command.CmdType}" };
                thread.Start();
            }
        }

        protected void Recieve(int client, BinaryReaderE reader = null)
        {
            if (reader == null) reader = this.reader;
            Command command = Command.ReadCommand(reader);
            new Thread((object value) =>
            {
                (Command command, int client) tuple = ((Command, int))value;
                Command.OnCommandRecieved(tuple.command, tuple.client);
            })
            { Name = "CommandInvoke" }.Start((command, client));
        }

        int GetId(PropertyInfo property)
        {
            return
            PropertyNames.FindKey(property.Name);
            //foreach (var item in Properties)
            //{
            //    if (item.Value == property) return item.Key;
            //}
            //throw new Exception("aaaa");
        }

        int GetIdType(Type type)
        {
            if (type.IsGenericType) type = type.GetGenericTypeDefinition();
            return Types.FindKey(type);
            //foreach (var item in Types)
            //{
            //    if (item.Value == type)
            //        return item.Key;
            //}
            //throw new Exception("aaaa");
        }

        //public Dictionary<int, PropertyInfo> Properties = new Dictionary<int, PropertyInfo>();
        public System.Collections.Concurrent.ConcurrentQueue<Command> CommandQueue = new System.Collections.Concurrent.ConcurrentQueue<Command>();
        internal DictCounter<string> PropertyNames { get; private protected set; } = new DictCounter<string>();
        internal DictCounter<Type> Types { get; private protected set; } = new DictCounter<Type>();
        //internal Dictionary<int, IDynamic> ClientNodeStorage { get; set; } = new Dictionary<int, IDynamic>();
        //internal Dictionary<int, IDynamic> ServerNodeStorage { get; set; } = new Dictionary<int, IDynamic>();

        private BinaryWriterE writer;
        private BinaryReaderE reader;

        public void AddType(Type type)
        {
            //if (type.IsGenericType) type = type.GetGenericTypeDefinition();
            if (type.GetTypeCodeExtande() == TypeCodeExtande.Structure)
            {
                if (Types.Add(type).WasContain) return;
                foreach (var item in type.GetProperties())
                {
                    PropertyNames.Add(item.Name);
                    AddType(item.PropertyType);
                }
                foreach (var item in type.GetFields())
                {
                    PropertyNames.Add(item.Name);
                    AddType(item.FieldType);
                }
            }
        }
        internal void SetTypeList(DictCounter<Type> types)
        {
            Types = types;
            writer.Types = types;
            reader.Types = types;
        }
        //internal virtual void SendTypes()
        //{
        //    new Command(Command.CommandType.SendTypeList)
        //    { dictTypes = Types.data.dict1 }.Write(writer);
        //}
       
        public event EventHandler<GetObj> ObjectGetted;

        protected void InvokeGetObj(object value)
        {
            ObjectGetted?.Invoke(this, new GetObj(new int[0], value));
        }

        //internal DynamicNode CreateNode(Type type, bool clientStorage)
        //{
        //    //TODO: reservUID
        //    int UID;
        //    if (clientStorage)
        //        UID = ++uidClientNode;
        //    else 
        //        UID = ++uidServerNode; // Изменить 
        //    DynamicNode node;
        //    TypeCodeExtande typeCode = type.GetTypeCodeExtande();
        //    switch (typeCode)
        //    {
        //        case TypeCodeExtande.Array:
        //            node = new NETDynamicArray(this, type, UID, clientStorage);
        //            break;
        //        case TypeCodeExtande.List:
        //            node = new NETDynamicList(this, type, UID, clientStorage);
        //            break;
        //        case TypeCodeExtande.Dictionary:
        //            node = new NETDynamicDictionary(this, type, UID, clientStorage);
        //            break;
        //        case TypeCodeExtande.Structure:
        //            node = new NETDynamicStruct(this, type, UID,clientStorage);
        //            break;
        //        default:
        //            node = new NETDynamic(this, type, UID, clientStorage);
        //            break;
        //    }
        //    if (clientStorage)
        //        ClientNodeStorage.Add(UID, node);
        //    else
        //        ServerNodeStorage.Add(UID, node);
        //    return node;
        //}

        public class GetObj : EventArgs
        {
            public GetObj(int[] vs, object value)
            {
                Vs = vs;
                Value = value;
            }

            public int[] Vs { get; }
            public object Value { get; }
        }
    }

    public enum StreamAcces
    {
        AllAcces,
        NotRecieveEditData,
        NotSendEditData
    }

    public enum TypeStartCmd : byte
    {
        Action,
        GetObjectRequest,
        SendObject,
        SendTypeList
    }

    public enum TypeCmd : byte
    {
      


        NodeChildNames,

        NodeEnd,


        TaskNumber,
    }

    public enum TypeCodeExtande : byte
    {
        Empty = 0,
        Object = 1,
        DBNull = 2,
        Boolean = 3,
        Char = 4,
        SByte = 5,
        Byte = 6,
        Int16 = 7,
        UInt16 = 8,
        Int32 = 9,
        UInt32 = 10,
        Int64 = 11,
        UInt64 = 12,
        Single = 13,
        Double = 14,
        Decimal = 15,
        DateTime = 16,
        String = 18,
        Array = 32,
        //SpecialObject = 33,
        List = 33,
        Dictionary = 34,
        //UntypedArray = 35,
        //UntypedList = 36,
        //UntypedDictionary = 37,
        Structure = 35,
        Error = 255,
    }
}
