using System;
using System.Collections.Generic;
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
            Console.WriteLine("Sent: {0} ", message);
        }
    }
}
