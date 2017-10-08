using System.Windows.Controls;
using System.Windows.Input;

namespace NowMine.View
{
    public partial class QueueControl : UserControl
    {
        public QueueControl()
        {
            InitializeComponent();
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
    }
}
