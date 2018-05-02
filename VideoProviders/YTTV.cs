using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NowMine.Interfaces;

namespace NowMine.VideoProviders
{
    class YTTV : IVideoProvider
    {
        public List<string> GetAfterLoadScripts()
        {
            List<string> scripts = new List<string>(); 
            //scripts.Add()
            return scripts;
        }

        public string GetVideoURL(string id)
        {
            return @"https://www.youtube.com/tv#/watch?v=" + id;
        }
    }
}
