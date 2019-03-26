using NowMine.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NowMine.View
{
    public partial class SearchControl : UserControl
    {
        private static bool m_SearchedOnce = false;
        //public EventHandler<string> OnSearch;

        public SearchControl()
        {
            InitializeComponent();
            ToogleSearchEnabled(false);
            DataContext = new SearchPanelViewModel();
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

    }
}