using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;

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

        public List<YouTubeInfo> getQueueInfo()
        {
            int queueCount = queue.Count;
            List<YouTubeInfo> qInfo = new List<YouTubeInfo>(queueCount);

            foreach (QueuePiece queuePiece in queue)
            {
                MusicPiece musicPiece = queuePiece.MusicPiece;
                qInfo.Add(musicPiece.Info);
            }
            return qInfo;
        }

        public void addToQueue(MusicPiece musicPiece, User user)
        {
            QueuePiece queuePiece = new QueuePiece();
            queuePiece.MusicPiece = musicPiece;
            queuePiece.User = user;

            if (queue.Count == 0)
            {
                queuePiece.MusicPiece.nowPlayingVisual();
                webPanel.playNow(musicPiece.Info.id);
                queue.Add(queuePiece);
                populateQueueBoard();
                return;
            }

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
            //int numberOfUsersInQueue = queue.GroupBy(q => q.User.name).ToList().Count;
            float songsPerUser = getSongsPerUser(user);
            List<QueuePiece> rev = new List<QueuePiece>(queue);
            rev.Reverse();
            foreach (QueuePiece qPiece in rev)
            {
                if (qPiece.User == user)
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
                if (!(songsPerUser < getSongsPerUser(qPiece.User)))
                {
                    pos = queue.IndexOf(qPiece) + 1;
                    return pos;
                }
                if (qPiece.MusicPiece == nowPlaying().MusicPiece)
                {
                    pos = 1;
                    return pos;
                }
            }
            //int numberOfUsersInQueue = uniq.Count;
            //Console.WriteLine("number of users in queue: " + numberOfUsersInQueue);
            //int thisUserQueuedSongs = queue.GroupBy(q => q.User.name).Select(grp => grp.Where(gq => gq.User.name == user.name)).ToList().Count;
            //Console.WriteLine("number of user songs in queue: " + thisUserQueuedSongs);
            return pos;
        }

        private float getSongsPerUser(User user)
        {
            List<User> uniq = new List<User> { user };
            int userQueuedSongs = 0;
            int numberOfUsersInQueue = 1;
            foreach (QueuePiece queuePiece in queue)
            {
                if (!uniq.Contains(queuePiece.User))
                {
                    uniq.Add(queuePiece.User);
                    numberOfUsersInQueue++;
                }
                if (queuePiece.User == user)
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
                result.MusicPiece.MouseDoubleClick += Queue_DoubleClick;
                stackPanel.Children.Add(result.MusicPiece);
            }
        }


        private void Queue_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var musicPiece = (MusicPiece)sender;
            
            toHistory(nowPlaying());
            //var selected = queue.Where(queuePiece => queuePiece.MusicPiece == musicPiece);
            //Console.WriteLine(selected.ToString());
            //deleteFromQueue(selected.First());
            //queue.Insert(0, musicPiece);
            var queuePiece = findQueuePiece(musicPiece);
            deleteFromQueue(queuePiece);
            queue.Insert(0, queuePiece);
            webPanel.playNow(musicPiece.Info.id);
            populateQueueBoard();
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
            queuePiece.MusicPiece.historyVisual();
            history.Add(queuePiece);
        }

        public QueuePiece getNextPiece()
        {
            //deleteFromQueue(nowPlaying());
            //return nowPlaying();
            //if (queue.Count >= 2)
            //{
                return queue[1];
            //}
            //return null;
        }

        public MusicPiece getNextMusicPiece()
        {
            QueuePiece nextQueue = getNextPiece();
            return nextQueue.MusicPiece;
        }

        public bool playNext()
        {
            QueuePiece nextQueue = getNextPiece();
            if (nextQueue.MusicPiece == null)
            {
                MusicPiece nextVideo = nextQueue.MusicPiece;
                nextVideo.nowPlayingVisual();
                toHistory(nowPlaying());
                webPanel.playNow(nextVideo.Info.id);
                //deleteFromQueue(nowPlaying());
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
            //IEnumerable<QueuePiece> queuePieces = queue.Where(q => q.MusicPiece == musicPiece);
            //foreach (QueuePiece queuePiece in queuePieces)
            //{
            //    return queuePiece;  
            //}
            //return queuePieces.First();
            //tuniedziaua
            foreach(QueuePiece qPiece in queue)
            {
                if (qPiece.MusicPiece == musicPiece)
                {
                    return qPiece;
                }
            }
            return null;
        }

        public class QueuePiece
        {
            
            public MusicPiece MusicPiece;
            public User User;
        }
    }
}
