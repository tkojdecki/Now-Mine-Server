using NowMine.Models;
using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NowMine.ViewModel
{
    public class MusicData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private DateTime? CreatedDate { get; set; }
        private DateTime? m_PlayedDate { get; set; }

        public ClipInfo YTInfo = null;
        public User User;
        
        public EventHandler<MusicData> OnClick;

        public string Title
        {
            get
            {
                return this.YTInfo?.Title;
            }
        }

        public string ChannelName
        {
            get
            {
                return this.YTInfo?.ChannelName;
            }
        }

        public string UserName
        {
            get
            {
                return this.User?.Name;
            }
        }

        public BitmapImage Image
        {
            get
            {
                return new BitmapImage(new Uri(this.YTInfo?.Thumbnail.Url, UriKind.RelativeOrAbsolute));
            }
        }

        private Color? m_Color = null;
        public Color Color
        {
            get
            {
                if (this.m_Color.HasValue)
                {
                    return this.m_Color.Value;
                }
                return this.User.Color;
            }
            set
            {
                this.m_Color = value;
            }
        }

        public MusicData(ClipInfo ytInfo, User user = null)
        {
            this.YTInfo = ytInfo;
            this.CreatedDate = DateTime.Now;
            //todo
            if (user == null)
            {
                this.User = User.serverUser;
            }
            else
            {
                this.User = user;
            }
        }

        public MusicData Copy()
        {
            MusicData md = new MusicData(this.YTInfo, this.User);

            return md;
        }

        //public void SetPlayedDate()
        //{
        //    this.m_PlayedDate = DateTime.Now;
        //}
    }
}
