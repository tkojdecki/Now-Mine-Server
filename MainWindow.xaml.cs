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
using Awesomium;
using Awesomium.Windows.Controls;
using Awesomium.Core;
using Awesomium.Core.Data;

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

        public MainWindow()
        {
            InitializeComponent();
            webPanel = new WebPanel(webPlayer, queuePanel);
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

            //columnSearch.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            //columnPlayer.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            //columnQueue.Background = new SolidColorBrush(Color.FromRgb(0, 0, 255));
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

        private void webPlayer_DocumentReady(object sender, DocumentReadyEventArgs e)
        {
            webPanel.BindMethods();
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
            
            //searchBoard.Visibility = Visibility.Collapsed;
            //searchButtons.Visibility = Visibility.Collapsed;
            //txtSearch.Visibility = Visibility.Collapsed;
            //searchButton.Visibility = Visibility.Collapsed;
            //playNextButton.Visibility = Visibility.Collapsed;
            columnSearch.Visibility = Visibility.Collapsed;
            //columnSearch.Width = 10.0f;
            columnDefinitionSearch.Width = new GridLength(0);
            //mainGrid.Children.Remove(columnSearch);

            columnQueue.Visibility = Visibility.Collapsed;
            columnDefinitionQueue.Width = new GridLength(0);

            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;

            //webPlayer.Reload(true);
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
            //searchBoard.Visibility = Visibility.Visible;
            //queueBoard.Visibility = Visibility.Visible;
            //queueScroll.Visibility = Visibility.Visible;
        }

        //private void refreshButton_Click(object sender, RoutedEventArgs e)
        //{
        //    webPlayer.Reload(true);
        //}
    }
}
