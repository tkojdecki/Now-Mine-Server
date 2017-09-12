using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace NowMine.Data
{
    class MusicData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private DateTime CreatedDate { get; set; }
        private DateTime PlayedDate { get; set; }

        public YouTubeInfo YTInfo = null;
        public User User;
        
        public EventHandler<MusicData> OnClick;

        public string Title
        {
            get
            {
                return this.YTInfo?.title;
            }
        }

        public string ChannelName
        {
            get
            {
                return this.YTInfo?.channelName;
            }
        }

        public BitmapImage Image
        {
            get
            {
                return new BitmapImage(new Uri(this.YTInfo?.thumbnail.Url, UriKind.RelativeOrAbsolute));
            }
        }

        public MusicData(YouTubeInfo ytInfo, User user = null)
        {
            YTInfo = ytInfo;
            CreatedDate = DateTime.Now;
            if (user == null)
            {
                User = User.serverUser;
            }
            else
            {
                User = user;
            }
        }

        public MusicData Copy()
        {
            return new MusicData(this.YTInfo, this.User);
        }

        public void SetPlayedDate()
        {
            this.PlayedDate = DateTime.Now;
        }
    }
}
