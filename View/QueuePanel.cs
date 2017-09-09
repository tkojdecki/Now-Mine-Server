using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace NowMine
{
    class QueuePanel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private ObservableCollection<MusicPiece> _queue;
        public ObservableCollection<MusicPiece> Queue
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
        private List<MusicPiece> history { get; }
        StackPanel stackPanel;
        WebPanel webPanel;

        public delegate void VideoEndedEventHandler(object s, EventArgs e);
        public event VideoEndedEventHandler VideoEnded;

        public delegate void PlayedNowEventHandler(object s, PlayedNowEventArgs e);
        public event PlayedNowEventHandler PlayedNow;

        protected virtual void OnPlayedNow(int qPos)
        {
            PlayedNow?.Invoke(this, new PlayedNowEventArgs() { qPos = qPos });
        }

        protected virtual void OnVideoEnded()
        {
            VideoEnded?.Invoke(this, EventArgs.Empty);
        }

        public QueuePanel(StackPanel stackPanel, WebPanel webPanel)
        {
            this.stackPanel = stackPanel;
            this.webPanel = webPanel;
            history = new List<MusicPiece>();
        }

        public List<QueuePieceToSend> getQueueInfo()
        {
            int queueCount = Queue.Count;
            List<QueuePieceToSend> qInfo = new List<QueuePieceToSend>(queueCount);

            /*
            foreach (MusicPiece musicPiece in Queue)
            {
                QueuePieceToSend qpts = new QueuePieceToSend(musicPiece.Info, musicPiece.User);
                qInfo.Add(qpts);
            }
            */
            return qInfo;
        }

        /*
        public int addToQueue(MusicPiece musicPiece)
        {
            musicPiece.MouseDoubleClick += Queue_DoubleClick;
            if (Queue.Count == 0 || !webPanel.isPlaying)
            {
                if (nowPlaying() != null)
                {
                    toHistory(nowPlaying());
                }
                musicPiece.nowPlayingVisual();
                webPanel.playNow(musicPiece.Info.id);
                Queue.Add(musicPiece);
                OnPropertyChanged("Queue");
                return 0;
            }
            int qPos = calculateQueuePostition(musicPiece.User);
            if (qPos < Queue.Count && qPos >= 0)
            {
                Queue.Insert(qPos, musicPiece);
            }
            else
            {
                Queue.Add(musicPiece);
            }
            OnPropertyChanged("Queue");
            return qPos;
        }
        */

        private int calculateQueuePostition(User user)
        {
            int pos = -1;
            float songsPerUser = getSongsPerUser(user);
            List<MusicPiece> rev = new List<MusicPiece>(Queue);
            rev.Reverse();
            foreach (MusicPiece mPiece in rev)
            {
                if (mPiece.User == user)
                {
                    if (mPiece == Queue.Last())
                    {
                        return -1;
                    }
                    else
                    {
                        pos = Queue.IndexOf(mPiece) + 2;
                        return pos;
                    }
                }
                if (!(songsPerUser < getSongsPerUser(mPiece.User)))
                {
                    pos = Queue.IndexOf(mPiece) + 1;
                    return pos;
                }
                if (mPiece == nowPlaying())
                {
                    pos = 1;
                    return pos;
                }
            }
            return pos;
        }

        private float getSongsPerUser(User user)
        {
            List<User> uniq = new List<User> { user };
            int userQueuedSongs = 0;
            int numberOfUsersInQueue = 1;
            foreach (MusicPiece musicPiece in Queue)
            {
                if (!uniq.Contains(musicPiece.User))
                {
                    uniq.Add(musicPiece.User);
                    numberOfUsersInQueue++;
                }
                if (musicPiece.User == user)
                {
                    userQueuedSongs++;
                }
            }
            return (float)userQueuedSongs / (float)numberOfUsersInQueue;
        }

        public void populateQueueBoard()
        {
            stackPanel.Children.Clear();
            foreach (MusicPiece result in Queue)
            {
                result.MouseDoubleClick += Queue_DoubleClick;
                stackPanel.Children.Add(result);
            }
        }

        private void Queue_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var musicPiece = (MusicPiece)sender;
            toHistory(nowPlaying());
            deleteFromQueue(musicPiece);
            musicPiece.nowPlayingVisual();
            Queue.Insert(0, musicPiece);

            //webPanel.playNow(musicPiece.Info.id);
            populateQueueBoard();
            int qPos = Queue.IndexOf(musicPiece);
            OnPlayedNow(qPos);
            e.Handled = true;
        }

        public void deleteFromQueue(MusicPiece queuePiece)
        {
            if (Queue.Contains(queuePiece))
            {
                Queue.Remove(queuePiece);
            }
        }

        public void toHistory(MusicPiece musicPiece)
        {
            musicPiece.setPlayedDate();
            deleteFromQueue(musicPiece);
            musicPiece.historyVisual();
            history.Add(musicPiece);
        }

        public MusicPiece getNextPiece()
        {
            if (Queue.Count >= 2)
            {
                return Queue[1];
            }
            return null;
        }

        public bool playNext()
        {
            MusicPiece nextVideo = getNextPiece();
            if (nowPlaying() != null)
            {
                toHistory(nowPlaying());
                webPanel.isPlaying = false;
            }
                
            if (nextVideo != null)
            {
                nextVideo.nowPlayingVisual();
                //toHistory(nowPlaying());
                //webPanel.playNow(nextVideo.Info.id);
                populateQueueBoard();
                return true;
            }
            else
            {
                populateQueueBoard();
                return false;
            }
                
        }

        public MusicPiece nowPlaying()
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

        internal void userChangeName(User user)
        {
            foreach (MusicPiece piece in Queue)
            {
                if (piece.User.Id == user.Id)
                {
                    piece.User = user;
                }
            }
            populateQueueBoard();
            OnPropertyChanged("Queue");
        }
    }

    public class PlayedNowEventArgs : EventArgs
    {
        public int qPos { get; set; }
    }
}
