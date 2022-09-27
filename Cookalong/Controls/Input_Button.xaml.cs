using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cookalong.Controls
{
    /// <summary>
    /// Interaction logic for Input_Button.xaml
    /// </summary>
    public partial class Input_Button : UserControl
    {
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

        public void Configure(string title, bool setColour = true, string colourOverride = "")
        {
            lblMessage.Content = title;

            if (setColour)
            {
                try
                {
                    var buttonType = string.IsNullOrEmpty(colourOverride) ? title : colourOverride;
                    var res = FindResource("Button" + buttonType);
                    bigBorder.BorderBrush = res as SolidColorBrush;
                }
                catch (Exception) { }
            }
        }
    }
}
