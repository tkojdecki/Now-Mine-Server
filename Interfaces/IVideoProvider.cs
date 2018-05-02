using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NowMine.Interfaces
{
    interface IVideoProvider
    {
        List<string> GetAfterLoadScripts();
        string GetVideoURL(string id);
    }
}
