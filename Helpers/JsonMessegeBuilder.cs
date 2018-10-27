using Newtonsoft.Json.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NowMineCommon.Models;
using Newtonsoft.Json.Linq;
using NowMine.Models;
using NowMineCommon.Enums;

namespace NowMine.Helpers
{
    static class JsonMessegeBuilder
    {
        //public readonly static string ResponseYes = string.Format("{{\"Success\": true}}");
        //public readonly static string ResponseNo = string.Format("{{\"Success\": false}}");
        public readonly static JObject SuccessTrue = new JObject(new JProperty("Success", true));
        public readonly static JObject SuccessFalse = new JObject(new JProperty("Success", false));

        //public static string SerializeQueuePieceToSend(ClipQueued[] listToSerialize)
        //{
        //    return JsonConvert.SerializeObject(listToSerialize);
        //}

        //public static ClipInfo DeserializeYoutubeInfo(string clipInfoSerialized)
        //{
        //    return JsonConvert.DeserializeObject(clipInfoSerialized) as ClipInfo;
        //}

        //public static User DeserializeUser(string serializedUser)
        //{
        //    return JsonConvert.DeserializeObject(serializedUser) as User;
        //}

        //public static string SerializeUsers(List<User> users)
        //{
        //    //return JsonConvert.SerializeObject(users, typeof(List<User>), null);
        //    return JsonConvert.SerializeObject(users);
        //}

        internal static string GetQueueClipResponse(int qPos, uint queueID, uint eventID)
        {
            JObject response = new JObject(SuccessTrue.Property("Success"),
                                        new JProperty("QueuePosition", qPos),
                                        new JProperty("QueueID", queueID),
                                        new JProperty("EventID", eventID));
            return response.ToString();

        }

        //internal static string SerializeGetQueue(ClipQueued[] ytInfo)
        //{ 
        //    return JsonConvert.SerializeObject(ytInfo);
        //}

        internal static string Serialize(object obj, CommandType cmdType)
        {
            //return JsonConvert.SerializeObject(obj);
            var Jobj = new JObject(SuccessTrue.Property("Success"),
                            new JProperty(cmdType.ToString(), JArray.FromObject(obj)));
            return Jobj.ToString();
        }


        internal static CommandType GetCommandType(string request)
        {
            return (CommandType)JObject.Parse(request)["Command"].ToObject(typeof(CommandType));               
        }

        //internal static string GetChangeName(string request)
        //{
        //    return JObject.Parse(request)["ChangeName"].ToObject(typeof(string)) as string;
        //}

        //internal static byte[] GetChangeColor(string request)
        //{
        //    return (byte[])JObject.Parse(request)["ChangeColor"].ToObject(typeof(byte[]));
        //}

        //internal static uint GetQueueID(string request)
        //{
        //    return (uint)JObject.Parse(request)["QueueID"].ToObject(typeof(uint));
        //}


        internal static T GetStandardRequestData<T>(string response, string dataNode)
        {
            //return JsonConvert.DeserializeObject<T>(response);
            //return (T)JObject.Parse(response)[dataNode].ToObject(typeof(T));

            var Jobj = JObject.Parse(response);
            return JsonConvert.DeserializeObject<T>(Jobj[dataNode].ToString());
            //return Jobj[dataNode].ToObject<T>();
            //return Jobj.ToObject<T>();
            //var ResponseData = Jobj.GetValue(dataNode);
            //return ResponseData.ToObject<T>();
        }

        internal static T GetStandardRequestData<T>(string response, CommandType dataNode)
        {
            return GetStandardRequestData<T>(response, dataNode.ToString());
        }
    }
}
