﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Google.Apis.YouTube.v3.Data;

namespace NowMine
{
    /// <summary>
    /// Interaction logic for ListObject.xaml
    /// </summary>
    public partial class MusicPiece : UserControl
    {
        
        private YouTubeInfo info = null;
        public MusicPiece()
        {
            InitializeComponent();
        }

        public MusicPiece(YouTubeInfo inf)
        {
            this.info = inf;
            InitializeComponent();
            lblTitle.Content = info.Title;
            lblChannelName.Content = info.ChannelName;
            setImage = info.thumbnail.Url;
        }

        public YouTubeInfo Info
        {
            get { return info; }
            set
            {
                info = value;
                setTitle = info.Title;
                setChannelName = info.ChannelName;
                setImage = info.thumbnail.Url;
            }
        }

        public MusicPiece copy()
        {
            MusicPiece musicPiece = new MusicPiece();
            musicPiece.info = this.info;
            musicPiece.lblTitle.Content = info.Title;
            musicPiece.lblChannelName.Content = info.ChannelName;
            musicPiece.setImage = info.thumbnail.Url;
            musicPiece.InitializeComponent();
            return musicPiece;
        }

        private string setTitle
        {
            set
            {
                lblTitle.Content = value;
            }
        }

        private string setChannelName
        {
            set
            {
                lblChannelName.Content = value;
            }
        }

        private string setImage
        {
            set
            {
                BitmapImage bmp = new BitmapImage(new Uri(value, UriKind.RelativeOrAbsolute));
                imgMain.Source = bmp;
            }
        }

        internal void nowPlayingVisual()
        {
            SolidColorBrush greenBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            this.border.BorderBrush = greenBrush;
        }

        internal void historyVisual()
        {
            SolidColorBrush greyBrush = new SolidColorBrush(Color.FromRgb(111, 111, 111));
            this.border.BorderBrush = greyBrush;
            this.lblTitle.BorderBrush = greyBrush;
            this.lblChannelName.BorderBrush = greyBrush;
        }
    }
}
