using EmptyTest.TCommand;
using EmptyTest.TExtensions;
using EmptyTest.TNode;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace EmptyTest.TStreamHandler
{
    public class PlayerClient : StreamWRHandler
    {
        public bool IsAuth;
        Server server;
        public bool IsPrivate;
        private Socket _handler;
        private Thread _thread;
        public Guid PrivateID;
        public int PublicID;
        public string Name;
        NetworkStream Stream;
        internal NETDynamicMainContainer ServerContainer => server.ServerContainer;
        internal NETDynamicMainContainer ClientContainer;
        public PlayerClient(Server server, Socket socket, DataHandler dataHandler) : base(dataHandler)
        {
            this.server = server;
            _handler = socket;
            Stream = new NetworkStream(socket);
            Reader = new BinaryReaderE(Stream);
            Writer = new BinaryWriterE(Stream);
            this.Types = server.Types;
            this.PropertyNames = server.PropertyNames;
            //this.AddType(typeof(Structures.MainContainer));
            ClientContainer = new NETDynamicMainContainer(this, dataHandler, true);
            //ClientContainer = (NETDynamicStruct)this.CreateNode(typeof(TStructures.Substruct1), true);
            ((DynamicNode)ClientContainer).SetValue(Activator.CreateInstance(typeof(TStructures.Substruct1)).SetAllNullStringEmpty());
            _thread = new Thread(Listner);
            _thread.IsBackground = true;
            _thread.Start();
        }
        //public string UserName
        //{
        //    get { return _userName; }
        //}
        private void Listner()
        {
            while (true)
            {
                try
                {
                    CommandInvoke(Recieve(), true, ActionCommand);
                }
                catch {
                    //Program.Server.EndClient(this);
                    return;
                }
            }
        }
        public void End()
        {
            try
            {
                _handler.Close();
                try
                {
                    _thread.Abort();
                }
                catch { } // г
            }
            catch (Exception exp) { Console.WriteLine("Error with end: {0}.", exp.Message); }
        }

        private static readonly ReadOnlyDictionary<Type, Action<PlayerClient,Command>> actions =
            new ReadOnlyDictionary<Type, Action<PlayerClient, Command>>(new Dictionary<Type, Action<PlayerClient, Command>>()
            {
                {typeof(CommandLogIn), (client, cmd)=>{ } },
                {typeof(CommandWelcomeClient), (client, cmd)=>{ } },
                {typeof(CommandGetTypeListRequest), (client, cmd)=>{ } },
                {typeof(CommandSendTypeList), (client, cmd)=>
                    {
                        var c = (CommandSendTypeList)cmd;
                        var Types =new DictCounter<Type>();
                        client.Types = Types;
                        foreach (var item in c.DictTypes)
                        {
                            Types.data.Add(item.Key, item.Value);
                        }
                    }
                },
                {typeof(CommandGetObjectRequest), (client, cmd)=>{ } },
                {typeof(CommandRecieveObject), (client, cmd)=>{ } },
                {typeof(CommandAction), (client, cmd)=>{ } },
                {typeof(CommandActionResult), (client, cmd)=>{ } },
                {typeof(CommandDisconnect), (client, cmd)=>{ } },
            });
        /// <summary>
        /// Обработка команд
        /// </summary>
        public override void ActionCommand(Command command)
        {
            actions[command.GetType()].Invoke(this, command);
            //switch (command.CmdType)
            //{
            //    case Command.CommandType.SendTypeList:
            //        if (this.Types == null || this.Types.Count == 0)
            //        {
            //            this.Types = new DictCounter<Type>();
            //            foreach (var item in command.dictTypes)
            //            {
            //                Types.data.Add(item.Key, item.Value);
            //            }
            //        }
            //        break;
            //    case Command.CommandType.GetObjectRequest:
            //        break;
            //    case Command.CommandType.RecieveObject:
            //        break;
            //    default:
            //        break;
            //}
        }


    }

}
