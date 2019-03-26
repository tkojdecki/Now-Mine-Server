using CefSharp;
using CefSharp.Wpf;
using NowMine.Helpers;
using NowMine.Queue;
using System;
using System.Windows;
using NowMine.WebHandlers;
using System.Timers;
using System.IO;
using NowMineCommon.Models;

namespace NowMine.ViewModel
{
    class WebPanelViewModel
    {
        ChromiumWebBrowser WebControl;
        public bool _isPlaying = false;
        public bool isPlaying
        {
            get
            {
                return _isPlaying;
            }
            set
            {
                _isPlaying = value;
                IsPlayingEvent?.Invoke(value);
            }
        }
        public event Action<bool> IsPlayingEvent;
        public IWebHandler WebHandler;
        private Timer aTimer;

        public WebPanelViewModel(ref ChromiumWebBrowser webControl)
        {
            this.WebControl = webControl;
            //this.WebHandler = new YTVanilla();
            //this.WebHandler = new YTTV();
            this.WebHandler = new YTNowMine();
            //this.WebHandler = new YTIFrame();
            WebControl.IsBrowserInitializedChanged += WebPlayer_IsBrowserInitializedChanged;
            QueueManager.PlayedNow += PlayNow;
            QueueManager.VideoQueued += VideoQueuedHandler;
            QueueManager.PlayedNext += PlayNow;
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
            string homeAddress = WebHandler.GetHomePage();
            if (homeAddress.StartsWith("http"))
                WebControl.Load(homeAddress);
            else
            {
                var html = File.ReadAllText(Directory.GetCurrentDirectory() + @"\Resources\" + homeAddress);
                WebControl.LoadHtml(html, @"local://home.html");
            }
        }

        private void PlayNow(string id)
        {
            switch(WebHandler.NextVideoType())
            {
                case Enumerates.NextVideoType.ChangeSite:
                    
                    Application.Current.Dispatcher.Invoke(new Action(() => {
                        this.WebControl.Address = WebHandler.GetVideoURL(id);
                        AddJsListener();
                    }));
                    break;
                case Enumerates.NextVideoType.Script:
                    WebControl.GetMainFrame().ExecuteJavaScriptAsync("changeVideo('" + id + "')");
                    break;
            }
            if (!isPlaying)
            {
                isPlaying = true;
            }
            //todo sprawdzenie czy przełączyło się
        }

        internal void PlayNow(int qPos, uint eventID)
        {
            if (QueueManager.Queue.Count > qPos)
            {
                PlayNow(QueueManager.Queue[qPos].ClipInfo.ID);
            }
        }

        internal void PlayNow(string videoID, uint eventID)
        {
            if(!string.IsNullOrEmpty(videoID))
                PlayNow(videoID);
            else
            {
                isPlaying = false;
            }
        }

        //functions to call from javascript
        public void getNextVideo()
        {
            isPlaying = false;
            var nextVideo = QueueManager.GetNextPiece();
            if (nextVideo != null)
            {
                PlayNow(nextVideo.ClipInfo.ID);
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
            Console.WriteLine("WebPanelViewModel ONERROR");
            //var nowPlaying = QueueManager.NowPlaying;
            WebHandler = WebHandler.GetErrorHandler();  //no
            isPlaying = true;
        }

        private void AddJsListener()
        {
            Console.WriteLine("Injecting listiner for DOM creation");
            aTimer = new Timer(10 * 60 * 3);
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
            var Scripts = WebHandler.GetAfterLoadScripts();
            foreach (var script in Scripts)
            {
                WebControl.GetMainFrame().ExecuteJavaScriptAsync(script);
            }
        }

        public void VideoQueuedHandler(ClipQueued clip, uint eventID)
        {
            if (!isPlaying)
                PlayNow(clip.ID);
        }

        internal void PlayedNowHandler(object s, GenericEventArgs<int> e)
        {
            int qPos = e.EventData == -1 ? QueueManager.Queue.Count - 1 : e.EventData;
            PlayNow(QueueManager.Queue[qPos].ClipInfo.ID);
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
