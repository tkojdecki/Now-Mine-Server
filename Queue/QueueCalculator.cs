using NowMine.Models;
using NowMine.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NowMine.Queue
{
    public static class QueueCalculator
    {
        public static int CalculateQueuePostition(User user)
        {
            if (QueueManager.Queue.Count == 0)
                return 0;
            //no
            int pos = -1;
            float songsPerUser = GetSongsPerUser(user);
            var rev = new List<MusicData>(QueueManager.Queue);
            rev.Reverse();
            foreach (var mPiece in rev)
            {
                if (mPiece.User == user)
                {
                    if (mPiece == QueueManager.Queue.Last())
                    {
                        return -1;
                    }
                    else
                    {
                        pos = QueueManager.Queue.IndexOf(mPiece) + 2;
                        return pos;
                    }
                }
                if (songsPerUser > GetSongsPerUser(mPiece.User))
                {
                    pos = QueueManager.Queue.IndexOf(mPiece) + 1;
                    return pos;
                }
            }
            pos = 1;
            return pos;
        }

        static private float GetSongsPerUser(User user)
        {
            List<User> uniq = new List<User> { user };
            int userQueuedSongs = 0;
            int numberOfUsersInQueue = 1;
            foreach (var musicPiece in QueueManager.Queue)
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
    }
}
