using CefSharp;
using CefSharp.Wpf;
using NowMine.Helpers;
using NowMine.Queue;
using NowMine.ViewModel;
using System;
using System.Windows;
using NowMine.Interfaces;
using NowMine.VideoProviders;
using System.Timers;

namespace NowMine
{
    class WebPanel
    {
        ChromiumWebBrowser webControl;
        MainWindow mainWindow;
        public bool isPlaying = false;
        public IVideoProvider VideoProvider;
        private Timer aTimer;

        public delegate void VideoEndedEventHandler(object source, GenericEventArgs<int> e);
        public event VideoEndedEventHandler VideoEnded;

        public WebPanel(ref ChromiumWebBrowser webControl, MainWindow mainWindow)
        {
            this.webControl = webControl;
            this.mainWindow = mainWindow;
            //this.VideoProvider = new YTVanilla();
            this.VideoProvider = new YTTV();
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
            switch(VideoProvider.NextVideoType())
            {
                case Enumerates.NextVideoType.ChangeSite:
                    
                    Application.Current.Dispatcher.Invoke(new Action(() => {
                        this.webControl.Address = VideoProvider.GetVideoURL(id);
                        AddJsListener();
                    }));
                    break;
                case Enumerates.NextVideoType.Script:
                    webControl.GetMainFrame().ExecuteJavaScriptAsync("changeVideo('" + id + "')");
                    break;
            }
        }

        internal void playNow(object s, GenericEventArgs<int> e)
        {
            int qPos = e.EventData;
            if (QueueManager.Queue.Count > qPos)
            {
                PlayNow(QueueManager.Queue[qPos].YTInfo.id);
            }
        }


        public void PlayNow(MusicData musicPiece, int qPos)
        {
            if (musicPiece != null)
            {
                PlayNow(musicPiece.YTInfo.id);
                //OnPlayedNow(qPos);
                Console.WriteLine("Played Now {0}!", musicPiece.YTInfo.id);
            }
        }

        public void PlayNow(GenericEventArgs<YoutubeQueued> ytQueued)
        {
            if (ytQueued != null)
            {
                PlayNow(ytQueued.EventData.id);
                //OnPlayedNow(ytQueued.EventData.QPos);
                Console.WriteLine("Played Now {0}!", ytQueued.EventData.id);
            }
        }

        //functions to call from javascript
        public void getNextVideo()
        {
            
            isPlaying = false;
            OnVideoEnded();
            var nextVideo = QueueManager.getNextPiece();
            //Application.Current.Dispatcher.Invoke(new Action(() => { QueueManager.toHistory(QueueManager.nowPlaying()); }));
            if (nextVideo != null)
            {
                PlayNow(nextVideo.YTInfo.id);
            //    //Application.Current.Dispatcher.Invoke(new Action(() => { nextVideo.nowPlayingVisual();}));
            //    if (!isPlaying)
            //    {
            //        isPlaying = true;
            //        //Application.Current.Dispatcher.Invoke(new Action(() => { QueueManager.}));
            }
            else
            {

            }
            QueueManager.playNext();
        }


        public void errorHandle()
        {
            Console.WriteLine("ONERROR");
            var nowPlaying = QueueManager.nowPlaying();
            VideoProvider = VideoProvider.GetErrorHandler();

            //Application.Current.Dispatcher.Invoke(new Action(() =>
            //{
            //    this.webControl.Address = VideoProvider.GetVideoURL(nowPlaying.YTInfo.id);
            //}));
            mainWindow.isYoutubePage = true;
            mainWindow.videoID = nowPlaying.YTInfo.id;
            isPlaying = true;
        }

        private void AddJsListener()
        {
            Console.WriteLine("Injecting listiner for DOM creation");
            aTimer = new Timer(10 * 60 * 1);
            aTimer.Elapsed += AddJSEventListener_OnTimedEvent;
            aTimer.Enabled = true;
            aTimer.Start();
        }

        private void AddJSEventListener_OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Adding Javascript Listener!");
            webControl.GetMainFrame().ExecuteJavaScriptAsync(@"(async function () { await CefSharp.BindObjectAsync('app', 'bound');})();");
            webControl.GetMainFrame().ExecuteJavaScriptAsync(@"var ytpl = document.getElementById('movie_player');");
            webControl.GetMainFrame().ExecuteJavaScriptAsync(@"if (ytpl !== null){ app.moviePlayerFound(); }");
        }

        public void DomLoaded()
        {
            Console.WriteLine("DOM Loaded!");
            var Scripts = VideoProvider.GetAfterLoadScripts();
            foreach (var script in Scripts)
            {
                webControl.GetMainFrame().ExecuteJavaScriptAsync(script);
            }
        }

        public void VideoQueuedHandler(object s, GenericEventArgs<YoutubeQueued> args)
        {
            if (!isPlaying)
                PlayNow(args);
            //else
            //    OnPlayedNow(args.EventData.QPos);
        }

        internal void PlayedNowHandler(object s, GenericEventArgs<int> e)
        {
            int qPos = e.EventData == -1 ? QueueManager.Queue.Count - 1 : e.EventData;
            PlayNow(QueueManager.Queue[qPos], qPos);
        }

        public void moviePlayerFound()
        {
            Console.WriteLine("movie Player Found!");
            Application.Current.Dispatcher.Invoke(new Action(() => { aTimer.Enabled = false; aTimer.Stop(); }));
            aTimer.Elapsed -= AddJSEventListener_OnTimedEvent;
            DomLoaded();
        }
    }
}
