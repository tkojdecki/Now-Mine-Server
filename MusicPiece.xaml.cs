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
            lblAuthor.Content = info.Author;
            setImage = info.thumbnail.Url;
        }

        public YouTubeInfo Info
        {
            get { return info; }
            set
            {
                info = value;
                setTitle = info.Title;
                setAuthor = info.Author;
                setImage = info.thumbnail.Url;
            }
        }

        public MusicPiece copy()
        {
            MusicPiece musicPiece = new MusicPiece();
            musicPiece.info = this.info;
            musicPiece.lblTitle.Content = info.Title;
            musicPiece.lblAuthor.Content = info.Author;
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

        private string setAuthor
        {
            set
            {
                lblAuthor.Content = value;
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
    }
}