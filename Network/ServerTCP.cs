using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Windows;
using NowMine.Helpers;
using NowMine.Queue;
using System.Threading.Tasks;
using NowMine.ViewModel;
using NowMine.Models;
using NowMineCommon.Models;
using NowMineCommon.Enums;

namespace NowMine
{
    class ServerTCP
    {
        readonly int TCP_PORT = 4444;
        public readonly IPAddress ServerIP = null;
        private readonly string HostName;
        //todo
        Dictionary<IPAddress, User> Users = new Dictionary<IPAddress, User>();
        //todo
        readonly int commandMessagePos = Encoding.UTF8.GetByteCount("Queue: ");

        object _lock = new Object(); // sync lock 
        List<Task> _connections = new List<Task>(); // pending connections


        public ServerTCP()
        {
            this.HostName = Dns.GetHostName();
            IPAddress[] IpA = Dns.GetHostAddresses(this.HostName);
            for (int i = 0; i < IpA.Length; i++)
            {
                if (IpA[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    ServerIP = IpA[i];
                }
            }
        }

        public Task StartListener()
        {
            return Task.Run(async () =>
            {
                var tcpListener = new TcpListener(ServerIP, TCP_PORT);
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

                if (!Users.ContainsKey(connectedIP))
                {
                    Console.WriteLine("TCP/ COMMAND FROM NON USER IP: {0}", tcpClient.RemoteEndPoint);
                    tcpClient.Send(BitConverter.GetBytes(0));
                    tcpClient.Disconnect(false);
                    return;
                }
                var user = Users[connectedIP];
                Console.WriteLine("TCP/ IP {0} is user {1}", tcpClient.RemoteEndPoint, user.Name);

                string jsonResponse = "";
                uint eventID;
                //string[] values = request.Split(' ');
                var command = JsonMessegeBuilder.GetCommandType(request);
                switch (command)
                {
                    case CommandType.GetQueue:
                        Console.WriteLine("TCP/ Get Queue!");
                        ClipQueued[] ytInfo = null;
                        Application.Current.Dispatcher.Invoke(new Action(() => { ytInfo = QueueManager.GetQueueInfo().ToArray(); }));
                        if (ytInfo != null) // && ytInfo.Length > 0
                        {
                            jsonResponse = JsonMessegeBuilder.Serialize(ytInfo, command);
                            Console.WriteLine("TCP/ Sending queue to: {0}  - {1}: Queue length : {2}", tcpClient.RemoteEndPoint, user.Name, ytInfo.Length);
                        }
                        else
                        {
                            jsonResponse = JsonMessegeBuilder.GetFailWithMessage(string.Empty);
                        }
                        break;

                    case CommandType.GetUsers:
                        Console.WriteLine("TCP/ Get Users!");
                        var usrlst = new List<User>(Users.Values.ToList());
                        usrlst.Add(User.serverUser);
                        Console.WriteLine("TCP/ Sending users to: {0} - {1}, Users length {2}", tcpClient.RemoteEndPoint, user.Name, Users.Count);
                        jsonResponse = JsonMessegeBuilder.Serialize(usrlst, command);
                        break;

                    case CommandType.GetEvents:
                        Console.WriteLine("TCP/ Get Events!");
                        var fromEventID = JsonMessegeBuilder.GetRequestData<uint>(request, command);
                        var eventList = EventManager.GetEventsFrom(fromEventID);
                        Console.WriteLine("TCP/ Sending events to: {0} - {1}, Events Lenth: {2} from ID {3}", tcpClient.RemoteEndPoint, user.Name, eventList.Count, fromEventID);
                        jsonResponse = JsonMessegeBuilder.Serialize(eventList, command);
                        break;

                    case CommandType.ChangeName:
                        Console.WriteLine("TCP/ Changing Name!");
                        var NewUserName = JsonMessegeBuilder.GetRequestString(request, command);
                        if (!Users.Values.Any(u => u.Name.Equals(NewUserName)))
                        {
                            Console.WriteLine("TCP/ Changing User {0} to {1} ({2})", user.Name, NewUserName, tcpClient.RemoteEndPoint);
                            user.Name = NewUserName;
                            QueueManager.OnGlobalPropertyChanged();
                            Console.WriteLine("TCP/ Changed Name; Sending Success");
                            eventID = EventManager.GetIDForEvent(command, NewUserName);
                            OnUserNameChange(NewUserName, user.Id, eventID);
                            jsonResponse = JsonMessegeBuilder.GetSuccessWithEvent(eventID);
                        }
                        else
                        {
                            Console.WriteLine("TCP/Change Name: Other user with same name; Sending 0");
                            jsonResponse = JsonMessegeBuilder.GetFailWithMessage("Not Unique");
                        }
                        break;

                    case CommandType.ChangeColor:
                        Console.WriteLine("TCP/ Changing Color!");
                        try
                        {
                            var changedColors = JsonMessegeBuilder.GetRequestString(request, command);
                            var NewUserColor = System.Convert.FromBase64String(changedColors);
                            user.UserColor = NewUserColor;
                            QueueManager.OnGlobalPropertyChanged();
                            eventID = EventManager.GetIDForEvent(command, NewUserColor);
                            OnUserColorChange(NewUserColor, user.Id, eventID);
                            jsonResponse = JsonMessegeBuilder.GetSuccessWithEvent(eventID);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("TCP/ On ChangeColor: {0}", ex.Message);
                            jsonResponse = JsonMessegeBuilder.GetFailWithMessage(String.Empty);
                        }
                        break;

                    case CommandType.PlayNext:
                        Console.WriteLine("TCP/ PlayNext from {0}", tcpClient.RemoteEndPoint);
                        if (QueueManager.nowPlaying().User.Id == user.Id)
                        {
                            Application.Current.Dispatcher.Invoke(new Action(() => { QueueManager.PlayNext(); }));
                            eventID = EventManager.GetIDForEvent(command, new object());
                            jsonResponse = JsonMessegeBuilder.GetSuccessWithEvent(eventID);
                        }
                        else
                            jsonResponse = JsonMessegeBuilder.GetFailWithMessage(String.Empty);
                        break;

                    case CommandType.QueueClip:
                        Console.WriteLine("TCP/ To Queue!");
                        var toQueue = JsonMessegeBuilder.GetRequestData<ClipInfo>(request, command);

                        if (toQueue == null || string.IsNullOrEmpty(toQueue.ID))
                        {
                            Console.WriteLine("TCP/ Failed to get Youtube Info from {0}", tcpClient.RemoteEndPoint);
                            jsonResponse = JsonMessegeBuilder.GetFailWithMessage("Failed at deserialization");
                            break;
                        }
                        Console.WriteLine(string.Format("TCP/ Adding to queue {0} ", toQueue.Title));
                        ClipQueued clipQueued = null;
                        var clipDataToQueue = new ClipData(toQueue, user);
                        Application.Current.Dispatcher.Invoke(new Action(() => { clipQueued = QueueManager.AddToQueue(clipDataToQueue); }));
                        Console.WriteLine("TCP/ Sending position of queued element at {0} with ID {1} to {2}", clipQueued.QPos, clipDataToQueue.QueueID, tcpClient.RemoteEndPoint);
                        eventID = EventManager.GetIDForEvent(command, clipQueued);
                        jsonResponse = JsonMessegeBuilder.GetQueueClipResponse(clipQueued, eventID);
                        break;

                    case CommandType.DeleteClip:
                        Console.WriteLine("TCP/ On DeletePiece from {0}", user.Name);
                        try
                        {
                            var queueIDToDelete = JsonMessegeBuilder.GetRequestData<uint>(request, command);
                            var isDeleted = QueueManager.DeleteFromQueue(queueIDToDelete, user.Id);
                            if (isDeleted)
                            {
                                eventID = EventManager.GetIDForEvent(command, queueIDToDelete);
                                jsonResponse = JsonMessegeBuilder.GetSuccessWithEvent(eventID);
                            }
                            else
                                jsonResponse = JsonMessegeBuilder.GetFailWithMessage(String.Empty);
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine("TCP/ On DeletePiece: {0}", ex.Message);
                            jsonResponse = JsonMessegeBuilder.GetFailWithMessage(String.Empty);
                        }
                        break;

                    default:
                        jsonResponse = JsonMessegeBuilder.GetFailWithMessage(String.Empty);
                        Console.WriteLine("TCP/ Can't interpret right");
                        break;
                }

                tcpClient.Send(BytesMessegeBuilder.GetBytesFromString(jsonResponse));
                Console.WriteLine(string.Format("TCP/ Sent to {0} - {1}", tcpClient.RemoteEndPoint, jsonResponse));
                
                tcpClient.Disconnect(false);
                Console.WriteLine(string.Format("TCP/ Disconnected with {0}", tcpClient.RemoteEndPoint));
            }
        }

        public void TCPConnectToNewUser(object s, GenericEventArgs<IPAddress> e)
        {
            if (e.EventData == ServerIP)
                return;
            try
            {
                using (TcpClient tcpclnt = new TcpClient())
                {
                    var ip = e.EventData;
                    Console.WriteLine("TCP/ Connecting to: " + ip.ToString());
                    Int32 userId;
                    if (Users.ContainsKey(ip))
                        userId = Users[ip].Id;
                    else
                        userId = Users.Count + 1;
                    byte[] userIdBytes = new byte[4]; 
                    userIdBytes = BitConverter.GetBytes(userId);
                    byte[] ipBytes = ServerIP.GetAddressBytes(); //4 bytes
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
                        if (Users.ContainsKey(ip))
                            Users[ip] = newUser;
                        else
                            Users.Add(ip, newUser);
                        stm.WriteByte(1);
                    }
                    else
                        stm.WriteByte(0);
                        
                    tcpclnt.Close();       
                }
            }
            catch(SocketException se)
            {
                Console.WriteLine("TCP/ SocketExeption Error..." + se.Message);
            }
            catch (Exception ee)
            {
                Console.WriteLine("TCP/ Error... " + ee.Message);
            }
        }

        public delegate void UserNameChangedEventHandler(byte[] message);
        public event UserNameChangedEventHandler UserNameChanged;
        protected virtual void OnUserNameChange(string userName, int userId, uint eventID)
        {
            var userNameBytes = Encoding.UTF8.GetBytes(userName);
            var message = BytesMessegeBuilder.GetChangeUserNameBytes(userName, userId, eventID);
            UserNameChanged?.Invoke(message);
        }

        public delegate void UserColorChangedEventHandler(byte[] message);
        public event UserColorChangedEventHandler UserColorChanged;
        protected virtual void OnUserColorChange(byte[] colorChanged, int userId, uint eventID)
        {
            var message = BytesMessegeBuilder.GetChangeColorBytes(colorChanged, userId, eventID);
            UserColorChanged?.Invoke(message);
        }
    }
}