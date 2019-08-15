using NowMine.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace NowMine.View
{
    public partial class SearchPanel : UserControl
    {
        private static bool m_SearchedOnce = false;
        //public EventHandler<string> OnSearch;

        public SearchPanel()
        {
            InitializeComponent();
            ToogleSearchEnabled(false);
            //DataContext = new SearchPanelViewModel();
            //ClearSearchText();
        }

        public void ToogleSearchEnabled(bool enable)
        {
            SearchButton.IsEnabled = enable;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = ResetSearchScroll();
            }
        }

        private void ClearSearch(object sender, RoutedEventArgs e)
        {
            CheckSearchedOnce(); //todo clear text twice; is this even a problem?
            ClearSearchText();
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            CheckSearchedOnce();
            e.Handled = ResetSearchScroll();
        }

        private bool ResetSearchScroll()
        {
            if (SearchButton.IsEnabled && !string.IsNullOrEmpty(SearchBox.Text))
            {
                SearchScroll.ScrollToTop();
            }
            return SearchButton.IsEnabled;
        }

        private void CheckSearchedOnce()
        {
        //used to clear initial tooltip
            if (!m_SearchedOnce)
            {
                m_SearchedOnce = true;
                ClearSearchText();
            }
        }

        private void ClearSearchText()
        {
            SearchBox.Text = string.Empty;
        }


        public void AnimateSquizePanel()
        {
            Storyboard sb = Resources["SquizePanel"] as Storyboard;
            sb.Begin(this);
            //SearchPanelWidth = 0d;
            //OnPropertyChanged(new DependencyPropertyChangedEventArgs(this, "SearchPanelWidth"));
        }

        public void AnimateExpandPanel()
        {
            Storyboard sb = Resources["ExpandPanel"] as Storyboard;
            sb.Begin(this);
        }
    }
}