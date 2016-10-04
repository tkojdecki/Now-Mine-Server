using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace NowMine
{
    class Server
    {
        QueuePanel queuePanel;
        UdpClient udp;
        const int TCP_PORT = 4444;
        IPAddress serverIP = null;
        Dictionary<IPAddress, User> users = new Dictionary<IPAddress, User>();
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
                        serverIP = IpA[i];

                    }
                }

                /* Initializes the Listener */
                TcpListener myList = new TcpListener(serverIP, TCP_PORT);


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

                    byte[] b = new byte[512];
                    int k = s.Receive(b);

                    char cc = ' ';
                    string recived = null;
                    Console.WriteLine("Recieved...");
                    for (int i = 0; i < k - 1; i++)
                    {
                        Console.Write(Convert.ToChar(b[i]));
                        cc = Convert.ToChar(b[i]);
                        recived += cc.ToString();
                    }
                    Console.WriteLine();

                    IPAddress connectedIP = ((IPEndPoint)s.RemoteEndPoint).Address;
                    User user = null;
                    if(!users.ContainsKey(connectedIP))
                    {
                        user = new User(connectedIP.ToString());
                        user.name = "User " + users.Count + 1;
                        users.Add(connectedIP, user);
                    }
                    else { user = users[connectedIP]; }

                    String[] values = recived.Split(' ');
                    switch (values[0])
                    {
                        case "1":
                            Console.WriteLine("doszło 1");
                            break;

                        case "PlayNext":
                            //queuePanel.playNext();
                            Application.Current.Dispatcher.Invoke(new Action(() => { queuePanel.playNext(); }));
                            Console.WriteLine("Playing Next!");
                            break;

                        case "Queue:":
                            String ytJSON = recived.Substring(7);
                            Console.WriteLine(ytJSON);
                            YouTubeInfo sendedInfo = JsonConvert.DeserializeObject<YouTubeInfo>(ytJSON);
                            sendedInfo.buildURL();
                            Application.Current.Dispatcher.Invoke(new Action(() => { queuePanel.addToQueue(new MusicPiece(sendedInfo), user); }));
                            //Application.Current.Dispatcher.Invoke(new Action(() => { user.addToQueue(new MusicPiece(sendedInfo)); }));
                            Console.WriteLine("Playing Next!");
                            break;

                        case "GetQueue":
                            YouTubeInfo[] ytInfo = null;
                            Application.Current.Dispatcher.Invoke(new Action(() => { ytInfo = queuePanel.getQueueInfo().ToArray(); }));
                            if (ytInfo != null && ytInfo.Count() > 0)
                            {
                                String[] message = new String[ytInfo.Count()];
                                for (int i = 0; i < ytInfo.Count(); i++)
                                {
                                    message[i] = JsonConvert.SerializeObject(ytInfo[i]);
                                }
                                TCPConnect(connectedIP, message);
                            }
                            break;

                        default:
                            Console.WriteLine("Can't interpret right");
                            break;

                    }
                    ASCIIEncoding asen = new ASCIIEncoding();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.ToString());
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

                sendServerIP(ip.Address);
                //Console.WriteLine("Connecting to: {0}", ip.Address.ToString());
                udpListener();
            }
        }

        private void sendServerIP(IPAddress ip)
        {
            { 
                TCPConnect(ip);
                //Console.WriteLine("Connecting to: {0}", ip.Address.ToString());
            }
        }

        private void TCPConnect(IPAddress ip)
        {
            try
            {
                TcpClient tcpclnt = new TcpClient();
                Console.WriteLine("Connecting.....");
                tcpclnt.Connect(ip, 4444);
                Console.WriteLine("Connected");

                String str = serverIP.ToString();
                Stream stm = tcpclnt.GetStream();

                ASCIIEncoding asen = new ASCIIEncoding();
                byte[] ba = asen.GetBytes(str);
                Console.WriteLine("Transmitting.....");

                stm.Write(ba, 0, ba.Length);
                tcpclnt.Close();
            }

            catch (Exception ee)
            {
                Console.WriteLine("Error..... " + ee.StackTrace);
            }
        }

        private void TCPConnect(IPAddress ip, String[] message)
        {
            try
            {
                TcpClient tcpclnt = new TcpClient();
                Console.WriteLine("Connecting.....");

                tcpclnt.Connect(ip, 4444);
                // use the ipaddress as in the server program

                Console.WriteLine("Connected");

                Stream stm = tcpclnt.GetStream();
                String msg = message.Length.ToString();
                ASCIIEncoding asen = new ASCIIEncoding();
                byte[] ba = asen.GetBytes(msg + " \n");
                

                stm.Write(ba, 0, ba.Length);
                
                for (int i = 0; i < message.Length; i++)
                {
                    Console.WriteLine("Transmitting: " + message[i]);
                    ba = asen.GetBytes(message[i] + "\n");
                    stm.Write(ba, 0, ba.Length);
                }
                tcpclnt.Close();
            }

            catch (Exception ee)
            {
                Console.WriteLine("Error..... " + ee.StackTrace);
            }
        }
    }
}