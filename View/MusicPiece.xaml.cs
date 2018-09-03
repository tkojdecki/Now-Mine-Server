using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using NowMine.Queue;
using NowMine.Models;

namespace NowMine
{
    public partial class MusicPiece : UserControl
    {  
        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        private string _channelName;
        public string ChannelName
        {
            get { return _channelName; }
            set
            {
                _channelName = value;
                OnPropertyChanged("ChannelName");
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private User _user;
        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged("User");
            }
        }
        
        public MusicPiece()
        {
            InitializeComponent();
        }

        // to the SearchBar
        public MusicPiece(ClipInfo inf)
        {
            InitializeComponent();
            User = User.serverUser;
            lbluserName.Visibility = Visibility.Hidden;
            //Created = DateTime.Now;
        }

        //to Queue
        public MusicPiece(ClipInfo inf, User user)
        {
            InitializeComponent();
            //Created = DateTime.Now;
            User = user;
        }

        public MusicPiece Copy()
        {
            MusicPiece musicPiece = new MusicPiece();
            //musicPiece.Created = DateTime.Now;
            musicPiece.User = this.User;
            musicPiece.InitializeComponent();
            return musicPiece;
        }

        //internal void HistoryVisual()
        //{
        //    SolidColorBrush greyBrush = new SolidColorBrush(Color.FromRgb(111, 111, 111));
        //    this.border.BorderBrush = greyBrush;
        //    this.lblTitle.BorderBrush = greyBrush;
        //    this.lblChannelName.BorderBrush = greyBrush;
        //}

        //public void setPlayedDate()
        //{
        //    //this.Played = DateTime.Now;
        //}

        public void SetImage(string url)
        {
            BitmapImage bmp = new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute));
            imgMain.Source = bmp;
        }
    }
}
