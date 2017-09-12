using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NowMine.View
{
    public partial class SearchControl : UserControl
    {
        public EventHandler<string> OnSearch;

        public SearchControl()
        {
            InitializeComponent();
            ToogleSearchEnabled(false);
            ClearSearchText();
        }

        public void ToogleSearchEnabled(bool enable)
        {
            SearchButton.IsEnabled = enable;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = Search();
            }
        }

        private void GotFocus(object sender, RoutedEventArgs e)
        {
            ClearSearchText();
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            e.Handled = Search();
        }

        private bool Search()
        {
            if (SearchButton.IsEnabled)
            {
                OnSearch.Invoke(this, SearchBox.Text);
                //ClearSearchText();
            }
            return SearchButton.IsEnabled;
        }

        private void ClearSearchText()
        {
            SearchBox.Text = "";
        }
        
    }
}
