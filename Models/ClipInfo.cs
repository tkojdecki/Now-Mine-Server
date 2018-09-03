using Google.Apis.YouTube.v3.Data;
using NowMine.ViewModel;

namespace NowMine.Models
{
    public class ClipInfo
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string ChannelName { get; set; }
        public Thumbnail Thumbnail { get; set; }
    }

    public class NetworkClipInfo : ClipInfo
    {
        public string userName;
        public int userId;
        public string color;

        public NetworkClipInfo(ClipInfo info)
        {
            this.ID = info.ID;
            this.Title = info.Title;
            this.ChannelName = info.ChannelName;
            this.Thumbnail = info.Thumbnail;
        }

        public NetworkClipInfo(ClipInfo info, User user)
        {
            this.ID = info.ID;
            this.Title = info.Title;
            this.ChannelName = info.ChannelName;
            this.Thumbnail = info.Thumbnail;
            this.userName = user.Name;
            this.userId = user.Id;
            this.color = user.Color.ToString();
        }
    }

    public class ClipQueued : ClipInfo
    {
        public int QPos { get; set; }
        public int UserID { get; set; }

        public ClipQueued (ClipInfo yi, int qPos, int userId)
        {
            this.ID = yi.ID;
            this.Title = yi.Title;
            this.ChannelName = yi.ChannelName;
            this.Thumbnail = yi.Thumbnail;
            this.QPos = qPos;
            this.UserID = userId;
        }

        public ClipQueued(ClipInfo yi, int qPos) //for PlayedNow from QueuePanel to send on udp played video from queue
        {
            this.ID = yi.ID;
            this.Title = yi.Title;
            this.ChannelName = yi.ChannelName;
            this.Thumbnail = yi.Thumbnail;
            this.QPos = qPos;
            this.UserID = 0;
        }

        public ClipQueued(MusicData musicPiece, int qPos)
        {
            this.ID = musicPiece.YTInfo.ID;
            this.Title = musicPiece.YTInfo.Title;
            this.ChannelName = musicPiece.YTInfo.ChannelName;
            this.Thumbnail = musicPiece.YTInfo.Thumbnail;
            this.QPos = qPos;
            this.UserID = musicPiece.User.Id;
        }
        
    }
}
