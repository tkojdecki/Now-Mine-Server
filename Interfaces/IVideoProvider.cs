using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NowMine.Enumerates;

namespace NowMine.Interfaces
{
    interface IVideoProvider
    {
        List<string> GetAfterLoadScripts();
        List<string> GetOnLoadScripts();
        string GetVideoURL(string id);
        string GetHomePage();
        IVideoProvider GetErrorHandler();
        NextVideoType NextVideoType();
    }
}
