using NowMine.Models;
using NowMine.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NowMine.Queue
{
    public static class QueueCalculator
    {
        static public int SortAndIndex(ref ObservableCollection<ClipData> queue, ClipData clipData)
        {
            queue = SortQueue(ref queue);
            return queue.IndexOf(clipData);
        }

        static public ObservableCollection<ClipData> SortQueue(ref ObservableCollection<ClipData> queue)
        {
            var queueDic = new Dictionary<User, Queue<ClipData>>();
            foreach (var q in queue)
            {
                if (!queueDic.ContainsKey(q.User))
                {
                    queueDic.Add(q.User, new Queue<ClipData>());
                }
                var list = queueDic[q.User];
                list.Enqueue(q);
            }
            var userList = queueDic.Keys.ToList();
            var sortedQueue = new ObservableCollection<ClipData>();
            while (queueDic.Values.Any(i => i.Count > 0))
            {
                foreach (var key in queueDic.Keys)
                {
                    var userQueue = queueDic[key];
                    ClipData clipData = null;
                    if (userQueue.Count > 0)
                        clipData = userQueue.Dequeue();
                    if (clipData != null)
                        sortedQueue.Add(clipData);
                }
            }
            return sortedQueue;
        }
    }
}