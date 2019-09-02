//using GameServer.ClientObject;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace gtk_test
//{
//    interface ISyncClientServerObjects
//    {
//        public bool IsSync { get; set; }
//        public void AddThis() => Objects.Add(this);
//        public void DestroyThis() => Objects.Remove(this);
//        public static List<ISyncClientServerObjects> Objects;
//        //public static event EventHandler Sync;
//        public void SendUpdate();
//        public void RecieveUpdate();
//        public static event EventHandler SendEvent;
//        public static event EventHandler RecieveEvent;
//        public JsonObject GetJsonObject();
//        public void UseJsonObject(JsonObject jsonObject);

//    }
//}
