using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Awesomium;
using Awesomium.Windows.Controls;
using Awesomium.Core;

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

        public void reinitialize(WebControl webControl, QueuePanel queuePanel)
        {
            this.webControl = webControl;
            this.queuePanel = queuePanel;
        }

        public void playNow(String id)
        {
            webControl.ExecuteJavascript("changeVideo('" + id + "')");
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
            playNow(nextVideo.Info.Id);
            queuePanel.populateQueueBoard();
            return null;
        }
    }
}
