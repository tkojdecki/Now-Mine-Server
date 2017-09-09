﻿using System;
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

        public string Title
        {
            get
            {
                return this.YTInfo?.title;
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

        public void SetPlayedDate()
        {
            this.PlayedDate = DateTime.Now;
        }
    }
}
