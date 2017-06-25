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

namespace NowMine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SearchPanel searchPanel;
        QueuePanel queuePanel;
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

            webPanel = new WebPanel(webPlayer, queuePanel, this);
            webPlayer.RegisterJsObject("app", webPanel);
            webPlayer.FrameLoadEnd += WebPlayer_FrameLoadEnd;
            webPlayer.Initialized += WebPlayer_Initialized;
            //webPlayer.LoadingStateChanged += WebPlayer_LoadingStateChanged;
            
            columnPlayer.Children.Add(webPlayer);

            queuePanel = new QueuePanel(queueBoard, webPanel);
            searchPanel = new SearchPanel(searchBoard, txtSearch, queuePanel);
            webPanel.reinitialize(webPlayer, queuePanel);

            this.serverTCP = new ServerTCP();
            serverThread = new Thread(() => serverTCP.ServerInit(queuePanel));
            serverThread.Name = "TCP Server Thread";
            serverThread.IsBackground = true;
            serverThread.Start();

            this.serverUDP = new ServerUDP(this.serverTCP);
            udpThread = new Thread(serverUDP.udpListener);
            udpThread.Name = "UDP Server Thread";
            udpThread.IsBackground = true;
            udpThread.Start();

            //serverTCP.MusicPieceReceived += queuePanel.addFromNetwork;
            serverTCP.MusicPieceReceived += serverUDP.sendQueuedPiece;
            serverTCP.UserNameChanged += serverUDP.sendQueuedPiece;
            searchPanel.VideoQueued += serverUDP.sendQueuedPiece;
            webPanel.VideoEnded += serverUDP.DeletedPiece;
            queuePanel.PlayedNow += serverUDP.playedNow;

            webPanel.VideoEnded += ChangeToIndex;
            DataContext = this;
            columnQueue.DataContext = queuePanel;   
        }

        private void InitializeChromium()
        {
            var settings = new CefSettings();
            settings.CefCommandLineArgs.Add("disable-gpu-vsync", "1");
            settings.CefCommandLineArgs.Add("disable-gpu", "1");
            //settings.LogSeverity = LogSeverity.Verbose;
            settings.RemoteDebuggingPort = 8088;
            //settings.CefCommandLineArgs.Add("no-proxy-server", "1");

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
                //if (queuePanel.nowPlaying() != null)
                //{
                //    webPanel.playNow(queuePanel.nowPlaying());
                //}
                var browser = sender as ChromiumWebBrowser;
                string bradres = "";
                if (webPanel.isPlaying)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() => { webPanel.playNow(queuePanel.nowPlaying()); }));
                }
                //Application.Current.Dispatcher.Invoke(new Action(() => { bradres = browser.Address; }));
                //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"var ytpl = document.getElementById('movie_player');");
                //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"ytpl.addEventListener('onStateChange', function asdf() { alert('asdf') });");
                //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"ytpl.addEventListener('onReady', function asdf() { alert('asdf') });");
            }

            //if (isYoutubePage && e.HttpStatusCode == 200 && e.Url.Contains("youtube.com") && !webPanel.isPlaying)
                //Application.Current.Dispatcher.Invoke(new Action(() => { queuePanel.playNext(); }));

            if (!this.videoID.Equals("") && e.Url.Contains(this.videoID) && e.Frame.IsMain) //&& !e.Browser.IsLoading
            {
                webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"var ytpl = document.getElementById('movie_player');");
                webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"ytpl.addEventListener('onStateChange', function onPlayerStateChange(event){if(event==0){app.getNextVideo();}});");
                //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"ytpl.stopVideo()");
                //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"alert(ytpl);");
                //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"");
            }
            if (!webPanel.isPlaying && isYoutubePage && e.Frame.IsMain && !this.videoID.Equals(""))
            {
                //if (bradres == LOCALSITEADDRESS && !webPanel.isPlaying)
            }
        }

        //private void WebPlayer_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        //{
        //    var browser = sender as ChromiumWebBrowser;
        //    string bradres = "";
        //    Application.Current.Dispatcher.Invoke(new Action(() => { bradres = browser.Address; }));
        //    if (isYoutubePage && !bradres.Contains(this.videoID))
        //    {
        //        ChangeToIndex
        //        webPanel.isPlaying = false;
                
        //        //Application.Current.Dispatcher.Invoke(new Action(() => { webPanel.getNextVideo(); })); ostatnio skomentowane

        //        //Application.Current.Dispatcher.Invoke(new Action(() => { queuePanel.playNext(); }));
                
                
        //    }
        //    Console.WriteLine("LoadingStateChanged " + bradres);
        //}

        public void ChangeToIndex(object s, EventArgs e)
        {
            Console.WriteLine("Back to index.html!");
            isYoutubePage = false;
            Application.Current.Dispatcher.Invoke(new Action(() => { this.webPlayer.Address = LOCALSITEADDRESS; }));
        }

        //private
        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            searchPanel.search();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                searchPanel.search();
            }
        }

        private void txtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            txtBox.Text = "";
        }

        private void playNextButton_Click(object sender, RoutedEventArgs e)
        {
            //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"var ytpl = document.getElementById('movie_player');");
            //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"ytpl.addEventListener('onStateChange', app.ytEnded);");
            //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"ytpl.addEventListener('onStateChange', function asdf() { alert('asdf') });");
            //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"alert(ytpl);");
            //if (isYoutubePage)
            //{
            //    Console.WriteLine("Back to index.html!");
            //    webPanel.isPlaying = false;
            //    Application.Current.Dispatcher.Invoke(new Action(() => { this.webPlayer.Address = @"local://index.html"; }));
            //    Application.Current.Dispatcher.Invoke(new Action(() => { webPanel.getNextVideo(); }));
            //    //webPanel.getNextVideo();
            //    //Application.Current.Dispatcher.Invoke(new Action(() => { queuePanel.playNext(); }));
            //    isYoutubePage = false;
            //}
            //else
            //{
            //    queuePanel.playNext();
            //}
            webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"var ytpl = document.getElementById('movie_player');");
            //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"ytpl.addEventListener('onStateChange', function onPlayerStateChange(event){if(event.data==0){app.getNextVideo();}});");
            webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"ytpl.addEventListener('onStateChange', function onPlayerStateChange(event){if(event==0){alert('asdf');}});");
        }

        private void Player_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (isMaximized)
            {
                minimizePlayer();
            }
            else
            {
                maximalizePlayer();
            }
        }

        private void maximalizePlayer()
        {
            isMaximized = true;
 
            columnSearch.Visibility = Visibility.Collapsed;
            columnDefinitionSearch.Width = new GridLength(0);

            columnQueue.Visibility = Visibility.Collapsed;
            columnDefinitionQueue.Width = new GridLength(0);

            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
        }

        private void minimizePlayer()
        {
            isMaximized = false;
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.SingleBorderWindow;

            columnSearch.Visibility = Visibility.Visible;
            columnDefinitionSearch.Width = new GridLength(360);

            columnQueue.Visibility = Visibility.Visible;
            columnDefinitionQueue.Width = new GridLength(360);
        }

        private void activateUI()
        {
            txtSearch.KeyDown += txtSearch_KeyDown;
            searchButton.IsEnabled = true;
            playNextButton.IsEnabled = true;
            webPlayer.MouseDoubleClick += Player_MouseDoubleClick;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Cef.Shutdown();
        }
    }
}