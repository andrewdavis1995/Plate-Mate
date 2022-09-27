using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Cookalong.Controls
{
    /// <summary>
    /// Interaction logic for CustomIcon.xaml
    /// </summary>
    public partial class CustomIcon : UserControl
    {
        public CustomIcon()
        {
            InitializeComponent();
        }

        public void SetImage(string img)
        {
            imgIcon.Source = new BitmapImage(new Uri("/Images/" + img + ".png", UriKind.Relative));
        }
    }
}
