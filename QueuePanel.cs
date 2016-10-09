using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace NowMine
{
    class QueuePanel
    {
        private List<QueuePiece> queue { get; }
        private List<QueuePiece> history { get; }
        StackPanel stackPanel;
        WebPanel webPanel;

        public QueuePanel(StackPanel stackPanel, WebPanel webPanel)
        {
            this.stackPanel = stackPanel;
            this.webPanel = webPanel;
            queue = new List<QueuePiece>();
            history = new List<QueuePiece>();
        }

        public List<QueuePieceToSend> getQueueInfo()
        {
            int queueCount = queue.Count;
            List<QueuePieceToSend> qInfo = new List<QueuePieceToSend>(queueCount);

            foreach (QueuePiece queuePiece in queue)
            {
                MusicPiece musicPiece = queuePiece.musicPiece;
                QueuePieceToSend qpts = new QueuePieceToSend(musicPiece.Info, queuePiece.user);
                qInfo.Add(qpts);
            }
            return qInfo;
        }

        public void addToQueue(MusicPiece musicPiece, User user)
        {
            QueuePiece queuePiece = new QueuePiece(musicPiece, user);

            if (queue.Count == 0)
            {
                queuePiece.musicPiece.nowPlayingVisual();
                webPanel.playNow(musicPiece.Info.id);
                queue.Add(queuePiece);
                populateQueueBoard();
                return;
            }

            musicPiece.userColorBrush(user);
            int qPos = calculateQueuePostition(user);
            //int qPos = -1;
            if (qPos < queue.Count && qPos >= 0)
            {
                queue.Insert(qPos, queuePiece);
            }
            else
            {
                queue.Add(queuePiece);
            }
            
            populateQueueBoard();
        }

        private int calculateQueuePostition(User user)
        {
            int pos = -1;
            float songsPerUser = getSongsPerUser(user);
            List<QueuePiece> rev = new List<QueuePiece>(queue);
            rev.Reverse();
            foreach (QueuePiece qPiece in rev)
            {
                if (qPiece.user == user)
                {
                    if (qPiece == queue.Last())
                    {
                        return -1;
                    }
                    else
                    {
                        pos = queue.IndexOf(qPiece) + 2;
                        return pos;
                    }
                }
                if (!(songsPerUser < getSongsPerUser(qPiece.user)))
                {
                    pos = queue.IndexOf(qPiece) + 1;
                    return pos;
                }
                if (qPiece.musicPiece == nowPlaying().musicPiece)
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
            foreach (QueuePiece queuePiece in queue)
            {
                if (!uniq.Contains(queuePiece.user))
                {
                    uniq.Add(queuePiece.user);
                    numberOfUsersInQueue++;
                }
                if (queuePiece.user == user)
                {
                    userQueuedSongs++;
                }
            }
            return (float)userQueuedSongs / (float)numberOfUsersInQueue;
        }

        public void populateQueueBoard()
        {
            stackPanel.Children.Clear();
            foreach (QueuePiece result in queue)
            {
                result.musicPiece.MouseDoubleClick += Queue_DoubleClick;
                stackPanel.Children.Add(result.musicPiece);
            }
        }

        private void Queue_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var musicPiece = (MusicPiece)sender;
            var queuePiece = findQueuePiece(musicPiece);
            if (queuePiece != null)
            {
                toHistory(nowPlaying());
                deleteFromQueue(queuePiece);
                queuePiece.musicPiece.nowPlayingVisual();
                queue.Insert(0, queuePiece);

                webPanel.playNow(musicPiece.Info.id);
                populateQueueBoard();
            }
            
        }

        public void deleteFromQueue(QueuePiece queuePiece)
        {
            if (queue.Contains(queuePiece))
            {
                queue.Remove(queuePiece);
            }
        }

        public void toHistory(QueuePiece queuePiece)
        {
            deleteFromQueue(queuePiece);
            queuePiece.musicPiece.historyVisual();
            history.Add(queuePiece);
        }

        public QueuePiece getNextPiece()
        {
            if (queue.Count >= 2)
            {
                return queue[1];
            }
            return null;
        }

        public MusicPiece getNextMusicPiece()
        {
            QueuePiece nextQueue = getNextPiece();
            return nextQueue.musicPiece;
        }

        public bool playNext()
        {
            QueuePiece nextQueue = getNextPiece();
            if (nextQueue.musicPiece != null)
            {
                MusicPiece nextVideo = nextQueue.musicPiece;
                nextVideo.nowPlayingVisual();
                toHistory(nowPlaying());
                webPanel.playNow(nextVideo.Info.id);
                populateQueueBoard();
                return true;
            }
            else
                return false;            
        }

        public QueuePiece nowPlaying()
        {
            return queue.First();
        }

        private QueuePiece findQueuePiece(MusicPiece musicPiece)
        {
            foreach(QueuePiece qPiece in queue)
            {
                if (qPiece.musicPiece == musicPiece)
                {
                    return qPiece;
                }
            }
            return null;
        }

        public class QueuePiece
        {
            
            public MusicPiece musicPiece { get; set; }
            public User user { get; set; }

            public QueuePiece (MusicPiece musicPiece, User user)
            {
                this.musicPiece = musicPiece;
                this.user = user;
            }
        }
    }
}
