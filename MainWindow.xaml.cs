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

namespace NowMine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<YouTubeInfo> queue = null;
        public MainWindow()
        {
            queue = new List<YouTubeInfo>();
            InitializeComponent();
            //List<YouTubeInfo> infos = YouTubeProvider.LoadVideosKey("Łona");
            //for (int i = 0; i < infos.Count; i++)
                //Console.WriteLine(infos[i].EmbedUrl);
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            List<SearchResult> list;
            SearchPanel searchPanel = new SearchPanel();
            list = searchPanel.getSearchList(txtSearch.Text);
            populateSearchBoard(list);
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
       private void populateSearchBoard(List<SearchResult> results)
        {
            searchBoard.Children.Clear();
            for (int i = 0; i < results.Count; i++)
            {
                //int angleMutiplier = i % 2 == 0 ? 1 : -1;
                //control.RenderTransform = new RotateTransform { Angle = GetRandom(30, angleMutiplier) };
                //control.SetValue(Canvas.LeftProperty, GetRandomDist(dragCanvas.ActualWidth - 150.0));
                //control.SetValue(Canvas.TopProperty, GetRandomDist(dragCanvas.ActualHeight - 150.0));
                //control.SelectedEvent += control_SelectedEvent;
                searchBoard.Children.Add(results[i]);
            }
        }
        /*
        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            //Console.Out.Write("asdf");
            //border.Visibility = Visibility.Hidden;
            if (searchExpander.IsExpanded == false)
                searchExpander.IsExpanded = true;
            else
                searchExpander.IsExpanded = false;
        }*/
    }
}
