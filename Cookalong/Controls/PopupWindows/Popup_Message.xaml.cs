using System.Windows.Controls;

namespace Cookalong.Controls.PopupWindows
{
    /// <summary>
    /// Interaction logic for Popup_Message.xaml
    /// </summary>
    public partial class Popup_Message : UserControl
    {
        Grid _owner;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="title">Title to display</param>
        /// <param name="message">Message to display</param>
        /// <param name="owner">Grid this belongs to</param>
        public Popup_Message(string title, string message, Grid owner)
        {
            InitializeComponent();
            _owner = owner;

            // update visuals
            cmdOkay.Configure("OK");
            lblMessage.Text = message;
            lblTitle.Content = title;
        }

        /// <summary>
        /// Event handler for the OK button
        /// </summary>
        private void cmdOkay_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _owner.Children.Remove(this);
        }
    }
}
