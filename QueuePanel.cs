using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NowMine
{
    class QueuePanel
    {
        public List<MusicPiece> queue { get; }

        public QueuePanel()
        {
            queue = new List<MusicPiece>();
        }

        public void addToQueue(MusicPiece musicPiece)
        {
            queue.Add(musicPiece);
        }

        public void deleteFromQueue(MusicPiece musicPiece)
        {
            queue.Remove(musicPiece);
        }
    }
}
