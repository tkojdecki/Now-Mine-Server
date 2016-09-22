using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Apis.YouTube.v3.Data;
namespace NowMine
{
    /// <summary>
    /// A simple data class
    /// </summary>
    public class YouTubeInfo
    {
        #region Data
        public string LinkUrl { get; set; }
        public string Id { get; set; }
        //public string EmbedUrl { get; set; }
        public string Title { get; set; }
        //public string Author { get; set; }
        public string ChannelName { get; set; }
        public Thumbnail thumbnail { get; set; }

        internal void buildURL()
        {
            //this.Author = this.ChannelName;
            this.LinkUrl = "http://www.youtube.com/embed/" + Id;
        }

        #endregion
    }
}
