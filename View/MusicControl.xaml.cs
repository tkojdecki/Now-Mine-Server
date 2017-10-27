using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using NowMine.ViewModel;

namespace NowMine.View
{
    public partial class MusicControl : UserControl
    {
        public bool UsernameVisible
        {
            get
            {
                return (bool)GetValue(UsernameVisibleProperty);
            }
            set
            {
                SetValue(UsernameVisibleProperty, value);
            }
        }

        public static readonly DependencyProperty UsernameVisibleProperty
            = DependencyProperty.Register(
                "UsernameVisible",
                typeof(bool),
                typeof(MusicControl),
                new UIPropertyMetadata(true, UsernameVisibilityChanged)
            );

        private static void UsernameVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MusicControl mc = (MusicControl) d;
            if (mc != null)
            {
                mc.SetupUsernameVisibility();
            }
        }

        public MusicControl()
        {
            InitializeComponent();

            Visibility = Visibility.Hidden;

            DataContextChanged += new DependencyPropertyChangedEventHandler(OnDataContextChanged);
        }

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MusicData musicData = (MusicData)this.DataContext;
            if (musicData != null)
            {
                MC_Border.BorderBrush = new SolidColorBrush(musicData.Color);

                this.Visibility = Visibility.Visible;
            }
        }

        public void SetupUsernameVisibility()
        {
            this.LabelUser.Visibility = this.UsernameVisible ? Visibility.Visible : Visibility.Hidden;
        }

        public void MusicControl_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MusicData musicData = (MusicData)this.DataContext;
            if (musicData != null)
            {
                musicData.OnClick.Invoke(this, musicData);
                e.Handled = true;
            }
        }

        public void MusicControl_OnMouseEnter(object sender, MouseEventArgs e)
        {
            MusicControl_MainGrid.Background = Brushes.LightSkyBlue;
        }

        public void MusicControl_OnMouseLeave(object sender, MouseEventArgs e)
        {
            MusicControl_MainGrid.Background = Brushes.White;
        }


    }
}
