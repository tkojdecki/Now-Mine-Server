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
        public string id { get; set; }
        //public string EmbedUrl { get; set; }
        public string title { get; set; }
        //public string Author { get; set; }
        public string channelName { get; set; }
        public Thumbnail thumbnail { get; set; }

        internal void buildURL()
        {
            //this.Author = this.ChannelName;
            this.LinkUrl = "http://www.youtube.com/embed/" + id;
        }

        #endregion
    }

    public class QueuePieceToSend : YouTubeInfo
    {
        public string userName;
        public string color;

        public QueuePieceToSend(YouTubeInfo info)
        {
            this.id = info.id;
            this.title = info.title;
            this.channelName = info.channelName;
            this.thumbnail = info.thumbnail;            
        }

        public QueuePieceToSend(YouTubeInfo info, User user)
        {
            this.id = info.id;
            this.title = info.title;
            this.channelName = info.channelName;
            this.thumbnail = info.thumbnail;
            this.userName = user.name;
            this.color = "#00ff00";
        }
    }
}
