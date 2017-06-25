using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using NowMine.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NowMine
{
    class ServerUDP
    {
        private UdpClient udp;
        private ServerTCP tcp;

        public ServerUDP(ServerTCP tcp)
        {
            this.tcp = tcp;
        }

        public void udpListener()
        {
            int PORT_NUMBER = 1234;

            Console.WriteLine("UDP/ Starting UDP Listener");
            if (udp == null)
                udp = new UdpClient(PORT_NUMBER);
            IAsyncResult ar_ = udp.BeginReceive(Receive, new object());
        }

        private void Receive(IAsyncResult ar)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 1234);
            byte[] bytes = udp.EndReceive(ar, ref ip);
            if (ip.Address == tcp.serverIP)
                return;
            string message = Encoding.ASCII.GetString(bytes);
            Console.WriteLine("UDP/ From {0} received: {1} ", ip.Address.ToString(), message);
            if (message.Equals("NowMine!"))
            {
                Console.WriteLine("TCP/ Connecting to: {0}", ip.Address.ToString());
                tcp.sendServerIP(ip.Address);
                //UDPSend(tcp.serverIP.ToString());
            }
            udpListener();
        }

        public void UDPSend(string message)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, 1234);
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            udp.Send(bytes, bytes.Length, ip);
            //udp.Close();
            Console.WriteLine("UDP Sent: {0} ", message);
        }

        public void UDPSend(byte[] message)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, 1234);
            udp.Send(message, message.Length, ip);
            //udp.Close();
            Console.WriteLine("UDP Sent: {0} ", message);
        }


        public void sendQueuedPiece(object o, MusicPieceReceivedEventArgs e)
        {
            MemoryStream ms = new MemoryStream();
            using (BsonWriter writer = new BsonWriter(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, e.YoutubeQueued);

                var data = ms.ToArray();
                byte[] queueString = Encoding.UTF8.GetBytes("Queue: ");
                byte[] message = BytesMessegeBuilder.MergeBytesArray(queueString, data);
                //message = BytesMessegeBuilder.MergeBytesArray(message, BitConverter.GetBytes(e.YoutubeQueued.userId)); //adding userid on end
                Console.WriteLine("UPD/ Sending to Broadcast: {0}", Convert.ToBase64String(message));
                UDPSend(message);
            }
        }

        public void sendQueuedPiece(object o, GenericEventArgs<byte[]> e)
        {
            UDPSend(e.EventData);
        }

        internal void playedNow(object s, PlayedNowEventArgs e)
        {
            byte[] playedNowBytes = Encoding.UTF8.GetBytes("PlayedNow: ");
            byte[] qPosBytes = BitConverter.GetBytes(e.qPos);
            byte[] message = BytesMessegeBuilder.MergeBytesArray(playedNowBytes, qPosBytes);
            UDPSend(message);
        }

        internal void DeletedPiece(object source, EventArgs args)
        {
            UDPSend("Delete: 0");
        }
    }
}
