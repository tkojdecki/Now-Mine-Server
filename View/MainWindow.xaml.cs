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
            this.Search.OnSearch += searchPanelVM.PerformSearch;
        }

        private void WebPlayer_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            string html = File.ReadAllText(Directory.GetCurrentDirectory() + "/index.html");
            this.webPlayer.LoadHtml(html, @"local://index.html");
            Console.WriteLine(html);
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
                                                schemeName: "local", //Optional param no schemename checking if null
                                                //hostName: "cefsharp", //Optional param no hostname checking if null
                                                defaultPage: "index.html" //Optional param will default to index.html
                                                )
            });

            Cef.Initialize(settings);
        }


        private void WebPlayer_Initialized(object sender, EventArgs e)
        {
            this.webPlayer.Address = LOCALSITEADDRESS;
        }

        private void AddJSEventListener_OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Adding Javascript Listener!");
            webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"var ytpl = document.getElementById('movie_player');");
            webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"ytpl.addEventListener('onStateChange', function onPlayerStateChange(event){if(event==0){app.getNextVideo();}});");
            var timer = sender as System.Timers.Timer;
            timer.Stop();
        }
        
        private void WebPlayer_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            Console.WriteLine("Frame Load End: " +  e.Frame + " " + e.HttpStatusCode + " " + e.Url);
            if (!isYoutubePage && e.HttpStatusCode == 200 && e.Url.Contains("youtube.com"))
            {
                Application.Current.Dispatcher.Invoke(new Action(() => { activateUI(); }));
                var browser = sender as ChromiumWebBrowser;
                if (webPanel.isPlaying)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() => { webPanel.PlayNow(QueueManager.nowPlaying(), 0); }));
                }
            }

            if (!this.videoID.Equals("") && e.Url.Contains(this.videoID) && e.Frame.IsMain)
            {
                //get next video after ending this
                Console.WriteLine("Injecting listiner for end video");
                //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"function alertload() { alert('now'); }");
                //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"document.onload = alertload;");
                //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"alertload();");
                //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"var ytpl = document.getElementById('movie_player');");
                var aTimer = new System.Timers.Timer(10  * 60 * 3);
                aTimer.Elapsed += AddJSEventListener_OnTimedEvent;
                //aTimer.Enabled = true;
                aTimer.Start();
                //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"setTimeout(ytplload, 2000);");

                //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"var ytpl; function ytplload() { ytpl = document.getElementById('movie_player'); }");
                //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"ytpl.addEventListener('onStateChange', function onPlayerStateChange(event){alert(event);})");
            }

            if (!this.videoID.Equals("") && !e.Url.Contains(this.videoID) && e.Url.Contains("google") && !e.Frame.IsMain)
            {
                Console.WriteLine("Clearing youtube page from non-video elements");
                ////clearing rest of window youtube page
                //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(string.Format("var player=document.evaluate(\"//*[@id=\'top\']/*[@id=\'container\']/*[@id=\'main\']\",document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue;"));
                //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"for(var childName in player.children){var childNode=player.children[childName];if(childNode.id && childNode.id != 'content-separator'){player.removeChild(childNode);}}");
                //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(string.Format("var toolbar=document.evaluate(\"//*[@id=\'masthead-container\']\",document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue;"));
                //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"toolbar.remove();");
                //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"player.style.position='relative';player.style.paddingBottom='56.25';player.style.overflow='hidden';player.style.maxWidth='100%';");
                //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"player.style.cssText = ''; player.style.position = 'absolute'; player.style.top = 0; player.style.left = 0;player.style.width='100%';player.style.height='100%';");
            }
        }

        public void ChangeToIndex(object s, EventArgs e)
        {
            Console.WriteLine("Stopping Video!");
            webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"ytpl.stopVideo();");
            Console.WriteLine("Back to index.html!");
            isYoutubePage = false;
            Application.Current.Dispatcher.Invoke(new Action(() => { this.webPlayer.WebBrowser.Stop(); }));
            //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"document.documentElement.innerHTML = '';");
            webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"ytpl.clearVideo();");
            webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"ytpl.destroy();");
            //Thread t = new Thread(new ThreadStart(WorkThreadFunction));
            //t.SetApartmentState(ApartmentState.STA);

            //t.Start();
            

            //Application.Current.Dispatcher.Invoke(new Action(() => { this.webPlayer.WebBrowser.Load(LOCALSITEADDRESS); }));
            //Application.Current.Dispatcher.Invoke(new Action(() => { this.webPlayer.WebBrowser.Load(@"http://www.google.pl"); }));
            string html = File.ReadAllText(Directory.GetCurrentDirectory() + "/index.html");
            //this.webPlayer.LoadHtml(html, @"local://index.html");
            //this.webPlayer.GetMainFrame().LoadUrl(@"local://index.html");
            this.webPlayer.GetMainFrame().LoadStringForUrl(html, @"local://index.html");
            //this.webPlayer.WebBrowser.Load(@"local://index.html")
            //Console.WriteLine(html);
            //QueueManager.playNext();
        }

        public void WorkThreadFunction()
        {
            Application.Current.Dispatcher.Invoke(new Action(() => { webPlayer.WebBrowser.Delete(); }));
            Application.Current.Dispatcher.Invoke(new Action(() => { webPlayer = null; }));
            Application.Current.Dispatcher.Invoke(new Action(() => { webPlayer = new ChromiumWebBrowser(); }));
            //Application.Current.Dispatcher.Invoke(new Action(() => { webPlayer.Initialized += WebPlayer_IsEnabledChanged; ; }));
            Application.Current.Dispatcher.Invoke(new Action(() => { webPlayer.Initialized += WebPlayer_Initialized; }));
            Application.Current.Dispatcher.Invoke(new Action(() => { webPlayer.Loaded += WebPlayer_Initialized; }));

            //Application.Current.Dispatcher.Invoke(new Action(() => { string html = File.ReadAllText(Directory.GetCurrentDirectory() + "/index.html"); }));

            //Application.Current.Dispatcher.Invoke(new Action(() => { this.webPlayer.GetMainFrame().LoadUrl(@"local://index.html"); }));
        }

        //private void WebPlayer_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    string html = File.ReadAllText(Directory.GetCurrentDirectory() + "/index.html");
        //    this.webPlayer.LoadHtml(html, @"local://index.html");
        //    Console.WriteLine(html);
        //}

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
            if(e.Key == Key.Pause)
            {
                webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"var ytpl = document.getElementById('movie_player');");
                webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"ytpl.addEventListener('onStateChange', function onPlayerStateChange(event){if(event==0){app.getNextVideo();}});");
            }
        }
    }
}