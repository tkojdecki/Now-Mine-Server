﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace NowMine
{
    class Server
    {
        public static void ServerInit()
        {
            //try
            //{
            //    IPAddress ipAd = IPAddress.Parse("192.168.0.19");
            //    // use local m/c IP address, and 

            //    // use the same in the client


            //    /* Initializes the Listener */
            //    TcpListener myList = new TcpListener(ipAd, 4444);

            //    /* Start Listeneting at the specified port */
            //    myList.Start();

            //    Console.WriteLine("The server is running at port 4444...");
            //    Console.WriteLine("The local End point is  :" +
            //                      myList.LocalEndpoint);
            //    Console.WriteLine("Waiting for a connection.....");
            ////m:
            //    Socket s = myList.AcceptSocket();
            //    Console.WriteLine("Connection accepted from " + s.RemoteEndPoint);

            //    byte[] b = new byte[100];
            //    int k = s.Receive(b);

            //    char cc = ' ';
            //    string test = null;
            //    Console.WriteLine("Recieved...");
            //    for (int i = 0; i < k - 1; i++)
            //    {
            //        Console.Write(Convert.ToChar(b[i]));
            //        cc = Convert.ToChar(b[i]);
            //        test += cc.ToString();
            //    }

            //    switch (test)
            //    {
            //        case "1":
            //            break;


            //    }
            //    ASCIIEncoding asen = new ASCIIEncoding();
            //    s.Send(asen.GetBytes("The string was recieved by the server."));
            //    s.Close();

            //    /* clean up */
            //    //goto m;
            //    //s.Close();
            //    myList.Stop();
            //    Console.ReadLine();

            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("Error..... " + e.StackTrace);
            //}
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            Socket internalSocket;
            byte[] recBuffer = new byte[256];

            serverSocket.Bind(new IPEndPoint(IPAddress.Parse("192.168.0.19"), 4444));
            serverSocket.Listen(1);
            
            internalSocket = serverSocket.Accept();
            internalSocket.Receive(recBuffer);
            Console.WriteLine(ASCIIEncoding.ASCII.GetString(recBuffer));

            Console.Read();
        }
    }
}