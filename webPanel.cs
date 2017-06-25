using CefSharp;
using CefSharp.Wpf;
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
        QueuePanel queuePanel;
        MainWindow mainWindow;
        public bool isPlaying = false;

        public delegate void VideoEndedEventHandler(object source, EventArgs args);
        public event VideoEndedEventHandler VideoEnded;

        public WebPanel(ChromiumWebBrowser webControl, QueuePanel queuePanel, MainWindow mainWindow)
        {
            this.webControl = webControl;
            this.queuePanel = queuePanel;
            this.mainWindow = mainWindow;
        }

        public void reinitialize(ChromiumWebBrowser webControl, QueuePanel queuePanel)
        {
            this.webControl = webControl;
            this.queuePanel = queuePanel;
        }

        protected virtual void OnVideoEnded()
        {
            if (VideoEnded != null)
            {
                VideoEnded(this, EventArgs.Empty);
            }
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



        //public void BindMethods()
        //{
        //    webControl.RegisterJsObject("app", this);
        //}

        //functions to call from javascript
        public void getNextVideo()
        {
            MusicPiece nextVideo = queuePanel.getNextPiece();
            Application.Current.Dispatcher.Invoke(new Action(() => { queuePanel.toHistory(queuePanel.nowPlaying()); }));
            if (nextVideo != null)
            {
                Application.Current.Dispatcher.Invoke(new Action(() => { nextVideo.nowPlayingVisual(); }));
                //playNow(nextVideo.Info.id);//w zależności od isyoutubepage - na stronie yt changevideo nie dziaua
                Application.Current.Dispatcher.Invoke(new Action(() => { queuePanel.populateQueueBoard(); }));
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
            //Console.WriteLine("ONERROR" + error.ToString());
            Console.WriteLine("ONERROR");
            MusicPiece nowPlaying = queuePanel.nowPlaying();
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.webControl.Address = @"http://www.youtube.com/watch?v=" + nowPlaying.Info.id;
                //webControl.GetMainFrame().ExecuteJavaScriptAsync("alert('asdf')"); <- dziaua
                //webControl.GetMainFrame().ExecuteJavaScriptAsync("app.ytEnded();");
            }));
            mainWindow.isYoutubePage = true;
            mainWindow.videoID = nowPlaying.Info.id;
            isPlaying = true;
        }

        //public void ytEnded()
        //{
        //    Console.WriteLine("asdfasdfasdf");
        //}

        internal void setYoutubeWrapper(bool isInitial)
        {
            this.webControl.Address = @"local://index.html";
            queuePanel.playNext();
        }
    }
}
