using NowMine.APIProviders;
using NowMine.Helpers;
using NowMine.Models;
using NowMine.Queue;
using NowMineCommon.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;

namespace NowMine.ViewModel
{
    class SearchPanelViewModel : INotifyPropertyChanged
    {
        public static Color SEARCH_COLOR = Color.FromRgb(0,0,0);
        IAPIProvider apiProvider = new YouTubeProvider();

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

        private ObservableCollection<ClipData> _searchList;
        public ObservableCollection<ClipData> SearchList
        {
            get
            {
                if (_searchList == null)
                    _searchList = new ObservableCollection<ClipData>();
                return _searchList;
            }
            set
            {
                _searchList = value;
                OnPropertyChanged("SearchList");
            }
        }

        private void AddToQueue(object sender, ClipData data)
        {
            ClipData newData = data.Copy();
            int qPos = QueueManager.AddToQueue(newData);
        }

        public List<ClipData> GetSearchList(String searchWord)
        {
            List<ClipData> list;
            List<ClipInfo> infoList = apiProvider.GetSearchClipInfos(searchWord);
            list = InfoToResults(infoList);
            return list;
        }

        private List<ClipData> InfoToResults(List<ClipInfo> infoList)
        {
            List<ClipData> list = new List<ClipData>();
            foreach (ClipInfo info in infoList)
            {
                ClipData result = new ClipData(info);
                list.Add(result);
            }
            return list;
        }

        //TODO refactor
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
            var searchList = this.GetSearchList(SearchText);
            var observableList = new ObservableCollection<ClipData>();
            searchList.ForEach(m => observableList.Add(m));
            SearchList = observableList;
        }

        //TODO refactor
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

        public void PerformSearch(object sender, string searchText)
        {
            SearchText = searchText;
            //TODO refactor
            SearchCommand.Execute(null);
        }

        private bool CanSearch()
        {
            //if (string.IsNullOrEmpty(SearchText)) -- enable jak sie cos wpisze dopiero
            //    return false;
            return true;
        }

        private void SearchObject()
        {
            var searchList = this.GetSearchList(SearchText);
            var observableList = new ObservableCollection<ClipData>();
            
            foreach (ClipData musicPiece in searchList)
            {
                musicPiece.Color = SEARCH_COLOR;
                musicPiece.OnClick += this.AddToQueue;
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
