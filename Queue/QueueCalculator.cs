using System.Collections.Generic;
using System.Linq;
using NowMine.ViewModel;

namespace NowMine.Queue
{
    public static class QueueCalculator
    {
        public static int calculateQueuePostition(User user)
        {
            int pos = -1;
            float songsPerUser = getSongsPerUser(user);
            List<MusicData> rev = new List<MusicData>(QueueManager.Queue);
            rev.Reverse();
            foreach (MusicData mPiece in rev)
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
                if (!(songsPerUser < getSongsPerUser(mPiece.User)))
                {
                    pos = QueueManager.Queue.IndexOf(mPiece) + 1;
                    return pos;
                }
            }
            return pos;
        }

        static private float getSongsPerUser(User user)
        {
            List<User> uniq = new List<User> { user };
            int userQueuedSongs = 0;
            int numberOfUsersInQueue = 1;
            foreach (MusicData musicPiece in QueueManager.Queue)
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
