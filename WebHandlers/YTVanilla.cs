using System.Collections.Generic;
using NowMine.Enumerates;

namespace NowMine.WebHandlers
{
    class YTVanilla : IWebHandler
    {
        public List<string> GetAfterLoadScripts()
        {
            var returnList = new List<string>();
            //clearing rest of window youtube page
            returnList.Add("var player=document.evaluate(\"//*[@id=\'top\']/*[@id=\'container\']/*[@id=\'main\']\",document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue;");
            returnList.Add("for(var childName in player.children){var childNode=player.children[childName];if(childNode.id && childNode.id != \'content-separator\'){player.removeChild(childNode);}}");
            returnList.Add("var toolbar=document.evaluate(\"//*[@id=\'masthead-container\']\",document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue;");
            returnList.Add(@"toolbar.remove();");
            returnList.Add("player.style.position=\'relative\';player.style.paddingBottom=\'56.25\';player.style.overflow=\'hidden\';player.style.maxWidth=\'100%\';");
            returnList.Add("player.style.cssText = \'\'; player.style.position = \'absolute\'; player.style.top = 0; player.style.left = 0;player.style.width=\'100%\';player.style.height=\'100%\';");
            return returnList;
        }

        public List<string> GetOnLoadScripts()
        {
            return new List<string>();
        }

        public string GetVideoURL(string id)
        {
            return @"http://www.youtube.com/watch?v=" + id;
        }

        public IWebHandler GetErrorHandler()
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
