using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace Cookalong.Controls
{
    /// <summary>
    /// Interaction logic for LHS_Option.xaml
    /// </summary>
    public partial class LHS_Option : UserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public LHS_Option()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Configure the buttons
        /// </summary>
        /// <param name="msg">The message to display</param>
        /// <param name="icon">Icon to display</param>
        internal void Configure(string msg, string icon)
        {
            txtText.Content = msg;
            imgIcon.Source = new BitmapImage(new Uri(
                "pack://application:,,,/" + Assembly.GetExecutingAssembly().FullName + ";component/Images/" + icon + ".png", UriKind.RelativeOrAbsolute));

        }

        /// <summary>
        /// Event handler for when the mouse enters this control
        /// </summary>
        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            Highlight.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Event handler for when the mouse leaves this control
        /// </summary>
        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            Highlight.Visibility = Visibility.Collapsed;
        }
    }
}
