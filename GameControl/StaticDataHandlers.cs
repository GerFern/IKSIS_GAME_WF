using EmptyTest.TStreamHandler;
using GameCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore
{
    public static class StaticDataHandlers
    {
        static bool _init = false;
        static DataHandler server;
        static DataHandler client;
        static void Init()
        {
            ReadOnlyDoubleDict<int, Type> types = null;
            ReadOnlyDoubleDict<int, string> names = null;
            server = new DataHandler(typeof(ServerContainer), types, names);
            client = new DataHandler(typeof(ClientContainer), types, names);
            _init = true;
        }

        public static DataHandler ServerDataHandler
        {
            get
            {
                if (_init) return server;
                else
                {
                    Init();
                    return server;
                }
            }
        }

        public static DataHandler ClientDataHandler
        {
            get
            {
                if (_init) return client;
                else
                {
                    Init();
                    return client;
                }
            }
        }
    }
}
