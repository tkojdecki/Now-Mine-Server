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
        private DateTime created { get; set; }
        private DateTime played { get; set; }

        public YouTubeInfo Info
        {
            get { return info; }
            set
            {
                info = value;
                setTitle = info.title;
                setChannelName = info.channelName;
                setImage = info.thumbnail.Url;
            }
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

        public MusicPiece()
        {
            InitializeComponent();
        }

        // to the SearchBar
        public MusicPiece(YouTubeInfo inf)
        {
            InitializeComponent();
            this.info = inf;
            lblTitle.Content = info.title;
            lblChannelName.Content = info.channelName;
            setImage = info.thumbnail.Url;
            lbluserName.Content = User.getServerUser().name;
            lbluserName.Visibility = Visibility.Hidden;
            created = DateTime.Now;
        }

        //to Queue
        public MusicPiece(YouTubeInfo inf, User user)
        {
            InitializeComponent();
            this.info = inf;
            lblTitle.Content = info.title;
            lblChannelName.Content = info.channelName;
            setImage = info.thumbnail.Url;
            created = DateTime.Now;
            lbluserName.Content = user.name;
        }

        public MusicPiece copy()
        {
            MusicPiece musicPiece = new MusicPiece();
            musicPiece.InitializeComponent();
            musicPiece.info = this.info;
            musicPiece.lblTitle.Content = info.title;
            musicPiece.lblChannelName.Content = info.channelName;
            musicPiece.setImage = info.thumbnail.Url;
            musicPiece.created = DateTime.Now;
            musicPiece.lbluserName.Content = lbluserName.Content;
            return musicPiece;
        }

        internal void nowPlayingVisual()
        {
            SolidColorBrush redBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            this.border.BorderBrush = redBrush;
        }

        internal void userColorBrush(User user)
        {
            SolidColorBrush userBrush = new SolidColorBrush(user.getColor());
            this.border.BorderBrush = userBrush;
            this.dropShadowEffect.Color = user.getColor();
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
            this.played = DateTime.Now;
        }
    }
}
