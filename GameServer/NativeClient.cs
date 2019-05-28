﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.ClientObject
{
    //[JsonConverter(typeof(TCPCommandConverter))]
    public abstract class JsonObject
    {
        [JsonIgnore]
        public static Type TypeDefBase => typeof(JsonObject);
        public static Type GetType(string typeName) => Type.GetType($"{TypeDefBase.Namespace}.{typeName}", false, true);
        public static T FromJson<T>(string json) where T : JsonObject =>
            JsonConvert.DeserializeObject<T>(json);

        public static JsonObject FromJson(string json, Type type) =>
            (JsonObject)JsonConvert.DeserializeObject(json, type);

        public string ToJson() =>
            JsonConvert.SerializeObject(this);

        public static JsonObject FromJsonSendTCP(string json)
        {
            var vs = json.Split(new char[] { '#' }, 3);
            string command = vs[1].ToUpper();
            string body = vs[2];
            Type type = GetType(command);
            return (JsonObject)FromJson(body, type);
        }

        public string ToJsonSendTCP() => $"#{GetType().Name}#{ToJson()}";


    }

    //[Newtonsoft.Json.JsonObject()]
    public class PlayerCMD : JsonObject
    {
        public int player;
        [Newtonsoft.Json.JsonConverter(typeof(TCPCommandConverter))]
        public JsonObject cmd;
    }

    public class ArrCMD : JsonObject
    {
        [Newtonsoft.Json.JsonProperty("")]
        [Newtonsoft.Json.JsonConverter(typeof(TCPCommandConverter))]
        public List<JsonObject> vs;
    }

    public class LogIn : JsonObject
    {
        public string name;
        public Guid guid;
    }

    public class PlayerGUID : JsonObject
    {
        [Newtonsoft.Json.JsonProperty("")]
        public Guid guid;
    }

    public class ID : JsonObject
    {
        [Newtonsoft.Json.JsonProperty("")]
        public int id;
    }

    public class CellColor : JsonObject
    {
        public Color color1;
        public Color color2;
    }
   
    public class Step : JsonObject
    {
        public int prefabID;
        public Point Point;
        public GameCore.Prefab.Rotate Rotate;
        public GameCore.Prefab.Rotate Flip;
    }

    //public class TurnAccept : TurnSend
    //{
    //    [JsonIgnore]
    //    public static Type TypeDef => typeof(TurnAccept);
    //    public int player;
    //}

    public class Win : JsonObject
    {
    }

    public class Name : JsonObject
    {
        [JsonProperty("")]
        public string playerName;
    }

    public class NewPlayer : JsonObject
    {
        public int id;
        public string playerName;
    }

    public class LostPlayer : Message
    {
    }

    public class Message : JsonObject
    {
        public string message;
    }

    public class Ready : JsonObject
    {
        public static Type TypeDef => typeof(Ready);
        public bool isReady;
    }

    public class Exit : JsonObject
    {

    }



    //public class PlayerCMDConverter : JsonConverter
    //{
    //    public override bool CanConvert(Type objectType)
    //    {
    //        return objectType == PlayerCMD.typeDef;
    //    }

    //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //    {
    //        JToken t = JToken.FromObject(value);

    //        if(value is PlayerCMD playerCMD)
    //        {
    //            JObject o = (JObject)t;
    //            o.
    //        }
    //        if (t.Type != JTokenType.Object)
    //        {
    //            t.WriteTo(writer);
    //        }
    //        else
    //        {
    //            JObject o = (JObject)t;
    //            IList<string> propertyNames = o.Properties().Select(p => p.Name).ToList();

    //            o.AddFirst(new JProperty("Keys", new JArray(propertyNames)));

    //            o.WriteTo(writer);
    //        }
    //    }
    //}

    public class TCPCommandConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == JsonObject.TypeDefBase || objectType.IsSubclassOf(JsonObject.TypeDefBase) || objectType.GetInterfaces().Contains(typeof(IList));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == JsonObject.TypeDefBase || objectType.IsSubclassOf(JsonObject.TypeDefBase))
            {
                if (reader.ValueType == typeof(string))
                {
                    string str = (string)reader.Value;
                    return JsonObject.FromJsonSendTCP(str);
                }
                return reader.Value;
            }
            else if( objectType.GetInterfaces().Contains(typeof(IList)))
            {
                JArray jArray = JArray.Load(reader);
                var strList = jArray.ToList();
                List<JsonObject> retList = new List<JsonObject>();
                foreach (var item in strList)
                {
                    retList.Add(JsonObject.FromJsonSendTCP(item.ToString()));
                }
                return retList;
            }
            else return JsonConvert.DeserializeObject(reader.ReadAsString(), objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is JsonObject jsonObject)
            {
                writer.WriteValue(jsonObject.ToJsonSendTCP());
            }
            else if(value is IEnumerable<JsonObject> vs)
            {
                JArray jArray = new JArray();
                foreach (var item in vs)
                {
                    jArray.Add(item.ToJsonSendTCP());
                }
                jArray.WriteTo(writer);
                //writer.WriteValue(jArray);
            }
            else writer.WriteValue(value);
            //JToken t = JToken.FromObject(value);

            //if (value is PlayerCMD playerCMD)
            //{
            //    JObject o = (JObject)t;
            //    o.
            //}
            //if (t.Type != JTokenType.Object)
            //{
            //    t.WriteTo(writer);
            //}
            //else
            //{
            //    JObject o = (JObject)t;
            //    IList<string> propertyNames = o.Properties().Select(p => p.Name).ToList();

            //    o.AddFirst(new JProperty("Keys", new JArray(propertyNames)));

            //    o.WriteTo(writer);
            //    writer.WriteValue($"#{value.GetType().Name}#{JsonConvert.SerializeObject(value, new JsonSerializerSettings())}");
            //}
        }
    }

}