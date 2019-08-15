using NowMine.APIProviders;
using NowMine.Helpers;
using NowMine.Queue;
using NowMineCommon.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;

namespace NowMine.ViewModel
{
    class SearchPanelViewModel : INotifyPropertyChanged
    {
        IAPIProvider apiProvider = new YouTubeProvider();

        private double _searchPanelWidth;

        public double SearchPanelWidth
        {
            get { return _searchPanelWidth; }
            set
            {
                _searchPanelWidth = value;
                OnPropertyChanged(nameof(SearchPanelWidth));
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
                OnPropertyChanged(nameof(SearchList));
            }
        }

        private void AddToQueue(object sender, ClipData data)
        {
            ClipData newData = data.Copy();
            QueueManager.AddToQueue(newData);
        }

        private ICommand _searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new RelayCommand(
                        param => this.Search(param.ToString()),
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

        private async void Search(string searchText)
        {
            var searchList = await GetSearchClipDataList(searchText);
            var observableList = new ObservableCollection<ClipData>();
            
            foreach (ClipData musicPiece in searchList)
            {
                //musicPiece.Color = SEARCH_COLOR;
                musicPiece.OnClick += this.AddToQueue;
                observableList.Add(musicPiece);
            }
            SearchList = observableList;
        }

        private async Task<List<ClipData>> GetSearchClipDataList(string searchWord)
        {
            List<ClipInfo> infoList = await apiProvider.GetSearchClipInfos(searchWord);
            List<ClipData> list = infoList.Select(i => new ClipData(i)).ToList();
            return list;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
