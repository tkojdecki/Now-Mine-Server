using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NowMine.Helpers;
using System.ComponentModel;
using NowMine.ViewModel;

namespace NowMine.Queue
{
    class QueueManager : INotifyPropertyChanged
    {
        static private ObservableCollection<MusicData> _queue;
        static public ObservableCollection<MusicData> Queue
        {
            get
            {
                if (_queue == null)
                    _queue = new ObservableCollection<MusicData>();
                return _queue;
            }
            set
            {
                _queue = value;
            }
        }

        static private ObservableCollection<MusicData> _history;
        static public ObservableCollection<MusicData> History
        {
            get
            {
                if (_history == null)
                    _history = new ObservableCollection<MusicData>();
                return _history;
            }
            set
            {
                _history = value;
            }
        }

        static public event EventHandler PlayedNext;

        public delegate void VideoQueuedEventArgs(object s, GenericEventArgs<YoutubeQueued> e);
        static public event VideoQueuedEventArgs VideoQueued;

        public delegate void PlayedNowEventHandler(object s, GenericEventArgs<int> e);
        static public event PlayedNowEventHandler PlayedNow;

        public delegate void RemovedPieceEventHandler(object s, GenericEventArgs<int> e);
        static public event RemovedPieceEventHandler RemovedPiece;

        static public void OnPlayedNow(int qPos)
        {
            PlayedNow?.Invoke(typeof(QueueManager), new GenericEventArgs<int>(qPos));
        }

        static public void OnRemovedPiece(int qPos)
        {
            RemovedPiece?.Invoke(typeof(QueueManager), new GenericEventArgs<int>(qPos));
        }

        static public void OnPlayedNext()
        {
            PlayedNext?.Invoke(typeof(QueueManager), EventArgs.Empty);
        }

        static public void OnVideoQueued(YoutubeQueued video)
        {
            if (video != null)
            {
                var e = new GenericEventArgs<YoutubeQueued>(video);
                VideoQueued?.Invoke(typeof(QueueManager), e);
            }
        }

        static public List<NetworkYoutubeInfo> getQueueInfo()
        {
            int queueCount = Queue.Count;
            List<NetworkYoutubeInfo> qInfo = new List<NetworkYoutubeInfo>(queueCount);
            foreach (var musicPiece in Queue)
            {
                NetworkYoutubeInfo qpts = new NetworkYoutubeInfo(musicPiece.YTInfo, musicPiece.User);
                qInfo.Add(qpts);
                var asdf = new int[2];
                asdf[0]++;
            }
            return qInfo;
        }

        public static int AddToQueue(MusicData musicPiece)
        {
            musicPiece.OnClick += SendToPlay;
            int qPos = QueueCalculator.calculateQueuePostition(musicPiece.User);
            if (qPos < Queue.Count && qPos >= 0)
            {
                Queue.Insert(qPos, musicPiece);
            }
            else
            {
                Queue.Add(musicPiece);
            }
            OnGlobalPropertyChanged("Queue");
            OnVideoQueued(new YoutubeQueued(musicPiece.YTInfo, qPos, musicPiece.User.Id));
            return qPos;
        }

        private static void SendToPlay(object sender, MusicData data)
        {
            int qPos = Queue.IndexOf(data);
            OnPlayedNow(qPos);

            toHistory(nowPlaying());
            deleteFromQueue(data);
            Queue.Insert(0, data);

            OnGlobalPropertyChanged("Queue");
        }

        /*
        static private void Queue_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var musicPiece = (MusicPiece)sender;
            toHistory(nowPlaying());
            deleteFromQueue(musicPiece);
            musicPiece.nowPlayingVisual();
            Queue.Insert(0, musicPiece);

            int qPos = Queue.IndexOf(musicPiece);
			OnPlayedNow(qPos);
            e.Handled = true;
        }
        */

        static public void deleteFromQueue(MusicData queuePiece)
        {
            if (Queue.Contains(queuePiece))
            {
                Queue.Remove(queuePiece);
            }
        }

        static public void deleteFromQueue(string videoID, int userID)
        {
            foreach (var piece in Queue)
            {
                if (piece.YTInfo.id == videoID && piece.User.Id == userID)
                {
                    OnRemovedPiece(Queue.IndexOf(piece));
                    Queue.Remove(piece);
                    OnGlobalPropertyChanged("Queue");
                    return;
                }
            }
        }


        static public void toHistory(MusicData musicPiece)
        {
            musicPiece.SetPlayedDate();
            deleteFromQueue(musicPiece);
            //musicPiece.historyVisual();
            History.Add(musicPiece);
        }

        static public MusicData getNextPiece()
        {
            if (Queue.Count >= 2)
            {
                return Queue[1];
            }
            return null;
        }

        static public bool playNext()
        {
            MusicData nextVideo = getNextPiece();
            if (nowPlaying() != null)
            {
                toHistory(nowPlaying());
            }
            OnGlobalPropertyChanged("Queue");
            OnPlayedNext();
            if (nextVideo != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        static public MusicData nowPlaying()
        {
            if (Queue.Count >= 1)
            {
                return Queue.First();
            }
            else
            {
                return null;
            }

        }

        static internal void RefreshQueueUserNames(User user)
        {
            foreach (MusicData piece in Queue)
            {
                if (piece.User.Id == user.Id)
                {
                    piece.User = user;
                }
            }
            OnGlobalPropertyChanged("Queue");
        }

        public static void ClearHistory()
        {
            History.Clear();
            OnGlobalPropertyChanged("History");
        }

        public static void ClearQueue()
        {
            //todo clear only current user
            while (Queue.Count > 1)
            {
                Queue.RemoveAt(Queue.Count - 1);
            }
            OnGlobalPropertyChanged("Queue");
        }

        public static event PropertyChangedEventHandler GlobalPropertyChanged = delegate { };
        public static void OnGlobalPropertyChanged(string propertyName)
        {
            GlobalPropertyChanged(
                typeof(QueueManager),
                new PropertyChangedEventArgs(propertyName));
        }

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        static void OnPropertyChanged(string propertyName)
        {
            var handler = StaticPropertyChanged;
            if (handler != null)
                handler(null, new PropertyChangedEventArgs(propertyName));
        }
    }
}