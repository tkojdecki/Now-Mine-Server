using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NowMine
{
    /// <summary>
    /// A simple data class
    /// </summary>
    public class YouTubeInfo
    {
        #region Data
        public string LinkUrl { get; set; }
        public string EmbedUrl { get; set; }
        public string ThumbNailUrl { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ChannelName { get; set; }
        #endregion
    }
}
