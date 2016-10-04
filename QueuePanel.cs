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
            QueuePiece queuePiece;
            queuePiece.MusicPiece = musicPiece;
            queuePiece.User = user;

            if (queue.Count == 0)
            {
                musicPiece.nowPlayingVisual();
                webPanel.playNow(musicPiece.Info.id);
                queue.Add(queuePiece);
                return;
            }
            
            int qPos = calculateQueuePostition(user);
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
            int numberOfUsersInQueue;
            int thisUserQueuedSongs;

            return pos;
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
            IEnumerable<QueuePiece> queuePieces = queue.Where(q => q.MusicPiece == musicPiece);
            foreach (QueuePiece queuePiece in queuePieces)
            {
                return queuePiece;  
            }
            return queuePieces.First();
            //tuniedziaua
        }

        public struct QueuePiece
        {
            
            public MusicPiece MusicPiece;
            public User User;
        }
    }
}
