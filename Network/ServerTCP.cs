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
using NowMine.Helpers;
using NowMine.Queue;
using System.Threading.Tasks;

namespace NowMine
{
    class ServerTCP
    {
        readonly int TCP_PORT = 4444;
        public IPAddress serverIP = null;
        private string hostName;
        Dictionary<IPAddress, User> users = new Dictionary<IPAddress, User>();
        readonly int commandMessagePos = Encoding.UTF8.GetByteCount("Queue: ");

        object _lock = new Object(); // sync lock 
        List<Task> _connections = new List<Task>(); // pending connections


        public ServerTCP()
        {
            this.hostName = Dns.GetHostName();
            IPAddress[] IpA = Dns.GetHostAddresses(this.hostName);
            for (int i = 0; i < IpA.Length; i++)
            {
                if (IpA[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    serverIP = IpA[i];
                }
            }
        }

        public Task StartListener()
        {
            return Task.Run(async () =>
            {
                //var tcpListener = TcpListener.Create(8000);
                var tcpListener = new TcpListener(serverIP, TCP_PORT);
                tcpListener.Start();
                while (true)
                {
                    //var tcpClient = await tcpListener.AcceptTcpClientAsync();
                    var tcpClient = await tcpListener.AcceptSocketAsync();
                    Console.WriteLine("TCP/ Client has connected");
                    Console.WriteLine("TCP/  IP: {0}", tcpClient.RemoteEndPoint);
                    var task = StartHandleConnectionAsync(tcpClient);
                    // if already faulted, re-throw any error on the calling context
                    if (task.IsFaulted)
                        task.Wait();
                }
            });
        }

        // Register and handle the connection
        private async Task StartHandleConnectionAsync(Socket tcpClient)
        {
            // start the new connection task
            var connectionTask = HandleConnectionAsync(tcpClient);

            // add it to the list of pending task 
            lock (_lock)
                _connections.Add(connectionTask);

            // catch all errors of HandleConnectionAsync
            try
            {
                await connectionTask;
                // we may be on another thread after "await"
            }
            catch (Exception ex)
            {
                // log the error
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                // remove pending task
                lock (_lock)
                    _connections.Remove(connectionTask);
            }
        }

        // Handle new connection
        private async Task HandleConnectionAsync(Socket tcpClient)
        {
            await Task.Yield();
            // continue asynchronously on another threads

            using (NetworkStream networkStream = new NetworkStream(tcpClient))
            {
                var buffer = new byte[4096];
                Console.WriteLine("TCP/ Reading from client");
                var byteCount = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                var request = Encoding.UTF8.GetString(buffer, 0, byteCount);
                Console.WriteLine("TCP/ Client wrote {0}", request);

                if (string.IsNullOrEmpty(request))
                {
                    Console.WriteLine("TCP/ EMPTY COMMAND IP: {0}", tcpClient.RemoteEndPoint);
                    return;
                }

                IPAddress connectedIP = ((IPEndPoint)tcpClient.RemoteEndPoint).Address;
                var user = users[connectedIP];
                if (user == null)
                {
                    Console.WriteLine("TCP/ COMMAND FROM NON USER IP: {0}", tcpClient.RemoteEndPoint);
                    return;
                }
                Console.WriteLine("TCP/ IP {0} is user {1}", tcpClient.RemoteEndPoint, user.Name);

                string[] values = request.Split(' ');
                switch (values[0])
                {
                    case "PlayNext":
                        Console.WriteLine("TCP/ PlayNext from {0}", tcpClient.RemoteEndPoint);
                        Application.Current.Dispatcher.Invoke(new Action(() => { QueueManager.playNext(); }));
                        break;

                    case "Queue:":
                        Console.WriteLine("TCP/ To Queue!");

                        YouTubeInfo toQueue;
                        using (MemoryStream msq = new MemoryStream(buffer, commandMessagePos, byteCount - commandMessagePos))
                        {
                            toQueue = BytesMessegeBuilder.DeserializeYoutubeInfo(msq);
                        }

                        if (toQueue == null)
                        {
                            Console.WriteLine("TCP/ Failed to get Youtube Info from {0}", tcpClient.RemoteEndPoint);
                            tcpClient.Send(BitConverter.GetBytes(-1));
                            break;
                        }
                        toQueue.buildURL();
                        Console.WriteLine(string.Format("TCP/ Adding to queue {0} ", toQueue.title));
                        int qPos = -2;
                        Application.Current.Dispatcher.Invoke(new Action(() => { qPos = QueueManager.addToQueue(new MusicPiece(toQueue, user)); }));
                        Console.WriteLine("TCP/ Sending position of queued element {0} to {1}", qPos, tcpClient.RemoteEndPoint);
                        tcpClient.Send(BitConverter.GetBytes(qPos));
                        break;

                    case "GetQueue":
                        Console.WriteLine("TCP/ Get Queue!");
                        QueuePieceToSend[] ytInfo = null;
                        Application.Current.Dispatcher.Invoke(new Action(() => { ytInfo = QueueManager.getQueueInfo().ToArray(); }));
                        if (ytInfo != null && ytInfo.Length > 0)
                        {
                            var qBytes = BytesMessegeBuilder.SerializeQueuePieceToSend(ytInfo);
                            Console.WriteLine("TCP/ Sending queue to: {0}  - {1}: Queue length : {2}", tcpClient.RemoteEndPoint, user.Name, ytInfo.Length);
                            tcpClient.Send(qBytes);
                        }
                        else
                        {
                            tcpClient.Send(BitConverter.GetBytes(0));
                        }
                        break;

                    case "GetUsers":

                        Console.WriteLine("TCP/ Get Users!");
                        var ms = BytesMessegeBuilder.SerializeUsers(users.Values.ToArray());
                        Console.WriteLine("TCP/ Sending users to: {0} - {1}, Users length {2}", tcpClient.RemoteEndPoint, user.Name, users.Count);
                        tcpClient.Send(ms.ToArray());
                        break;

                    case "ChangeName":
                        Console.WriteLine("TCP/ Changing Name!");
                        if (!users.Values.Any(u => u.Name.Equals(values[1])))
                        {
                            Console.WriteLine("TCP/ Changing User {0} to {1} ({3})", user.Name, values[1], tcpClient.RemoteEndPoint);
                            user.Name = values[1];
                            Application.Current.Dispatcher.Invoke(new Action(() => { QueueManager.RefreshQueueUserNames(user); }));
                            OnUserNameChange(values[1], user.Id);
                            Console.WriteLine("TCP/ Changed Name; Sending 1");
                            tcpClient.Send(BitConverter.GetBytes(1));
                        }
                        else
                        {
                            Console.WriteLine("TCP/Change Name: Other user with same name; Sending 0");
                            tcpClient.Send(BitConverter.GetBytes(0));
                        }
                        break;

                    default:
                        Console.WriteLine("TCP/ Can't interpret right");
                        break;
                }
                Console.WriteLine(string.Format("TCP/ Disconnecting with {0}", tcpClient.RemoteEndPoint));
                tcpClient.Disconnect(false);
            }
        }

        public void TCPConnectToNewUser(object s, GenericEventArgs<IPAddress> e)
        {
            if (e.EventData == serverIP)
                return;
            try
            {
                using (TcpClient tcpclnt = new TcpClient())
                {
                    var ip = e.EventData;
                    Console.WriteLine("TCP/ Connecting to: " + ip.ToString());
                    Int32 userId;
                    if (users.ContainsKey(ip))
                        userId = users[ip].Id;
                    else
                        userId = users.Count + 1;
                    byte[] userIdBytes = new byte[4]; 
                    userIdBytes = BitConverter.GetBytes(userId);
                    byte[] ipBytes = serverIP.GetAddressBytes(); //4 bytes
                    byte[] message = BytesMessegeBuilder.MergeBytesArray(ipBytes, userIdBytes);
                    tcpclnt.Connect(ip, 4444);
                    Console.WriteLine("TCP/ Connected");
                    Stream stm = tcpclnt.GetStream();
                    //stm.ReadTimeout = 250;
                    //stm.WriteTimeout = 250;
                    Console.WriteLine("TCP/ Transmitting: " + Convert.ToBase64String(message));
                    stm.Write(message, 0, message.Length);
                    stm.Flush();
                    byte[] msgLengthBytes = new byte[4];
                    stm.Read(msgLengthBytes, 0, 4);
                    int msgLength = BitConverter.ToInt32(msgLengthBytes, 0);
                    byte[] bytes = new byte[msgLength];
                    stm.Flush();
                    stm.Read(bytes, 0, msgLength);
                    var newUser = BytesMessegeBuilder.DeserializeUser(bytes);
                    if (newUser != null && newUser.Id == userId)
                    {
                        if (users.ContainsKey(ip))
                            users[ip] = newUser;
                        else
                            users.Add(ip, newUser);
                        stm.WriteByte(1);
                    }
                    else
                        stm.WriteByte(0);
                        
                    tcpclnt.Close();       
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine("TCP/ Error... " + ee.Message);
            }
        }

        public delegate void UserNameChangedEventHandler(object s, GenericEventArgs<byte[]> e);
        public event UserNameChangedEventHandler UserNameChanged;
        protected virtual void OnUserNameChange(string userName, int userId)
        {
            var userNameBytes = Encoding.UTF8.GetBytes(userName);
            var userData = BytesMessegeBuilder.MergeBytesArray(userNameBytes, BitConverter.GetBytes(userId));
            var message = BytesMessegeBuilder.MergeBytesArray(UnicodeEncoding.UTF8.GetBytes("ChangeName: "), userData);
            UserNameChanged?.Invoke(this, new GenericEventArgs<byte[]>(message));
        }
    }
}