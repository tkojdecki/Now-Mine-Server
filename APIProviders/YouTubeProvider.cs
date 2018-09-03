using System;
using System.Collections.Generic;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using NowMine.Models;

namespace NowMine.APIProviders
{

    public class YouTubeProvider : IAPIProvider
    {
        private const string SEARCH = "http://gdata.youtube.com/feeds/api/videos?q={0}&alt=rss&&max-results=20&v=1";
        YouTubeService youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            ApiKey = "AIzaSyC5zI6qk0KkTtePHp5yh23fcPgSLnio2V4",
            ApplicationName = "Play Mine!"
        });

        /// <summary>
        /// Returns a List<see cref="ClipInfo">YouTubeInfo</see> which represent
        /// the YouTube videos that matched the keyWord input parameter
        /// </summary>
        public List<ClipInfo> GetSearchClipInfos(string keyWord)
        {
            var SearchRequest = youtubeService.Search.List("snippet");
            SearchRequest.Q = keyWord; // Replace with your search term.
            SearchRequest.MaxResults = 20;
        
            //TRY!!! - przy braku neta się sypie
            var SearchList = SearchRequest.Execute();
            List<ClipInfo> resultInfo = new List<ClipInfo>();

            foreach (var SearchItem in SearchList.Items)
            {
                ClipInfo result = new ClipInfo();
                switch (SearchItem.Id.Kind)
                {
                    case "youtube#video":
                        result.Title = SearchItem.Snippet.Title;
                        result.ChannelName = SearchItem.Snippet.ChannelTitle;
                        result.ID = SearchItem.Id.VideoId;
                        result.Thumbnail = SearchItem.Snippet.Thumbnails.Default__;
                        resultInfo.Add(result);
                        break;

                    case "youtube#channel":
                        break;

                    case "youtube#playlist":
                        break;
                }
            }
            return resultInfo;
        }
    }
}
