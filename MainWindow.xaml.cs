using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CefSharp.Wpf;
using CefSharp;
using System.IO;
using CefSharp.SchemeHandler;
using System.Net;
using System.Collections.ObjectModel;
using System.ComponentModel;
using NowMine.ViewModel;
using NowMine.Queue;

namespace NowMine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WebPanel webPanel;
        Thread serverThread;
        Thread udpThread;
        ServerTCP serverTCP;
        ServerUDP serverUDP;
        private bool isMaximized = false;
        public ChromiumWebBrowser webPlayer;
        public bool isYoutubePage = false;
        public string videoID = "";
        private const string LOCALSITEADDRESS= @"local://index.html";

        public MainWindow()
        {            
            InitializeComponent();
            InitializeChromium();
            webPlayer = new ChromiumWebBrowser();

            webPanel = new WebPanel(webPlayer, this);
            webPlayer.RegisterJsObject("app", webPanel);
            webPlayer.FrameLoadEnd += WebPlayer_FrameLoadEnd;
            webPlayer.Initialized += WebPlayer_Initialized;
            
            RowPlayer.Children.Add(webPlayer);

            webPanel.reinitialize(webPlayer);
            //var queuePanelVM = new QueuePanelViewModel();
            var searchPanelVM = new SearchPanelViewModel();

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
            webPanel.VideoEnded += serverUDP.DeletedPiece;
            webPanel.VideoEnded += ChangeToIndex;
            //webPanel.PlayedNow += serverUDP.playedNow;
            QueueManager.PlayedNow += serverUDP.playedNow;
            QueueManager.VideoQueued += webPanel.VideoQueuedHandler;
            QueueManager.VideoQueued += serverUDP.sendQueuedPiece;

            DataContext = this;
            //columnQueue.DataContext = queuePanelVM;
            columnSearch.DataContext = searchPanelVM;
        }

        private void InitializeChromium()
        {
            var settings = new CefSettings();
            settings.CefCommandLineArgs.Add("disable-gpu", "1");
            //settings.LogSeverity = LogSeverity.Verbose;
            settings.RemoteDebuggingPort = 8088;

            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = "local",
                SchemeHandlerFactory = new FolderSchemeHandlerFactory(
                                                rootFolder: @"..\..\..\Resources",
                                                schemeName: "local" //Optional param no schemename checking if null
                                                //hostName: "cefsharp", //Optional param no hostname checking if null
                                                //defaultPage: "test.html" //Optional param will default to index.html
                                                )
            });
            Cef.Initialize(settings);
        }


        private void WebPlayer_Initialized(object sender, EventArgs e)
        {
            this.webPlayer.Address = LOCALSITEADDRESS;
        }
        
        private void WebPlayer_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            Console.WriteLine("Frame Load End: " +  e.Frame + " " + e.HttpStatusCode + " " + e.Url);
            if (!isYoutubePage && e.HttpStatusCode == 200 && e.Url.Contains("youtube.com"))
            {
                Application.Current.Dispatcher.Invoke(new Action(() => { activateUI(); }));
                var browser = sender as ChromiumWebBrowser;
                string bradres = "";
                if (webPanel.isPlaying)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() => { webPanel.PlayNow(QueueManager.nowPlaying(), 0); }));
                }
            }

            if (!this.videoID.Equals("") && e.Url.Contains(this.videoID) && e.Frame.IsMain)
            {
                webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"var ytpl = document.getElementById('movie_player');");
                webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"ytpl.addEventListener('onStateChange', function onPlayerStateChange(event){if(event==0){app.getNextVideo();}});");
            }
        }

        public void ChangeToIndex(object s, EventArgs e)
        {
            Console.WriteLine("Back to index.html!");
            isYoutubePage = false;
            Application.Current.Dispatcher.Invoke(new Action(() => { this.webPlayer.Address = LOCALSITEADDRESS; }));
        }

        //private
        
        private void Player_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (isMaximized)
                minimizePlayer();
            else
                maximalizePlayer();
        }

        private void maximalizePlayer()
        {
        //todo
            isMaximized = true;
 
            columnSearch.Visibility = Visibility.Collapsed;
            columnDefinitionSearch.Width = new GridLength(0);

            RowQueue.Visibility = Visibility.Collapsed;
            RowDefinitionQueue.Height = new GridLength(0);

            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
        }

        private void minimizePlayer()
        {
        //todo
            isMaximized = false;
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.SingleBorderWindow;

            columnSearch.Visibility = Visibility.Visible;
            columnDefinitionSearch.Width = new GridLength(375);

            RowQueue.Visibility = Visibility.Visible;
            RowDefinitionQueue.Height = new GridLength(120);
        }

        private void activateUI()
        {
            webPlayer.HorizontalAlignment = HorizontalAlignment.Stretch;
            webPlayer.VerticalAlignment = VerticalAlignment.Stretch;

            this.QueueControl.DataContext = new QueuePanelViewModel();

            Search.OnSearch += ScrollSearchQueue;
            webPlayer.MouseDoubleClick += Player_MouseDoubleClick;
            Search.ToogleSearchEnabled(true);
        }

        private void ScrollSearchQueue(Object sender, string searchText)
        {
            SearchScroll.ScrollToTop();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Cef.Shutdown();
        }
    }
}