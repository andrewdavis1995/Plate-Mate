using Andrew_2_0_Libraries.Models;
using Cookalong.Helpers;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cookalong.Controls.PopupWindows
{
    /// <summary>
    /// Interaction logic for Popup_Method.xaml
    /// </summary>
    public partial class Popup_Method : UserControl
    {
        Action? _cancelCallback;
        Action<MethodStep?> _confirmCallback;
        Action<string>? _errorCallback;
        MethodStep ?_existing;

        const int MIN_LENGTH = 5;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="existingText">The text to set on the input</param>
        /// <param name="cancelCallback">Callback for cancel button</param>
        /// <param name="confirmCallback">Callback for confirm button</param>
        public Popup_Method(MethodStep? existing, Action cancelCallback, Action<MethodStep?> confirmCallback, Action<string>? errorCallback)
        {
            InitializeComponent();

            // save callbacks
            _cancelCallback = cancelCallback;
            _confirmCallback = confirmCallback;
            _errorCallback = errorCallback;
            _existing = existing;

            txtMethodContent.Text = existing?.GetMethod();

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
        /// Event handler for cancel button
        /// </summary>
        private void cmdCancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _cancelCallback?.Invoke();
        }

        /// <summary>
        /// Event handler for confirm button
        /// </summary>
        private void cmdConfirm_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // check length
            if (txtMethodContent.Text.Length < MIN_LENGTH)
            {
                _errorCallback?.Invoke($"Message is too short. Must be at least {MIN_LENGTH} characters");
            }
            else
            {
                _existing?.UpdateText(txtMethodContent.Text);
                _confirmCallback?.Invoke(_existing);
            }
        }

        /// <summary>
        /// Event handler for when the text changes
        /// </summary>
        private void txtMethodContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            // update remaining characters
            var length = txtMethodContent.Text.Trim().Length;
            var remaining = txtMethodContent.MaxLength - length;
            lblRemainingChars.Content = "Remaining Characters: " + remaining;
        }

        /// <summary>
        /// Event handler for when text is about to change
        /// </summary>
        private void txtMethodContent_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // don't allow forbidden characters
            e.Handled = StringHelper.ForbiddenCharacter(e.Text);
        }

        /// <summary>
        /// Event handler for when text is about to change
        /// </summary>
        private void txtMethodContent_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                // check if a space is allowed
                e.Handled = !StringHelper.IsSpaceAllowed(txtMethodContent.Text);
            }
        }
    }
}
