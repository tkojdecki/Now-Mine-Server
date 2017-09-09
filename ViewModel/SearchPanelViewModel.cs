﻿using NowMine.Helpers;
using NowMine.Queue;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using NowMine.Data;

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

        private ObservableCollection<MusicData> _searchList;
        public ObservableCollection<MusicData> SearchList
        {
            get
            {
                if (_searchList == null)
                    _searchList = new ObservableCollection<MusicData>();
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
        /*
            var musicPiece = (MusicPiece)sender;
            var queueMusicPiece = musicPiece.copy();
            queueMusicPiece.MouseDoubleClick -= SearchResult_MouseDoubleClick;
            queueMusicPiece.userColorBrush();
            queueMusicPiece.lbluserName.Visibility = System.Windows.Visibility.Visible;
            int qPos = QueueManager.addToQueue(queueMusicPiece);
            e.Handled = true;
            */
        }

        public List<MusicData> GetSearchList(String searchWord)
        {
            List<MusicData> list;
            List<YouTubeInfo> infoList = youtubeProvider.LoadVideosKey(searchWord);
            list = InfoToResults(infoList);
            return list;
        }

        private List<MusicData> InfoToResults(List<YouTubeInfo> infoList)
        {
            List<MusicData> list = new List<MusicData>();
            foreach (YouTubeInfo info in infoList)
            {
                MusicData result = new MusicData(info);
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
            var searchList = this.GetSearchList(SearchText);
            var observableList = new ObservableCollection<MusicData>();
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
            var searchList = this.GetSearchList(SearchText);
            var observableList = new ObservableCollection<MusicData>();
            
            foreach (MusicData musicPiece in searchList)
            {
                //musicPiece.MouseDoubleClick += SearchResult_MouseDoubleClick;
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
