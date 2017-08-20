using NowMine.Helpers;
using NowMine.Queue;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NowMine.ViewModel
{
    class SearchPanelViewModel : INotifyPropertyChanged
    {
        YouTubeProvider youtubeProvider = new YouTubeProvider();

        private string _searchText;
        public string SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                if (value == _searchText)
                    return;

                _searchText = value;
                OnPropertyChanged("SearchText");
            }
        }

        private ObservableCollection<MusicPiece> _searchList;
        public ObservableCollection<MusicPiece> SearchList
        {
            get
            {
                if (_searchList == null)
                    _searchList = new ObservableCollection<MusicPiece>();
                return _searchList;
            }
            set
            {
                _searchList = value;
                OnPropertyChanged("SearchList");
            }
        }

        private void SearchResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var musicPiece = (MusicPiece)sender;
            var queueMusicPiece = musicPiece.copy();
            queueMusicPiece.MouseDoubleClick -= SearchResult_MouseDoubleClick;
            queueMusicPiece.userColorBrush();
            queueMusicPiece.lbluserName.Visibility = System.Windows.Visibility.Visible;
            int qPos = QueueManager.addToQueue(queueMusicPiece);
            e.Handled = true;
        }

        public List<MusicPiece> getSearchList(String searchWord)
        {
            List<MusicPiece> list;
            List<YouTubeInfo> infoList = youtubeProvider.LoadVideosKey(searchWord);
            list = infoToResults(infoList);
            return list;
        }

        private List<MusicPiece> infoToResults(List<YouTubeInfo> infoList)
        {
            List<MusicPiece> list = new List<MusicPiece>();
            foreach (YouTubeInfo info in infoList)
            {
                MusicPiece result = new MusicPiece(info);
                list.Add(result);
            }
            return list;
        }

        private ICommand _searchClickCommand;
        public ICommand SearchClickCommand
        {
            get
            {
                if (_searchClickCommand == null)
                {
                    _searchClickCommand = new RelayCommand(
                        param => this.SearchClickObject(),
                        param => this.CanSearchClick()
                    );
                }
                return _searchClickCommand;
            }
        }

        private bool CanSearchClick()
        {
            //if (string.IsNullOrEmpty(SearchText)) -- enable jak sie cos wpisze dopiero
            //    return false;
            return true;
        }

        private void SearchClickObject()
        {
            var searchList = getSearchList(SearchText);
            var observableList = new ObservableCollection<MusicPiece>();
            searchList.ForEach(m => observableList.Add(m));
            SearchList = observableList;
        }

        private ICommand _searchCommand;

        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new RelayCommand(
                        param => this.SearchObject(),
                        param => this.CanSearch()
                    );
                }
                return _searchCommand;
            }
        }

        private bool CanSearch()
        {
            //if (string.IsNullOrEmpty(SearchText)) -- enable jak sie cos wpisze dopiero
            //    return false;
            return true;
        }

        private void SearchObject()
        {
            var searchList = getSearchList(SearchText);
            var observableList = new ObservableCollection<MusicPiece>();
            foreach (MusicPiece musicPiece in searchList)
            {
                musicPiece.MouseDoubleClick += SearchResult_MouseDoubleClick;
                observableList.Add(musicPiece);
            }
            SearchList = observableList;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
