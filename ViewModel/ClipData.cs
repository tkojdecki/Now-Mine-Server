using NowMine.Models;
using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NowMineCommon.Models;

namespace NowMine.ViewModel
{
    public class ClipData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private DateTime? CreatedDate { get; set; }
        private DateTime? m_PlayedDate { get; set; }

        public ClipInfo ClipInfo;
        public User User;
        
        public EventHandler<ClipData> OnClick;

        public string Title
        {
            get
            {
                return this.ClipInfo?.Title;
            }
        }

        public string ChannelName
        {
            get
            {
                return this.ClipInfo?.ChannelName;
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
                return new BitmapImage(this.ClipInfo?.Thumbnail);
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
                return this.User.GetColor();
            }
            set
            {
                this.m_Color = value;
            }
        }

        public ClipData(ClipInfo ytInfo, User user = null)
        {
            this.ClipInfo = ytInfo;
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

        public ClipData Copy()
        {
            ClipData md = new ClipData(this.ClipInfo, this.User);

            return md;
        }

        //public void SetPlayedDate()
        //{
        //    this.m_PlayedDate = DateTime.Now;
        //}
    }
}
