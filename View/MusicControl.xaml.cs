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
            MusicControl_MainGrid.Background = Brushes.Aquamarine;
        }

        public void MusicControl_OnMouseLeave(object sender, MouseEventArgs e)
        {
            MusicControl_MainGrid.Background = Brushes.White;
        }
    }
}
