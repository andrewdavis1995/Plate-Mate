using System;
using System.Windows.Controls;
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
            SetButtonAppearance_(cmdCancel, "Cancel");
            SetButtonAppearance_(cmdConfirm, "Confirm");
        }

        /// <summary>
        /// Sets the appearance of a button
        /// </summary>
        /// <param name="button">The button to update</param>
        /// <param name="msg">Message to display</param>
        void SetButtonAppearance_(Input_Button button, string msg)
        {
            button.lblMessage.Content = msg;

            try
            {
                var res = FindResource("Button" + msg);
                button.bigBorder.BorderBrush = res as SolidColorBrush;
            }
            catch (Exception)
            {
                var res = FindResource("ButtonDefault");
                button.bigBorder.BorderBrush = res as SolidColorBrush;
            }
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
