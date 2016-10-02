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
<<<<<<< HEAD
        public string id { get; set; }
        //public string EmbedUrl { get; set; }
        public string title { get; set; }
        //public string Author { get; set; }
        public string channelName { get; set; }
=======
        public string Id { get; set; }
        //public string EmbedUrl { get; set; }
        public string Title { get; set; }
        //public string Author { get; set; }
        public string ChannelName { get; set; }
>>>>>>> 3e8e90aa518eb41b9e9876abd6e8831da805f7c6
        public Thumbnail thumbnail { get; set; }

        internal void buildURL()
        {
            //this.Author = this.ChannelName;
<<<<<<< HEAD
            this.LinkUrl = "http://www.youtube.com/embed/" + id;
=======
            this.LinkUrl = "http://www.youtube.com/embed/" + Id;
>>>>>>> 3e8e90aa518eb41b9e9876abd6e8831da805f7c6
        }

        #endregion
    }
}
