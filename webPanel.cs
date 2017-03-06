﻿using CefSharp;
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
        MainWindow mainWindow;
        public bool isPlaying = false;

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
            if (nextVideo != null)
            {
                Application.Current.Dispatcher.Invoke(new Action(() => { nextVideo.nowPlayingVisual(); }));
                //nextVideo.nowPlayingVisual();
                Application.Current.Dispatcher.Invoke(new Action(() => { queuePanel.toHistory(queuePanel.nowPlaying()); }));
                //queuePanel.toHistory(queuePanel.nowPlaying());

                playNow(nextVideo.Info.id);
                Application.Current.Dispatcher.Invoke(new Action(() => { queuePanel.populateQueueBoard(); }));
                //queuePanel.populateQueueBoard();
                isPlaying = true;
            }
            else
            {
                isPlaying = false;
                //queueEnded  <---tutaj skończyłem

                if (mainWindow.isYoutubePage)
                {
                    mainWindow.isYoutubePage = false;
                    
                }
            }
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
        }

        public void ytEnded()
        {
            Console.WriteLine("asdfasdfasdf");
        }

        internal void setYoutubeWrapper(bool isInitial)
        {
            this.webControl.Address = @"local://index.html";
            queuePanel.playNext();
        }
    }
}
