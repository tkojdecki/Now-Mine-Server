using NowMine.Enumerates;
using System.Collections.Generic;

namespace NowMine.WebHandlers
{
    class YTNowMine : IWebHandler
    {
        private const string LOCALSITEADDRESS = @"local://index.html";

        public List<string> GetAfterLoadScripts()
        {
            return new List<string>();
        }

        public List<string> GetOnLoadScripts()
        {
            return new List<string>();
        }

        public string GetVideoURL(string id)
        {
            return LOCALSITEADDRESS;
        }

        public IWebHandler GetErrorHandler()
        {
            return new YTTV();
        }

        public string GetHomePage()
        {
            return LOCALSITEADDRESS;
        }

        public NextVideoType NextVideoType()
        {
            return Enumerates.NextVideoType.Script;
        }
    }
}
