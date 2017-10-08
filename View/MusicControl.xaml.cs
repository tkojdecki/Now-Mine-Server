using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using NowMine.Data;

namespace NowMine.View
{
    public partial class MusicControl : UserControl
    {

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
                Visibility = Visibility.Visible;
            }
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
