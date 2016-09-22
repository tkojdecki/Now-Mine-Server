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
        Server server;

        public MainWindow()
        {
            InitializeComponent();
            webPanel = new WebPanel(webPlayer, queuePanel);
            queuePanel = new QueuePanel(queueBoard, webPanel);
            searchPanel = new SearchPanel(searchBoard, txtSearch, queuePanel);
            webPanel.reinitialize(webPlayer, queuePanel);
            
            server = new Server();
            serverThread = new Thread(() => server.ServerInit(queuePanel));
            serverThread.IsBackground = true;
            serverThread.Start();

            udpThread = new Thread(server.udpListener);
            udpThread.IsBackground = true;
            udpThread.Start();
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
    }
}
