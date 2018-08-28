using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using CefSharp.Wpf;
using CefSharp;
using CefSharp.SchemeHandler;
using NowMine.ViewModel;
using NowMine.Queue;
using System.Timers;
using System.IO;

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
            //webPlayer.IsBrowserInitializedChanged += WebPlayer_IsBrowserInitializedChanged;

            webPanel = new WebPanel(ref webPlayer, this);
            //webPlayer.RegisterJsObject("app", webPanel);
            //webPlayer.FrameLoadEnd += WebPlayer_FrameLoadEnd;
            webPlayer.Initialized += WebPlayer_Initialized;
            webPlayer.IsBrowserInitializedChanged += WebPlayer_IsBrowserInitializedChanged;

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
            //webPanel.VideoEnded += serverUDP.DeletedPiece; -- on playednow hendlowane
            //webPanel.VideoEnded += ChangeToIndex;
            //webPanel.PlayedNow += serverUDP.playedNow;
            QueueManager.PlayedNow += serverUDP.playedNow;
            QueueManager.PlayedNow += webPanel.playNow;
            QueueManager.PlayedNext += serverUDP.playedNext;
            QueueManager.VideoQueued += webPanel.VideoQueuedHandler;
            QueueManager.VideoQueued += serverUDP.sendQueuedPiece;
            QueueManager.RemovedPiece += serverUDP.DeletedPiece;
            DataContext = this;
            //columnQueue.DataContext = queuePanelVM;
            columnSearch.DataContext = searchPanelVM;
            this.Search.OnSearch += searchPanelVM.PerformSearch;
        }

        private void WebPlayer_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && !(bool)e.OldValue)
            {
                string html = File.ReadAllText(Directory.GetCurrentDirectory() + @"\" + webPanel.VideoProvider.GetHomePage());
                this.webPlayer.LoadHtml(html, @"local://home.html");
            }
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
                IsSecure = true,
                SchemeHandlerFactory = new FolderSchemeHandlerFactory(
                                                rootFolder: @"..\..\..\Resources",
                                                schemeName: "local", //Optional param no schemename checking if null
                                                                     //hostName: "cefsharp", //Optional param no hostname checking if null
                                                defaultPage: "index.html" //Optional param will default to index.html
                                                )
            });
            settings.FocusedNodeChangedEnabled = true;
            Cef.Initialize(settings);
        }


        private void WebPlayer_Initialized(object sender, EventArgs e)
        {
            webPlayer.JavascriptObjectRepository.Register("app", webPanel, true);
            activateUI();
        }
        
        private void WebPlayer_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            //Console.WriteLine("Frame Load End: " +  e.Frame + " " + e.HttpStatusCode + " " + e.Url);
            //if (!isYoutubePage && e.HttpStatusCode == 200 && e.Url.Contains("youtube.com"))
            //{
            //    Application.Current.Dispatcher.Invoke(new Action(() => { activateUI(); }));
            //    var browser = sender as ChromiumWebBrowser;
            //    if (webPanel.isPlaying)
            //    {
            //        Application.Current.Dispatcher.Invoke(new Action(() => { webPanel.PlayNow(QueueManager.nowPlaying(), 0); }));
            //    }
            //}

            //if (!this.videoID.Equals("") && e.Url.Contains(this.videoID) && e.Frame.IsMain)
            //{
                //Console.WriteLine("Injecting listiner for DOM creation");
                //string jsListenerScript = "document.addEventListener('DOMContentLoaded', function(){ alert('DomLoaded'); });";
                //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(jsListenerScript);
                //var aTimer = new System.Timers.Timer(10  * 60 * 3);
                //aTimer.Elapsed += AddJSEventListener_OnTimedEvent;
                //aTimer.Enabled = true;
                //aTimer.Start();
            //}

            //if (!this.videoID.Equals("") && !e.Url.Contains(this.videoID) && e.Url.Contains("google") && !e.Frame.IsMain)
            //if (!e.Browser.IsLoading && e.Frame.IsMain && e.Browser.HasDocument)
            //{
            //    Console.WriteLine("Clearing youtube page from non-video elements");
            //    //clearing rest window etc.
            //    var Scripts = webPanel.VideoProvider.GetAfterLoadScripts();
            //    foreach(var script in Scripts)
            //    {
            //        webPlayer.GetMainFrame().ExecuteJavaScriptAsync(script);
            //    }
            //}
        }

        public void ChangeToIndex(object s, EventArgs e)
        {
            ChangeToIndex();
        }

        public void ChangeToIndex()
        {
            Console.WriteLine("Stopping Video!");
            Console.WriteLine("Back to index.html!");
            isYoutubePage = false;

            Application.Current.Dispatcher.Invoke(new Action(() => { this.webPlayer.WebBrowser.Load(webPanel.VideoProvider.GetHomePage()); }));
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

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case (Key.F5):
                    webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"window.location.reload();");
                    break;
                case (Key.Pause):
                    //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"var ytpl = document.getElementById('movie_player');");
                    //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"ytpl.addEventListener('onStateChange', function onPlayerStateChange(event){if(event==0){app.getNextVideo();}});");
                    this.webPlayer.Load(@"http://www.google.pl");
                    break;
            }
        }
    }
}