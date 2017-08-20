using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Xml.Linq;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;


namespace NowMine
{

    public class YouTubeProvider
    {
        #region Data
        private const string SEARCH = "http://gdata.youtube.com/feeds/api/videos?q={0}&alt=rss&&max-results=20&v=1";
        YouTubeService youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            ApiKey = "AIzaSyC5zI6qk0KkTtePHp5yh23fcPgSLnio2V4",
            ApplicationName = "Play Mine!"
        });
        #endregion

        #region Connecting
        public void connectToYoutube()
        {
            youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyC5zI6qk0KkTtePHp5yh23fcPgSLnio2V4",
                ApplicationName = "Play Mine!"
            });
        }

        #endregion

        #region Load Videos From Feed
        /// <summary>
        /// Returns a List<see cref="YouTubeInfo">YouTubeInfo</see> which represent
        /// the YouTube videos that matched the keyWord input parameter
        /// </summary>
        public List<YouTubeInfo> LoadVideosKey(string keyWord)
        {
            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = keyWord; // Replace with your search term.
            searchListRequest.MaxResults = 20;

            // Call the search.list method to retrieve results matching the specified query term.

            //TRY!!! - przy braku neta się sypie
            var searchListResponse = searchListRequest.Execute();

            List<string> videos = new List<string>();
            //List<string> channels = new List<string>();
            //List<string> playlists = new List<string>();

            List<YouTubeInfo> resultInfo = new List<YouTubeInfo>();

            foreach (var MusicPiece in searchListResponse.Items)
            {
                YouTubeInfo result = new YouTubeInfo();
                switch (MusicPiece.Id.Kind)
                {
                    case "youtube#video":
                        videos.Add(String.Format("{0} ({1})", MusicPiece.Snippet.Title, MusicPiece.Id.VideoId));
                        result.title = MusicPiece.Snippet.Title;
                        result.channelName = MusicPiece.Snippet.ChannelTitle;
                        result.id = MusicPiece.Id.VideoId;
                        result.LinkUrl = "http://www.youtube.com/embed/" + MusicPiece.Id.VideoId;
                        result.thumbnail = MusicPiece.Snippet.Thumbnails.Default__;
                        resultInfo.Add(result);
                        break;

                    case "youtube#channel":
                        //channels.Add(String.Format("{0} ({1})", MusicPiece.Snippet.Title, MusicPiece.Id.ChannelId));
                        //result.ChannelName = MusicPiece.Snippet.Title;
                        break;

                    case "youtube#playlist":
                        //playlists.Add(String.Format("{0} ({1})", MusicPiece.Snippet.Title, MusicPiece.Id.PlaylistId));
                        //result.Title = MusicPiece.Snippet.Title;
                        break;
                }
            }
            return resultInfo;
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Simple helper methods that tunrs a link string into a embed string
        /// for a YouTube item. 
        /// turns 
        /// http://www.youtube.com/watch?v=hV6B7bGZ0_E
        /// into
        /// http://www.youtube.com/embed/hV6B7bGZ0_E
        /// </summary>
        private static string GetEmbedUrlFromLink(string link)
        {
            return "asdf";
        }


        private static string GetThumbNailUrlFromLink(XElement element)
        {
            return "asdf";
        }

        #endregion
    }
}
