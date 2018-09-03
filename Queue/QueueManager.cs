using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NowMine.Helpers;
using System.ComponentModel;
using NowMine.ViewModel;
using NowMine.Models;

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

        public delegate void VideoQueuedEventArgs(object s, GenericEventArgs<ClipQueued> e);
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

        static public void OnVideoQueued(ClipQueued video)
        {
            if (video != null)
            {
                var e = new GenericEventArgs<ClipQueued>(video);
                VideoQueued?.Invoke(typeof(QueueManager), e);
            }
        }

        static public List<NetworkClipInfo> getQueueInfo()
        {
            int queueCount = Queue.Count;
            List<NetworkClipInfo> qInfo = new List<NetworkClipInfo>(queueCount);
            foreach (var musicPiece in Queue)
            {
                NetworkClipInfo qpts = new NetworkClipInfo(musicPiece.YTInfo, musicPiece.User);
                qInfo.Add(qpts);
            }
            return qInfo;
        }

        public static int AddToQueue(MusicData musicPiece)
        {
            musicPiece.OnClick += SendToPlay;
            int qPos = QueueCalculator.CalculateQueuePostition(musicPiece.User);
            if (qPos < Queue.Count && qPos >= 0)
            {
                Queue.Insert(qPos, musicPiece);
            }
            else
            {
                Queue.Add(musicPiece);
            }
            OnGlobalPropertyChanged();
            OnVideoQueued(new ClipQueued(musicPiece.YTInfo, qPos, musicPiece.User.Id));
            return qPos;
        }

        private static void SendToPlay(object sender, MusicData data)
        {
            int qPos = Queue.IndexOf(data);
            OnPlayedNow(qPos);

            ToHistory(nowPlaying());
            DeleteFromQueue(data);
            Queue.Insert(0, data);

            OnGlobalPropertyChanged();
        }

        static public void DeleteFromQueue(MusicData queuePiece)
        {
            if (Queue.Contains(queuePiece))
            {
                Queue.Remove(queuePiece);
            }
        }

        static public void DeleteFromQueue(string videoID, int userID)
        {
            foreach (var piece in Queue)
            {
                if (piece.YTInfo.ID == videoID && piece.User.Id == userID)
                {
                    OnRemovedPiece(Queue.IndexOf(piece));
                    Queue.Remove(piece);
                    OnGlobalPropertyChanged();
                    return;
                }
            }
        }


        static public void ToHistory(MusicData musicPiece)
        {
            //musicPiece.SetPlayedDate();
            DeleteFromQueue(musicPiece);
            //musicPiece.historyVisual();
            History.Add(musicPiece);
        }

        static public MusicData GetNextPiece()
        {
            if (Queue.Count >= 2)
            {
                return Queue[1];
            }
            return null;
        }

        static public void PlayNext()
        {
            if (nowPlaying() != null)
            {
                ToHistory(nowPlaying());
            }
            OnGlobalPropertyChanged();
            OnPlayedNext();
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
            OnGlobalPropertyChanged();
        }

        public static event PropertyChangedEventHandler GlobalPropertyChanged = delegate { };
        public static void OnGlobalPropertyChanged(string propertyName = "Queue")
        {
            GlobalPropertyChanged(
                typeof(QueueManager),
                new PropertyChangedEventArgs(propertyName));
        }

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        static void OnPropertyChanged(string propertyName)
        {
            var handler = StaticPropertyChanged;
            if (handler != null)
                handler(null, new PropertyChangedEventArgs(propertyName));
        }
    }
}