using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NowMine.Helpers;
using System.ComponentModel;

namespace NowMine.Queue
{
    class QueueManager : INotifyPropertyChanged
    {
        static private ObservableCollection<MusicPiece> _queue;
        static public ObservableCollection<MusicPiece> Queue
        {
            get
            {
                if (_queue == null)
                    _queue = new ObservableCollection<MusicPiece>();
                return _queue;
            }
            set
            {
                _queue = value;
            }
        }

        static private ObservableCollection<MusicPiece> _history;
        static private ObservableCollection<MusicPiece> History
        {
            get
            {
                if (_history == null)
                    _history = new ObservableCollection<MusicPiece>();
                return _history;
            }
            set
            {
                _history = value;
            }
        }

        public delegate void VideoQueuedEventArgs(object s, GenericEventArgs<YoutubeQueued> e);
        static public event VideoQueuedEventArgs VideoQueued;

        static public void OnVideoQueued(YoutubeQueued video)
        {
            if (video != null)
            {
                var e = new GenericEventArgs<YoutubeQueued>(video);                
                VideoQueued?.Invoke(typeof(QueueManager), e);
            }
        }

        public delegate void PlayedNowEventHandler(object s, GenericEventArgs<int> e);
        static public event PlayedNowEventHandler PlayedNow;

        static public void OnPlayedNow(int qPos)
        {
            PlayedNow?.Invoke(typeof(QueueManager), new GenericEventArgs<int>(qPos));
        }


        static public List<QueuePieceToSend> getQueueInfo()
        {
            int queueCount = Queue.Count;
            List<QueuePieceToSend> qInfo = new List<QueuePieceToSend>(queueCount);

            foreach (MusicPiece musicPiece in Queue)
            {
                QueuePieceToSend qpts = new QueuePieceToSend(musicPiece.Info, musicPiece.User);
                qInfo.Add(qpts);
            }
            return qInfo;
        }

        static public int addToQueue(MusicPiece musicPiece)
        {
            musicPiece.MouseDoubleClick += Queue_DoubleClick;
            int qPos = QueueCalculator.calculateQueuePostition(musicPiece.User);
            musicPiece.userColorBrush();
            if (qPos < Queue.Count && qPos >= 0)
            {
                Queue.Insert(qPos, musicPiece);
            }
            else
            {
                Queue.Add(musicPiece);
            }
            OnGlobalPropertyChanged("Queue");
            OnVideoQueued(new YoutubeQueued(musicPiece.Info, qPos, musicPiece.User.Id));
            return qPos;
        }

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

        static public void deleteFromQueue(MusicPiece queuePiece)
        {
            if (Queue.Contains(queuePiece))
            {
                Queue.Remove(queuePiece);
                OnGlobalPropertyChanged("Queue");
            }
        }

        static public void toHistory(MusicPiece musicPiece)
        {
            musicPiece.setPlayedDate();
            deleteFromQueue(musicPiece);
            musicPiece.historyVisual();
            History.Add(musicPiece);
        }

        static public MusicPiece getNextPiece()
        {
            if (Queue.Count >= 2)
            {
                return Queue[1];
            }
            return null;
        }

        static public bool playNext()
        {
            MusicPiece nextVideo = getNextPiece();
            if (nowPlaying() != null)
            {
                toHistory(nowPlaying());
            }

            if (nextVideo != null)
            {
                nextVideo.nowPlayingVisual();
                return true;
            }
            else
            {
                return false;
            }

        }

        static public MusicPiece nowPlaying()
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
            foreach (MusicPiece piece in Queue)
            {
                if (piece.User.Id == user.Id)
                {
                    piece.User = user;
                    piece.userColorBrush();
                }
            }
            OnGlobalPropertyChanged("Queue");
        }

        static event PropertyChangedEventHandler GlobalPropertyChanged = delegate { };
        static void OnGlobalPropertyChanged(string propertyName)
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
