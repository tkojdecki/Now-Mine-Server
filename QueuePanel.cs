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
        StackPanel stackPanel;
        WebPanel webPanel;

        public QueuePanel(StackPanel stackPanel, WebPanel webPanel)
        {
            this.stackPanel = stackPanel;
            this.webPanel = webPanel;
            queue = new List<MusicPiece>();
        }

        public void addToQueue(MusicPiece musicPiece)
        {
            if (queue.Count == 0)
            {
                webPanel.playNow(musicPiece.Info.Id);
            }
            queue.Add(musicPiece);
            populateQueueBoard();
        }

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
            //webPanel.playNow(new Uri(musicPiece.Info.LinkUrl));
            deleteFromQueue(musicPiece);
            populateQueueBoard();
        }

        public void deleteFromQueue(MusicPiece musicPiece)
        {
            queue.Remove(musicPiece);
        }

        public MusicPiece getNextVideo()
        {
            deleteFromQueue(nowPlaying());
            return nowPlaying();
        }

        private MusicPiece nowPlaying()
        {
            return queue.ElementAt(0);
        }
    }
}
