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
        ChromiumWebBrowser webPlayer;

        public MainWindow()
        {
            InitializeComponent();
            InitializeChromium();
            webPlayer = new ChromiumWebBrowser();

            webPanel = new WebPanel(webPlayer, queuePanel);
            webPlayer.RegisterJsObject("app", webPanel);
            webPlayer.FrameLoadEnd += WebPlayer_FrameLoadEnd;
            webPlayer.Initialized += WebPlayer_Initialized;
            
            columnPlayer.Children.Add(webPlayer);

            queuePanel = new QueuePanel(queueBoard, webPanel);
            searchPanel = new SearchPanel(searchBoard, txtSearch, queuePanel);
            webPanel.reinitialize(webPlayer, queuePanel);

            this.serverTCP = new ServerTCP();
            serverThread = new Thread(() => serverTCP.ServerInit(queuePanel));
            serverThread.IsBackground = true;
            serverThread.Start();

            this.serverUDP = new ServerUDP(this.serverTCP);
            udpThread = new Thread(serverUDP.udpListener);
            udpThread.IsBackground = true;
            udpThread.Start();
            

        }

        private void InitializeChromium()
        {
            var settings = new CefSettings();
            settings.CefCommandLineArgs.Add("disable-gpu-vsync", "1");
            settings.CefCommandLineArgs.Add("disable-gpu", "1");
            settings.LogSeverity = LogSeverity.Verbose;
            settings.CefCommandLineArgs.Add("no-proxy-server", "1");

            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = "local",
                SchemeHandlerFactory = new FolderSchemeHandlerFactory(
                                                rootFolder: @"..\..\..\Resources",
                                                schemeName: "local", //Optional param no schemename checking if null
                                                //hostName: "cefsharp", //Optional param no hostname checking if null
                                                defaultPage: "YoutubeWrapper.html" //Optional param will default to index.html
                                                )
            });

            Cef.Initialize(settings);
        }


        private void WebPlayer_Initialized(object sender, EventArgs e)
        {
            this.webPlayer.Address = @"local://index.html";
        }

        private void WebPlayer_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            Console.WriteLine("Frame Load End: " +  e.Frame + " " + e.HttpStatusCode + " " + e.Url);
            if (e.HttpStatusCode == 200 && e.Url.Contains("youtube.com"))
            {
                Application.Current.Dispatcher.Invoke(new Action(() => { activateUI(); }));
            }
        }

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
            queuePanel.playNext();
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