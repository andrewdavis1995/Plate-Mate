using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cookalong.Controls
{
    /// <summary>
    /// Interaction logic for Input_Button.xaml
    /// </summary>
    public partial class Input_Button : UserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Input_Button()
        {
            InitializeComponent();
        }

        /// <summary>
        /// When the mouse enters the control
        /// </summary>
        private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Highlight.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// When the mouse exits the control
        /// </summary>
        private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Highlight.Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// Configures the appearance of the button
        /// </summary>
        /// <param name="title">Text to show on the button</param>
        /// <param name="setColour">Whether to set colour</param>
        /// <param name="colourOverride">Colour to set</param>
        public void Configure(string title)
        {
            lblMessage.Content = title;
        }
    }
}
