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
            webControl.ExecuteJavascript("changeVideo('" + id + "')");
        }

        public void playNow(MusicPiece musicPiece)
        {
<<<<<<< HEAD
            webControl.ExecuteJavascript("changeVideo(\'" + musicPiece.Info.id + "')");
=======
            webControl.ExecuteJavascript("changeVideo(\'" + musicPiece.Info.Id + "')");
>>>>>>> 3e8e90aa518eb41b9e9876abd6e8831da805f7c6
        }

        public void BindMethods()
        {
            JSValue result = webControl.CreateGlobalJavascriptObject("app");
            if (result.IsObject)
            {
                JSObject appObject = result;
                appObject.Bind("getNextVideo", getNextVideo);
            }
        }

        private JSValue getNextVideo(object obj, JavascriptMethodEventArgs jsMethodArgs)
        {
            MusicPiece nextVideo = queuePanel.getNextVideo();
            nextVideo.nowPlayingVisual();
            queuePanel.toHistory(queuePanel.nowPlaying());
<<<<<<< HEAD
            playNow(nextVideo.Info.id);
=======
            playNow(nextVideo.Info.Id);
>>>>>>> 3e8e90aa518eb41b9e9876abd6e8831da805f7c6
            queuePanel.populateQueueBoard();
            return null;
        }
    }
}
