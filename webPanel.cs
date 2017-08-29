using CefSharp;
using CefSharp.Wpf;
using NowMine.Helpers;
using NowMine.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls ;

namespace NowMine
{
    class WebPanel
    {
        ChromiumWebBrowser webControl;
        MainWindow mainWindow;
        public bool isPlaying = false;

        public delegate void VideoEndedEventHandler(object source, GenericEventArgs<int> e);
        public event VideoEndedEventHandler VideoEnded;

        public delegate void PlayedNowEventHandler(object s, GenericEventArgs<int> e);
        public event PlayedNowEventHandler PlayedNow;

        public void OnPlayedNow(int qPos)
        {
            PlayedNow?.Invoke(this, new GenericEventArgs<int>(qPos));
        }

        public WebPanel(ChromiumWebBrowser webControl, MainWindow mainWindow)
        {
            this.webControl = webControl;
            this.mainWindow = mainWindow;
        }

        public void reinitialize(ChromiumWebBrowser webControl)
        {
            this.webControl = webControl;
        }

        protected virtual void OnVideoEnded()
        {
            VideoEnded?.Invoke(this, new GenericEventArgs<int>(0));
        }

        private void PlayNow(String id)
        {
            if (!isPlaying)
            {
                isPlaying = true;
            }
            webControl.GetMainFrame().ExecuteJavaScriptAsync("changeVideo('" + id + "')");
        }

        public void PlayNow(MusicPiece musicPiece, int qPos)
        {
            if (musicPiece != null)
            {
                PlayNow(musicPiece.Info.id);
                OnPlayedNow(qPos);
                Console.WriteLine("Played Now {0}!", musicPiece.Info.id);
            }
                
        }

        public void PlayNow(GenericEventArgs<YoutubeQueued> ytQueued)
        {
            if (ytQueued != null)
            {
                PlayNow(ytQueued.EventData.id);
                OnPlayedNow(ytQueued.EventData.QPos);
                Console.WriteLine("Played Now {0}!", ytQueued.EventData.id);
            }

        }

        //functions to call from javascript
        public void getNextVideo()
        {
            MusicPiece nextVideo = QueueManager.getNextPiece();
            Application.Current.Dispatcher.Invoke(new Action(() => { QueueManager.toHistory(QueueManager.nowPlaying()); }));
            if (nextVideo != null)
            {
                Application.Current.Dispatcher.Invoke(new Action(() => { nextVideo.nowPlayingVisual(); }));
                if (!isPlaying)
                    isPlaying = true;
            }
            else
            {
                isPlaying = false;
            }
            OnVideoEnded();
        }

        public void errorHandle()
        {
            Console.WriteLine("ONERROR");
            MusicPiece nowPlaying = QueueManager.nowPlaying();
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.webControl.Address = @"http://www.youtube.com/watch?v=" + nowPlaying.Info.id;
            }));
            mainWindow.isYoutubePage = true;
            mainWindow.videoID = nowPlaying.Info.id;
            isPlaying = true;
        }

        internal void setYoutubeWrapper(bool isInitial)
        {
            this.webControl.Address = @"local://index.html";
            QueueManager.playNext();
        }

        public void VideoQueuedHandler(object s, GenericEventArgs<YoutubeQueued> args)
        {
            if (!isPlaying)
                PlayNow(args);
        }

        internal void PlayedNowHandler(object s, GenericEventArgs<int> e)
        {
            int qPos = e.EventData == -1 ? QueueManager.Queue.Count - 1 : e.EventData;
            PlayNow(QueueManager.Queue[qPos], qPos);
        }
    }
}
