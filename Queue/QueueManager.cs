﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using NowMine.Helpers;
using System.ComponentModel;
using NowMine.Data;

namespace NowMine.Queue
{
    class QueueManager : INotifyPropertyChanged
    {
        static private ObservableCollection<MusicData> _queue;
        static public ObservableCollection<MusicData> Queue
        {
            get
            {
                if (_queue == null)
                    _queue = new ObservableCollection<MusicData>();
                return _queue;
            }
            set
            {
                _queue = value;
            }
        }

        static private ObservableCollection<MusicData> _history;
        static private ObservableCollection<MusicData> History
        {
            get
            {
                if (_history == null)
                    _history = new ObservableCollection<MusicData>();
                return _history;
            }
            set
            {
                _history = value;
            }
        }

        public delegate void VideoQueuedEventArgs(object s, MusicPieceReceivedEventArgs e);
        static public event VideoQueuedEventArgs VideoQueued;

        public delegate void PlayedNowEventHandler(object s, GenericEventArgs<int> e);
        static public event PlayedNowEventHandler PlayedNow;

        static public void OnPlayedNow(int qPos)
        {
            PlayedNow?.Invoke(typeof(QueueManager), new GenericEventArgs <int>(qPos));
        }

        static public void OnVideoQueued(YoutubeQueued video)
        {
            if (video != null)
            {
                var e = new MusicPieceReceivedEventArgs();
                e.YoutubeQueued = video;
                VideoQueued?.Invoke(typeof(QueueManager), e);
            }
        }

        static public List<QueuePieceToSend> getQueueInfo()
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

        public static int AddToQueue(MusicData musicPiece)
        {
            musicPiece.OnClick += Queue_DoubleClick;
            int qPos = QueueCalculator.calculateQueuePostition(musicPiece.User);
            if (qPos < Queue.Count && qPos >= 0)
            {
                Queue.Insert(qPos, musicPiece);
            }
            else
            {
                Queue.Add(musicPiece);
            }
            OnGlobalPropertyChanged("Queue");
            OnVideoQueued(new YoutubeQueued(musicPiece.YTInfo, qPos, musicPiece.User.Id));
            return qPos;
        }

        private static void Queue_DoubleClick(object sender, MusicData data)
        {
            toHistory(nowPlaying());
            deleteFromQueue(data);
            Queue.Insert(0, data);

            int qPos = Queue.IndexOf(data);
            OnPlayedNow(qPos);
        }

        /*
        static private void Queue_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var musicPiece = (MusicPiece)sender;
            toHistory(nowPlaying());
            deleteFromQueue(musicPiece);
            musicPiece.nowPlayingVisual();
            Queue.Insert(0, musicPiece);

            int qPos = Queue.IndexOf(musicPiece);
            OnPlayedNow(qPos);
            e.Handled = true;
        }
        */

        static public void deleteFromQueue(MusicData queuePiece)
        {
            if (Queue.Contains(queuePiece))
            {
                Queue.Remove(queuePiece);
                OnGlobalPropertyChanged("Queue");
            }
        }

        static public void toHistory(MusicData musicPiece)
        {
            musicPiece.SetPlayedDate();
            deleteFromQueue(musicPiece);
            //musicPiece.historyVisual();
            History.Add(musicPiece);
        }

        static public MusicData getNextPiece()
        {
            if (Queue.Count >= 2)
            {
                return Queue[1];
            }
            return null;
        }

        static public bool playNext()
        {
            MusicData nextVideo = getNextPiece();
            if (nowPlaying() != null)
            {
                toHistory(nowPlaying());
            }

            if (nextVideo != null)
            {
                //nextVideo.nowPlayingVisual();
                return true;
            }
            else
            {
                return false;
            }

        }

        static public MusicData nowPlaying()
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

        static internal void userChangeName(User user)
        {
            foreach (MusicData piece in Queue)
            {
                if (piece.User.Id == user.Id)
                {
                    piece.User = user;
                }
            }
            OnGlobalPropertyChanged("Queue");
        }

        static event PropertyChangedEventHandler GlobalPropertyChanged = delegate { };
        static void OnGlobalPropertyChanged(string propertyName)
        {
            GlobalPropertyChanged(
                typeof(QueueManager),
                new PropertyChangedEventArgs(propertyName));
        }

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        static void OnPropertyChanged(string propertyName)
        {
            var handler = StaticPropertyChanged;
            if (handler != null)
                handler(null, new PropertyChangedEventArgs(propertyName));
        }
    }
}
