using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using NowMine.ViewModel;

namespace NowMine.View
{
    public partial class ClipControl : UserControl
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
                typeof(ClipControl),
                new UIPropertyMetadata(true, UsernameVisibilityChanged)
            );

        private static void UsernameVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ClipControl mc = (ClipControl) d;
            if (mc != null)
            {
                mc.SetupUsernameVisibility();
            }
        }

        public ClipControl()
        {
            InitializeComponent();

            Visibility = Visibility.Hidden;

            DataContextChanged += new DependencyPropertyChangedEventHandler(OnDataContextChanged);
        }

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ClipData musicData = (ClipData)this.DataContext;
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

        public void ClipControl_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ClipData musicData = (ClipData)this.DataContext;
            if (musicData != null)
            {
                musicData.OnClick.Invoke(this, musicData);
                e.Handled = true;
            }
        }

        public void ClipControl_OnMouseEnter(object sender, MouseEventArgs e)
        {
            ClipControl_MainGrid.Background = Brushes.LightSkyBlue;
        }

        public void ClipControl_OnMouseLeave(object sender, MouseEventArgs e)
        {
            ClipControl_MainGrid.Background = Brushes.White;
        }
    }
}
