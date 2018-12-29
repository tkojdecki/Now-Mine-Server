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
        WebPanelViewModel webPanel;
        private bool isMaximized = false;
        public ChromiumWebBrowser webPlayer;
        private const string LOCALSITEADDRESS= @"local://index.html";
        //public event EventArgs Shutdown;

        public MainWindow()
        {            
            InitializeComponent();
            InitializeChromium();
            webPlayer = new ChromiumWebBrowser();

            webPanel = new WebPanelViewModel(ref webPlayer);
            webPlayer.Initialized += AddJSActivateUI;
            RowPlayer.Children.Add(webPlayer);
            RowPlayer.DataContext = webPlayer;

            //var queuePanelVM = new QueuePanelViewModel();
            var searchPanelVM = new SearchPanelViewModel();

            DataContext = this;
            //columnQueue.DataContext = queuePanelVM;
            columnSearch.DataContext = searchPanelVM;
            Search.OnSearch += searchPanelVM.PerformSearch;
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
            //settings.FocusedNodeChangedEnabled = true;
            Cef.Initialize(settings);
        }


        private void AddJSActivateUI(object sender, EventArgs e)
        {
            webPlayer.JavascriptObjectRepository.Register("app", webPanel, true);
            activateUI();
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
            //Shutdown?.Invoke();
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
                    //    //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"var ytpl = document.getElementById('movie_player');");
                    //    //webPlayer.GetMainFrame().ExecuteJavaScriptAsync(@"ytpl.addEventListener('onStateChange', function onPlayerStateChange(event){if(event==0){app.getNextVideo();}});");
                    //this.webPlayer.Load(@"https://www.youtube.com/tv#/watch/video/control?v=QBZfeWjy8ck&resume");
                    this.webPlayer.Load(@"https://www.youtube.com/watch?v=RlJ9zB74G_U&autoplay=1");
                    //this.webPlayer.Load(@"http://www.google.pl");
                    break;
            }
        }
    }
}