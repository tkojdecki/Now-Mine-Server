using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NowMine.ViewModel;

namespace NowMine.View
{
    public partial class QueuePanel : UserControl
    {
        public QueuePanel()
        {
            InitializeComponent();
            DataContextChanged += new DependencyPropertyChangedEventHandler(OnDataContextChanged);

        }

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(this.DataContext is QueuePanelViewModel))
            {
                return;
            }

            QueuePanelViewModel qpvm = (QueuePanelViewModel)this.DataContext;
            if (qpvm != null)
            {
                qpvm.SettingsChanged += OnSettingsChanged;
            }
        }

        private void OnSettingsChanged(object sender, QueuePanelSettings queuePanelSettings)
        {
            ColumnDefinitionNowPlaying.Width = new GridLength(queuePanelSettings.NowPlayingVisibility ? 360 : 0);
        }

        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (ScrollView == null)
            {
                return;
            }

            if (e.Delta < 0)
            {
                ScrollView.LineRight();
            }
            else
            {
                ScrollView.LineLeft();
            }

            e.Handled = true;
        }

        private void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = this.FindResource("ContextMenu") as ContextMenu;
            cm.PlacementTarget = sender as QueuePanel;
            cm.IsOpen = true;
        }
    }
}