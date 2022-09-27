using Andrew_2_0_Libraries.Models;
using System;
using System.Data;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace Cookalong.Controls.PopupWindows
{
    /// <summary>
    /// Interaction logic for Popup_Method.xaml
    /// </summary>
    public partial class Popup_Method : UserControl
    {
        Action? _cancelCallback;
        Action<string> _confirmCallback;
        DraggableObject _draggableObject;

        const int MIN_LENGTH = 5;

        public Popup_Method(DraggableObject draggableObject, Action cancelCallback, Action<string> confirmCallback)
        {
            InitializeComponent();
            _cancelCallback = cancelCallback;
            _confirmCallback = confirmCallback;
            _draggableObject = draggableObject;

            txtMethodContent.Text = draggableObject != null ? draggableObject.txtData.Text : string.Empty;

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

        /// TODO: Refactor this to a new file (used multiple places)
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

        private void cmdConfirm_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (txtMethodContent.Text.Length < MIN_LENGTH)
            {
                RecipeMenu.Instance.ShowError($"Message is too short. Must be at least {MIN_LENGTH} characters");
            }
            else
            {
                _confirmCallback?.Invoke(txtMethodContent.Text);
            }
        }

        private void txtMethodContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            var length = txtMethodContent.Text.Trim().Length;
            var remaining = txtMethodContent.MaxLength - length;
            lblRemainingChars.Content = "Remaining Characters: " + remaining;
        }

        private void txtMethodContent_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char[] forbidden = { '@', '~', '\"', '#' };
            if (forbidden.Any(c => e.Text.Contains(c)))
                e.Handled = true;
        }

        private void txtMethodContent_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                // don't allow space if nothing else
                if (txtMethodContent.Text.Length == 0)
                    e.Handled = true;
                // don't allow double space
                else if (txtMethodContent.Text.Last() == ' ')
                    e.Handled = true;
            }
        }
    }
}
