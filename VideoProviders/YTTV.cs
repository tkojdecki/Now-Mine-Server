using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NowMine.Interfaces;
using NowMine.Enumerates;

namespace NowMine.VideoProviders
{
    class YTTV : IVideoProvider
    {
        public List<string> GetAfterLoadScripts()
        {
            List<string> scripts = new List<string>();
            scripts.Add(@"var ytpl = document.getElementById('movie_player');");
            scripts.Add(@"ytpl.addEventListener('onStateChange', function onPlayerStateChange(event){if(event==0){app.getNextVideo();}});");
            scripts.Add("var header=document.evaluate(\"//*[@id=\'watch\']/div[2]\",document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue;");
            scripts.Add(@"header.remove();");  
            scripts.Add("var legend=document.evaluate(\"//*[@id=\'legend\']\",document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue;");
            scripts.Add(@"legend.remove();");
            scripts.Add("var films=document.evaluate(\"//*[@id=\'bottom-half\']/div[4]/div[2]\",document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue;");
            scripts.Add(@"films.remove();");
            return scripts;
        }

        public List<string> GetOnLoadScripts()
        {
            List<string> scripts = new List<string>();
            scripts.Add("var node=document.getElementById('player');");
            scripts.Add("var config={attributes:true,childList:true};");
            scripts.Add("var callback = function(mutationsList){for(var mutation of mutationsList){if(mutation.type=='attributes'){if(mutation.target.id=='player'){app.domLoaded();}};}};");
            scripts.Add("var observer=new MutationObserver(callback);");
            scripts.Add("observer.observe(node, config);");
            return scripts;
        }

        public string GetVideoURL(string id)
        {
            return @"https://www.youtube.com/tv#/watch?v=" + id;
        }

        public IVideoProvider GetErrorHandler()
        {
            return this;
        }

        public string GetHomePage()
        {
            return "home.html";
        }

        public NextVideoType NextVideoType()
        {
            return Enumerates.NextVideoType.ChangeSite;
        }
    }
}
