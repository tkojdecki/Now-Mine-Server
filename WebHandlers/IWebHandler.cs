using System.Collections.Generic;
using NowMine.Enumerates;

namespace NowMine.WebHandlers
{
    interface IWebHandler
    {
        List<string> GetAfterLoadScripts();
        List<string> GetOnLoadScripts();
        string GetVideoURL(string id);
        string GetHomePage();
        IWebHandler GetErrorHandler();
        NextVideoType NextVideoType();
    }
}
