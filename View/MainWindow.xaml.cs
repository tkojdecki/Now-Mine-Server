using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using CefSharp.Wpf;
using CefSharp;
using CefSharp.SchemeHandler;
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
            serverThread = new Thread(() => serverTCP.ServerInit());
            serverThread.Name = "TCP Server Thread";
            serverThread.IsBackground = true;
            serverThread.Start();

            this.serverUDP = new ServerUDP();
            udpThread = new Thread(serverUDP.udpListener);
            udpThread.Name = "UDP Server Thread";
            udpThread.IsBackground = true;
            udpThread.Start();

            serverTCP.UserNameChanged += serverUDP.sendData;
            serverUDP.NewUser += serverTCP.TCPConnectToNewUser;
            webPanel.VideoEnded += serverUDP.DeletedPiece;
            QueueManager.PlayedNow += serverUDP.playedNow;
            QueueManager.PlayedNow += webPanel.PlayedNowHandler;
            QueueManager.VideoQueued += webPanel.VideoQueuedHandler;
            QueueManager.VideoQueued += serverUDP.sendQueuedPiece;

            webPanel.VideoEnded += ChangeToIndex;
            DataContext = this;
            //columnQueue.DataContext = queuePanelVM;
            columnSearch.DataContext = searchPanelVM;
            this.Search.OnSearch += searchPanelVM.PerformSearch;
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
                    Application.Current.Dispatcher.Invoke(new Action(() => { webPanel.playNow(QueueManager.nowPlaying()); }));
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
            //columnDefinitionQueue.Width = new GridLength(0);

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
            columnDefinitionSearch.Width = new GridLength(360);

            RowQueue.Visibility = Visibility.Visible;
            //columnDefinitionQueue.Width = new GridLength(360);
        }

        private void activateUI()
        {
            //txtSearch.KeyDown += txtSearch_KeyDown;
            //searchButton.IsEnabled = true;
            //playNextButton.IsEnabled = true;
            Search.ToogleSearchEnabled(true);
            Search.OnSearch += ScrollSearchQueue;
            webPlayer.MouseDoubleClick += Player_MouseDoubleClick;
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