using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using NowMine.Helpers;
using NowMine.Queue;

namespace NowMine.ViewModel
{
    class QueuePanelViewModel : INotifyPropertyChanged
    {
        private const string QUEUE_TOOLTIP = "Double click items to add them to queue!\nRight-click for menu.";
        private const string HISTORY_TOOLTIP = "Play something to add it to history!\nRight-click for menu.";

        private bool m_NowPlayingVisibility = true;
        public bool NowPlayingVisibility
        {
            get
            {
                return this.m_NowPlayingVisibility;
            }
            set
            {
                if (value != m_NowPlayingVisibility)
                {
                    this.m_NowPlayingVisibility = value;
                    OnSettingsChanged();
                }
            }
        }

        private bool m_HistoryVisible = false;
        public bool HistoryVisible
        {
            get
            {
                return this.m_HistoryVisible;
            }
            set
            {
                if (value != m_HistoryVisible)
                {
                    this.m_HistoryVisible = value;
                    OnSettingsChanged();
                    OnPropertyChanged("ObservedQueue");
                    OnPropertyChanged("QueueTooltipContent");
                    OnPropertyChanged("QueueTooltipVisibility");
                }
            }
        }

        public bool QueueEmpty
        {
            get { return ObservedQueue.Count == 0; }
        }

        public ObservableCollection<MusicData> ObservedQueue
        {
            get
            {
                if (!HistoryVisible)
                {
                    if (QueueManager.Queue.Count > 1)
                    {
                        ObservableCollection<MusicData> queue = new ObservableCollection<MusicData>(QueueManager.Queue);
                        queue.RemoveAt(0);
                        return queue;
                    }
                    else
                    {
                        return new ObservableCollection<MusicData>();
                    }
                }
                else
                {
                    return QueueManager.History;
                }
            }
        }

        public MusicData NowPlaying
        {
            get
            {
                if (QueueManager.nowPlaying() != null)
                {
                    MusicData md = QueueManager.nowPlaying().Copy();
                    //md.Color = Color.FromRgb(255, 0, 0);
                    md.OnClick += ToggleNowPlayingVisibility;
                    return md;
                }
                else
                {
                    return null;
                }
            }
        }

        public string QueueTooltipContent
        {
            get
            {
                if (HistoryVisible)
                {
                    return HISTORY_TOOLTIP;
                }
                else
                {
                    return QUEUE_TOOLTIP;
                }
            }
        }

        public Visibility QueueTooltipVisibility
        {
            get
            {
                if (QueueEmpty)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        public QueuePanelViewModel()
        {
            QueueManager.GlobalPropertyChanged += QueueManager_OnGlobalPropertyChanged;
        }

        private void QueueManager_OnGlobalPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
        //todo magic strings
            OnPropertyChanged("NowPlaying");
            OnPropertyChanged("ObservedQueue");
            OnPropertyChanged("QueueTooltipVisibility");
        }

        private RelayCommand _toggleNowPlaying;
        public ICommand ToggleNowPlayingVisibilityCommand
        {
            get
            {
                if (_toggleNowPlaying == null)
                {
                    _toggleNowPlaying = new RelayCommand(param => this.ToggleNowPlayingVisibility(null, null));
                }
                return _toggleNowPlaying;
            }
        }

        private RelayCommand _toggleHistory;
        public ICommand ToggleHistoryVisibleCommand
        {
            get
            {
                if (_toggleHistory == null)
                {
                    _toggleHistory = new RelayCommand(param => this.ToggleHistoryVisibility(null, null));
                }
                return _toggleHistory;
            }
        }

        private RelayCommand _clearQueue;
        public ICommand ClearQueueCommand
        {
            get
            {
                if (_clearQueue == null)
                {
                    _clearQueue = new RelayCommand(param => this.ClearQueue(null, null));
                }
                return _clearQueue;
            }
        }

        public void ToggleNowPlayingVisibility(object sender, object args)
        {
            NowPlayingVisibility = !NowPlayingVisibility;
        }

        public void ToggleHistoryVisibility(object sender, object args)
        {
            HistoryVisible = !HistoryVisible;
        }

        public void ClearQueue(object sender, object args)
        {
            if (HistoryVisible)
            {
                QueueManager.ClearHistory();
            }
            else
            {
                QueueManager.ClearQueue();
            }
        }

        public event EventHandler<QueuePanelSettings> SettingsChanged;

        protected void OnSettingsChanged()
        {
            this.SettingsChanged?.Invoke(this, GetSettings());
        }

        private QueuePanelSettings GetSettings()
        {
            return new QueuePanelSettings(this.NowPlayingVisibility, this.HistoryVisible);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); 
        }
    }

    class QueuePanelSettings
    {
        public bool NowPlayingVisibility;

        public bool HistoryVisible;

        public QueuePanelSettings()
        {
            NowPlayingVisibility = true;
        }

        public QueuePanelSettings(bool nowPlayingVisibility, bool historyVisible)
        {
            NowPlayingVisibility = nowPlayingVisibility;
            HistoryVisible = historyVisible;
        }
    }
}
