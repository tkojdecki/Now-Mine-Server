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
        
        public WebPanel(WebControl webControl)
        {
            this.webControl = webControl;
        }

        public void playNow(Uri url)
        {
            webControl.Source = url;
        }
    }
}
