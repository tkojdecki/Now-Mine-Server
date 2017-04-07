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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;

namespace NowMine
{
    public class MusicPieceReceivedEventArgs : EventArgs
    {
        public YoutubeQueued YoutubeQueued { get; set; }
    }

    class ServerTCP
    {
        QueuePanel queuePanel;
        readonly int TCP_PORT = 4444;
        public IPAddress serverIP = null;
        Dictionary<IPAddress, User> users = new Dictionary<IPAddress, User>();
        readonly int commandMessagePos = Encoding.UTF8.GetByteCount("Queue: ");

        public delegate void MusicPieceReceivedEventHandler(object s, MusicPieceReceivedEventArgs e);
        public event MusicPieceReceivedEventHandler MusicPieceReceived;
        protected virtual void OnMusicPieceReceived(YoutubeQueued youtubeQueued)
        {
            if (MusicPieceReceived!=null)
            {
                var e = new MusicPieceReceivedEventArgs();
                e.YoutubeQueued = youtubeQueued;
                MusicPieceReceived(this, e);
            }
        }

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
                    byte[] bArray = new byte[k];
                    char cc = ' ';
                    string recived = null;
                    Console.WriteLine("Recieved...");
                    for (int i = 0; i < k - 1; i++)
                    {
                        Console.Write(Convert.ToChar(b[i]));
                        cc = Convert.ToChar(b[i]);
                        recived += cc.ToString();
                        bArray[i] = b[i];
                    }
                    Console.WriteLine();
                    //string test1 = Convert.ToBase64String(b);
                    //string test2 = Encoding.UTF8.GetString(b);
                    if (string.IsNullOrEmpty(recived))
                        continue;

                    IPAddress connectedIP = ((IPEndPoint)s.RemoteEndPoint).Address;
                    //User user = null;
                    //if(!users.ContainsKey(connectedIP))
                    //{
                    //    Console.WriteLine("New User!");
                    //    user = new User("User " + users.Count + 1, users.Count + 1);
                    //    users.Add(connectedIP, user);
                    //}
                    //else { user = users[connectedIP]; }
                    var user = users[connectedIP];
                    Console.WriteLine("IP {0} is user {1}", s.RemoteEndPoint, user.Name);

                    string[] values = recived.Split(' ');
                    switch (values[0])
                    {
                        case "PlayNext":
                            Console.WriteLine("PlayNext from {0}", s.RemoteEndPoint);
                            Application.Current.Dispatcher.Invoke(new Action(() => { queuePanel.playNext(); }));
                            break;

                        case "Queue:":
                            Console.WriteLine("To Queue!");

                            YouTubeInfo toQueue;            
                            using (MemoryStream ms = new MemoryStream(bArray, commandMessagePos, bArray.Length - commandMessagePos))
                            using (BsonReader reader = new BsonReader(ms))
                            {
                                JsonSerializer serializer = new JsonSerializer();
                                toQueue = serializer.Deserialize<YouTubeInfo>(reader);
                            }
                            if (toQueue == null)
                            {
                                Console.WriteLine("Failed to get Youtube Info from {0}", s.RemoteEndPoint);
                                s.Send(BitConverter.GetBytes(-1));
                                break;
                            }
                            toQueue.buildURL();
                            Console.WriteLine(string.Format("Adding to queue {0} ", toQueue.title));
                            int qPos = -2;
                            Application.Current.Dispatcher.Invoke(new Action(() => {qPos = queuePanel.addToQueue(new MusicPiece(toQueue, user)); }));
                            Console.WriteLine("Sending position of queued element {0} to {1}", qPos, s.RemoteEndPoint);
                            s.Send(BitConverter.GetBytes(qPos));
                            s.Disconnect(false);
                            Console.WriteLine(string.Format("Disconnecting with {0}", s.RemoteEndPoint));
                            var youtubeQueued = new YoutubeQueued(toQueue, qPos, user.Id);
                            OnMusicPieceReceived(youtubeQueued);
                            //serverUDP.UDPSend(bArray);
                            //tutaj kurwa send z numerem kolejki
                            continue;
                            //break;

                        case "GetQueue":
                            Console.WriteLine("Get Queue!");
                            QueuePieceToSend[] ytInfo = null;
                            Application.Current.Dispatcher.Invoke(new Action(() => { ytInfo = queuePanel.getQueueInfo().ToArray(); }));
                            if (ytInfo != null) //&& ytInfo.Count() > 0
                            {
                                MemoryStream ms = new MemoryStream();
                                using (BsonWriter writer = new BsonWriter(ms))
                                {
                                    JsonSerializer serializer = new JsonSerializer();
                                    //serializer.Serialize()
                                    serializer.Serialize(writer, ytInfo, typeof(QueuePieceToSend[]));
                                    Console.WriteLine("TCP/ Sending to: {0} : {1}", s.RemoteEndPoint, Convert.ToBase64String(ms.ToArray()));
                                    s.Send(ms.ToArray());
                                }
                            }
                            break;

                        case "GetUsers":
                            Console.WriteLine("Get Users!");
                            if (users != null)
                            {
                                MemoryStream ms = new MemoryStream();
                                using (BsonWriter writer = new BsonWriter(ms))
                                {
                                    JsonSerializer serializer = new JsonSerializer();
                                    //serializer.Serialize()
                                    serializer.Serialize(writer, users, typeof(User[]));
                                    Console.WriteLine("TCP/ Sending to: {0} : {1}", s.RemoteEndPoint, Convert.ToBase64String(ms.ToArray()));
                                    s.Send(ms.ToArray());
                                }
                            }
                            break;

                        default:
                            Console.WriteLine("Can't interpret right");
                            break;
                    }
                    Console.WriteLine(string.Format("Disconnecting with {0}", s.RemoteEndPoint));
                    s.Disconnect(false);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message + " : " + e.Source);
            }
        }

        public void sendServerIP(IPAddress ip)
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
                User user = null;
                if (!users.ContainsKey(ip))
                {
                    Console.WriteLine("New User!");
                    user = new User("User " + users.Count + 1, users.Count + 1);
                    users.Add(ip, user);
                }
                else
                    user = users[ip];
                
                using (TcpClient tcpclnt = new TcpClient())
                {
                    Console.WriteLine("TCP/ Connecting to: " + ip.ToString());
                    tcpclnt.Connect(ip, 4444);
                    Console.WriteLine("TCP/ Connected");

                    byte[] ipBytes = serverIP.GetAddressBytes();
                    Stream stm = tcpclnt.GetStream();

                    byte[] idBytes = BitConverter.GetBytes(user.Id);
                    Console.WriteLine("TCP/ Transmitting: " + ipBytes);

                    stm.Write(ipBytes, 0, ipBytes.Length);
                    stm.Write(idBytes, 0, idBytes.Length);
                    tcpclnt.Close();
                    
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine("Error... " + ee.Message);
            }
        }

        private void TCPConnect(IPAddress ip, String[] message)
        {
            try
            {
                TcpClient tcpclnt = new TcpClient();
                Console.WriteLine("TCP/ Connecting To: " + ip.ToString());

                tcpclnt.Connect(ip, 4444);
                // use the ipaddress as in the server program

                Console.WriteLine("TCP/ Connected");

                Stream stm = tcpclnt.GetStream();
                String msg = message.Length.ToString();
                ASCIIEncoding asen = new ASCIIEncoding();
                byte[] ba = asen.GetBytes(msg + " \n");
                

                stm.Write(ba, 0, ba.Length);
                
                for (int i = 0; i < message.Length; i++)
                {
                    Console.WriteLine("TCP/ Transmitting: " + message[i]);
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