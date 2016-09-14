using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace NowMine
{
    class Server
    {
        QueuePanel queuePanel;
        UdpClient udp;
        const int TCP_PORT = 4444;
        IPAddress tcpIp = null;
        public void ServerInit(QueuePanel queuePanel)
        {
            this.queuePanel = queuePanel;
            
            try
            {
                string hostName = Dns.GetHostName();
                IPAddress[] IpA = Dns.GetHostAddresses(hostName);
                for (int i = 0; i < IpA.Length; i++)
                {
                    //Console.WriteLine("IP Address {0}: {1} ", i, IpA[i].ToString());
                    if (IpA[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        tcpIp = IpA[i];
                    }
                }

                /* Initializes the Listener */
                TcpListener myList = new TcpListener(tcpIp, TCP_PORT);

                /* Start Listeneting at the specified port */
                myList.Start();

                Console.WriteLine("The server is running at port 4444...");
                Console.WriteLine("The local End point is  :" +
                                  myList.LocalEndpoint);
                Console.WriteLine("Waiting for a connection.....");
                //m:
                while (true)
                {
                    Socket s = myList.AcceptSocket();
                    Console.WriteLine("Connection accepted from " + s.RemoteEndPoint);

                    byte[] b = new byte[100];
                    int k = s.Receive(b);

                    char cc = ' ';
                    string test = null;
                    Console.WriteLine("Recieved...");
                    for (int i = 0; i < k - 1; i++)
                    {
                        Console.Write(Convert.ToChar(b[i]));
                        cc = Convert.ToChar(b[i]);
                        test += cc.ToString();
                    }

                    switch (test)
                    {
                        case "1":
                            Console.WriteLine("doszło 1");
                            break;

                        case "Play Next":
                            queuePanel.getNextVideo();
                            Console.WriteLine("Playing Next!");
                            break;

                        default:
                            Console.WriteLine("Can't interpret right");
                            break;

                    }
                    ASCIIEncoding asen = new ASCIIEncoding();
                    //s.Send(asen.GetBytes("The string was recieved by the server."));
                    //s.Close();

                    /* clean up */
                    //goto m;
                    //s.Close();
                    //myList.Stop();
                    //Console.ReadLine();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        }

        public void udpListener()
        {
            int PORT_NUMBER = 1234;

            Console.WriteLine("Starting UDP Listener");
            if (udp == null)
                udp = new UdpClient(PORT_NUMBER);
            IAsyncResult ar_ = udp.BeginReceive(Receive, new object());
        }

        private void Receive(IAsyncResult ar)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 1234);
            byte[] bytes = udp.EndReceive(ar, ref ip);
            string message = Encoding.ASCII.GetString(bytes);
            Console.WriteLine("From {0} received: {1} ", ip.Address.ToString(), message);
            if (message.Equals("NowMine!"))
            {
                Console.WriteLine("Connecting to: {0}", ip.Address.ToString());
                //UDPSend(tcpIp.ToString());
                TCPConnect(ip);
                //Console.WriteLine("Connecting to: {0}", ip.Address.ToString());
            }
        }

        private void TCPConnect(IPEndPoint ip)
        {
            try
            {
                TcpClient tcpclnt = new TcpClient();
                Console.WriteLine("Connecting.....");

                tcpclnt.Connect(ip.Address.ToString(), 4444);
                // use the ipaddress as in the server program

                Console.WriteLine("Connected");

                String str = "DAWAJ KURWO";
                Stream stm = tcpclnt.GetStream();

                ASCIIEncoding asen = new ASCIIEncoding();
                byte[] ba = asen.GetBytes(str);
                Console.WriteLine("Transmitting.....");

                stm.Write(ba, 0, ba.Length);

                //byte[] bb = new byte[100];
                //int k = stm.Read(bb, 0, 100);

                //for (int i = 0; i < k; i++)
                //    Console.Write(Convert.ToChar(bb[i]));

                tcpclnt.Close();
            }

            catch (Exception ee)
            {
                Console.WriteLine("Error..... " + ee.StackTrace);
            }
        }

        //public void UDPSend(string message)
        //{
        //    UdpClient client = new UdpClient();
        //    IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, 1234);
        //    byte[] bytes = Encoding.ASCII.GetBytes(message);
        //    client.Send(bytes, bytes.Length, ip);
        //    client.Close();
        //    Console.WriteLine("Sent: {0} ", message);
        //}
    }
}