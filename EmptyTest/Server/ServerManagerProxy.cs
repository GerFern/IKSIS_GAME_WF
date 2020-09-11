//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Text;

//namespace EmptyTest.Proxy
//{
//    class ServerManagerProxy<TServer, TClient> : DispatchProxy
//    {
//        Server<TServer, TClient> owner;
//        TServer real;
//        public static TServer Create<TServer, TClient>(TServer real, Server<TServer, TClient> owner)
//        {
//            object var = Create<TServer, ServerManagerProxy<TServer, TClient>>();
//            ServerManagerProxy<TServer, TClient> proxy = (ServerManagerProxy<TServer, TClient>)var;
//            proxy.owner = owner;
//            proxy.real = real;
//            return (TServer)var;
//        }
//        protected override object Invoke(MethodInfo targetMethod, object[] args)
//        {
//            owner.
//        }
//    }
//}
