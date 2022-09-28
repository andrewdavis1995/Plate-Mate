using System;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace Cookalong.Controls.PopupWindows
{
    /// <summary>
    /// Interaction logic for ConfirmationPopup.xaml
    /// </summary>
    public partial class Popup_Confirmation : UserControl
    {
        Action? _cancelCallback;
        Action _confirmCallback;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="title">Title to display</param>
        /// <param name="message">Message to display</param>
        /// <param name="cancelCallback">Function to call on cancel</param>
        /// <param name="confirmCallback">Function to call on confirm</param>
        public Popup_Confirmation(string title, string message, Action? cancelCallback, Action confirmCallback)
        {
            InitializeComponent();

            // configure texts
            lblMessage.Text = message;
            lblTitle.Content = title;

            _cancelCallback = cancelCallback;
            _confirmCallback = confirmCallback;

            // configure button appearance
            ConfigureButtons_();
        }

        /// <summary>
        /// Configures the buttons
        /// </summary>
        private void ConfigureButtons_()
        {
            ControlHelper.SetButtonAppearance_(cmdCancel, "Cancel");
            ControlHelper.SetButtonAppearance_(cmdConfirm, "Confirm");
        }

        /// <summary>
        /// Event handler for cancel
        /// </summary>
        private void cmdCancel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _cancelCallback?.Invoke();
        }

        /// <summary>
        /// Event handler for confirm button
        /// </summary>
        private void cmdConfirm_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _confirmCallback?.Invoke();
        }
    }
}
