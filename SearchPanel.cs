using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NowMine
{
    class SearchPanel
    {
        //List<SearchResult> resultsList;


        public List<SearchResult> getSearchList(String searchWord)
        {
            List<SearchResult> list;
            List<YouTubeInfo> infoList = YouTubeProvider.LoadVideosKey(searchWord);
            //this.resultsList = infoToResults(infoList);
            list = infoToResults(infoList);
            return list;
            //PopulateSearchBoard(this.resultsList);
        }

        private List<SearchResult> infoToResults(List<YouTubeInfo> infoList)
        {
            List<SearchResult> list = new List<SearchResult>();
            foreach (YouTubeInfo info in infoList)
            {
                SearchResult result = new SearchResult(info);
                list.Add(result);
            }
            return list;
        }

        //private void PopulateSearchBoard(List<SearchResult> results)
        //{
        //    MainWindow.searchBoard.Children.Clear();
        //    for (int i = 0; i < infos.Count; i++)
        //    {
        //        ListObject control = new ListObject { Info = infos[i] };
        //        //int angleMutiplier = i % 2 == 0 ? 1 : -1;
        //        //control.RenderTransform = new RotateTransform { Angle = GetRandom(30, angleMutiplier) };
        //        //control.SetValue(Canvas.LeftProperty, GetRandomDist(dragCanvas.ActualWidth - 150.0));
        //        //control.SetValue(Canvas.TopProperty, GetRandomDist(dragCanvas.ActualHeight - 150.0));
        //        //control.SelectedEvent += control_SelectedEvent;
        //        searchPanel.Children.Add(control);
        //    }
        //}

        //private void txtSearchBar_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        if (txtSearchBar.Text != string.Empty)
        //        {
        //            List<YouTubeInfo> infos = YouTubeProvider.LoadVideosKey(txtSearchBar.Text);
        //            PopulateSearchList(infos);
        //        }
        //        else
        //        {
        //            MessageBox.Show("you need to enter a search word", "Error",
        //                MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //    }
        //}
    }
}
