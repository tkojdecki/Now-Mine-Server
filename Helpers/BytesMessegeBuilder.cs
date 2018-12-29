using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using NowMine.Models;
using NowMineCommon.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NowMineCommon.Enums;

namespace NowMine.Helpers
{
    static class BytesMessegeBuilder
    {
        public static byte[] MergeBytesArray(byte[] firstArray, byte[] secondArray)
        {
            byte[] result = new byte[firstArray.Length + secondArray.Length];
            System.Buffer.BlockCopy(firstArray, 0, result, 0, firstArray.Length);
            System.Buffer.BlockCopy(secondArray, 0, result, firstArray.Length, secondArray.Length);
            return result;
        }

        public static byte[] GetBytesFromString(string toBytes)
        {
            return Encoding.UTF8.GetBytes(toBytes);
        }

        public static string GetStringFromBytes(byte[] toString)
        {
            return Encoding.UTF8.GetString(toString);
        }

        public static byte[] GetChangeUserNameBytes(string userName, int userId, uint eventID)
        {
            var userNameBytes = Encoding.UTF8.GetBytes(userName);
            var userData = MergeBytesArray(BitConverter.GetBytes(userId), userNameBytes);
            userData = MergeBytesArray(userData, BitConverter.GetBytes(eventID));
            return MergeBytesArray(BitConverter.GetBytes((int)CommandType.ChangeName), userData);
        }

        public static byte[] GetChangeColorBytes(byte[] colorChanged, int userId, uint eventID)
        {
            var userData = MergeBytesArray(colorChanged, BitConverter.GetBytes(userId));
            userData = MergeBytesArray(userData, BitConverter.GetBytes(eventID));
            return MergeBytesArray(BitConverter.GetBytes((int)CommandType.ChangeColor), userData);
        }

        public static byte[] GetPlayedNextBytes(uint queueID, uint eventID)
        {
            var commandBytes = BitConverter.GetBytes((int)CommandType.PlayNext);
            var queueIDBytes = BitConverter.GetBytes(queueID);
            var message = MergeBytesArray(commandBytes, queueIDBytes);
            var eventIDBytes = BitConverter.GetBytes(eventID);
            return MergeBytesArray(message, eventIDBytes);
        }

        public static byte[] GetQueuePieceBytes(ClipQueued clip, uint eventID)
        {
            MemoryStream ms = new MemoryStream();
            using (BsonDataWriter writer = new BsonDataWriter(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, clip);

                var data = ms.ToArray();
                byte[] queueString = BitConverter.GetBytes((int)CommandType.QueueClip);
                var message = MergeBytesArray(queueString, data);
                var eventIdBytes = BitConverter.GetBytes(eventID);
                return MergeBytesArray(message, eventIdBytes);
            }
        }

        internal static byte[] GetRemovedPieceBytes(uint queueID, uint eventID)
        {
            var command = BitConverter.GetBytes((int)CommandType.DeleteClip);
            var queueIDBytes = BitConverter.GetBytes(queueID);
            var message = MergeBytesArray(command, queueIDBytes);
            var eventIDBytes = BitConverter.GetBytes(eventID);
            return MergeBytesArray(message, eventIDBytes);
        }

        public static byte[] GetPlayedNowBytes(int qPos, uint eventID)
        {
            byte[] playedNowBytes = BitConverter.GetBytes((int)CommandType.PlayNow);
            byte[] qPosBytes = BitConverter.GetBytes(qPos);
            var message = MergeBytesArray(playedNowBytes, qPosBytes);
            var eventIDBytes = BitConverter.GetBytes(eventID);
            return MergeBytesArray(message, eventIDBytes);
        }

        public static User DeserializeUser(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            using (BsonDataReader reader = new BsonDataReader(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<User>(reader);
            }
        }

        public static byte[] GetShutdownBytes(uint eventID)
        {
            var commandBytes = BitConverter.GetBytes((int)CommandType.ServerShutdown);
            return MergeBytesArray(commandBytes, BitConverter.GetBytes(eventID));
        }
    }
}