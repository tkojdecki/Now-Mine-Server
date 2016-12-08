using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NowMine
{
    class WebPanel
    {
        ChromiumWebBrowser webControl;
        QueuePanel queuePanel;
        public bool isPlaying = false;

        public WebPanel(ChromiumWebBrowser webControl, QueuePanel queuePanel)
        {
            this.webControl = webControl;
            this.queuePanel = queuePanel;
        }

        public void reinitialize(ChromiumWebBrowser webControl, QueuePanel queuePanel)
        {
            this.webControl = webControl;
            this.queuePanel = queuePanel;
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
            if (!isPlaying)
            {
                isPlaying = true;
            }
            webControl.GetMainFrame().ExecuteJavaScriptAsync("changeVideo('" + musicPiece.Info.id + "')");
        }

        public void BindMethods()
        {
            webControl.RegisterJsObject("app", this);
        }

        public void getNextVideo()
        {
            MusicPiece nextVideo = queuePanel.getNextMusicPiece();
            if (nextVideo != null)
            {
                isPlaying = true;
                Application.Current.Dispatcher.Invoke(new Action(() => { nextVideo.nowPlayingVisual(); }));
                //nextVideo.nowPlayingVisual();
                Application.Current.Dispatcher.Invoke(new Action(() => { queuePanel.toHistory(queuePanel.nowPlaying()); }));
                //queuePanel.toHistory(queuePanel.nowPlaying());

                playNow(nextVideo.Info.id);
                Application.Current.Dispatcher.Invoke(new Action(() => { queuePanel.populateQueueBoard(); }));
                //queuePanel.populateQueueBoard();
            }
            else
            {
                isPlaying = false;
            }
        }
    }
}
