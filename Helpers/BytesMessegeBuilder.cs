using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.IO;

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

        public static byte[] SerializeQueuePieceToSend(QueuePieceToSend[] listToSerialize)
        {
            MemoryStream ms = new MemoryStream();
            using (BsonDataWriter writer = new BsonDataWriter(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, listToSerialize, typeof(QueuePieceToSend[]));
                return ms.ToArray();
            }
        }

        public static YouTubeInfo DeserializeYoutubeInfo(MemoryStream ms)
        {
            using (BsonDataReader reader = new BsonDataReader(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<YouTubeInfo>(reader);
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

        public static byte[] SerializeUsers(User[] users)
        {
            MemoryStream ms = new MemoryStream();
            using (BsonDataWriter writer = new BsonDataWriter(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, users, typeof(User[]));
                return ms.ToArray();
            }
        }
    }
}
