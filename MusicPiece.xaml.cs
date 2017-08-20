using System;
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
using System.ComponentModel;
using NowMine.Queue;

namespace NowMine
{
    /// <summary>
    /// Interaction logic for ListObject.xaml
    /// </summary>
    public partial class MusicPiece : UserControl, INotifyPropertyChanged
    {  
        private DateTime Created { get; set; }
        private DateTime Played { get; set; }

        private YouTubeInfo _info = null;
        public YouTubeInfo Info
        {
            get { return _info; }
            set
            {
                _info = value;
                Title = _info.title;
                ChannelName = _info.channelName;
                setImage(_info.thumbnail.Url);
            }
        }

        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        private string _channelName;
        public string ChannelName
        {
            get
            {
                return _channelName;
            }
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
            get
            {
                return _user;
            }
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
        public MusicPiece(YouTubeInfo inf)
        {
            InitializeComponent();
            Info = inf;
            setImage(_info.thumbnail.Url);
            User = User.serverUser;
            lbluserName.Visibility = Visibility.Hidden;
            Created = DateTime.Now;
        }

        //to Queue
        public MusicPiece(YouTubeInfo inf, User user)
        {
            InitializeComponent();
            Info = inf;
            Created = DateTime.Now;
            User = user;
            setImage(_info.thumbnail.Url);
        }

        public MusicPiece copy()
        {
            MusicPiece musicPiece = new MusicPiece();
            musicPiece.InitializeComponent();
            musicPiece.Info = this._info;
            
            musicPiece.Created = DateTime.Now;
            musicPiece.User = this.User;
            musicPiece.setImage(_info.thumbnail.Url);
            return musicPiece;
        }

        internal void nowPlayingVisual()
        {
            SolidColorBrush redBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            this.border.BorderBrush = redBrush;
        }

        internal void userColorBrush()
        {
            SolidColorBrush userBrush = new SolidColorBrush(User.getColor());
            this.border.BorderBrush = userBrush;
            this.dropShadowEffect.Color = User.getColor();
            this.recBackground.Fill = userBrush;
            this.recBackground.Opacity = 0.3d;
        }

        internal void historyVisual()
        {
            SolidColorBrush greyBrush = new SolidColorBrush(Color.FromRgb(111, 111, 111));
            this.border.BorderBrush = greyBrush;
            this.lblTitle.BorderBrush = greyBrush;
            this.lblChannelName.BorderBrush = greyBrush;
        }

        public void setPlayedDate()
        {
            this.Played = DateTime.Now;
        }

        public void setImage(string url)
        {
            BitmapImage bmp = new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute));
            imgMain.Source = bmp;
        }

        private void SearchResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var musicPiece = (MusicPiece)sender;
            var queueMusicPiece = musicPiece.copy();
            queueMusicPiece.userColorBrush();
            queueMusicPiece.lbluserName.Visibility = System.Windows.Visibility.Visible;
            int qPos = QueueManager.addToQueue(queueMusicPiece);
        }
    }
}
