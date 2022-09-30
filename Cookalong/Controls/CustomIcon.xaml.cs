using System;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Cookalong.Controls
{
    /// <summary>
    /// Interaction logic for CustomIcon.xaml
    /// </summary>
    public partial class CustomIcon : UserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CustomIcon()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the image on the icon
        /// </summary>
        /// <param name="img">The name of the image to set</param>
        public void SetImage(string img)
        {
            imgIcon.Source = new BitmapImage(new Uri(
                "pack://application:,,,/" + Assembly.GetExecutingAssembly().FullName + ";component/Images/" + img + ".png", UriKind.RelativeOrAbsolute));
        }
    }
}
