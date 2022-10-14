using System;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using Cookalong.Helpers;

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
        /// <param name="cancelMsg">Message to display on the cancel message</param>
        /// <param name="confirmMsg">Message to display on the confirm message</param>
        public Popup_Confirmation(string title, string message, Action? cancelCallback, Action confirmCallback, string cancelMsg = "Cancel", string confirmMsg = "Confirm")
        {
            InitializeComponent();

            // configure texts
            lblMessage.Text = message;
            lblTitle.Content = title;

            _cancelCallback = cancelCallback;
            _confirmCallback = confirmCallback;

            // configure button appearance
            ConfigureButtons_(cancelMsg, confirmMsg);
        }

        /// <summary>
        /// Configures the buttons
        /// </summary>
        /// <param name="cancelMsg">Message to display on the cancel message</param>
        /// <param name="confirmMsg">Message to display on the confirm message</param>
        private void ConfigureButtons_(string cancelMsg, string confirmMsg)
        {
            ControlHelper.SetButtonAppearance_(cmdCancel, cancelMsg);
            ControlHelper.SetButtonAppearance_(cmdConfirm, confirmMsg);
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
