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

namespace NowMine
{
    /// <summary>
    /// Interaction logic for ListObject.xaml
    /// </summary>
    public partial class MusicPiece : UserControl
    {
        
        private YouTubeInfo info = null;
        public User user = null;

        public MusicPiece()
        {
            InitializeComponent();
        }

        public MusicPiece(YouTubeInfo inf)
        {
            this.info = inf;
            InitializeComponent();
<<<<<<< HEAD
            lblTitle.Content = info.title;
            lblChannelName.Content = info.channelName;
            setImage = info.thumbnail.Url;
            lbluserName.Content = "Server";
        }

        public MusicPiece(YouTubeInfo inf, User user)
        {
            this.user = user;
            this.info = inf;
            InitializeComponent();
            lblTitle.Content = info.title;
            lblChannelName.Content = info.channelName;
=======
            lblTitle.Content = info.Title;
            lblChannelName.Content = info.ChannelName;
>>>>>>> 3e8e90aa518eb41b9e9876abd6e8831da805f7c6
            setImage = info.thumbnail.Url;
            lbluserName.Content = this.user.name;
        }

        public YouTubeInfo Info
        {
            get { return info; }
            set
            {
                info = value;
<<<<<<< HEAD
                setTitle = info.title;
                setChannelName = info.channelName;
=======
                setTitle = info.Title;
                setChannelName = info.ChannelName;
>>>>>>> 3e8e90aa518eb41b9e9876abd6e8831da805f7c6
                setImage = info.thumbnail.Url;
            }
        }

        public MusicPiece copy()
        {
            MusicPiece musicPiece = new MusicPiece();
            musicPiece.info = this.info;
<<<<<<< HEAD
            musicPiece.lblTitle.Content = info.title;
            musicPiece.lblChannelName.Content = info.channelName;
=======
            musicPiece.lblTitle.Content = info.Title;
            musicPiece.lblChannelName.Content = info.ChannelName;
>>>>>>> 3e8e90aa518eb41b9e9876abd6e8831da805f7c6
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
