using System.Windows.Controls;
using System.Windows.Input;
using NowMine.Data;

namespace NowMine.View
{
    /// <summary>
    /// Logika interakcji dla klasy MusicControl.xaml
    /// </summary>
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
    }
}
