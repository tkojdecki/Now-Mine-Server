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
        public List<MusicPiece> queue { get; }
<<<<<<< HEAD
        //to Dictionary
=======
>>>>>>> 3e8e90aa518eb41b9e9876abd6e8831da805f7c6
        public List<MusicPiece> history { get; }
        StackPanel stackPanel;
        WebPanel webPanel;

        public QueuePanel(StackPanel stackPanel, WebPanel webPanel)
        {
            this.stackPanel = stackPanel;
            this.webPanel = webPanel;
            queue = new List<MusicPiece>();
            history = new List<MusicPiece>();
<<<<<<< HEAD
        }

        public YouTubeInfo[] getQueueInfo()
        {
            int queueCount = queue.Count;
            YouTubeInfo[] qInfo = new YouTubeInfo[queueCount];
            for (int i = 0; i < queueCount; i++)
            {
                qInfo[i] = queue[i].Info;
            }
            return qInfo;
=======
>>>>>>> 3e8e90aa518eb41b9e9876abd6e8831da805f7c6
        }

        public void addToQueue(MusicPiece musicPiece)
        {
            if (queue.Count == 0)
            {
                musicPiece.nowPlayingVisual();
<<<<<<< HEAD
                webPanel.playNow(musicPiece.Info.id);
                queue.Add(musicPiece);
                return;
            }
            
            int qPos = calculateQueuePostition(musicPiece.user);
            if (qPos < queue.Count && qPos >= 0)
            {
                queue.Insert(qPos, musicPiece);
            }
            else
            {
                queue.Add(musicPiece);
=======
                webPanel.playNow(musicPiece.Info.Id);
>>>>>>> 3e8e90aa518eb41b9e9876abd6e8831da805f7c6
            }
            
            populateQueueBoard();
        }

<<<<<<< HEAD
        private int calculateQueuePostition(User user)
        {
            int pos = -1;
            int numberOfUsersInQueue;
            int thisUserQueuedSongs;

            return pos;
        }

=======
>>>>>>> 3e8e90aa518eb41b9e9876abd6e8831da805f7c6
        public void populateQueueBoard()
        {
            stackPanel.Children.Clear();
            foreach (MusicPiece result in queue)
            {
                result.MouseDoubleClick += Queue_DoubleClick;
                stackPanel.Children.Add(result);
            }
        }


        private void Queue_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var musicPiece = (MusicPiece)sender;
            deleteFromQueue(musicPiece);
            musicPiece.nowPlayingVisual();
            toHistory(nowPlaying());
            queue.Insert(0, musicPiece);
<<<<<<< HEAD
            webPanel.playNow(musicPiece.Info.id);
=======
            webPanel.playNow(musicPiece.Info.Id);
>>>>>>> 3e8e90aa518eb41b9e9876abd6e8831da805f7c6
            populateQueueBoard();
        }

        public void deleteFromQueue(MusicPiece musicPiece)
        {
            if (queue.Contains(musicPiece))
            {
                queue.Remove(musicPiece);
            }
        }

        public void toHistory(MusicPiece musicPiece)
        {
            deleteFromQueue(musicPiece);
            musicPiece.historyVisual();
            history.Add(musicPiece);
        }

        public MusicPiece getNextVideo()
        {
            //deleteFromQueue(nowPlaying());
            //return nowPlaying();
            if (queue.Count >= 2)
            {
                return queue.ElementAt(1);
            }
            return null;
        }

        public bool playNext()
        {
            MusicPiece nextVideo = getNextVideo();
            if (nextVideo != null)
            {
                nextVideo.nowPlayingVisual();
                toHistory(nowPlaying());
<<<<<<< HEAD
                webPanel.playNow(nextVideo.Info.id);
=======
                webPanel.playNow(nextVideo.Info.Id);
>>>>>>> 3e8e90aa518eb41b9e9876abd6e8831da805f7c6
                //deleteFromQueue(nowPlaying());
                populateQueueBoard();
                return true;
            }
            else
                return false;            
        }

        public MusicPiece nowPlaying()
        {
            return queue.ElementAt(0);
        }
    }
}
