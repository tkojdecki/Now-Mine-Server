using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Xml.Linq;

namespace NowMine
{

    /// Uses XLINQ to create a List<see cref="YouTubeInfo">YouTubeInfo</see> using an Rss feed.
    /// 
    /// The following links are useful information regarding how the YouTube API works 
    /// 
    /// Example url
    ///
    /// http://gdata.youtube.com/feeds/api/videos?q=football+-soccer&alt=rss&orderby=published&start-index=11&max-results=10&v=1
    ///
    ///
    /// API Notes
    /// http://code.google.com/apis/youtube/2.0/developers_guide_protocol_api_query_parameters.html
    /// </summary>
    public class YouTubeProvider
    {
        #region Data
        private const string SEARCH = "http://gdata.youtube.com/feeds/api/videos?q={0}&alt=rss&&max-results=20&v=1";
        #endregion

        #region Load Videos From Feed
        /// <summary>
        /// Returns a List<see cref="YouTubeInfo">YouTubeInfo</see> which represent
        /// the YouTube videos that matched the keyWord input parameter
        /// </summary>
        public static List<YouTubeInfo> LoadVideosKey(string keyWord)
        {
            try
            {
                var xraw = XElement.Load(string.Format(SEARCH, keyWord));                
                var xroot = XElement.Parse(xraw.ToString());
                //Console.Out.WriteLine("xroot: " + xroot);
                var links = (from item in xroot.Element("channel").Descendants("item")
                             select new YouTubeInfo
                             {
                                 LinkUrl = item.Element("link").Value,
                                 EmbedUrl = GetEmbedUrlFromLink(item.Element("link").Value),
                                 ThumbNailUrl = GetThumbNailUrlFromLink(item),
                                 Title = item.Element("title").Value,
                                 Author = item.Element("author").Value                                 
                             }).Take(20);

                return links.ToList<YouTubeInfo>();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message, "ERROR");
            }
            return null;
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
            try
            {
                string embedUrl = link.Substring(0, link.IndexOf("&")).Replace("watch?v=", "embed/");
                return embedUrl;
            }
            catch
            {
                return link;
            }
        }


        private static string GetThumbNailUrlFromLink(XElement element)
        {

            XElement group = null;
            XElement thumbnail = null;
            string thumbnailUrl = @"../Images/logo.png";

            foreach (XElement desc in element.Descendants())
            {
                if (desc.Name.LocalName == "group")
                {
                    group = desc;
                    break;
                }
            }

            if (group != null)
            {
                foreach (XElement desc in group.Descendants())
                {
                    if (desc.Name.LocalName == "thumbnail")
                    {
                        thumbnailUrl = desc.Attribute("url").Value.ToString();
                        break;
                    }
                }
            }

            return thumbnailUrl;
        }

        #endregion
    }
}
