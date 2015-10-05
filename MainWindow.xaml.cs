using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace NowMine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SearchPanel searchPanel = null;
        QueuePanel queuePanel = null;


        public MainWindow()
        {
            queuePanel = new QueuePanel();
            searchPanel = new SearchPanel();
            InitializeComponent();
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            List<MusicPiece> list;
            list = searchPanel.getSearchList(txtSearch.Text);
            populateSearchBoard(list);
        }

       private void populateSearchBoard(List<MusicPiece> results)
        {
            searchBoard.Children.Clear();
            foreach (MusicPiece result in results)
            {
                result.MouseDoubleClick += SearchResult_MouseDoubleClick;
                searchBoard.Children.Add(result);
            }
        }

        private void SearchResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var musicPiece = (MusicPiece)sender;
            
            if (webPlayer.Source == new Uri("about:blank"))
            {
                webPlayer.Source = new Uri(musicPiece.Info.LinkUrl);
            }
            var queueMusicPiece = musicPiece.copy();
            queuePanel.addToQueue(queueMusicPiece);
            populateQueueBoard();
        }

        private void populateQueueBoard()
        {
            queueBoard.Children.Clear();
            List<MusicPiece> queue = queuePanel.queue;
            foreach (MusicPiece result in queue)
            {
                result.MouseDoubleClick += Queue_DoubleClick;
                queueBoard.Children.Add(result);
            }
        }

        private void Queue_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var musicPiece = (MusicPiece)sender;
            webPlayer.Source = new Uri(musicPiece.Info.LinkUrl);
            queuePanel.deleteFromQueue(musicPiece);
            populateQueueBoard();
        }




        /*
private void txtSearchBar_KeyDown(object sender, KeyEventArgs e)
{
    if (e.Key == Key.Enter)
    {
        if (txtSearchBar.Text != string.Empty)
        {
            List<YouTubeInfo> infos = YouTubeProvider.LoadVideosKey(txtSearchBar.Text);
            PopulateSearchList(infos);
        }
        else
        {
            MessageBox.Show("you need to enter a search word", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
*/
    }
}
