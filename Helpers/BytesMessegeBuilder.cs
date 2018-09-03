using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using NowMine.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NowMine.Helpers
{
    class BytesMessegeBuilder
    {
        public static byte[] MergeBytesArray(byte[] firstArray, byte[] secondArray)
        {
            byte[] result = new byte[firstArray.Length + secondArray.Length];
            System.Buffer.BlockCopy(firstArray, 0, result, 0, firstArray.Length);
            System.Buffer.BlockCopy(secondArray, 0, result, firstArray.Length, secondArray.Length);
            return result;
        }

        public static byte[] SerializeQueuePieceToSend(NetworkClipInfo[] listToSerialize)
        {
            MemoryStream ms = new MemoryStream();
            using (BsonDataWriter writer = new BsonDataWriter(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, listToSerialize, typeof(NetworkClipInfo[]));
                return ms.ToArray();
            }
        }

        public static ClipInfo DeserializeYoutubeInfo(MemoryStream ms)
        {
            using (BsonDataReader reader = new BsonDataReader(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<ClipInfo>(reader);
            }
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

        public static byte[] SerializeUsers(List<User> users)
        {
            MemoryStream ms = new MemoryStream();
            using (BsonDataWriter writer = new BsonDataWriter(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, users.ToArray(), typeof(User[]));
                return ms.ToArray();
            }
        }
    }
}
