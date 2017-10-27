﻿using Google.Apis.YouTube.v3.Data;
using NowMine.ViewModel;

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
        public string title { get; set; }
        public string channelName { get; set; }
        public Thumbnail thumbnail { get; set; }

        internal void buildURL()
        {
            this.LinkUrl = "http://www.youtube.com/embed/" + id;
        }

        #endregion
    }

    public class NetworkYoutubeInfo : YouTubeInfo
    {
        public string userName;
        public string color;

        public NetworkYoutubeInfo(YouTubeInfo info)
        {
            this.id = info.id;
            this.title = info.title;
            this.channelName = info.channelName;
            this.thumbnail = info.thumbnail;
        }

        public NetworkYoutubeInfo(YouTubeInfo info, User user)
        {
            this.id = info.id;
            this.title = info.title;
            this.channelName = info.channelName;
            this.thumbnail = info.thumbnail;
            this.userName = user.Name;
            this.color = user.Color.ToString();
        }
    }

    public class YoutubeQueued : YouTubeInfo
    {
        public int QPos { get; set; }
        public int UserID { get; set; }

        public YoutubeQueued (YouTubeInfo yi, int qPos, int userId)
        {
            this.LinkUrl = yi.LinkUrl;
            this.id = yi.id;
            this.title = yi.title;
            this.channelName = yi.channelName;
            this.thumbnail = yi.thumbnail;
            this.QPos = qPos;
            this.UserID = userId;
        }

        public YoutubeQueued(YouTubeInfo yi, int qPos) //for PlayedNow from QueuePanel to send on udp played video from queue
        {
            this.LinkUrl = yi.LinkUrl;
            this.id = yi.id;
            this.title = yi.title;
            this.channelName = yi.channelName;
            this.thumbnail = yi.thumbnail;
            this.QPos = qPos;
            this.UserID = 0;
        }

        public YoutubeQueued(MusicData musicPiece, int qPos)
        {
            this.LinkUrl = musicPiece.YTInfo.LinkUrl;
            this.id = musicPiece.YTInfo.id;
            this.title = musicPiece.YTInfo.title;
            this.channelName = musicPiece.YTInfo.channelName;
            this.thumbnail = musicPiece.YTInfo.thumbnail;
            this.QPos = qPos;
            this.UserID = musicPiece.User.Id;
        }
        
    }
}
