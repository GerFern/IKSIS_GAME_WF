using EmptyTest.TExtensions;
using EmptyTest.TNode;
using EmptyTest.TStreamHandler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace EmptyTest.TCommand
{
    public abstract class Command
    {
        public int ByteCounter;
        public bool IsReaded { get; }
        public abstract CommandType CmdType { get; }

        public Guid PlayerGUID;
        public Dictionary<int, object> NodeValues;
        protected Command()
        {
            IsReaded = false;
        }
        public Command(BinaryReaderE reader) : this()
        {
            InvokeReadWrite(reader);
        }
        //public Command(CommandType commandType)
        //{
        //    IsReaded = false;
        //    this.CmdType = commandType;
        //}
        public static event Action<Command> CommandRecieved;
        public static event Action<Command, int> CommandRecievedFromClient;
        internal static void OnCommandRecieved(Command command) => CommandRecieved?.Invoke(command);
        internal static void OnCommandRecieved(Command command, int client) => CommandRecievedFromClient?.Invoke(command, client);

        public bool getThisNodeValue;
        public bool writeStartCommand;
        public DynamicNode Node;
        //public int nodeChildrenCount;
        public int[] nodeChildNames;
        public Dictionary<int, (int uid, object value)> nodeChildValues;

        //public void WriteCMD(TypeCmd cmd, BinaryWriterE writer)
        //{
        //    writer.WriteEnum(cmd);
        //    if (cmd == TypeCmd.NodeChildNames)
        //    {
        //        writer.Write7(nodeChildNames.Length);
        //        foreach (var item in nodeChildNames)
        //        {
        //            writer.Write7(item);
        //        }
        //    }
        //    else if (cmd == TypeCmd.NodeEnd)
        //    {

        //    }
        //}

        //public TypeCmd ReadCMD(BinaryReaderE reader)
        //{
        //    TypeCmd cmd = reader.ReadEnum<TypeCmd>();

        //    if (cmd == TypeCmd.NodeChildNames)
        //    {
        //        nodeChildNames = new int[
        //        reader.Read7()];
        //        nodeChildNames.ForEachSet(() => reader.Read7());
        //    }


        //    return cmd;
        //}



        //public void SendCommand(BinaryWriterE binaryWriterE)
        //{
        //    lock (binaryWriterE.locked)
        //    {
        //        binaryWriterE.WriteEnum(commandType);
        //        switch (commandType)
        //        {
        //            case CommandType.SendTypeList:
        //                InvokeTypeList(binaryWriterE, dictTypes);
        //                break;
        //            case CommandType.GetObjectRequest:
        //                break;
        //            case CommandType.RecieveObject:
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //}

        public void Write(BinaryWriterE writer)
        {
            lock (writer.locked)
            {
                this.InvokeReadWrite(writer);
                //if (this.CmdType == null) Console.WriteLine("Комманда для отправки не определенна");
                //else
                //{
                //    writer.WriteEnum(this.CmdType);
                //    switch (CmdType)
                //    {
                //        case CommandType.GetObjectRequest:
                //            InvokeGetObjectRequest(writer, ref this.Node.__UID);
                //            break;
                //        case CommandType.SendTypeList:
                //            InvokeTypeList(writer, dictTypes);
                //            break;
                //        case CommandType.RecieveObject:
                //            List<NETDynamic> nodeList = new List<NETDynamic>();
                //            Node.GetRecourciveSimpleNode(nodeList);
                //            NodeValues = nodeList.ToDictionary<NETDynamic, int, object>(node => node.__UID, node => node.__This);
                //            InvokeRecieveObject(writer, NodeValues);
                //            break;
                //    }
                //}
            }
        }

        //public Command(BinaryReaderE reader)
        //{
        //    InvokeReadWrite(reader);
        //}

        public static Command ReadCommand(BinaryReaderE reader)
        {
            lock (reader.locked)
            {
                CommandType commandType = reader.ReadEnum<CommandType>();
                Console.WriteLine($"Принято {commandType}");
                switch (commandType)
                {
                    case CommandType.LogIn:
                        return new CommandLogIn(reader);
                    case CommandType.WelcomeClient:
                        return new CommandWelcomeClient(reader);
                    case CommandType.GetTypeListRequest:
                        return new CommandGetTypeListRequest(reader);
                    case CommandType.SendTypeList:
                        return new CommandSendTypeList(reader);
                    case CommandType.GetObjectRequest:
                        return new CommandGetObjectRequest(reader);
                    case CommandType.RecieveObject:
                        return new CommandRecieveObject(reader);
                    case CommandType.Action:
                        return new CommandAction(reader);
                    case CommandType.ActionResult:
                        return new CommandActionResult(reader);
                    case CommandType.Disconnect:
                        return new CommandDisconnect(reader);
                    default:
                        throw new Exception("AAA");
                }
            }
        }

        //public void Read(BinaryReaderE reader)
        //{
        //    lock (reader.locked)
        //    {
        //        CmdType = reader.ReadEnum<CommandType>();
        //        Console.WriteLine($"Принято {CmdType}");
        //        switch (CmdType)
        //        {
        //            case CommandType.RecieveObject:
        //                NodeValues = new Dictionary<int, object>();
        //                InvokeRecieveObject(reader, this.NodeValues);
        //                break;
        //            case CommandType.SendTypeList:
        //                dictTypes = new Dictionary<int, Type>();
        //                InvokeTypeList(reader, dictTypes);
        //                break;
        //            case CommandType.GetObjectRequest:
        //                InvokeGetObjectRequest(reader, ref this.UIDNode);
        //                break;
        //            default:
        //                Console.WriteLine($"Неизвестный номер комманды: {CmdType}. Возможно все сгинет в пламени!");
        //                break;
        //        }
        //    }
        //}

        protected abstract void InvokeReadWrite(IReaderWriterAlliaces alliaces);

        protected static void InvokeRecieveObject(IReaderWriterAlliaces alliaces, Dictionary<int, object> nodeValues)
        {
            int count = default;
            if(alliaces.IsWrite)
            {
                count = nodeValues.Count;
                alliaces.InvokeInt7(ref count);
                foreach (var item in nodeValues)
                {
                    int key = item.Key;
                    object value = item.Value;
                    alliaces.InvokeInt7(ref key);
                    alliaces.InvokeSimpleObject(ref value);
                }
            }
            else
            {
                alliaces.InvokeInt7(ref count);
                for (int i = 0; i < count; i++)
                {
                    int key = default;
                    object value = null;
                    alliaces.InvokeInt7(ref key);
                    alliaces.InvokeSimpleObject(ref value);
                    nodeValues.Add(key, value);
                }
            }
        }

        //public static Command ReadCommand(BinaryReaderE reader, int client)
        //{
        //    Command command = new Command();
        //    command.Read(reader);
        //    Command.CommandRecievedFromClient?.Invoke(command, client);
        //    return command;
        //}


        //protected void Write_GetObjectRequest(BinaryWriterE writer)
        //{
        //    if (writeStartCommand) writer.Write((byte)TypeStartCmd.GetObjectRequest);
        //    writer.Write7(UIDNode);
        //    int l = nodeChildNames.Length;
        //    if (l==0)
        //        writer.Write((byte)TypeCmd.NodeEnd);
        //    else
        //    {
        //        writer.Write((byte)TypeCmd.NodeChildNames);
        //        writer.Write7(l);
        //        foreach (var item in nodeChildNames)
        //        {
        //            writer.Write7(item);
        //        }
        //        writer.Write((byte)TypeCmd.NodeEnd);
        //    }
        //}

        protected void GetObjectRequest(BinaryWriterE writer, int UID, ICollection<int> chieldIDs)
        {
            lock (writer.locked)
            {
                writer.Write((byte)TypeStartCmd.GetObjectRequest);
                writer.Write7(UID);
                writer.Write((byte)TypeCmd.NodeChildNames);
                writer.Write7(chieldIDs.Count);
                foreach (var item in chieldIDs)
                {
                    writer.Write7(item);
                }
                writer.Write((byte)TypeCmd.NodeEnd);
            }
        }


//        public static Command CreateCommand(BinaryReaderE reader)
//        {
//            //Command command = new Command(readerE.ReadEnum<CommandType>());
//            Command command;
//Re:
//            CommandType commandType = reader.ReadEnum<CommandType>();
//            switch (commandType)
//            {
//                case CommandType.LogIn:
//                    command = new CommandLogIn();
//                    break;
//                case CommandType.WelcomeClient:
//                    command = new CommandWelcomeClient();
//                    break;
//                case CommandType.GetTypeList:
//                    command = new CommandGetTypeList();
//                    break;
//                case CommandType.SendTypeList:
//                    command = new CommandSendTypeList();
//                    break;
//                case CommandType.GetObjectRequest:
//                    command = new CommandGetObjectRequest();
//                    break;
//                case CommandType.RecieveObject:
//                    command = new CommandRecieveObject();
//                    break;
//                case CommandType.Action:
//                    command = new CommandAction();
//                    break;
//                case CommandType.ActionResult:
//                    command = new CommandActionResult();
//                    break;
//                case CommandType.Disconnect:
//                    command = new CommandDisconnect();
//                    break;
//                default:
//                    Console.WriteLine($"Неизвестный код комманды {commandType}. Возможно потоки чтения записи не синхронизированны");
//                    goto Re;
//            }
//            command.InvokeReadWrite(reader);
//            command.Read(reader);
//            //Command.CommandRecieved?.Invoke(command);
//            return command;
//        }

//        public static Command CreateCommand(BinaryReaderE readerE, int client)
//        {
//            //Command command = new Command(readerE.ReadEnum<CommandType>());
//            Command command = new Command();
//            command.Read(readerE);
//            //Command.CommandRecievedFromClient?.Invoke(command, client);
//            return command;
//        }

        protected static void InvokeTypeList(IReaderWriterAlliaces alliaces, Dictionary<int, Type> dictTypes)
        {
            int length = default;
            if (alliaces.IsWrite) length = dictTypes.Count;
            alliaces.InvokeInt7(ref length);
            if (alliaces.IsWrite)
            {
                dictTypes.ForEachGet((int key, Type value) =>
                {
                    alliaces.InvokeInt7(ref key);
                    string name = value.FullName;
                    alliaces.InvokeString(ref name);
                });
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    int key = default;
                    alliaces.InvokeInt7(ref key);
                    string name = default;
                    alliaces.InvokeString(ref name);
                    dictTypes[key] = Type.GetType(name);
                }
            }
        }

        protected static void InvokeGetObjectRequest(IReaderWriterAlliaces alliaces, ref int UID)
        {
            alliaces.InvokeInt7(ref UID);
        }

        protected static void InvokeRecieveObject(IReaderWriterAlliaces alliaces, StreamWRHandler handler, DynamicNode node)
        {
            List<int> uidList = new List<int>();
            int[] uids = null;
            if(alliaces.IsWrite)
            {
                node.GetRecourciveUID(uidList);
                uids = uidList.ToArray();
            }
            alliaces.InvokeIntArray(ref uids);
            foreach (var item in uids)
            {
            }
        }


        public enum CommandType : byte
        {
            [Description("Авторизация")]
            LogIn = 10,
            [Description("Авторизация пройдена")]
            WelcomeClient,
            [Description("Запрос списка структур")]
            GetTypeListRequest,
            [Description("Получение списка структур")]
            SendTypeList,
            [Description("Запрос объекта")]
            GetObjectRequest,
            [Description("Получение объекта")]
            RecieveObject,
            [Description("Отправить действие")]
            Action,
            [Description("Результат действия")]
            ActionResult,
            [Description("Отсоединение")]
            Disconnect
        }

        public enum ReadWriteAllias
        {
            Boolean,
            Byte,
            Int16,
            Int32,
            Float,
            Double,
            String,
            Int7,
            Enum
        }
    }
}
