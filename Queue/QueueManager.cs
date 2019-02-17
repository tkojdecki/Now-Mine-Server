using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NowMine.Helpers;
using System.ComponentModel;
using NowMine.ViewModel;
using NowMineCommon.Models;
using NowMineCommon.Enums;

namespace NowMine.Queue
{
    class QueueManager : INotifyPropertyChanged
    {
        static private ObservableCollection<ClipData> _queue;
        static public ObservableCollection<ClipData> Queue
        {
            get
            {
                if (_queue == null)
                    _queue = new ObservableCollection<ClipData>();
                return _queue;
            }
            set
            {
                _queue = value;
            }
        }

        static private ObservableCollection<ClipData> _history;
        static public ObservableCollection<ClipData> History
        {
            get
            {
                if (_history == null)
                    _history = new ObservableCollection<ClipData>();
                return _history;
            }
            set
            {
                _history = value;
            }
        }

        public delegate void PlayedNextEventHandler(string nextVideoID, uint eventID);
        static public event PlayedNextEventHandler PlayedNext;

        public delegate void VideoQueuedEventArgs(ClipQueued clip, uint eventID);
        static public event VideoQueuedEventArgs VideoQueued;

        public delegate void PlayedNowEventHandler(int qPos, uint eventID);
        static public event PlayedNowEventHandler PlayedNow;

        public delegate void RemovedPieceEventHandler(byte[] message);
        static public event RemovedPieceEventHandler RemovedPiece;

        private static uint _queueIDCount;
        private static uint QueueIDCount
        {
            get
            {
                return ++_queueIDCount;
            }
        }

        static public void OnPlayedNow(int qPos, uint eventID)
        {
            PlayedNow?.Invoke(qPos, eventID);
        }

        static public void OnRemovedPiece(uint queueID, uint eventID)
        {
            var message = BytesMessegeBuilder.GetRemovedPieceBytes(queueID, eventID);
            RemovedPiece?.Invoke(message);
        }

        static public void OnPlayedNext(string nextVideoID, uint eventID)
        {
            //var bytes = BytesMessegeBuilder.GetPlayedNextBytes(queueID, eventID);
            PlayedNext?.Invoke(nextVideoID, eventID);
        }

        static public void OnVideoQueued(ClipQueued video, uint eventID)
        {
            if (video != null)
            {
                VideoQueued?.Invoke(video, eventID);
            }
        }

        static public List<ClipQueued> GetQueueInfo()
        {
            int queueCount = Queue.Count;
            List<ClipQueued> qInfo = new List<ClipQueued>(queueCount);
            foreach (var musicPiece in Queue)
            {
                ClipQueued qpts = new ClipQueued(musicPiece.ClipInfo, Queue.IndexOf(musicPiece), musicPiece.User.Id, musicPiece.QueueID);
                qInfo.Add(qpts);
            }
            return qInfo;
        }

        public static ClipQueued AddToQueue(ClipData musicPiece)
        {
            musicPiece.OnClick += SendToPlay;
            Queue.Add(musicPiece);
            int qPos = QueueCalculator.SortAndIndex(ref _queue, musicPiece);
            musicPiece.QueueID = QueueIDCount;
            OnGlobalPropertyChanged();
            var clip = new ClipQueued(musicPiece.ClipInfo, qPos, musicPiece.User.Id, musicPiece.QueueID);
            uint eventID = EventManager.GetIDForEvent(CommandType.QueueClip, clip);
            OnVideoQueued(clip, eventID);
            return clip;
        }

        private static void SendToPlay(object sender, ClipData data)
        {
            int qPos = Queue.IndexOf(data);
            var clip = new ClipQueued(data.ClipInfo, qPos, data.User.Id, data.QueueID);
            uint eventID = EventManager.GetIDForEvent(CommandType.PlayNow, clip);
            OnPlayedNow(qPos, eventID);

            ToHistory(nowPlaying());
            DeleteFromQueue(data, false);
            Queue.Insert(0, data);

            OnGlobalPropertyChanged();
        }

        static public void DeleteFromQueue(ClipData queuePiece, bool sendUDPMessage)
        {
            if (Queue.Contains(queuePiece))
            {
                Queue.Remove(queuePiece);
                uint queueID = queuePiece.QueueID;
                uint eventID = EventManager.GetIDForEvent(CommandType.PlayNext, queueID);
                if(sendUDPMessage)
                    OnRemovedPiece(queueID, eventID);
                QueueCalculator.SortQueue(ref _queue);
                OnGlobalPropertyChanged();
            }

        }

        static public bool DeleteFromQueue(uint queueID, int userID)
        {
            foreach (var clip in Queue)
            {
                if (clip.QueueID == queueID && clip.User.Id == userID)
                {
                    DeleteFromQueue(clip, true);
                    //uint eventID = EventManager.GetIDForEvent(CommandType.PlayNext, queueID);
                    //OnRemovedPiece(clip.QueueID, eventID);
                    //OnGlobalPropertyChanged();
                    QueueCalculator.SortQueue(ref _queue);
                    OnGlobalPropertyChanged();
                    return true;
                }
            }
            return false;
        }


        static public void ToHistory(ClipData musicPiece)
        {
            //DeleteFromQueue(musicPiece, true);
            History.Add(musicPiece);
        }

        static public ClipData GetNextPiece()
        {
            if (Queue.Count >= 2)
            {
                return Queue[1];
            }
            return null;
        }

        static public void PlayNext()
        {
            var _nowPlaying = nowPlaying();
            if (_nowPlaying != null)
            {
                ToHistory(_nowPlaying);
                DeleteFromQueue(_nowPlaying, false);
            }
            OnGlobalPropertyChanged();
            uint eventID = EventManager.GetIDForEvent(CommandType.PlayNext, _nowPlaying.QueueID);
            _nowPlaying = nowPlaying();
            string nowPlayingID = string.Empty;
            if (_nowPlaying != null)
                nowPlayingID = _nowPlaying.ClipInfo.ID;
            OnPlayedNext(nowPlayingID, eventID);
        }

        static public ClipData nowPlaying()
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
        public event PropertyChangedEventHandler PropertyChanged;

        static void OnPropertyChanged(string propertyName)
        {
            var handler = StaticPropertyChanged;
            if (handler != null)
                handler(null, new PropertyChangedEventArgs(propertyName));
        }
    }
}