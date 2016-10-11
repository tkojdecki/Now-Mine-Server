using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Awesomium;
using Awesomium.Windows.Controls;
using Awesomium.Core;
using Awesomium.Core.Data;

namespace NowMine
{
    class WebPanel
    {
        WebControl webControl;
        QueuePanel queuePanel;
        public bool isPlaying = false;

        public WebPanel(WebControl webControl, QueuePanel queuePanel)
        {
            this.webControl = webControl;
            this.queuePanel = queuePanel;
        }

        public void setSession()
        {
            WebSession webSession = WebCore.CreateWebSession(WebPreferences.Default);
            webSession.AddDataSource("settings", new ResourceDataSource(ResourceType.Embedded));
            webControl.WebSession = webSession;
            webControl.Source = new Uri("asset://settings/YoutubeWrapper.html");
        }

        public void reinitialize(WebControl webControl, QueuePanel queuePanel)
        {
            this.webControl = webControl;
            this.queuePanel = queuePanel;
            setSession();
        }

        public void playNow(String id)
        {
            if (!isPlaying)
            {
                isPlaying = true;
            }
            webControl.ExecuteJavascript("changeVideo('" + id + "')");
        }

        public void playNow(MusicPiece musicPiece)
        {
            if (!isPlaying)
            {
                isPlaying = true;
            }
            webControl.ExecuteJavascript("changeVideo(\'" + musicPiece.Info.id + "')");
        }

        public void BindMethods()
        {
            JSValue result = webControl.CreateGlobalJavascriptObject("app");
            if (result.IsObject)
            {
                JSObject appObject = result;
                appObject.Bind("getNextVideo", playNextQueued);
            }
        }

        private JSValue playNextQueued(object obj, JavascriptMethodEventArgs jsMethodArgs)
        {
            MusicPiece nextVideo = queuePanel.getNextMusicPiece();
            if (nextVideo != null)
            {
                isPlaying = true;
                nextVideo.nowPlayingVisual();
                queuePanel.toHistory(queuePanel.nowPlaying());

                playNow(nextVideo.Info.id);
                queuePanel.populateQueueBoard();
            }
            else
            {
                isPlaying = false;
            }
            
            return null;
        }
    }
}
