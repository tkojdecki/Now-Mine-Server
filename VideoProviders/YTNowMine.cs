using NowMine.Enumerates;
using NowMine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NowMine.VideoProviders
{
    class YTNowMine : IVideoProvider
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

        public IVideoProvider GetErrorHandler()
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
