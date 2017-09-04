using CefSharp;
using CefSharp.Wpf;
using NowMine.Helpers;
using NowMine.Queue;
using System;
using System.Windows;

namespace NowMine
{
    class WebPanel
    {
        ChromiumWebBrowser webControl;
        MainWindow mainWindow;
        public bool isPlaying = false;

        public delegate void VideoEndedEventHandler(object source, GenericEventArgs<int> e);
        public event VideoEndedEventHandler VideoEnded;

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

        public void playNow(String id)
        {
            if (!isPlaying)
            {
                isPlaying = true;
            }
            webControl.GetMainFrame().ExecuteJavaScriptAsync("changeVideo('" + id + "')");
            Console.WriteLine("playnow!");
        }

        public void playNow(MusicPiece musicPiece)
        {
            if (musicPiece != null)
            {
                if (!isPlaying)
                {
                    isPlaying = true;
                }
                webControl.GetMainFrame().ExecuteJavaScriptAsync("changeVideo('" + musicPiece.Info.id + "')");
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

        public void VideoQueuedHandler(object s, MusicPieceReceivedEventArgs args)
        {
            if (!isPlaying)
                playNow(args.YoutubeQueued.id);
        }

        internal void PlayedNowHandler(object s, GenericEventArgs<int> e)
        {
            playNow(QueueManager.Queue[e.EventData]);
        }
    }
}
