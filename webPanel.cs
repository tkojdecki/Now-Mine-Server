using CefSharp;
using CefSharp.Wpf;
using NowMine.Helpers;
using NowMine.Queue;
using NowMine.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
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

        public WebPanel(ref ChromiumWebBrowser webControl, MainWindow mainWindow)
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
            //string html = File.ReadAllText(Directory.GetCurrentDirectory() + "/index.html");
            //Application.Current.Dispatcher.Invoke(new Action(() => { this.webControl.WebBrowser.LoadHtml(html, @"local://index.html"); }));
            //Console.WriteLine(html);
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
            QueueManager.playNext();
            //}
            //else
            //{
            //    isPlaying = false;
            //}

            //Application.Current.Dispatcher.Invoke(new Action(() => { this.webControl.Stop(); }));
            //Application.Current.Dispatcher.Invoke(new Action(() => { this.webControl.WebBrowser.Load(@"local://index.html"); }));
            //Application.Current.Dispatcher.Invoke(new Action(() => { this.webControl.WebBrowser.Load(@"E:/now-mine/Resources/index.html"); }));
            //Application.Current.Dispatcher.Invoke(new Action(() => { this.webControl.WebBrowser.Load(@"http://www.google.pl"); }));
            //Application.Current.Dispatcher.Invoke(new Action(() => { this.webControl.Reload(); }));
            //Application.Current.Dispatcher.Invoke(new Action(() => { QueueManager.playNext(); }));
        }


        public void errorHandle()
        {
            Console.WriteLine("ONERROR");
            var nowPlaying = QueueManager.nowPlaying();
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                //this.webControl.Address = @"http://www.youtube.com/watch?v=" + nowPlaying.YTInfo.id;
                this.webControl.Address = @"https://www.youtube.com/tv#/watch?v=" + nowPlaying.YTInfo.id;


                //this.webControl.FrameLoadEnd += ClearVideo_FrameLoadEnd;
            }));
            mainWindow.isYoutubePage = true;
            mainWindow.videoID = nowPlaying.YTInfo.id;
            isPlaying = true;
        }

        //private void ClearVideo_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        //{
        //    if (e.HttpStatusCode == 200 && e.Url.Contains("youtube.com"))
        //    {
        //        webControl.GetMainFrame().ExecuteJavaScriptAsync(string.Format("var player=document.evaluate(\"//*[@id=\'top\']/*[@id=\'container\']/*[@id=\'main\']\",document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue;"));
        //        webControl.GetMainFrame().ExecuteJavaScriptAsync(@"for(var childName in player.children){var childNode=player.children[childName];if(childNode.id && childNode.id != 'content-separator'){player.removeChild(childNode);}}");
        //        webControl.GetMainFrame().ExecuteJavaScriptAsync(string.Format("var toolbar=document.evaluate(\"//*[@id=\'masthead-container\']\",document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue;"));
        //        webControl.GetMainFrame().ExecuteJavaScriptAsync(@"toolbar.remove();");
        //        webControl.FrameLoadEnd -= ClearVideo_FrameLoadEnd;
        //    }
        //}

        //internal void setYoutubeWrapper(bool isInitial)
        //{
        //    this.webControl.Address = @"local://index.html";
        //    QueueManager.playNext();
        //}

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
    }
}
