using NowMine.Queue;
using System.Globalization;
using System.Threading;
using System.Windows;

namespace NowMine
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Thread serverThread;
        Thread udpThread;
        ServerTCP serverTCP;
        ServerUDP serverUDP;

        public App()
        {
            Thread thrd = Thread.CurrentThread;
            thrd.Name = "NowMine UI";
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            this.serverTCP = new ServerTCP();
            serverThread = new Thread(() => serverTCP.StartListener());
            serverThread.Name = "TCP Server Thread";
            serverThread.IsBackground = true;
            serverThread.Start();

            this.serverUDP = new ServerUDP();
            udpThread = new Thread(serverUDP.udpListener);
            udpThread.Name = "UDP Server Thread";
            udpThread.IsBackground = true;
            udpThread.Start();

            serverTCP.UserNameChanged += serverUDP.sendData;
            serverTCP.UserColorChanged += serverUDP.sendData;
            serverUDP.NewUser += serverTCP.TCPConnectToNewUser;
            
            QueueManager.PlayedNow += serverUDP.playedNow;
            QueueManager.PlayedNext += serverUDP.playedNext;
            QueueManager.VideoQueued += serverUDP.SendQueuedPiece;
            QueueManager.RemovedPiece += serverUDP.sendData;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            uint eventID = EventManager.GetIDForEvent(NowMineCommon.Enums.CommandType.ServerShutdown, new object());
            serverUDP.SendShutdown(eventID);
        }
    }
}
