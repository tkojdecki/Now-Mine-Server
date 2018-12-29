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
        public readonly static JProperty SuccessTrue = new JProperty(JsonNodes.Success, true);
        public readonly static JProperty SuccessFalse = new JProperty(JsonNodes.Success, false);

        internal static string GetFailWithMessage(string message)
        {
            var Jobj = new JObject(SuccessFalse,
                                new JProperty(JsonNodes.ErrorMessage, message));
            return Jobj.ToString();
        }

        internal static string GetSuccessWithEvent(uint eventID)
        {
            var Jobj = new JObject(SuccessTrue,
                                        new JProperty(JsonNodes.EventID, eventID));
            return Jobj.ToString();
        }

        internal static string GetQueueClipResponse(ClipQueued clip, uint eventID)
        {
            var Jobj = new JObject(SuccessTrue,
                                        new JProperty(JsonNodes.QueuePosition, clip.QPos),
                                        new JProperty(JsonNodes.QueueID, clip.QueueID),
                                        new JProperty(JsonNodes.EventID, eventID));
            return Jobj.ToString();

        }

        internal static string Serialize(object obj, CommandType cmdType)
        {
            //return JsonConvert.SerializeObject(obj);
            var Jobj = new JObject(SuccessTrue,
                            new JProperty(cmdType.ToString(), JArray.FromObject(obj)));
            return Jobj.ToString();
        }


        internal static CommandType GetCommandType(string request)
        {
            return (CommandType)JObject.Parse(request)[JsonNodes.Command].ToObject(typeof(CommandType));               
        }

        internal static T GetRequestData<T>(string response, string dataNode)
        {
            var Jobj = JObject.Parse(response);
            return JsonConvert.DeserializeObject<T>(Jobj[dataNode].ToString());
        }

        internal static T GetRequestData<T>(string response, CommandType dataNode)
        {
            return GetRequestData<T>(response, dataNode.ToString());
        }

        internal static string GetRequestString(string response, CommandType dataNode)
        {
            var Jobj = JObject.Parse(response);
            return Jobj[dataNode.ToString()].ToString();
        }
    }
}
