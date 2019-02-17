using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NowMine.Enumerates;

namespace NowMine.WebHandlers
{
    class YTIFrame : IWebHandler
    {
        public List<string> GetAfterLoadScripts()
        {
            var scripts = new List<string>();
            scripts.Add(@"var ytpl = document.getElementById('movie_player');");
            scripts.Add(@"ytpl.addEventListener('onStateChange', function onPlayerStateChange(event){if(event==0){app.getNextVideo();}});");
            return scripts;
        }

        public IWebHandler GetErrorHandler()
        {
            return new YTVanilla();
        }

        public string GetHomePage()
        {
            //return "iframe.html";
            return @"http://fritzthescientis.byethost18.com/testy/iframe.html";
        }

        public List<string> GetOnLoadScripts()
        {
            return new List<string>();
        }

        public string GetVideoURL(string id)
        {
            return $"https://www.google.pl/";
        }

        public NextVideoType NextVideoType()
        {
            return Enumerates.NextVideoType.Script;
        }
    }
}
