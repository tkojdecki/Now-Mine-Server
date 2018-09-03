using CefSharp;
using CefSharp.Wpf;
using NowMine.Helpers;
using NowMine.Queue;
using System;
using System.Windows;
using NowMine.WebHandlers;
using System.Timers;
using System.IO;
using NowMine.Models;

namespace NowMine.ViewModel
{
    class WebPanelViewModel
    {
        ChromiumWebBrowser WebControl;
        public bool isPlaying = false;
        public IWebHandler VideoProvider;
        private Timer aTimer;

        public WebPanelViewModel(ref ChromiumWebBrowser webControl)
        {
            this.WebControl = webControl;
            //this.VideoProvider = new YTVanilla();
            this.VideoProvider = new YTTV();
            WebControl.IsBrowserInitializedChanged += WebPlayer_IsBrowserInitializedChanged;
            QueueManager.PlayedNow += PlayNow;
            QueueManager.VideoQueued += VideoQueuedHandler;
        }

        private void WebPlayer_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && !(bool)e.OldValue)
            {
                LoadHomePage();
            }
        }

        public void LoadHomePage()
        {
            string html = File.ReadAllText(Directory.GetCurrentDirectory() + @"\" + VideoProvider.GetHomePage());
            WebControl.LoadHtml(html, @"local://home.html");
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
                        this.WebControl.Address = VideoProvider.GetVideoURL(id);
                        AddJsListener();
                    }));
                    break;
                case Enumerates.NextVideoType.Script:
                    WebControl.GetMainFrame().ExecuteJavaScriptAsync("changeVideo('" + id + "')");
                    break;
            }
        }

        internal void PlayNow(object s, GenericEventArgs<int> e)
        {
            int qPos = e.EventData;
            if (QueueManager.Queue.Count > qPos)
            {
                PlayNow(QueueManager.Queue[qPos].YTInfo.ID);
            }
        }

        //functions to call from javascript
        public void getNextVideo()
        {
            isPlaying = false;
            var nextVideo = QueueManager.GetNextPiece();
            if (nextVideo != null)
            {
                PlayNow(nextVideo.YTInfo.ID);
                isPlaying = true;
            }
            else
            {
                LoadHomePage();
            }
            QueueManager.PlayNext();
        }

        public void ErrorHandle()
        {
            Console.WriteLine("ONERROR");
            var nowPlaying = QueueManager.nowPlaying();
            VideoProvider = VideoProvider.GetErrorHandler();  //no
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
            WebControl.GetMainFrame().ExecuteJavaScriptAsync(@"(async function () { await CefSharp.BindObjectAsync('app', 'bound');})();");
            WebControl.GetMainFrame().ExecuteJavaScriptAsync(@"var ytpl = document.getElementById('movie_player');");
            WebControl.GetMainFrame().ExecuteJavaScriptAsync(@"if (ytpl !== null){ app.moviePlayerFound(); }");
        }

        public void DomLoaded()
        {
            Console.WriteLine("DOM Loaded!");
            var Scripts = VideoProvider.GetAfterLoadScripts();
            foreach (var script in Scripts)
            {
                WebControl.GetMainFrame().ExecuteJavaScriptAsync(script);
            }
        }

        public void VideoQueuedHandler(object s, GenericEventArgs<ClipQueued> args)
        {
            if (!isPlaying)
                PlayNow(args.EventData.ID);
        }

        internal void PlayedNowHandler(object s, GenericEventArgs<int> e)
        {
            int qPos = e.EventData == -1 ? QueueManager.Queue.Count - 1 : e.EventData;
            PlayNow(QueueManager.Queue[qPos].YTInfo.ID);
        }

        public void MoviePlayerFound()
        {
            Console.WriteLine("movie Player Found!");
            Application.Current.Dispatcher.Invoke(new Action(() => { aTimer.Enabled = false; aTimer.Stop(); }));
            aTimer.Elapsed -= AddJSEventListener_OnTimedEvent;
            DomLoaded();
        }
    }
}
